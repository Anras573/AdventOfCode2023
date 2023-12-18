using System.Numerics;

namespace AdventOfCode;

public partial class Day17 : BaseDay
{
    private readonly string[] _input;

    public Day17()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private (int[,] grid, int width, int height) ParseInput()
    {
        var (width, height) = (_input[0].Length, _input.Length);

        var grid = new int[width, height];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                grid[x, y] = int.Parse(_input[x][y].ToString());
            }
        }
        
        return (grid, width, height);
    }
    
    private long Part01()
    {
        var range = new Range(0, 3);
        var (grid, width, height) = ParseInput();

        return CalculateHeatLoss(grid, width, height, range);
    }
    
    private long Part02()
    {
        var range = new Range(4, 10);
        var (grid, width, height) = ParseInput();

        return CalculateHeatLoss(grid, width, height, range);
    }

    private static long CalculateHeatLoss(int[,] grid, int width, int height, Range range)
    {
        var start = new Vector2(0, 0);
        var end = new Vector2(width - 1, height - 1);
        
        var queue = new PriorityQueue<State, int>();
        var cost = new Dictionary<State, int>();

        foreach (var initialState in InitialStates(start))
        {
            cost[initialState] = 0;
            queue.Enqueue(initialState, 0);
        }
        
        while (queue.Count > 0)
        {
            var state= queue.Dequeue();
            
            if (state.Position == end && state.Steps > range.Start.Value)
                return cost[state];

            foreach (var adjacent in AdjacentStates(state, range))
            {
                if (adjacent.Position.X < 0
                    || adjacent.Position.X >= width
                    || adjacent.Position.Y < 0
                    || adjacent.Position.Y >= height)
                    continue;
                
                var adjacentCost = cost.GetValueOrDefault(adjacent, int.MaxValue / 2);
                
                if (cost[state] + grid[(int)adjacent.Position.X, (int)adjacent.Position.Y] < adjacentCost)
                {
                    cost[adjacent] = cost[state] + grid[(int)adjacent.Position.X, (int)adjacent.Position.Y];
                    queue.Enqueue(adjacent, cost[adjacent]);
                }
            }
        }

        return -1;
    }
    
    private static IEnumerable<State> InitialStates(Vector2 origin)
    {
        yield return new State(origin, Direction.Right, 0);
        yield return new State(origin, Direction.Down, 0);
    }
    
    private static IEnumerable<State> AdjacentStates(State current, Range range)
    {
        if (current.Steps >= range.Start.Value)
        {
            var left = RotateLeft(current.Direction);
            var leftPos = NextPosition(current.Position, left);
            yield return new State(leftPos, left, 1);
                
            var right = RotateRight(current.Direction);
            var rightPos = NextPosition(current.Position, right);
            yield return new State(rightPos, right, 1);
        }
            
        if (current.Steps < range.End.Value)
        {
            var nextPosition = NextPosition(current.Position, current.Direction);
            yield return new State(nextPosition, current.Direction, current.Steps + 1);
        }
    }
    
    private static Vector2 NextPosition(Vector2 position, Direction direction) {
        return direction switch
        {
            Direction.Right => position + new Vector2(1, 0),
            Direction.Left => position + new Vector2(-1, 0),
            Direction.Up => position + new Vector2(0, -1),
            Direction.Down => position + new Vector2(0, 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
        
    private static Direction RotateLeft(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Left,
            Direction.Left => Direction.Down,
            Direction.Down => Direction.Right,
            Direction.Right => Direction.Up,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private static Direction RotateRight(Direction direction)
    {
        return direction switch
        {
            Direction.Up => Direction.Right,
            Direction.Right => Direction.Down,
            Direction.Down => Direction.Left,
            Direction.Left => Direction.Up,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    private record State(Vector2 Position, Direction Direction, int Steps);
    private enum Direction { Up, Left, Down, Right }
}