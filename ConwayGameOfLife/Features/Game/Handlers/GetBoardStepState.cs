using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

/// <summary>
/// Gets N state of the board based on the client request, this action mutates the state of the board
/// </summary>
public static class GetBoardStepState
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

            if (steps > 1_000_000)
            {
                return TypedResults.Problem(title: "The limit of steps is 100000",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var nextCells = board.GetStepGeneration(steps);
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