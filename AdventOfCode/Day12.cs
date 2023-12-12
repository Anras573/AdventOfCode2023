namespace AdventOfCode;

public partial class Day12 : BaseDay
{
    private readonly string[] _input;

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var sum = 0L;

        foreach (var line in _input)
        {
            var parts = line.Split(' ');
            var springs = parts[0];
            var groups = parts[1].Split(',').Select(int.Parse).ToArray();
            
            sum += DP(springs, groups, new CacheKey(0, 0, 0), new Dictionary<CacheKey, long>());
        }
        
        return sum;
    }

    private static long DP(string springs, int[] groups, CacheKey key, Dictionary<CacheKey, long> cache)
    {
        if (cache.TryGetValue(key, out var dp)) return dp;
        
        var (springIndex, groupIndex, length) = key;
        
        if (springIndex == springs.Length)
        {
            if (groupIndex == groups.Length - 1 && length == groups[groupIndex])
            {
                groupIndex++;
                length = 0;
            }
            
            return groupIndex == groups.Length && length == 0 ? 1 : 0;
        }

        var result = 0L;

        if (".?".Contains(springs[springIndex]))
        {
            if (length == 0)
                result += DP(springs, groups, new CacheKey(springIndex + 1, groupIndex, 0), cache);
            else if (groupIndex < groups.Length && length == groups[groupIndex])
                result += DP(springs, groups, new CacheKey(springIndex + 1, groupIndex + 1, 0), cache);
        }

        if ("#?".Contains(springs[springIndex]))
            result += DP(springs, groups, new CacheKey(springIndex + 1, groupIndex, length + 1), cache);
        
        cache[key] = result;
        
        return result;
    }
    
    private long Part02()
    {
        var sum = 0L;

        foreach (var line in _input)
        {
            var parts = line.Split(' ');
            var springs = $"{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}?{parts[0]}";
            var gString = $"{parts[1]},{parts[1]},{parts[1]},{parts[1]},{parts[1]}";
            var groups = gString.Split(',').Select(int.Parse).ToArray();
            
            sum += DP(springs, groups, new CacheKey(0, 0, 0), new Dictionary<CacheKey, long>());
        }
        
        return sum; 
    }
    
    private record CacheKey(int SpringIndex, int GroupIndex, int Length);
}