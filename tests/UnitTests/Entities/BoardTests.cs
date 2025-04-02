using System.Text.Json;
using ConwayGameOfLife.Entities;

namespace UnitTests.Entities;

public class BoardTests
{
    private void AssertBoardsEqual(
        List<List<bool>> expected,
        List<List<bool>> actual)
    {
        Assert.Equal(expected.Count, actual.Count);
        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], actual[i]);
        }
    }

    [Fact]
    public void Test_InvalidJson_ThrowsException()
    {
        var exception = Assert.Throws<ApplicationException>(() =>
            Board.Create(""));
        Assert.Equal("Board creation failed. Invalid state", exception.Message);
    }

    [Fact]
    public void Test_BlinkerOscillator()
    {
        var initialState =
            "[[false,true,false],[false,true,false],[false,true,false]]";
        var board = Board.Create(initialState);

        // In Conway's Game of Life, a vertical blinker transforms
        // into a horizontal blinker:
        // [ [false, false, false],
        //   [true,  true,  true],
        //   [false, false, false] ]
        var expected = new List<List<bool>>
        {
            new List<bool> { false, false, false },
            new List<bool> { true, true, true },
            new List<bool> { false, false, false }
        };

        var nextGen = board.GetNextGeneration();

        AssertBoardsEqual(expected, nextGen);
    }

    [Fact]
    public void Test_BlockStillLife()
    {
        // A 2x2 block is a still life.
        var initialState = "[[true,true],[true,true]]";
        var board = Board.Create(initialState);

        var expected = new List<List<bool>>
        {
            new List<bool> { true, true },
            new List<bool> { true, true }
        };

        var nextGen = board.GetNextGeneration();

        AssertBoardsEqual(expected, nextGen);
    }

    [Fact]
    public void Test_SingleLiveCellDies()
    {
        // A single live cell should die (underpopulation).
        string initialState = "[[true]]";
        var board = Board.Create(initialState);

        var expected = new List<List<bool>>
        {
            new List<bool> { false }
        };

        var nextGen = board.GetNextGeneration();

        AssertBoardsEqual(expected, nextGen);
    }

    [Fact]
    public void Test_StateCacheClearedOnStateChange()
    {
        // Start with a 2x2 block (still life).
        var initialState = "[[true,true],[true,true]]";
        var board = Board.Create(initialState);

        // Generation 1 should be identical.
        var gen1 = board.GetNextGeneration();
        var expectedGen1 = new List<List<bool>>
        {
            new List<bool> { true, true },
            new List<bool> { true, true }
        };
        AssertBoardsEqual(expectedGen1, gen1);

        // Generation 2 should also be the same.
        var gen2 = board.GetNextGeneration();
        var expectedGen2 = new List<List<bool>>
        {
            new List<bool> { true, true },
            new List<bool> { true, true }
        };
        AssertBoardsEqual(expectedGen2, gen2);
    }

    [Fact]
    public void Test_GetStepGeneration_BlinkerOscillator()
    {
        // Arrange: vertical blinker oscillator.
        // A vertical blinker:
        // [ [false, true, false],
        //   [false, true, false],
        //   [false, true, false] ]
        // Transforms into a horizontal blinker in one generation and
        // returns to the vertical form after two generations.
        var initialState =
            "[[false,true,false],[false,true,false],[false,true,false]]";
        var board = Board.Create(initialState);

        // Expected state after 2 steps is the same as the initial state.
        var expected = new List<List<bool>>
        {
            new List<bool> { false, true, false },
            new List<bool> { false, true, false },
            new List<bool> { false, true, false }
        };

        // Act: Update the board 2 generations.
        var result = board.GetStepGeneration(2);

        // Assert
        AssertBoardsEqual(expected, result);
    }

    [Fact]
    public void Test_CreateBoardFromCells()
    {
        // Arrange: create an initial configuration.
        var cells = new List<List<bool>>
        {
            new List<bool> { true, false },
            new List<bool> { false, true }
        };

        // Act: Create a board from list of cells.
        var board = Board.Create(cells);
        var retrievedCells = board.Cells;

        // Assert: The created board should match the input configuration.
        AssertBoardsEqual(cells, retrievedCells);
    }

    [Fact]
    public void Test_CellsPropertyReflectsStateChange()
    {
        // Arrange: start with a specific state.
        var initialState = "[[true,false],[false,true]]";
        var board = Board.Create(initialState);

        // Act: Advance one generation.
        var nextGen = board.GetNextGeneration();
        var cellsFromProperty = board.Cells;

        // Assert: The property should reflect the updated state.
        AssertBoardsEqual(nextGen, cellsFromProperty);
    }
}