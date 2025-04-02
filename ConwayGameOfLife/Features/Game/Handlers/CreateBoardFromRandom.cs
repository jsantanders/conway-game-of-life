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
/// Creates a new board at random with a size requested by the client
/// </summary>
public static class CreateBoardFromRandom
{
    public static async Task<Results<Created<BoardResponse>, ProblemHttpResult>> Handler(
        [FromServices] AppDbContext dbContext,
        [FromBody] [Validate] RandomBoardRequest req
    )
    {
        try
        {
            var random = new Random();

            var cells = new List<List<bool>>();
            for (var i = 0; i < req.Rows; i++)
            {
                var row = new List<bool>();
                for (var j = 0; j < req.Columns; j++)
                {
                    row.Add(random.NextDouble() < 0.5);
                }

                cells.Add(row);
            }

            var board = Board.Create(cells);

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

public record RandomBoardRequest(int Rows, int Columns);

public class RandomBoardRequestValidator : AbstractValidator<RandomBoardRequest>
{
    public RandomBoardRequestValidator()
    {
        RuleFor(x => x.Rows)
            .LessThan(1000).WithMessage("Max rows limit is 1000")
            .NotEmpty().WithMessage("Rows cannot be empty");
        RuleFor(x => x.Columns)
            .LessThan(1000).WithMessage("Max columns limit is 1000")
            .NotEmpty().WithMessage("Columns cannot be empty");
    }
}