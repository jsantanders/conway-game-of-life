using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ConwayGameOfLife.Entities;

/// <summary>
/// Main class for the game board
/// </summary>
public class Board
{
    private Board()
    {
        //EF constructor 
    }


    /// <summary>
    /// Gets or sets the board id
    /// </summary>
    public Guid Id { get; private set; }

    private string _state;

    /// <summary>
    /// Gets or sets the current state of the board
    /// </summary>
    public string State
    {
        get => _state;
        private set
        {
            _state = value;
            _cellsCache = null;
        }
    }

    [NotMapped] public List<List<bool>> Cells => GetCells();

    [NotMapped] private List<List<bool>>? _cellsCache;

    private List<List<bool>> GetCells()
    {
        if (_cellsCache == null)
        {
            try
            {
                _cellsCache = JsonSerializer.Deserialize<List<List<bool>>>(State)
                              ?? throw new ApplicationException("Board state is invalid.");
            }
            catch (Exception e)
            {
                throw new ApplicationException("Board state is invalid.");
            }
        }

        return _cellsCache;
    }

    private static List<List<bool>> GetNextCells(List<List<bool>> cells)
    {
        var rows = cells.Count;
        var cols = cells[0].Count;
        var next = new List<List<bool>>(rows);

        for (var i = 0; i < rows; i++)
        {
            var rowNext = new List<bool>(cols);
            for (var j = 0; j < cols; j++)
            {
                var liveNeighbors = 0;
                for (var di = -1; di <= 1; di++)
                {
                    for (int dj = -1; dj <= 1; dj++)
                    {
                        if (di == 0 && dj == 0)
                        {
                            continue;
                        }

                        var ni = i + di;
                        var nj = j + dj;
                        if (ni >= 0 && ni < rows && nj >= 0 && nj < cols)
                        {
                            if (cells[ni][nj])
                            {
                                liveNeighbors++;
                            }
                        }
                    }
                }

                var cellAlive = cells[i][j];
                if (cellAlive &&
                    (liveNeighbors < 2 || liveNeighbors > 3))
                {
                    rowNext.Add(false);
                }
                else if (!cellAlive && liveNeighbors == 3)
                {
                    rowNext.Add(true);
                }
                else
                {
                    rowNext.Add(cellAlive);
                }
            }

            next.Add(rowNext);
        }

        return next;
    }

    /// <summary>
    /// Calculate the next generation of the current state of the board
    /// </summary>
    /// <returns>The next state of the board</returns>
    public List<List<bool>> GetNextGeneration()
    {
        var cells = GetCells();
        var next = GetNextCells(cells);

        State = JsonSerializer.Serialize(next);
        return next;
    }

    public List<List<bool>> GetStepGeneration(int steps)
    {
        var cells = GetCells();
        for (var i = 0; i < steps; i++)
        {
            cells = GetNextCells(cells);
        }

        State = JsonSerializer.Serialize(cells);
        return cells;
    }

    /// <summary>
    /// Creates a new board based on an initial state
    /// </summary>
    /// <param name="initialState">The initial state of the board</param>
    /// <returns>A new board with the initial state</returns>
    /// <exception cref="ApplicationException">A ApplicationException is thrown is the initial state is no valid</exception>
    public static Board Create(string initialState)
    {
        try
        {
            var cells = JsonSerializer.Deserialize<List<List<bool>>>(initialState) ??
                        throw new ApplicationException("Board state is invalid.");
        }
        catch (Exception e)
        {
            throw new ApplicationException("Board creation failed. Invalid state", e);
        }

        return new Board()
        {
            Id = Guid.NewGuid(),
            State = initialState
        };
    }

    /// <summary>
    /// Create a new board based on initial cells
    /// </summary>
    /// <param name="cells">initial cells of the board</param>
    /// <returns>A new board with initial cells</returns>
    public static Board Create(List<List<bool>> cells)
    {
        return new Board()
        {
            Id = Guid.NewGuid(),
            State = JsonSerializer.Serialize(cells)
        };
    }
}