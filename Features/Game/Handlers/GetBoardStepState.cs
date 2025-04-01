using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

public class GetBoardStepState
{
    public static async Task<Results<Ok<NextStateBoardResponse>, ProblemHttpResult>> Handler(
        [FromServices] AppDbContext dbContext,
        [FromRoute] Guid id,
        [FromRoute] int steps
    )
    {
        try
        {
            var board = await dbContext.Boards.FindAsync(id);
            if (board == null)
            {
                return TypedResults.Problem(title: "Board not found.", statusCode: StatusCodes.Status404NotFound);
            }
            if (steps <= 0)
            {
                return TypedResults.Problem(title: "Invalid step value", statusCode: StatusCodes.Status400BadRequest);
            }

            var nextCells = new List<List<bool>>();
            for (var i = 0; i < steps; i++)
            {
                nextCells = board.GetNextGeneration();
            }

            board.State = JsonSerializer.Serialize(nextCells);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(new NextStateBoardResponse(nextCells));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: "Getting board state failed",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}