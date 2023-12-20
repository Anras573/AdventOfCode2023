namespace AdventOfCode;

public partial class Day19 : BaseDay
{
    private readonly string[] _input;

    public Day19()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");
    
    private long Part01()
    {
        var workflows = new Dictionary<string, Workflow>();
        var parts = new List<Part>();

        var isWorkflow = true;
        
        foreach (var line in _input)
        {
            if (string.IsNullOrEmpty(line))
            {
                isWorkflow = false;
                continue;
            }

            if (isWorkflow)
            {
                var workflow = Workflow.Parse(line);
                workflows.Add(workflow.Name, workflow);
            }
            else
                parts.Add(Part.Parse(line));
        }

        var accepted = new List<Part>();
        
        var firstWorkflow = workflows["in"];

        foreach (var part in parts)
        {
            var currentWorkflow = firstWorkflow;
            
            while (true)
            {
                var result = currentWorkflow.Run(part);

                if (result == "A")
                {
                    accepted.Add(part);
                    break;
                }
                if (result == "R")
                        break;

                currentWorkflow = workflows[result];
            }
        }
        
        return accepted.Sum(a => a.X + a.M + a.A + a.S);
    }
    
    private long Part02()
    {
        var workflows = _input
            .TakeWhile(line => !string.IsNullOrEmpty(line))
            .Select(Workflow.Parse)
            .ToDictionary(workflow => workflow.Name);

        var initialParts = new Dictionary<string, Range>
        {
            { "X", new Range(1, 4000) },
            { "M", new Range(1, 4000) },
            { "A", new Range(1, 4000) },
            { "S", new Range(1, 4000) }
        };

        var completedRanges = new List<Dictionary<string, Range>>();

        var queue = new Queue<(Dictionary<string, Range> parts, string workflowName, int steps)>();
        queue.Enqueue((initialParts, "in", 0));
        
        while (queue.Count > 0)
        {
            var (parts, workflowName, step) = queue.Dequeue();
            
            var workflow = workflows[workflowName];
            var rule = workflow.Rules[step];

            foreach (var range in SplitRange(parts, rule, workflowName, step))
            {
                if (range.workflowName == "A")
                    completedRanges.Add(range.parts);
                else if (range.workflowName != "R")
                    queue.Enqueue((range.parts, range.workflowName, range.steps));
            }
        }

        var sum = 0L;
        foreach (var completed in completedRanges)
        {
            var x = Convert.ToInt64(completed["X"].End.Value - completed["X"].Start.Value + 1);
            var m = Convert.ToInt64(completed["M"].End.Value - completed["M"].Start.Value + 1);
            var a = Convert.ToInt64(completed["A"].End.Value - completed["A"].Start.Value + 1);
            var s = Convert.ToInt64(completed["S"].End.Value - completed["S"].Start.Value + 1);
            
            sum += x * m * a * s;
        }
        
        return sum;

        List<(Dictionary<string, Range> parts, string workflowName, int steps)> SplitRange(
            Dictionary<string, Range> parts, Rule rule, string workflowName, int step)
        {
            var result = new List<(Dictionary<string, Range> parts, string workflowName, int steps)>();
            
            var passed = parts.ToDictionary(e => e.Key, e => e.Value);
            var failed = parts.ToDictionary(e => e.Key, e => e.Value);
            
            if (string.IsNullOrEmpty(rule.Identifier))
            {
                result.Add((parts, rule.Result, 0));
            }
            else
            {
                var low = parts[rule.Identifier].Start.Value;
                var high = parts[rule.Identifier].End.Value;
                
                if (rule.IsGreaterThan)
                {
                    passed[rule.Identifier] = new Range(rule.Value + 1, high);
                    failed[rule.Identifier] = new Range(low, rule.Value);
                }
                else
                {
                    passed[rule.Identifier] = new Range(low, rule.Value - 1);
                    failed[rule.Identifier] = new Range(rule.Value, high);
                }
                
                result.Add((passed, rule.Result, 0));
                result.Add((failed, workflowName, step + 1));
            }

            return result;
        }
    }

    private record Part(int X, int M, int A, int S)
    {
        public static Part Parse(string line)
        {
            var parts = line[1..^1]
                .Split(",")
                .Select(p => p.Split("=")[1])
                .ToArray();
            
            return new Part(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
        }
    }
    
    private record Workflow(string Name, List<Rule> Rules)
    {
        public static Workflow Parse(string line)
        {
            var delimiter = line.IndexOf('{');
            var name = line[..delimiter];
            var parts = line[(delimiter + 1)..^1].Split(",");

            var rules = parts.Select(Rule.Parse).ToList();
            
            return new Workflow(name, rules);
        }

        public string Run(Part part)
        {
            var result = string.Empty;
            foreach (var rule in Rules)
            {
                var partValue = rule.Identifier switch
                {
                    "X" => part.X,
                    "M" => part.M,
                    "A" => part.A,
                    "S" => part.S,
                    _ => 0
                };
                
                if (rule.TryValidate(partValue, out var r))
                {
                    result = r;
                    break;
                }
            }

            return result;
        }
    }

    private record Rule(string Identifier, bool IsGreaterThan, int Value, string Result)
    {
        public static Rule Parse(string line)
        {
            try
            {
                if (!line.Contains(':'))
                    return new Rule("", false, 0, line);

                var identifier = line[0].ToString().ToUpper();
                var isGreaterThan = line[1] == '>';
                var delimiter = line.IndexOf(':');
                var value = int.Parse(line[2..delimiter]);
                var result = line[(delimiter + 1)..];

                return new Rule(identifier, isGreaterThan, value, result);
            }
            catch
            {
                throw new Exception(line);
            }
            
        }
        
        public bool TryValidate(int input, out string result)
        {
            if (string.IsNullOrEmpty(Identifier))
            {
                result = Result;
                return true;
            }
            
            switch (IsGreaterThan)
            {
                case true when input > Value:
                    result = Result;
                    return true;
                case false when input < Value:
                    result = Result;
                    return true;
                default:
                    result = string.Empty;
                    return false;
            }
        }
    }
}