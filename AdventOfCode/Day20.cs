using System.Text.RegularExpressions;

namespace AdventOfCode;

public partial class Day20 : BaseDay
{
    private readonly string[] _input;

    public Day20()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private long Part01()
    {
        var modules = ParseModules();
        
        var processQueue = new Queue<string>();
        var lowPulses = 0L;
        var highPulses = 0L;

        for (var i = 0; i < 1000; i++)
        {
            modules["button"].IncomingPulses.Enqueue(("finger", PulseType.Low));
            processQueue.Enqueue("button");
            
            while (processQueue.TryDequeue(out var moduleName))
            {
                var (pulsesSent, pulseType) = modules[moduleName].Process(modules, processQueue);

                if (pulseType == PulseType.Low)
                    lowPulses += pulsesSent;
                else
                    highPulses += pulsesSent;
            }
        }

        return highPulses * lowPulses;
    }

    private Dictionary<string, Module> ParseModules()
    {
        var modules = new Dictionary<string, Module>();
        
        var noopModule = new NoopModule("rx");
        modules.Add("rx", noopModule);
        
        foreach (var line in _input)
        {
            var data = ExtractWords(line).ToList();
            var name = data[0];
        
            Module module = line[0] switch
            {
                '%' => new FlipFlopModule(name),
                '&' => new ConjunctionModule(name),
                _ => new BroadcastModule(name)
            };

            modules.Add(name, module);
        }

        foreach (var line in _input)
        {
            var data = ExtractWords(line).ToList();
            var name = data[0];
        
            foreach (var target in data.Skip(1))
            {
                modules[name].OutgoingPulses.Add(target);
                modules[target].LastReceivedPulses.Add(name, PulseType.Low);
            }
        }

        var buttonModule = new BroadcastModule("button");
        buttonModule.OutgoingPulses.Add("broadcaster");

        modules.Add("button", buttonModule);

        return modules;
        
        IEnumerable<string> ExtractWords(string line)
        {
            return Regex.Matches(line, "[a-zA-z]+").Select(a => a.Value);
        }
    }

    private long Part02()
    {
        var modules = ParseModules();

        var rxConjunctions = modules.Values
            .Where(m => m.OutgoingPulses.Contains("lg"))
            .Select(m => (Key: m.Name, Value: 0L))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        
        var processQueue = new Queue<string>();

        for (var i = 1; ; i++)
        {
            modules["button"].IncomingPulses.Enqueue(("finger", PulseType.Low));
            processQueue.Enqueue("button");
            
            while (processQueue.TryDequeue(out var moduleName))
            {
                var (_, pulseType) = modules[moduleName].Process(modules, processQueue);

                if (rxConjunctions.TryGetValue(moduleName, out var value)
                    && value == 0
                    && pulseType == PulseType.High) 
                    rxConjunctions[moduleName] = i;
            }
            
            if (rxConjunctions.All(kvp => kvp.Value != 0))
                                 break;
        }

        return rxConjunctions.Values.Aggregate(1L, (a, b) => a = FindLCM(a, b));
    }
    
    private static long FindLCM(long a, long b)
    {
        return a / FindGCD(a, b) * b;
    }

    private static long FindGCD(long a, long b)
    {
        while (b != 0)
        {
            var temp = b;
            b = a % b;
            a = temp;
        }

        return a;
    }

    private abstract record Module(
        string Name,
        Queue<(string source, PulseType pulseType)> IncomingPulses,
        List<string> OutgoingPulses,
        Dictionary<string, PulseType> LastReceivedPulses)
    {
        protected bool FlipFlopState { get; set; }
        protected Module(string name) :
            this(
                name,
                new Queue<(string source, PulseType pulseType)>(),
                new List<string>(),
                new Dictionary<string, PulseType>())
        { }
        public abstract (long pulsesSent, PulseType pulseType) Process(Dictionary<string, Module> modules, Queue<string> processQueue);
    }
    
    private record FlipFlopModule(string Name) : Module(Name)
    {
        public override (long pulsesSent, PulseType pulseType) Process(Dictionary<string, Module> modules, Queue<string> processQueue)
        {
            var pulsesSent = 0L;
            var pulseType = PulseType.Low;

            if (!IncomingPulses.TryDequeue(out var pulse) || pulse.pulseType != PulseType.Low)
                return (pulsesSent, pulseType);
            
            FlipFlopState = !FlipFlopState;
            pulseType = FlipFlopState ? PulseType.High : PulseType.Low;
                
            foreach (var outgoing in OutgoingPulses)
            {
                modules[outgoing].IncomingPulses.Enqueue((Name, pulseType));
                processQueue.Enqueue(outgoing);
                pulsesSent++;
            }

            return (pulsesSent, pulseType);
        }
    }

    private record ConjunctionModule(string Name) : Module(Name)
    {
        public override (long pulsesSent, PulseType pulseType) Process(Dictionary<string, Module> modules, Queue<string> processQueue)
        {
            var pulsesSent = 0L;
            var pulseType = PulseType.Low;

            if (!IncomingPulses.TryDequeue(out var pulse))
                return (pulsesSent, pulseType);

            LastReceivedPulses[pulse.source] = pulse.pulseType;
            pulseType = LastReceivedPulses.Values.All(p => p == PulseType.High) ? PulseType.Low : PulseType.High;

            foreach (var outgoing in OutgoingPulses)
            {
                modules[outgoing].IncomingPulses.Enqueue((Name, pulseType));
                processQueue.Enqueue(outgoing);
                pulsesSent++;
            }
            
            return (pulsesSent, pulseType);
        }
    }

    private record BroadcastModule(string Name) : Module(Name)
    {
        public override (long pulsesSent, PulseType pulseType) Process(Dictionary<string, Module> modules, Queue<string> processQueue)
        {
            var pulsesSent = 0L;
            var pulseType = PulseType.Low;

            if (!IncomingPulses.TryDequeue(out var pulse))
                return (pulsesSent, pulseType);

            pulseType = pulse.pulseType;
            
            foreach (var outgoing in OutgoingPulses)
            {
                modules[outgoing].IncomingPulses.Enqueue((Name, pulseType));
                processQueue.Enqueue(outgoing);
                pulsesSent++;
            }
            
            return (pulsesSent, pulseType);
        }
    }

    private record NoopModule(string Name) : Module(Name)
    {
        public override (long pulsesSent, PulseType pulseType) Process(Dictionary<string, Module> modules, Queue<string> processQueue)
        {
            return (0, PulseType.Low);
        }
    }
    
    private enum PulseType { High, Low }
}