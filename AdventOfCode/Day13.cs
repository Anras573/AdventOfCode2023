using System.Text;

namespace AdventOfCode;

public partial class Day13 : BaseDay
{
    private readonly string[] _input;
    private readonly Dictionary<int, Reflection> _initialMapReflections = new();

    public Day13()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var sum = 0L;

        var maps = ParseMaps();
        
        foreach (var (map, index) in maps.Select((m, i) => (m, i)))
        {
            sum += FindReflection(map, index).IndexValue;
        }
        
        return sum;
    }

    private List<Map> ParseMaps()
    {
        var maps = new List<Map>();
        var rows = new List<string>();
        string[] columns;

        foreach (var line in _input)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                columns = new string[rows[0].Length];
                
                for (var i = 0; i < rows[0].Length; i++)
                {
                    columns[i] = string.Join("", rows.Select(r => r[i]).ToList());
                }
                
                maps.Add(new Map(rows.ToArray(), columns));
                
                rows = new List<string>();
                continue;
            }
            
            rows.Add(line);
        }
        
        columns = new string[rows[0].Length];
        
        for (var i = 0; i < rows[0].Length; i++)
        {
            columns[i] = string.Join("", rows.Select(r => r[i]).ToList());
        }
        
        maps.Add(new Map(rows.ToArray(), columns));
        
        return maps;
    }

    private ReflectionResult FindReflection(Map map, int mapIndex)
    {
        for (var i = 1; i < map.Rows.Length; i++)
        {
            if (map.Rows.Take(i).Reverse().Zip(map.Rows.Skip(i)).Any(x => x.First != x.Second)) continue;
                
            if (_initialMapReflections.TryGetValue(mapIndex, out var reflection))
            {
                if (reflection.IsRow && reflection.Index == i)
                    continue;
            }
                    
            _initialMapReflections[mapIndex] = new Reflection(i, true);
            return new ReflectionResult(true, i * 100);
        }
            
        for (var i = 1; i < map.Columns.Length; i++)
        {
            if (map.Columns.Take(i).Reverse().Zip(map.Columns.Skip(i)).Any(x => x.First != x.Second)) continue;
                
            if (_initialMapReflections.TryGetValue(mapIndex, out var reflection))
            {
                if (!reflection.IsRow && reflection.Index == i)
                    continue;
            }
                    
            _initialMapReflections[mapIndex] = new Reflection(i, false);
            return new ReflectionResult(true, i);
        }
            
        return new ReflectionResult(false, 0);
    }
    
    private long Part02()
    {
        var sum = 0L;

        var maps = ParseMaps();

        foreach (var (map, index) in maps.Select((m, i) => (m, i)))
        {
            var block = map.AsBlock();
            foreach (var (c, i) in block.Select((c, i) => (c, i)))
            {
                if(!".#".Contains(c)) continue;
                
                var sb = new StringBuilder(block);
                sb[i] = sb[i] == '.' ? '#' : '.';

                var newMap = Map.FromBlock(sb.ToString());
                var reflectionResult = FindReflection(newMap, index);
                
                if (!reflectionResult.IsReflection) continue;
                
                sum += reflectionResult.IndexValue;
                break;
            }
        }
        
        return sum; 
    }

    private record Map(string[] Rows, string[] Columns)
    {
        public static Map FromBlock(string block)
        {
            var rows = block.Split(Environment.NewLine);
            var columns = new string[rows[0].Length];
            
            for (var i = 0; i < rows[0].Length; i++)
            {
                columns[i] = string.Join("", rows.Select(r => r[i]).ToList());
            }
            
            return new Map(rows, columns);
        }
        public string AsBlock() => string.Join(Environment.NewLine, Rows);
    }
    private record ReflectionResult(bool IsReflection, int IndexValue);
    private record Reflection(int Index, bool IsRow);
}