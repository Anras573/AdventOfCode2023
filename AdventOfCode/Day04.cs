namespace AdventOfCode;

public partial class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private int Part01()
    {
        var sum = 0;

        foreach (var line in _input)
        {
            var winningNumbers = line
                .Split('|')[0]
                .Split(':')[1]
                .Split(' ')
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(int.Parse)
                .ToHashSet();
            
            var myNumbers = line
                .Split('|')[1]
                .Split(' ')
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(int.Parse)
                .ToHashSet();
            
            var matches = myNumbers.Intersect(winningNumbers).Count();
            
            // 1 point for the first match, then doubled each time for each of the matches after the first
            sum += matches > 0 ? (int) Math.Pow(2, matches - 1) : 0;
        }
        
        return sum;
    }

    private long Part02()
    {
        var scratchCards = new int[_input.Length];

        for (var i = 0; i < scratchCards.Length; i++)
        {
            scratchCards[i] = 1;
        }
        
        foreach (var (line, index) in _input.Select((l, i) => (l, i)))
        {
            var winningNumbers = line
                .Split('|')[0]
                .Split(':')[1]
                .Split(' ')
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(int.Parse)
                .ToHashSet();
            
            var myNumbers = line
                .Split('|')[1]
                .Split(' ')
                .Where(s => string.IsNullOrWhiteSpace(s) == false)
                .Select(int.Parse)
                .ToHashSet();
            
            var matches = myNumbers.Intersect(winningNumbers).Count();

            if (matches <= 0) continue;
            
            foreach (var i in Enumerable.Range(index + 1, matches))
            {
                scratchCards[i] += scratchCards[index];
            }
        }

        return scratchCards.Aggregate(0L, (current, scratchCard) => current + scratchCard);
    }
}
