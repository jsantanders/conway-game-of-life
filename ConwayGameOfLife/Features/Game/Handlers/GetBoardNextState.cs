using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

/// <summary>
/// Gets the next board state, this action mutates the current state of the board
/// </summary>
public static class GetBoardNextState
{
    public static async Task<Results<Ok<NextStateBoardResponse>, ProblemHttpResult>> Handler(
        [FromServices] AppDbContext dbContext,
        [FromRoute] Guid id
    )
    {
        try
        {
            var board = await dbContext.Boards.FindAsync(id);
            if (board == null)
            {
                Log.Information("Board {id} not found", id);
                return TypedResults.Problem(title: "Board not found.", statusCode: StatusCodes.Status404NotFound);
            }

            var nextCells = board.GetNextGeneration();
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

public record NextStateBoardResponse(List<List<bool>> Cells);