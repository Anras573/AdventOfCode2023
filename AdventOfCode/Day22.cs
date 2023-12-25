namespace AdventOfCode;

public partial class Day22 : BaseDay
{
    private readonly string[] _input;

    public Day22()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private long Part01()
    {
        var blocks = ParseBlocks();
        
        blocks = LetTheBlocksFall(blocks);

        var supports = FindSupports(blocks);

        var numberOfFallingBlocks = DisintegrateBlocks(blocks, supports);

        return numberOfFallingBlocks.Count(x => x == 0);
    }

    private long Part02()
    {
        var blocks = ParseBlocks();
        
        blocks = LetTheBlocksFall(blocks);

        var supports = FindSupports(blocks);

        var numberOfFallingBlocks = DisintegrateBlocks(blocks, supports);

        return numberOfFallingBlocks.Sum();
    }
    
    private Block[] ParseBlocks()
    {
        var blocks = new Block[_input.Length];
            
        foreach (var (line, i) in _input.Select((line, i) => (line, i)))
        {
            var dimensions = line
                .Split(',', '~')
                .Select(int.Parse)
                .ToArray();

            blocks[i] = new Block(
                X: new Range(dimensions[0], dimensions[3]),
                Y: new Range(dimensions[1], dimensions[4]),
                Z: new Range(dimensions[2], dimensions[5])
            );
        }

        return blocks;
    }
    
    private static Block[] LetTheBlocksFall(Block[] blocks)
    {
        blocks = blocks.OrderBy(b => b.Bottom).ToArray();

        for (var i = 0; i < blocks.Length; i++)
        {
            var newBottom = 1;
            for (var j = 0; j < i; j++)
            {
                if (blocks[i].IntersectsXY(blocks[j]))
                    newBottom = Math.Max(newBottom, blocks[j].Top + 1);
            }
            var fall = blocks[i].Bottom - newBottom;
            blocks[i] = blocks[i] with
            {
                Z = new Range(blocks[i].Bottom - fall, blocks[i].Top - fall)
            };
        }

        return blocks;
    }
    
    private static Supports FindSupports(Block[] blocks)
    {
        var blocksAbove = blocks.ToDictionary(b => b, _ => new HashSet<Block>());
        var blocksBelow = blocks.ToDictionary(b => b, _ => new HashSet<Block>());

        for (var i = 0; i < blocks.Length; i++)
        {
            for (var j = i + 1; j < blocks.Length; j++)
            {
                var zNeighbours = blocks[j].Bottom == 1 + blocks[i].Top;
                
                if (!zNeighbours || !blocks[i].IntersectsXY(blocks[j])) continue;
                
                blocksAbove[blocks[i]].Add(blocks[j]);
                blocksBelow[blocks[j]].Add(blocks[i]);
            }
        }
        
        var supports = new Supports(blocksAbove, blocksBelow);
        return supports;
    }
    
    private static IEnumerable<int> DisintegrateBlocks(Block[] blocks, Supports supports)
    {
        foreach (var disintegratedBlock in blocks)
        {
            var queue = new Queue<Block>();
            queue.Enqueue(disintegratedBlock);
            
            var fallingBlocks = new HashSet<Block>();
            while (queue.TryDequeue(out var block))
            {
                fallingBlocks.Add(block);
                
                var blocksToFall = supports
                    .Above[block]
                    .Where(above => supports
                        .Below[above]
                        .IsSubsetOf(fallingBlocks));
                
                foreach (var blockToFall in blocksToFall)
                    queue.Enqueue(blockToFall);
            }
            
            // Don't count the disintegrated block itself.
            yield return fallingBlocks.Count - 1;
        }
    }
    
    private record Supports(Dictionary<Block, HashSet<Block>> Above, Dictionary<Block, HashSet<Block>> Below);

    private record Block(Range X, Range Y, Range Z)
    {
        public int Top => Z.End.Value;
        public int Bottom => Z.Start.Value;
        
        public bool IntersectsXY(Block other)
        {
            return Overlaps(X, other.X) && Overlaps(Y, other.Y);
            
            bool Overlaps(Range range1, Range range2)
            {
                return range1.Start.Value <= range2.End.Value && range2.Start.Value <= range1.End.Value;
            }
        }
    }
}