using System.Numerics;

namespace AdventOfCode;

public partial class Day14 : BaseDay
{
    private readonly string[] _input;

    public Day14()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var (map, width, height) = ParseInput();
        
        MoveRocksNorth(map, width, height);

        return CalculateLoad(map, height);
    }
    
    private long Part02()
    {
        const long iterations = 1_000_000_000;
        var (map, width, height) = ParseInput();
        
        var cache = new Dictionary<string, long>();
        
        // First move rocks north, then west, then south, then east
        for (long i = 1; i <= iterations; i++)
        {
            var mapString = string.Join("", map.Select(kvp => kvp.Value));
            
            if (cache.TryGetValue(mapString, out var value))
            {
                var loopLength = i - value;
                var remainingIterations = iterations - i;
                var remainingIterationsModulo = remainingIterations % loopLength;
                
                for (var j = 0; j <= remainingIterationsModulo; j++)
                    Cycle();
                
                return CalculateLoad(map, height);
            }
            
            cache.Add(mapString, i);
            
            Cycle();
        }
        
        return CalculateLoad(map, height);

        void Cycle()
        {
            MoveRocksNorth(map, width, height);
            MoveRocksWest(map, width, height);
            MoveRocksSouth(map, width, height);
            MoveRocksEast(map, width, height);
        }
    }

    private (Dictionary<Vector2, char> map, int width, int height) ParseInput()
    {
        var (width, height) = (_input[0].Length, _input.Length);
        var map = new Dictionary<Vector2, char>();

        foreach (var (row, y) in _input.Select((row, y) => (row, y)))
        {
            foreach (var (c, x) in row.Select((c, x) => (c, x)))
            {
                map.Add(new Vector2(x, y), c);
            }
        }
        
        return (map, width, height);
    }

    // Move all rocks (o) north (up) until they hit a wall (#) or another rock (o)
    private static void MoveRocksNorth(Dictionary<Vector2, char> map, int width, int height)
    {
        for (var y = 1; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (map[new Vector2(x, y)] is not 'O') continue;
                
                var pos = new Vector2(x, y);
                var newPos = new Vector2(x, y - 1);

                if (map[newPos] is not '.')
                    continue;
                
                while(newPos.Y > 0 && map[new Vector2(x, newPos.Y - 1)] is '.')
                    newPos.Y--;

                map[newPos] = 'O';
                map[pos] = '.';
            }
        }
    }

    // Move all rocks (o) west (left) until they hit a wall (#) or another rock (o)
    private static void MoveRocksWest(Dictionary<Vector2, char> map, int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = 1; x < width; x++)
            {
                if (map[new Vector2(x, y)] is not 'O') continue;
                
                var pos = new Vector2(x, y);
                var newPos = new Vector2(x - 1, y);

                if (map[newPos] is not '.')
                    continue;
                
                while(newPos.X > 0 && map[new Vector2(newPos.X - 1, y)] is '.')
                    newPos.X--;

                map[newPos] = 'O';
                map[pos] = '.';
            }
        }
    }

    // Move all rocks (o) south (down) until they hit a wall (#) or another rock (o)
    private static void MoveRocksSouth(Dictionary<Vector2, char> map, int width, int height)
    {
        for (var y = height - 2; y >= 0; y--)
        {
            for (var x = 0; x < width; x++)
            {
                if (map[new Vector2(x, y)] is not 'O') continue;
                
                var pos = new Vector2(x, y);
                var newPos = new Vector2(x, y + 1);

                if (map[newPos] is not '.')
                    continue;
                
                while(newPos.Y < height - 1 && map[new Vector2(x, newPos.Y + 1)] is '.')
                    newPos.Y++;

                map[newPos] = 'O';
                map[pos] = '.';
            }
        }
    }

    // Move all rocks (o) east (right) until they hit a wall (#) or another rock (o)
    private static void MoveRocksEast(Dictionary<Vector2, char> map, int width, int height)
    {
        for (var y = 0; y < height; y++)
        {
            for (var x = width - 2; x >= 0; x--)
            {
                if (map[new Vector2(x, y)] is not 'O') continue;
                
                var pos = new Vector2(x, y);
                var newPos = new Vector2(x + 1, y);

                if (map[newPos] is not '.')
                    continue;
                
                while(newPos.X < width - 1 && map[new Vector2(newPos.X + 1, y)] is '.')
                    newPos.X++;

                map[newPos] = 'O';
                map[pos] = '.';
            }
        }
    }
    
    private static long CalculateLoad(Dictionary<Vector2, char> map, int height)
    {
        return map
            .Where(kvp => kvp.Value == 'O')
            .Aggregate(0L, (current, rock) => current + (height - (int)rock.Key.Y));
    }
}