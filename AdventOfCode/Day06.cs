namespace AdventOfCode;

public partial class Day06 : BaseDay
{
    private readonly string[] _input;

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var times = _input[0].Remove(0, _input[0].IndexOf(':') + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var distances = _input[1].Remove(0, _input[1].IndexOf(':') + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

        var numsOfWins = new List<int>();

        for (var i = 0; i < times.Length; i++)
        {
            var time = times[i];
            var distance = distances[i];
            
            var numOfWins = 0;

            for (var j = 0; j < time; j++)
            {
                var diff = time - j;
                var speed = j * diff;

                if (speed > distance)
                    numOfWins++;
            }
            
            numsOfWins.Add(numOfWins);
        }
        
        return numsOfWins.Aggregate((first, second) => first * second);
    }

    private long Part02()
    {
        var numOfWins = 0L;
        var times = _input[0].Remove(0, _input[0].IndexOf(':') + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var time = long.Parse(string.Join(string.Empty, times));
        var distances = _input[1].Remove(0, _input[1].IndexOf(':') + 1).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var distance = long.Parse(string.Join(string.Empty, distances));

        for (var j = 0; j < time; j++)
        {
            var diff = time - j;
            var speed = j * diff;

            if (speed > distance)
                numOfWins++;
        }
        
        return numOfWins;
    }
}