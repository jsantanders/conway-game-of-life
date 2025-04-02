# Conway's Game of Life

This project implements Conway’s Game of Life using .NET with a focus on simulating board evolution and detecting cycles.
The board state is stored in a JSON format in a database, and the application provides an API endpoint to retrieve the board's states.

## Running the Code

1. Install [.NET 9 SDK](https://dotnet.microsoft.com/download).
2. Build the solution with `dotnet build`.
3. Run the project with `dotnet run`.
4. Use the endpoint:
   `POST /boards/`

### Architecure (Vertical Slices Architecture)
In this architecture, each "slice" contains all layers related to a specific feature. For example, the board slice includes:
- **Domain Logic:** The `Board` entity (with logic for generating the next generation, caching, etc.).
- **Presentation:** The endpoints (e.g., `POST /boards`, `GET /boards/{id}/next`).
- **Persistence:** EF Core support (using SQLite) embedded into the same slice.

## Assumptions

- The board is finite.
- The state is serialized as a 2D boolean JSON array.
- A cycle is detected when a state recurs.

The API uses **Entity Framework Core** with a **SQLite** database for persistence,