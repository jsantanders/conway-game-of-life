using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

public class GetBoardCurrentState
{
    public static async Task<Results<Ok<CurrentStateBoardResponse>, ProblemHttpResult>> Handler(
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

            return TypedResults.Ok(new CurrentStateBoardResponse(board.Cells));
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: "Getting board state failed", statusCode: 500);
        }
    }
}

public record CurrentStateBoardResponse(List<List<bool>> Cells);