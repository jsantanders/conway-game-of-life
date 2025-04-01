using Carter;
using ConwayGameOfLife.Attributes;
using ConwayGameOfLife.Features.Game.Handlers;

namespace ConwayGameOfLife.Features.Game;

public class BoardModule() : CarterModule("boards")
{
    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var root = app.MapGroup("")
            .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);

        root.MapPost("", CreateBoardFromState.Handler);
        root.MapPost("/random", CreateBoardFromRandom.Handler);
        root.MapGet("/{id:guid}/next", GetBoardNextState.Handler);
        root.MapGet("/{id:guid}/step/{steps:int}", GetBoardStepState.Handler);
        root.MapGet("/{id:guid}/final", GetBoardFinalState.Handler);
    }
}