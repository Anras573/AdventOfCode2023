namespace AdventOfCode;

public partial class Day15 : BaseDay
{
    private readonly string[] _input;

    public Day15()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var result = 0L;
        var parts = _input[0].Split(',');

        foreach (var part in parts)
        {
            var currentValue = 0L;

            foreach (var c in part)
            {
                currentValue += c;
                currentValue *= 17;
                currentValue %= 256;
            }
            
            result += currentValue;
        }
        
        return result;
    }
    
    private long Part02()
    {
        var boxes = new List<Label>[256];
        
        for (var i = 0; i < 256; i++)
            boxes[i] = new List<Label>();
        
        var parts = _input[0].Split(',');
        
        foreach (var part in parts)
        {
            var currentValue = 0L;
            
            foreach (var c in part.Split('=', '-')[0])
            {
                currentValue += c;
                currentValue *= 17;
                currentValue %= 256;
            }

            if (part.Contains('='))
            {
                var split = part.Split('=');
                
                var label = boxes[currentValue].FirstOrDefault(l => l.Key == split[0]);
                if (label is not null)
                {
                    var index = boxes[currentValue].IndexOf(label);
                    boxes[currentValue][index] = label with { Value = long.Parse(split[1]) };
                }
                else
                {
                    boxes[currentValue].Add(new Label(split[0], long.Parse(split[1])));
                }
            }
            else
            {
                var split = part.Split('-');
                
                var label = boxes[currentValue].FirstOrDefault(l => l.Key == split[0]);
                if (label != default)
                    boxes[currentValue].Remove(label);
            }
        }

        var sum = 0L;
        
        foreach (var (box, i) in boxes.Select((box, i) => (box, i)))
        {
            foreach (var (focalLength, slot, key) in box.Select((kvp, slot) => (kvp.Value, slot, kvp.Key)))
            {
                sum += focalLength * (i + 1) * (slot + 1);
            } 
        }
        
        return sum;
    }

    private record Label(string Key, long Value);
}