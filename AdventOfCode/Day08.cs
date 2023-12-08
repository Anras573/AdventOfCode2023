namespace AdventOfCode;

public partial class Day08 : BaseDay
{
    private readonly string[] _input;

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var instructions = _input[0].ToCharArray();
        Dictionary<string, (string Left, string Right)> network = new();
        
        foreach (var line in _input.Skip(2))
        {
            var split = line.Split("=", StringSplitOptions.TrimEntries);
            var node = split[0];
            var directions = split[1]
                .Replace("(", string.Empty)
                .Replace(",", string.Empty)
                .Replace(")", string.Empty)
                .Split(' ');
            var left = directions[0];
            var right = directions[1];
            network.Add(node, (left, right));
        }
        
        var index = 0;
        
        var currentNode = "AAA";

        while (currentNode != "ZZZ")
        {
            var direction = instructions[index++ % instructions.Length];
            
            if (direction == 'L')
                currentNode = network[currentNode].Left;
            else
                currentNode = network[currentNode].Right;
        }
        
        return index;
    }
    private long Part02()
    {
        var instructions = _input[0].ToCharArray();
        Dictionary<string, (string Left, string Right)> network = new();
        var currentNodes = new List<string>();
        
        foreach (var line in _input.Skip(2))
        {
            var split = line.Split("=", StringSplitOptions.TrimEntries);
            var node = split[0];
            var directions = split[1]
                .Replace("(", string.Empty)
                .Replace(",", string.Empty)
                .Replace(")", string.Empty)
                .Split(' ');
            var left = directions[0];
            var right = directions[1];
            network.Add(node, (left, right));
            
            if (node[^1] == 'A')
                currentNodes.Add(node);
        }
        
        var cycles = currentNodes.Select(current =>
        {
            var step = 0;
            while (current[^1] != 'Z')
            {
                var direction = instructions[step++ % instructions.Length];
                if (direction == 'L')
                    current = network[current].Left;
                else
                    current = network[current].Right;
            }

            var init = step;
            do
            {
                var direction = instructions[step++ % instructions.Length];
                if (direction == 'L')
                    current = network[current].Left;
                else
                    current = network[current].Right;
            } while (current[^1] != 'Z');

            return (long)step - init;
        })
            .ToList();

        var lcm = LeastCommonMultiple(cycles);

        return lcm;
    }

    private static long LeastCommonMultiple(List<long> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            throw new ArgumentException("Input list must not be null or empty.");
        }

        var lcm = numbers[0];

        for (var i = 1; i < numbers.Count; i++)
        {
            lcm = FindLCM(lcm, numbers[i]);
        }

        return lcm;
    }

    private static long FindLCM(long a, long b)
    {
        return a / FindGCD(a, b) * b;
    }

    private static long FindGCD(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }
}