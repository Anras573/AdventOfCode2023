using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day10 : BaseDay
{
    private readonly string[] _input;
    
    private readonly Dictionary<(char pipe, CompassDirection inFrom), CompassDirection> _pipeDirChange = new()
    {
        {('|', CompassDirection.N), CompassDirection.S },
        {('|', CompassDirection.S), CompassDirection.N },
        {('-', CompassDirection.E), CompassDirection.W },
        {('-', CompassDirection.W), CompassDirection.E },
        {('L', CompassDirection.N), CompassDirection.E },
        {('L', CompassDirection.E), CompassDirection.N },
        {('J', CompassDirection.N), CompassDirection.W },
        {('J', CompassDirection.W), CompassDirection.N },
        {('7', CompassDirection.S), CompassDirection.W },
        {('7', CompassDirection.W), CompassDirection.S },
        {('F', CompassDirection.E), CompassDirection.S },
        {('F', CompassDirection.S), CompassDirection.E },
    };
    
    private HashSet<Vector2> _visited = new();

    public Day10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var (map, maxX, maxY) = GenerateMap();
        var start = map.FirstOrDefault(m => m.Value == 'S').Key;

        var path = string.Empty;
        foreach (var pipe in "|-LJ7F")
        {
            var (isLoop, possiblePath) = CheckLoop(pipe);
            
            if (!isLoop) continue;
            
            path = possiblePath;
            break;
        }
        
        return path.Length / 2;

        (bool isLoop, string path) CheckLoop(char replaceS)
        {
            var sb = new StringBuilder("S");
            var isLoop = false;

            var cur = start;
            var curDir = replaceS switch
            {
                '|' => CompassDirection.N,
                '-' => CompassDirection.W,
                'L' => CompassDirection.N,
                'J' => CompassDirection.N,
                '7' => CompassDirection.W,
                'F' => CompassDirection.S,
                _ => throw new ArgumentOutOfRangeException(nameof(replaceS), $"{replaceS} is not a valid pipe")
            };

            var tempVisited = new HashSet<Vector2> { start };

            while (map.TryGetValue(MoveDirection(cur, curDir), out var nextPipe)
                   && (nextPipe is 'S' || _pipeDirChange.ContainsKey((nextPipe, Flip(curDir)))))
            {
                if (nextPipe == 'S')
                {
                    isLoop = true;
                    _visited = tempVisited;
                    break;
                }

                sb.Append(nextPipe);
                cur = MoveDirection(cur, curDir);
                curDir = _pipeDirChange[(nextPipe, Flip(curDir))];
                tempVisited.Add(cur);
            }

            return (isLoop, sb.ToString());
        }

        Vector2 MoveDirection(Vector2 original, CompassDirection direction)
        {
            const int distance = 1;
            return direction switch
            {
                CompassDirection.N => original + new Vector2(0, -distance),
                CompassDirection.E => original + new Vector2(distance, 0),
                CompassDirection.S => original + new Vector2(0, distance),
                CompassDirection.W => original + new Vector2(-distance, 0),
                _ => throw new ArgumentException("Direction is not valid", nameof(direction))
            };
        }
    }
    private long Part02()
    {
        var (map, maxX, maxY) = GenerateMap();
        var keys = map.Keys.ToList();
        
        foreach(var k in keys.Where(x => !_visited.Contains(x)))
        {
            map[k] = '.';
        }
        
        var cleanedMap = new List<string>();
        for (var y = 0; y <= maxY; y++)
        {
            var sb = new StringBuilder();
            for (var x = 0; x <= maxX; x++)
            {
                sb.Append(map[new Vector2(x, y)]);
            }

            var input = Regex.Replace(sb.ToString(), "F-*7|L-*J", string.Empty);
            
            cleanedMap.Add(Regex.Replace(input, "F-*J|L-*7", "|"));
        }
        
        var ans = 0;

        foreach (var l in cleanedMap)
        {
            var parity = 0;
            foreach(var c in l)
            {
                switch (c)
                {
                    case '|':
                        parity++;
                        break;
                    case '.' when parity % 2 == 1:
                        ans++;
                        break;
                }
            }
        }

        return ans;
    }
    
    private (Dictionary<Vector2, char> map, int maxX, int maxY) GenerateMap()
    {
        var maxX = 0;
        var maxY = _input.Length - 1;
        var map = new Dictionary<Vector2, char>();
            
        for (var y = 0; y < _input.Length; y++)
        {
            var line = _input[y];
            maxX = Math.Max(maxX, line.Length - 1);
                
            for (var x = 0; x < line.Length; x++)
            {
                map.Add(new Vector2(x, y), line[x]);
            }
        }
            
        return (map, maxX, maxY);
    }
    
    private enum CompassDirection { N, E, S, W }
    
    private static CompassDirection Flip(CompassDirection dir)
    {
        return dir switch
        {
            CompassDirection.N => CompassDirection.S,
            CompassDirection.S => CompassDirection.N,
            CompassDirection.E => CompassDirection.W,
            CompassDirection.W => CompassDirection.E,
            _ => throw new ArgumentException("Direction is not valid", nameof(dir))
        } ;
    }
}