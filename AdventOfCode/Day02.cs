namespace AdventOfCode;

public partial class Day02 : BaseDay
{
    private readonly string[] _input;

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {SumOfPossibleGames()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {SumOfLeastPossibleCubes()}");

    private int SumOfPossibleGames()
    {
        const int blue = 14;
        const int green = 13;
        const int red = 12;

        var sum = 0;
        
        foreach (var line in _input)
        {
            var splitLine = line.Split(' ');
            
            var id = int.Parse(splitLine[1].Remove(splitLine[1].Length - 1));
            var isPossible = true;

            for (var i = 2; i < splitLine.Length; i += 2)
            {
                var num = int.Parse(splitLine[i]);
                var color = splitLine[i + 1]
                    .Replace(",", string.Empty)
                    .Replace(";", string.Empty);
                
                if (color is "blue" && num > blue)
                {
                    isPossible = false;
                    break;
                }
                
                if (color is "green" && num > green)
                {
                    isPossible = false;
                    break;
                }
                
                if (color is "red" && num > red)
                { 
                    isPossible = false;
                    break;
                }
            }
            
            if (isPossible)
            {
                sum += id;
            }
        }
        
        return sum;
    }

    private int SumOfLeastPossibleCubes()
    {
        var power = 0;
        
        foreach (var line in _input)
        {
            var splitLine = line.Split(' ');
            var blue = 0;
            var green = 0;
            var red = 0;
            
            for (var i = 2; i < splitLine.Length; i += 2)
            {
                var num = int.Parse(splitLine[i]);
                var color = splitLine[i + 1]
                    .Replace(",", string.Empty)
                    .Replace(";", string.Empty);
                
                switch (color)
                {
                    case "blue" when num > blue:
                        blue = num;
                        break;
                    case "green" when num > green:
                        green = num;
                        break;
                    case "red" when num > red:
                        red = num;
                        break;
                }
            }
            
            power += blue * green * red;
        }

        return power;
    }
}
