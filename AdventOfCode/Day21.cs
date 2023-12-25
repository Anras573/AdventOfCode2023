using System.Numerics;

namespace AdventOfCode;

public partial class Day21 : BaseDay
{
    private readonly string[] _input;

    public Day21()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private long Part01()
    {
        var directions = new List<Vector2> { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) };
        
        var startPosition = Enumerable
            .Range(0, _input.Length)
            .SelectMany(row => Enumerable
                .Range(0, _input[row].Length)
                .Where(col => _input[row][col] == 'S')
                .Select(col => new Vector2(col, row)))
            .Single();
        
        var visited = new HashSet<Vector2> { startPosition };
        
        const int steps = 64;

        foreach (var _ in Enumerable.Range(0, steps))
        {
            var next = new HashSet<Vector2>();
            
            var destinations = visited
                .SelectMany(vector2 => directions
                    .Select(direction => vector2 + direction)
                    .Where(destination => _input[(int) destination.Y][(int) destination.X] != '#'));
            
            foreach (var destination in destinations)
            {
                next.Add(destination);
            }
            
            visited = next;
        }
        
        return visited.Count;
    }
    
    private long Part02()
    {
        const int totalSteps = 26501365;
        var (width, height) = (_input[0].Length, _input.Length);
        
        // assume input is square
        if (width != height)
            throw new Exception("Input is not square");
        
        var startPosition = Enumerable
            .Range(0, _input.Length)
            .SelectMany(row => Enumerable
                .Range(0, _input[row].Length)
                .Where(col => _input[row][col] == 'S')
                .Select(col => new Vector2(col, row)))
            .Single();
        
        var directions = new List<Vector2> { new(0, -1), new(1, 0), new(0, 1), new(-1, 0) };
        var stepsTaken = 0;
        var visited = new HashSet<Vector2> { startPosition };
        var sequences = new List<(int steps, int visitedCount)>();
        var trips = 0;
        
        for (; trips < 3; trips++)
        {
            for (; stepsTaken < trips * width + startPosition.X; stepsTaken++)
            {
                var nextOpen = visited
                    .SelectMany(position => directions
                        .Select(dir => position + dir)
                        .Where(dest => _input[Modulo((int)dest.Y, height)][Modulo((int)dest.X, width)] != '#'))
                    .ToHashSet();
                
                visited = nextOpen;
            }
            
            sequences.Add((stepsTaken, visited.Count));
        }
        
        var result = 0L;
        
        for (var i = 0; i < trips; i++)
        {
            long term = sequences[i].visitedCount;
            for (var j = 0; j < trips; j++)
            {
                if (i == j)
                    continue;
                
                term *= (totalSteps - sequences[j].steps) / (sequences[i].steps - sequences[j].steps);
            }
            
            result += term;
        }
        
        return result;
        
        int Modulo(int num, int mod)
        {
            return (num % mod + mod) % mod;
        }
    }
}