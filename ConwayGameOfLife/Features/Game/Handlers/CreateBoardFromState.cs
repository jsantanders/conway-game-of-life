using System.Text.Json;
using ConwayGameOfLife.Attributes;
using ConwayGameOfLife.Database;
using ConwayGameOfLife.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

/// <summary>
/// Creates a board from a state requested by the client
/// </summary>
public static class CreateBoardFromState
{
    public static async Task<Results<Created<BoardResponse>, ProblemHttpResult>> Handler(
        [FromServices] AppDbContext dbContext,
        [FromBody] [Validate] BoardRequest req
    )
    {
        try
        {
            var board = Board.Create(req.Cells);
            dbContext.Boards.Add(board);
            await dbContext.SaveChangesAsync();

            return TypedResults.Created($"boards/{board.Id}/current", new BoardResponse(board.Id));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: "Board creation failed", statusCode: 500);
        }
    }
}

public record BoardRequest(List<List<bool>> Cells);

public record BoardResponse(Guid Id);

public class BoardRequestValidator : AbstractValidator<BoardRequest>
{
    public BoardRequestValidator()
    {
        RuleFor(x => x.Cells)
            .NotEmpty().WithMessage("board should not be empty")
            .Must(c =>
            {
                var colCount = c[0].Count;
                return c.All(row => row.Count == colCount);
            }).WithMessage("All rows in the board must have the same number of columns.")
            .Must(c =>
            {
                if (c.Count > 1000 || c[0].Count > 1000) return false;
                return true;
            }).WithMessage("Max game board size is 1000");
    }
}