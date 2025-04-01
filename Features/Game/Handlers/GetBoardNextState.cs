using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

public class GetBoardNextState
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
                return TypedResults.Problem(title: "Board not found.", statusCode: StatusCodes.Status404NotFound);
            }

            var nextCells = board.GetNextGeneration();
            board.State = JsonSerializer.Serialize(nextCells);
            await dbContext.SaveChangesAsync();

            return TypedResults.Ok(new NextStateBoardResponse(nextCells));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: "Getting board state failed", statusCode: 500);
        }
    }
}

public record NextStateBoardResponse(List<List<bool>> Cells);