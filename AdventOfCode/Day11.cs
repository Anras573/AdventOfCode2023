using System.Numerics;

namespace AdventOfCode;

public partial class Day11 : BaseDay
{
    private readonly string[] _input;

    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var map = StarMap.FromInput(_input);
        return map.SumOfShortestDistance();
    }
    
    private long Part02()
    {
        // Need to account for off by one error in the expansion
        var map = StarMap.FromInput(_input, 1_000_000 - 1);
        return map.SumOfShortestDistance();
    }

    private record StarMap(List<Vector2> Galaxies)
    {
        public static StarMap FromInput(string[] input, int expansion = 1)
        {
            var rowsToAdd = Enumerable
                .Range(0, input.Length)
                .Where(row => input[row].All(c => c == '.'))
                .ToArray();
            
            var colsToAdd = Enumerable
                .Range(0, input[0].Length)
                .Where(col => input.All(l => l[col] == '.'))
                .ToArray();

            var galaxies = new List<Vector2>();
            for (var row = 0; row < input.Length; row++)
            {
                var rowOffset = rowsToAdd.Count(r => r <= row) * expansion;
                for (var col = 0; col < input[0].Length; col++)
                {
                    if (input[row][col] != '#') continue;
                
                    var colOffset = colsToAdd.Count(c => c <= col) * expansion;
                    galaxies.Add(new Vector2(row + rowOffset, col + colOffset));
                }
            }

            return new StarMap(galaxies);
        }
        
        public long SumOfShortestDistance()
        {
            var pairs = Galaxies
                .SelectMany((g, i) => Galaxies.Skip(i + 1).Select(g2 => (g, g2)))
                .ToList();

            return pairs
                .Select(pair => pair.g2 - pair.g)
                .Aggregate(0L, (current, vectorTo) => current + ((int)Math.Abs(vectorTo.Y) + (int)Math.Abs(vectorTo.X)));
        }
    }
}