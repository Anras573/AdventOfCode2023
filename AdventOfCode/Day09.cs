namespace AdventOfCode;

public partial class Day09 : BaseDay
{
    private readonly string[] _input;

    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var oasis = _input
            .Select(i => i
                .Split(' ')
                .Select(int.Parse)
                .ToArray())
            .ToArray();

        var sum = oasis.Sum(LastNumber);

        return sum;
        
        int LastNumber(int[] history)
        {
            var diff = Enumerable
                .Range(0, history.Length - 1)
                .Select(i => history[i + 1] - history[i])
                .ToArray();

            if (diff.All(d => d == 0))
                return history[^1];

            return history[^1] + LastNumber(diff);
        }
    }
    private long Part02()
    {
        var oasis = _input
            .Select(i => i
                .Split(' ')
                .Select(int.Parse)
                .ToArray())
            .ToArray();

        var sum = oasis.Sum(LastNumber);

        return sum;
        
        int LastNumber(int[] history)
        {
            var diff = Enumerable
                .Range(0, history.Length - 1)
                .Select(i => history[i + 1] - history[i])
                .ToArray();

            if (diff.All(d => d == 0))
                return history[^1];

            return history[0] - LastNumber(diff);
        }
    }
}