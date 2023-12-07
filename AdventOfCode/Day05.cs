namespace AdventOfCode;

public partial class Day05 : BaseDay
{
    private readonly string[] _input;
    
    private List<long> Seeds { get; set; } = new();
    private List<AlmanacLine> SeedsToSoil { get; } = new();
    private List<AlmanacLine> SoilToFertilizer { get; } = new();
    private List<AlmanacLine> FertilizerToWater { get; } = new();
    private List<AlmanacLine> WaterToLight { get; } = new();
    private List<AlmanacLine> LightToTemperature { get; } = new();
    private List<AlmanacLine> TemperatureToHumidity { get; } = new();
    private List<AlmanacLine> HumidityToLocation { get; } = new();

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
        ParseInput();
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var lowestLocation = long.MaxValue;
 
        foreach (var seed in Seeds)
        {
            var soil = (from line in SeedsToSoil select line.IsInRange(seed) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (soil == -1L) soil = seed;
            
            var fertilizer = (from line in SoilToFertilizer select line.IsInRange(soil) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (fertilizer == -1L) fertilizer = soil;
            
            var water = (from line in FertilizerToWater select line.IsInRange(fertilizer) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (water == -1L) water = fertilizer;
            
            var light = (from line in WaterToLight select line.IsInRange(water) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (light == -1L) light = water;
            
            var temperature = (from line in LightToTemperature select line.IsInRange(light) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (temperature == -1L) temperature = light;
            
            var humidity = (from line in TemperatureToHumidity select line.IsInRange(temperature) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (humidity == -1L) humidity = temperature;
            
            var location = (from line in HumidityToLocation select line.IsInRange(humidity) into l where l.isInRange select l.result).FirstOrDefault(-1L);
            if (location == -1L) location = humidity;
            
            if (location < lowestLocation) lowestLocation = location;
        }
        
        return lowestLocation;
    }

    private long Part02()
    {
        var ranges = new List<Range>();
        for (var i = 0; i < Seeds.Count; i += 2)
        {
            ranges.Add(new Range(Seeds[i], Seeds[i] + Seeds[i + 1] - 1));
        }

        var orderedMaps = new List<List<AlmanacRangeLine>>
        {
            SeedsToSoil
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            SoilToFertilizer
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            FertilizerToWater
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            WaterToLight
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            LightToTemperature
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            TemperatureToHumidity
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList(),
            HumidityToLocation
                .Select(AlmanacRangeLine.Parse)
                .OrderBy(s => s.From)
                .ToList()
        };
        
        foreach (var map in orderedMaps)
        {
            var newRanges = new List<Range>();
            foreach (var range in ranges)
            {
                var newRange = range;
                foreach (var mapping in map)
                {
                    if (range.From < mapping.From)
                    {
                        newRanges.Add(newRange with { To = Math.Min(newRange.To, mapping.From - 1) });
                        newRange.From = mapping.From;
                        
                        if (newRange.From > newRange.To)
                            break;
                    }

                    if (newRange.From <= mapping.To)
                    {
                        newRanges.Add(new Range(newRange.From + mapping.Diff, Math.Min(newRange.To, mapping.To) + mapping.Diff));
                        newRange.From = mapping.To + 1;
                        
                        if (newRange.From > newRange.To)
                            break;
                    }
                }

                if (newRange.From <= newRange.To)
                    newRanges.Add(newRange);
            }
            
            ranges = newRanges;
        }
        
        return ranges.Min(r => r.From);
    }

    private void ParseInput()
    {
        var offset = 0;
        Seeds = _input[offset].Split(' ').Skip(1).Select(long.Parse).ToList();

        offset += 3;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            SeedsToSoil.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            SoilToFertilizer.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            FertilizerToWater.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            WaterToLight.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            LightToTemperature.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (!string.IsNullOrWhiteSpace(_input[offset]))
        {
            TemperatureToHumidity.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
        
        offset += 2;
        
        while (offset < _input.Length)
        {
            HumidityToLocation.Add(AlmanacLine.Parse(_input[offset]));
            offset++;
        }
    }
    
    private record AlmanacLine(long Destination, long Source, long Count, long Diff)
    {
        public static AlmanacLine Parse(string line)
        {
            var parts = line.Split(' ').Select(long.Parse).ToArray();
            var diff = parts[0] - parts[1];
            return new AlmanacLine(parts[0], parts[1], parts[2], diff);
        }
        
        public (bool isInRange, long result) IsInRange(long part)
        {
            var result = -1L;
            var isInRange = false;
            
            if (part >= Source && part < Source + Count)
            {
                result = Destination + (part - Source); 
                isInRange = true;
            }
            
            return (isInRange, result);
        }
    }

    private record struct Range(long From, long To);

    private record AlmanacRangeLine(long From, long To, long Diff)
    {
        public static AlmanacRangeLine Parse(AlmanacLine line)
            => new(line.Source, line.Source + line.Count - 1, line.Diff);
    }
}