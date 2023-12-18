using System.Numerics;

namespace AdventOfCode;

public partial class Day16 : BaseDay
{
    private readonly string[] _input;

    public Day16()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private (char[,] map, int width, int height) ParseInput()
    {
        var (width, height) = (_input[0].Length, _input.Length);
        var map = new char[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                map[x, y] = _input[y][x];
            }
        }
        
        return (map, width, height);
    }
    
    private long Part01()
    {
        var (map, width, height) = ParseInput();

        var startingBeam = new Beam(new Vector2(-1, 0), Direction.Right);
        
        var energizedCount = SimulateBeam(startingBeam, width, height, map);

        return energizedCount;
    }

    private static long SimulateBeam(Beam startingBeam, int width, int height, char[,] map)
    {
        var energized = new HashSet<Vector2>();
        var beams = new Queue<Beam>();
        var hasBeenUsed = new HashSet<Vector2>();
        beams.Enqueue(startingBeam);

        while (beams.TryDequeue(out var beam))
        {
            var nextPos = beam.Direction switch
            {
                Direction.Right => beam.Position + new Vector2(1, 0),
                Direction.Left => beam.Position + new Vector2(-1, 0),
                Direction.Up => beam.Position + new Vector2(0, -1),
                Direction.Down => beam.Position + new Vector2(0, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
            
            if (nextPos.X < 0
                || nextPos.X >= width
                || nextPos.Y < 0
                || nextPos.Y >= height)
            {
                continue;
            }
            
            energized.Add(nextPos);
            
            var nextChar = map[(int)nextPos.X, (int)nextPos.Y];
            
            switch (nextChar)
            {
                case '.':
                    beams.Enqueue(beam with { Position = nextPos });
                    break;
                case '|' when beam.Direction == Direction.Left || beam.Direction == Direction.Right:
                    if (!hasBeenUsed.Add(nextPos))
                    {
                        continue;
                    }

                    beams.Enqueue(new Beam(Position: nextPos, Direction: Direction.Up));
                    beams.Enqueue(new Beam(Position: nextPos, Direction: Direction.Down));
                    break;
                case '-'when beam.Direction == Direction.Up || beam.Direction == Direction.Down:
                    if (!hasBeenUsed.Add(nextPos))
                    {
                        continue;
                    }
                    
                    beams.Enqueue(new Beam(Position: nextPos, Direction: Direction.Left));
                    beams.Enqueue(new Beam(Position: nextPos, Direction: Direction.Right));
                    break;
                case '/': 
                {
                    var newDir = beam.Direction switch
                    {
                        Direction.Up => Direction.Right,
                        Direction.Right => Direction.Up,
                        Direction.Down => Direction.Left,
                        Direction.Left => Direction.Down,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    beams.Enqueue(new Beam(Position: nextPos, Direction: newDir));
                    break;
                }
                case '\\':
                {
                    var newDir = beam.Direction switch
                    {
                        Direction.Up => Direction.Left,
                        Direction.Left => Direction.Up,
                        Direction.Down => Direction.Right,
                        Direction.Right => Direction.Down,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    beams.Enqueue(new Beam(Position: nextPos, Direction: newDir));
                    break;
                }
                default:
                    beams.Enqueue(beam with { Position = nextPos });
                    break; 
            }
        }

        return energized.Count;
    }

    private long Part02()
    {
        var (map, width, height) = ParseInput();

        long[] allCombinations =
        [
            .. Enumerable.Range(0, height).Select(row => SimulateBeam(new Beam(new Vector2(-1, row), Direction.Right), width, height, map)),
            .. Enumerable.Range(0, height).Select(row => SimulateBeam(new Beam(new Vector2(width, row), Direction.Left), width, height, map)),
            .. Enumerable.Range(0, width).Select(col => SimulateBeam(new Beam(new Vector2(col, -1), Direction.Down), width, height, map)),
            .. Enumerable.Range(0, width).Select(col => SimulateBeam(new Beam(new Vector2(col, height), Direction.Up), width, height, map))
        ];

        return allCombinations.Max();
    }

    private record Beam(Vector2 Position, Direction Direction);
    private enum Direction { Up, Left, Down, Right }
}