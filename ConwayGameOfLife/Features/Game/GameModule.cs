using Carter;
using ConwayGameOfLife.Attributes;
using ConwayGameOfLife.Features.Game.Handlers;

namespace ConwayGameOfLife.Features.Game;

/// <summary>
/// Carter module for the board game
/// </summary>
public class GameModule() : CarterModule()
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var root = app.MapGroup("boards")
            .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory)
            .WithTags("Game board");

        root.MapPost("", CreateBoardFromState.Handler);
        root.MapPost("/random", CreateBoardFromRandom.Handler);
        root.MapGet("/{id:guid}/current", GetBoardCurrentState.Handler);
        root.MapPost("/{id:guid}/next", GetBoardNextState.Handler);
        root.MapPost("/{id:guid}/steps/{steps:int}", GetBoardStepState.Handler);
        root.MapPost("/{id:guid}/final", GetBoardFinalState.Handler);
    }
}