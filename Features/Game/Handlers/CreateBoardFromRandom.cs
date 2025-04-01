using System.Text.Json;
using ConwayGameOfLife.Attributes;
using ConwayGameOfLife.Database;
using ConwayGameOfLife.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

public static class CreateBoardFromRandom
{
    public static async Task<Results<Ok<BoardResponse>, ProblemHttpResult>> Handler(
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

            var board = new Board
            {
                Id = Guid.NewGuid(),
                State = JsonSerializer.Serialize(cells)
            };

            dbContext.Boards.Add(board);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(new BoardResponse(board.Id));
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
        RuleFor(x => x.Rows).NotEmpty().WithMessage("Rows cannot be empty");
        RuleFor(x => x.Columns).NotEmpty().WithMessage("Columns cannot be empty");
    }
}