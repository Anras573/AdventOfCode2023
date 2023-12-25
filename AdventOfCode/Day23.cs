using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day23 : BaseDay
{
    private readonly string[] _input;

    public Day23()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static readonly Complex Left = -Complex.One;
    private static readonly Complex Right = Complex.One;
    private static readonly Complex[] Directions = new[] { Up, Right, Down, Left };
    private static readonly Dictionary<char, Complex[]> Exits = new()
    {
        ['<'] = [Left],
        ['>'] = [Right],
        ['^'] = [Up],
        ['v'] = [Down],
        ['.'] = Directions,
        ['#'] = []
    };
    
    private long Part01()
    {
        var map = ParseMap();
        
        var (nodes, edges) = ParseGraph(map);

        var (start, goal) = (nodes[0], nodes[^1]);

        var cache = new Dictionary<(Node, long), int>();
        
        return LongestPath(start, goal, 0, edges, cache);
    }
    
    private long Part02()
    {
        var map = ParseMapWithoutSlopes();
        
        var (nodes, edges) = ParseGraph(map);

        var (start, goal) = (nodes[0], nodes[^1]);

        var cache = new Dictionary<(Node, long), int>();
        
        return LongestPath(start, goal, 0, edges, cache);
    }
    
    private Dictionary<Complex, char> ParseMap()
    {
        var map = new Dictionary<Complex, char>();

        foreach (var row in Enumerable.Range(0, _input.Length))
        {
            foreach (var col in Enumerable.Range(0, _input[0].Length))
            {
                var pos = new Complex(col, row);
                map[pos] = _input[row][col];
            }
        }

        return map;
    }
    
    private Dictionary<Complex, char> ParseMapWithoutSlopes()
    {
        var map = new Dictionary<Complex, char>();

        foreach (var row in Enumerable.Range(0, _input.Length))
        {
            var line = GetSlopesRegex().Replace(_input[row], ".");
            foreach (var col in Enumerable.Range(0, _input[0].Length))
            {
                var pos = new Complex(col, row);
                map[pos] = line[col];
            }
        }

        return map;
    }

    private static (Node[] nodes, List<Edge> edges) ParseGraph(Dictionary<Complex, char> dictionary)
    {
        {
            var nodePositions = dictionary
                .Keys
                .OrderBy(k => k.Imaginary)
                .ThenBy(k => k.Real)
                .Where(pos => IsFree(dictionary, pos) && !IsRoad(dictionary, pos))
                .ToArray();

            var nodes = Enumerable
                .Range(0, nodePositions.Length)
                .Select(i => new Node(1L << i))
                .ToArray();

            var edges = new List<Edge>();

            for (var i = 0; i < nodePositions.Length; i++)
            {
                for (var j = 0; j < nodePositions.Length; j++)
                {
                    if (i == j) continue;
                
                    var distance = Distance(dictionary, nodePositions[i], nodePositions[j]);
                    if (distance > 0)
                        edges.Add(new Edge(nodes[i], nodes[j], distance));
                }
            }

            return (nodes, edges);
        }

        bool IsFree(Dictionary<Complex, char> map, Complex pos)
        {
            return map.ContainsKey(pos) && map[pos] != '#';
        }

        bool IsRoad(Dictionary<Complex, char> map, Complex pos)
        {
            return IsFree(map, pos)
                   && Directions
                       .Count(dir => IsFree(map, pos + dir)) == 2;
        }

        int Distance(Dictionary<Complex, char> map, Complex crossRoadA, Complex crossRoadB)
        {
            var visited = new HashSet<Complex> { crossRoadA };
            
            var queue = new Queue<(Complex pos, int distance)>();
            queue.Enqueue((crossRoadA, 0));
            
            while (queue.Count > 0)
            {
                var (pos, distance) = queue.Dequeue();

                foreach (var dir in Exits[map[pos]])
                {
                    var posT = pos + dir;
                    if (posT == crossRoadB) return distance + 1;

                    if (!IsRoad(map, posT) || !visited.Add(posT)) continue;
                    
                    queue.Enqueue((posT, distance + 1));
                }
                
                if (pos == crossRoadB) return distance;
                
                if (!visited.Add(pos)) continue;
                
                foreach (var dir in Directions)
                {
                    var newPos = pos + dir;
                    if (IsFree(map, newPos))
                    {
                        queue.Enqueue((newPos, distance + 1));
                    }
                }
            }

            return -1;
        }
    }

    private static int LongestPath(Node node, Node goal, long visited, List<Edge> edges, Dictionary<(Node, long), int> cache)
    {
        if (node == goal)
            return 0;
            
        if ((visited & node.Value) != 0)
            return int.MinValue;
            
        var key = (node, visited);

        if (cache.TryGetValue(key, out var value))
            return value;
            
        value = edges
            .Where(e => e.Start == node)
            .Select(e => e.Distance + LongestPath(e.End, goal, visited | node.Value, edges, cache))
            .Max();
        cache[key] = value;

        return value;
    }
    
    private record Node(long Value);

    private record Edge(Node Start, Node End, int Distance);

    [GeneratedRegex("\\^|v|<|>")]
    private static partial Regex GetSlopesRegex();
}