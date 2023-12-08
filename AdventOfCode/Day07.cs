namespace AdventOfCode;

public partial class Day07 : BaseDay
{
    private readonly string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 1: {Part01()}");

    public override ValueTask<string> Solve_2() => new($"Solution to {ClassPrefix} {CalculateIndex()}, part 2: {Part02()}");

    private long Part01()
    {
        var cards = new [] {'2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J' , 'Q', 'K', 'A'};
        
        var handsWithBid = _input.Select(line =>
        {
            var split = line.Split(' ' );
            var hand = split[0];
            var bid = long.Parse(split[1]);
            return new HandWithBid(hand, bid);
        }).ToArray();

        var orderedHands = handsWithBid
            .Select(hwb => new HandWithBidAndStrength(hwb.Hand, hwb.Bid, StrengthOfHand(cards, hwb.Hand)))
            .OrderByDescending(h => h.Strength)
            .ThenBy(h => Array.IndexOf(cards, h.Hand[0]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[1]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[2]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[3]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[4]))
            .ToArray();
        
        var wins = orderedHands.Select((h, i) => h.Bid * (i + 1)).Sum();
        
        return wins;
    }
    
    private static int StrengthOfHand(char[] cards, string hand)
    {
        if (cards.Any(c => hand.All(h => h == c)))
        {
            return 1;
        }

        if (cards.Any(c => hand.Count(h => h == c) == 4))
        {
            return 2;
        }

        if (cards
            .Any(c1 => cards
                .Where(c2 => c1 != c2)
                .Any(c2 => hand.Count(h => h == c1) == 3 && hand.Count(h => h == c2) == 2)))
        {
            return 3;
        }
        
        if (cards.Any(c => hand.Count(h => h == c) == 3))
        {
            return 4;
        }
        
        if (cards
            .Any(c1 => cards
                .Where(c2 => c1 != c2)
                .Any(c2 => hand.Count(h => h == c1) == 2 && hand.Count(h => h == c2) == 2)))
        {
            return 5;
        }
        
        if (cards.Any(c => hand.Count(h => h == c) == 2))
        {
            return 6;
        }

        return 7;
    }

    private long Part02()
    {
        var cards = new [] {'J', '2', '3', '4', '5', '6', '7', '8', '9', 'T' , 'Q', 'K', 'A'};
        
        var handsWithBid = _input.Select(line =>
        {
            var split = line.Split(' ' );
            var hand = split[0];
            var bid = long.Parse(split[1]);
            return new HandWithBid(hand, bid);
        }).ToArray();

        var orderedHands = handsWithBid.Select(handWithBid =>
        {
            var jokers = handWithBid.Hand
                .Select((c, i) => (c, i))
                .Where(c => c.c == 'J')
                .Select(c => c.i)
                .ToArray();
            
            var hands = Jokerize(handWithBid.Hand, jokers, cards)
                .ToArray()
                .Select(h => (Hand: h, Strength: StrengthOfHand(cards, h)))
                .OrderByDescending(h => h.Strength)
                .ThenBy(h => Array.IndexOf(cards, h.Hand[0]))
                .ThenBy(h => Array.IndexOf(cards, h.Hand[1]))
                .ThenBy(h => Array.IndexOf(cards, h.Hand[2]))
                .ThenBy(h => Array.IndexOf(cards, h.Hand[3]))
                .ThenBy(h => Array.IndexOf(cards, h.Hand[4]))
                .ToArray();
            
            return new HandWithBidAndStrength(handWithBid.Hand, handWithBid.Bid, hands[^1].Strength);
        })
            .OrderByDescending(h => h.Strength)
            .ThenBy(h => Array.IndexOf(cards, h.Hand[0]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[1]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[2]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[3]))
            .ThenBy(h => Array.IndexOf(cards, h.Hand[4]))
            .ToArray();
        
        var wins = orderedHands.Select((h, i) => h.Bid * (i + 1)).Sum();
        
        return wins;
    }

    private IEnumerable<string> Jokerize(string hand, int[] positionsOfJokers, char[] cards)
    {
        if (positionsOfJokers.Length == 0)
        {
            yield return hand;
        }
        else
        {
            foreach (var card in cards.Where(c => c != 'J'))
            {
                var jokerHand = hand.ToCharArray();
                jokerHand[positionsOfJokers[0]] = card;
                var hand2 = new string(jokerHand);
                
                foreach (var j in Jokerize(hand2, positionsOfJokers[1..], cards))
                    yield return j;
            }
        }
    }
    
    private record HandWithBid(string Hand, long Bid);
    private record HandWithBidAndStrength(string Hand, long Bid, int Strength);
}