namespace AdventOfCode;

public partial class Day01 : BaseDay
{
    private readonly string[] _input;

    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {SumOfFirstAndLastDigit()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {SumOfFirstAndLastNumber()}");

    private int SumOfFirstAndLastDigit()
    {
        var nums = _input.Select(l =>
        {
            var numbers = l.ToCharArray().Where(char.IsDigit).ToArray();

            return int.Parse($"{numbers[0]}{numbers[^1]}");
        });

        return nums.Sum();
    }
    
    private int SumOfFirstAndLastNumber()
    {
        var nums = _input.Select(l =>
        {
            var newLine = l
                .Replace("one", "one1one")
                .Replace("two", "two2two")
                .Replace("three", "three3three")
                .Replace("four", "four4four")
                .Replace("five", "five5five")
                .Replace("six", "six6six")
                .Replace("seven", "seven7seven")
                .Replace("eight", "eight8eight")
                .Replace("nine", "nine9nine");

            var numbers = newLine.ToCharArray().Where(char.IsDigit).ToArray();

            return int.Parse($"{numbers[0]}{numbers[^1]}");
        });

        return nums.Sum();
    }
}
