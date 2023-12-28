namespace AdventOfCode;

public partial class Day24 : BaseDay
{
    private readonly string[] _input;

    public Day24()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        const long min = 200_000_000_000_000;
        const long max = 400_000_000_000_000;
        
        var hailStones = _input.Select(HailStone.Parse).ToArray();
        
        var intersections = hailStones
            .SelectMany((_, i) => hailStones.Skip(i + 1), (h1, h2) => (h1, h2))
            .Count(pair => pair.h1.IntersectsXY(pair.h2, min, max));
        
        return intersections;
    }

    private long Part02()
    {
        var originals = _input.Select(HailStone.Parse).ToArray();
        
        // In theory we only need 3 hailstones to calculate the intersection on all 3 axis, so we'll try with the first one, and the last 2
        var hailStones = new [] { originals[0], originals[^2], originals[^1] };
        
        var n = 0;
        var modifiers = new (int x, int y) [] 
        {
            (-1, -1),  (-1, 1), (1, -1), (1, 1)
        };
        
        while (true)
        {
            for (var x = 0; x <= n; x++)
            {
                var y = n - x;
                
                foreach (var (modX, modY) in modifiers)
                {
                    var ax = x * modX;
                    var ay = y * modY;
                    
                    var h1 = hailStones[0];
                    h1.Adjust(ax, ay);
                    
                    // If the velocity is 0 on both axis, we can't calculate Z (due to divide by zero exception)
                    if (h1.Velocity.X == 0 && h1.Velocity.Y == 0)
                        continue;
                    
                    var intersection = Intersection.Invalid;
                    var p = Intersection.Invalid;
        
                    foreach (var h2 in hailStones.Skip(1))
                    {
                        h2.Adjust(ax, ay);
                        p = h1.IntersectsXY(h2);
                         
                        if (!p.Valid)
                            break;
                        
                        if (!intersection.Valid)
                        {
                            intersection = p;
                            continue;
                        }
        
                        if (intersection != p)
                        {
                            break;
                        }
                    }
                             
                    if (!p.Valid || p != intersection)
                        continue;
        
                    decimal? az = null;
                    decimal? nz = null;
                    
                    foreach (var (h2, i) in hailStones.Select((h2, i) => (h2, i)).Skip(1))
                    {
                        h2.Adjust(ax, ay);
                        
                        // If the velocity is 0 on both axis, we can't calculate Z (due to divide by zero exception)
                        if (h2.Velocity.X == 0 && h2.Velocity.Y == 0)
                            continue;
                        
                        nz = h1.GetZ(h2, intersection.Position);
                                                     
                        if (!az.HasValue)
                        {
                            az = nz;
                            continue;
                        }
        
                        if (az != nz)
                        {
                            break;
                        }
                    }
                      
                    if (!nz.HasValue || az != nz) continue;
                    
                    var z = h1.Position.Z + h1.GetT(intersection.Position) * (h1.Velocity.Z - az);
                    return (long)(z + intersection.Position.X + intersection.Position.Y);
                }
            }
        
            n++;
        }
    }

    private record Intersection(Vector2D Position, bool Valid = true)
    {
        public static readonly Intersection Invalid = new(Vector2D.Zero, false);
    }
    
    private record struct HailStone(Vector3D Position, Vector3D Velocity, Vector3D Alpha, decimal XySlope = 0)
    {
        public static HailStone Parse(string line)
        {
            var parts = line.Split(" @ ");
            var position = parts[0].Split(", ").Select(decimal.Parse).ToArray();
            var velocity = parts[1].Split(", ").Select(decimal.Parse).ToArray();
            var alpha = Vector3D.Zero;
            var xySlope = velocity[0] == 0 ? decimal.MaxValue : velocity[1] / velocity[0];
            return new HailStone(new Vector3D(position[0], position[1], position[2]), new Vector3D(velocity[0], velocity[1], velocity[2]), alpha, xySlope);
        }
        
        public bool IntersectsXY(HailStone other, long min, long max)
        {
            var intersection = IntersectsXY(other);
            
            if (!intersection.Valid)
                return false;
            
            return intersection.Position.X >= min
                   && intersection.Position.X <= max
                   && intersection.Position.Y >= min
                   && intersection.Position.Y <= max;
        }
        
        public Intersection IntersectsXY(HailStone other)
        {
            // We're either going parallel or have intersected earlier
            if (XySlope == other.XySlope)
                return Intersection.Invalid;
            
            var x = 0m;
            var y = 0m;
            
            if (XySlope == decimal.MaxValue)
            {
                x = Position.X;
                y = other.XySlope * (x - other.Position.X) + other.Position.Y;
            }
            else if (other.XySlope == decimal.MaxValue)
            {
                x = other.Position.X;
                y = XySlope * (x - Position.X) + Position.Y;
            }
            else
            {
                x = (Position.Y - other.Position.Y - Position.X * XySlope + other.Position.X * other.XySlope) / (other.XySlope - XySlope);
                y = Position.Y + XySlope * (x - Position.X);
            }
            
            x = Math.Round(x, 1);
            y = Math.Round(y, 1);
            
            var future = Sign(x - Position.X) == Sign(Velocity.X);
            var otherFuture = Sign(x - other.Position.X) == Sign(other.Velocity.X);
            
            if (!(future && otherFuture))
                return Intersection.Invalid;

            return new Intersection(new Vector2D(x, y));
            
            
            int Sign(decimal value)
            {
                return value switch
                {
                    < 0 => -1,
                    > 0 => 1,
                    _ => 0
                };
            }
        }
        
        public void Adjust(decimal ax, decimal ay)
        {
            var velocity = new Vector3D(Velocity.X - ax - Alpha.X, Velocity.Y - ay - Alpha.Y, Velocity.Z);
            var xySlope = velocity.X == 0 ? decimal.MaxValue : velocity.Y / velocity.X;
            var alpha = new Vector3D(ax, ay, Alpha.Z);
            
            Velocity = velocity;
            XySlope = xySlope;
            Alpha = alpha;
        }

        public decimal GetT(Vector2D intersection)
        {
            if (Velocity.X == 0)
                return (intersection.Y - Position.Y) / Velocity.Y;
            
            return (intersection.X - Position.X) / Velocity.X;
        }
        
        public decimal? GetZ(HailStone other, Vector2D intersection)
        {
            var tSelf = GetT(intersection);
            var tOther = other.GetT(intersection);

            if (tSelf == tOther)
                return null;
            
            var zSelf = (Position.Z - other.Position.Z + tSelf * Velocity.Z - tOther * other.Velocity.Z) / (tSelf - tOther);
            return zSelf;
        }
    }

    private record Vector3D(decimal X, decimal Y, decimal Z)
    {
        public static Vector3D Zero => new(0, 0, 0);
        
        public static Vector3D operator -(Vector3D value) => Zero - value;
        public static Vector3D operator -(Vector3D v1, Vector3D v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    private record Vector2D(decimal X, decimal Y)
    {
        public static Vector2D Zero => new(0, 0);
    }
}