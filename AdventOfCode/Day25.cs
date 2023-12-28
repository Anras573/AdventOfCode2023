namespace AdventOfCode;

public partial class Day25 : BaseDay
{
    private readonly string[] _input;

    public Day25()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        // Not a huge fan of using the Karger's Algorithm to solve this one. https://en.wikipedia.org/wiki/Karger%27s_algorithm
        var random = new Random();
        var vertices = new List<string>();
        var edges = new List<(string, string)>();
        
        foreach (var line in _input)
        {
            var connections = line.Replace(":", string.Empty).Split(" ");
            var root = connections[0];
            
            if (!vertices.Contains(root))
                vertices.Add(root);

            foreach (var con in connections.Skip(1))
            {
                if (!vertices.Contains(con))
                    vertices.Add(con);
                
                if (edges.Contains((root, con)) || edges.Contains((con, root)))
                    continue;
                
                edges.Add((root, con));
            }
        }

        var subsets = new List<List<string>>();

        while (Cuts(subsets, edges) != 3)
        {
            subsets = [];
            subsets.AddRange(vertices.Select(vertex => (List<string>) [vertex]));

            int i;

            while (subsets.Count > 2)
            {
                i = random.Next() % edges.Count;
                
                var subset1 = subsets.First(s => s.Contains(edges[i].Item1));
                var subset2 = subsets.First(s => s.Contains(edges[i].Item2));
                
                if (subset1 == subset2)
                    continue;
                
                subsets.Remove(subset2);
                subset1.AddRange(subset2);
            }

        }

        return subsets.Aggregate(1, (a, b) => a * b.Count);

        int Cuts(List<List<string>> subsets, List<(string, string)> edges)
        {
            if (subsets.Count == 0)
                return 0;
            
            var cuts = 0;

            foreach (var edge in edges)
            {
                var s1 = subsets.First(s => s.Contains(edge.Item1));
                var s2 = subsets.First(s => s.Contains(edge.Item2));
                
                if (s1 != s2)
                    cuts++;
            }
            
            return cuts;
        }
    }

    private long Part02()
    {
        return 0L;
    }
}