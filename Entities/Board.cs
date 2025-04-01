using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ConwayGameOfLife.Entities;

public class Board
{
    public required Guid Id { get; set; }
    private string _state;
    
    public required string State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                _cellsCache = null;
            }
        }
    }
    
    [NotMapped]
    private List<List<bool>>? _cellsCache;
    
    private List<List<bool>> GetCells()
    {
        if (_cellsCache == null)
        {
            _cellsCache = JsonSerializer.Deserialize<List<List<bool>>>(State)
                          ?? throw new ApplicationException("Board state is invalid.");
        }
        return _cellsCache;
    }
    
    public List<List<bool>> GetNextGeneration()
    {
        var cells = GetCells();
        
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
}