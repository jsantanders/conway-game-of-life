using System.Text.Json;
using ConwayGameOfLife.Database;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace ConwayGameOfLife.Features.Game.Handlers;

/// <summary>
/// Gets the final board state, this action mutates the current state of the board
/// </summary>
public static class GetBoardFinalState
{
    public static async Task<Results<Ok<NextStateBoardResponse>, ProblemHttpResult>> Handler(
        [FromServices] AppDbContext dbContext,
        [FromRoute] Guid id,
        [FromQuery] int? maxAttempts
    )
    {
        try
        {
            var maxIter = maxAttempts ?? 100;
            
            if (maxIter > 1_000_000)
            {
                maxIter = 1_000_000;
            }
            if (maxIter < 1)
            {
                return TypedResults.Problem(title: "Invalid number of iterations.",
                    statusCode: StatusCodes.Status400BadRequest);
            }

            var board = await dbContext.Boards.FindAsync(id);
            if (board == null)
            {
                Log.Information("Board {id} not found", id);
                return TypedResults.Problem(title: "Board not found.", statusCode: StatusCodes.Status404NotFound);
            }

            var concluded = false;
            var seenStates = new HashSet<string> { board.State };
            var finalState = new List<List<bool>>();

            for (var i = 0; i < maxIter; i++)
            {
                var nextGen = board.GetNextGeneration();
                var nextGenStr = JsonSerializer.Serialize(nextGen);

                if (!seenStates.Add(nextGenStr))
                {
                    finalState = nextGen;
                    concluded = true;
                    break;
                }
            }

            if (!concluded)
            {
                return TypedResults.Problem(
                    title: "Board did not reach a steady or cycling state after the maximum attempts.",
                    statusCode: StatusCodes.Status400BadRequest
                );
            }

            await dbContext.SaveChangesAsync();
            return TypedResults.Ok(new NextStateBoardResponse(finalState));
        }
        catch (ApplicationException e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: e.Message, statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception e)
        {
            Log.Error(e, e.Message);
            return TypedResults.Problem(title: "Getting board state failed",
                statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}