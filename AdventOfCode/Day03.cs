namespace AdventOfCode;

public partial class Day03 : BaseDay
{
    private readonly string[] _input;

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {SumOfParts()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {SumOfGearRatios()}");

    private int SumOfParts()
    {
        var (width , height) = (_input[0].Length, _input.Length);
        
        var parts = new List<int>();

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var c = _input[y][x];
                
                if (!char.IsDigit(c)) continue;

                var num = c.ToString();
                
                while (x + 1 < width && char.IsDigit(_input[y][x + 1]))
                {
                    num += _input[y][x + 1];
                    x++;
                }
                
                // Check x-axis
                if (x - num.Length >= 0 && _input[y][x - num.Length] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }
                
                if (x + 1 < width && _input[y][x + 1] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }

                // Check y-axis
                if (y - 1 >= 0 && x - num.Length >= 0)
                {
                    var dx = x - num.Length;
                    var shouldContinue = false;
                    while (dx < x + 1)
                    {
                        if (_input[y - 1][dx] is not '.')
                        {
                            parts.Add(int.Parse(num));
                            shouldContinue = true;
                            break;
                        }

                        dx++;
                    }
                    
                    if (shouldContinue) continue;
                }

                if (y + 1 < height)
                {
                    var dx = x - num.Length + 1;
                    var shouldContinue = false;
                    while (dx <= x)
                    {
                        if (_input[y + 1][dx] is not '.')
                        {
                            parts.Add(int.Parse(num));
                            shouldContinue = true;
                            break;
                        }
                        
                        dx++;
                    }
                    
                    if (shouldContinue) continue;
                }
                
                // Check corners
                
                if (x - num.Length >= 0 && y - 1 >= 0 && _input[y - 1][x - num.Length] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }
                
                if (x - num.Length >= 0 && y + 1 < height && _input[y + 1][x - num.Length] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }
                
                if (x + 1 < width && y - 1 >= 0 && _input[y - 1][x + 1] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }
                
                if (x + 1 < width && y + 1 < height && _input[y + 1][x + 1] is not '.')
                {
                    parts.Add(int.Parse(num));
                    continue;
                }
            }
        }
        
        return parts.Sum();
    }

    private long SumOfGearRatios()
    {
        long sum = 0;
        var (width , height) = (_input[0].Length, _input.Length);

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (_input[y][x] != '*') continue;

                var gears = new List<int>();

                var checkLeftUp = true;
                var checkLeftDown = true;
                var checkRightUp = true;
                var checkRightDown = true;
                
                // Check left
                if (x > 0 && char.IsDigit(_input[y][x - 1]))
                {
                    var num = string.Empty;
                    var dx = x - 1;
                    while(dx >= 0 && char.IsDigit(_input[y][dx]))
                    {
                        num = _input[y][dx] + num;
                        dx--;
                    } 
                    
                    gears.Add(int.Parse(num));
                }
                
                // Check right
                if (x < width && char.IsDigit(_input[y][x + 1]))
                {
                    var num = string.Empty;
                    var dx = x + 1;
                    while(dx < width && char.IsDigit(_input[y][dx]))
                    {
                        num = num + _input[y][dx];
                        dx++;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                // Check up
                if (y > 0 && char.IsDigit(_input[y - 1][x]))
                {
                    var num = string.Empty;
                    var dy = y - 1;
                    var dx = x;
                    
                    // Check left
                    while(dy >= 0 && dx >= 0 && char.IsDigit(_input[dy][dx]))
                    {
                        num = _input[dy][dx] + num;
                        dx--;
                        checkLeftUp = false;
                    }
                    
                    dx = x + 1;
                    // Check right
                    while(dy >= 0 && dx <= width && char.IsDigit(_input[dy][dx]))
                    {
                        num = num + _input[dy][dx];
                        dx++;
                        checkRightUp = false;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                // Check down
                if (y < height && char.IsDigit(_input[y + 1][x]))
                {
                    var num = string.Empty;
                    var dy = y + 1;
                    var dx = x;
                    
                    // Check left
                    while(dy < height && dx >= 0 && char.IsDigit(_input[dy][dx]))
                    {
                        num = _input[dy][dx] + num;
                        dx--;
                        checkLeftDown = false;
                    }
                    
                    dx = x + 1;
                    // Check right
                    while(dy < height && dx <= width && char.IsDigit(_input[dy][dx]))
                    {
                        num = num + _input[dy][dx];
                        dx++;
                        checkRightDown = false;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                // Check corners
                if (checkLeftUp && x > 0 && y > 0 && char.IsDigit(_input[y - 1][x - 1]))
                {
                    var num = string.Empty;
                    var dy = y - 1;
                    var dx = x - 1;
                    
                    while(dy >= 0 && dx >= 0 && char.IsDigit(_input[dy][dx]))
                    {
                        num = _input[dy][dx] + num;
                        dx--;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                if (checkLeftDown && x > 0 && y < height && char.IsDigit(_input[y + 1][x - 1]))
                {
                    var num = string.Empty;
                    var dy = y + 1;
                    var dx = x - 1;
                    
                    while(dy < height && dx >= 0 && char.IsDigit(_input[dy][dx]))
                    {
                        num = _input[dy][dx] + num;
                        dx--;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                if (checkRightUp && x < width && y > 0 && char.IsDigit(_input[y - 1][x + 1]))
                {
                    var num = string.Empty;
                    var dy = y - 1;
                    var dx = x + 1;
                    
                    while(dy >= 0 && dx < width && char.IsDigit(_input[dy][dx]))
                    {
                        num = num + _input[dy][dx];
                        dx++;
                    }
                    
                    gears.Add(int.Parse(num));
                }
                
                if (checkRightDown && x < width && y < height && char.IsDigit(_input[y + 1][x + 1]))
                {
                    var num = string.Empty;
                    var dy = y + 1;
                    var dx = x + 1;
                    
                    while(dy < height && dx < width && char.IsDigit(_input[dy][dx]))
                    {
                        num = num + _input[dy][dx];
                        dx++;
                    }
                    
                    gears.Add(int.Parse(num));
                }

                if (gears.Count <= 1) continue;
                
                var ratio = gears[0];
                var currentSum = gears.Skip(1).Aggregate(ratio, (current, gear) => current * gear);
                sum += currentSum   ;
            }
        }
        
        return sum;
    }
}
