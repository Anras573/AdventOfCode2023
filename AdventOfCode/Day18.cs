using System.Numerics;

namespace AdventOfCode;

public partial class Day18 : BaseDay
{
    private readonly string[] _input;

    public Day18()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private long Part01()
    {
        var instructions = _input.Select(Instruction.Parse).ToList();

        return CalculateCubicMeters(instructions);
    }
    
    private long Part02()
    {
        var instructions = _input.Select(Instruction.ParsePart2).ToList();
        
        return CalculateCubicMeters(instructions);
    }
    
    private static long CalculateCubicMeters(List<Instruction> instructions)
    {
        var position = Vector2.Zero;
        var vertices = new List<Vector2> { position };
        var perimeter = 0L;
        
        foreach (var instruction in instructions)
        {
            position = instruction.Direction switch
            {
                Direction.Up => position with { Y = position.Y - instruction.Steps },
                Direction.Left => position with { X = position.X - instruction.Steps },
                Direction.Down => position with { Y = position.Y + instruction.Steps },
                Direction.Right => position with { X = position.X + instruction.Steps },
                _ => throw new ArgumentOutOfRangeException(nameof(instruction.Direction), instruction.Direction, null)
            };
            
            perimeter += instruction.Steps;
            vertices.Add(position);
        }
        
        return Shoelace(vertices, perimeter);
    }

    private static long Shoelace(List<Vector2> vertices, long perimeter)
    {
        var sum = 0L;
        for (var i = 0; i < vertices.Count - 1; i++)
        {
            var a = vertices[i];
            var b = vertices[i + 1];
            sum += (long)a.X * (long)b.Y - (long)b.X * (long)a.Y;
        }
        return (Math.Abs(sum) + perimeter) / 2 + 1;
    }

    private record Instruction(Direction Direction, long Steps)
    {
        public static Instruction Parse(string line)
        {
            var input = line.Split(' ');
            var directionAsString = input[0];
            var steps = int.Parse(input[1]);
            
            var direction = directionAsString switch
            {
                "U" => Direction.Up,
                "L" => Direction.Left,
                "D" => Direction.Down,
                "R" => Direction.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(directionAsString), directionAsString, null)
            };

            return new Instruction(direction, steps);
        }

        public static Instruction ParsePart2(string line)
        {
            var input = line.Split(' ');
            
            var hex = input[2]
                .Replace("#", string.Empty)
                .Replace("(", string.Empty)
                .Replace(")", string.Empty);
            
            var steps = Convert.ToInt64(hex[0..5], 16);
            
            var directionAsChar = hex[5];
            
            var direction = directionAsChar switch
            {
                '0' => Direction.Right,
                '1' => Direction.Down,
                '2' => Direction.Left,
                '3' => Direction.Up,
                _ => throw new ArgumentOutOfRangeException(nameof(directionAsChar), directionAsChar, null)
            };
            
            return new Instruction(direction, steps);
        }
    }
    private enum Direction { Up, Left, Down, Right }
}