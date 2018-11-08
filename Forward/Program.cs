using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forward
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Rule> rules = new List<Rule>{
                new Rule {product = 'L',recipe = new List<char>{'A'}},
                new Rule {product = 'K',recipe = new List<char>{'L'}},
                new Rule {product = 'A',recipe = new List<char>{'D'}},
                new Rule {product = 'M',recipe = new List<char>{'D'}},
                new Rule {product = 'Z',recipe = new List<char>{'F','B'}},
                new Rule {product = 'F',recipe = new List<char>{'C','D'}},
                new Rule {product = 'D',recipe = new List<char>{'A'}},
            };
            List<char> facts = new List<char>("ABC".ToList());
            char goal = 'Z';
            SolveForward(rules, facts, goal);
            Console.ReadKey(true);
            SolveBackward(rules, facts, goal);
            Console.ReadKey(true);

            rules = new List<Rule>{
                new Rule {product = 'Z',recipe = new List<char>{'D', 'C'}},
                new Rule {product = 'D',recipe = new List<char>{'C'}},
                new Rule {product = 'C',recipe = new List<char>{'B'}},
                new Rule {product = 'B',recipe = new List<char>{'A'}},
                new Rule {product = 'A',recipe = new List<char>{'D'}},
                new Rule {product = 'D',recipe = new List<char>{'T'}},
                new Rule {product = 'A',recipe = new List<char>{'G'}},
                new Rule {product = 'B',recipe = new List<char>{'H'}},
                new Rule {product = 'C',recipe = new List<char>{'J'}},
            };
            facts = new List<char>("T".ToList());
            goal = 'Z';
            SolveForward(rules, facts, goal);
            Console.ReadKey(true);
            SolveBackward(rules, facts, goal);
            Console.ReadKey(true);

            //rules = new List<Rule>{
            //    new Rule {product = 'Z',recipe = new List<char>{'D'}},
            //    new Rule {product = 'D',recipe = new List<char>{'C'}},
            //    new Rule {product = 'C',recipe = new List<char>{'B'}},
            //    new Rule {product = 'B',recipe = new List<char>{'A'}},
            //    new Rule {product = 'G',recipe = new List<char>{'A'}},
            //    new Rule {product = 'Z',recipe = new List<char>{'G'}},
            //};
            //facts = new List<char>("A".ToList());
            //goal = 'Z';
            //SolveForward(rules, facts, goal);
            //Console.ReadKey(true);


            //rules = new List<Rule>{
            //};
            //facts = new List<char>("A".ToList());
            //goal = 'A';
            //SolveForward(rules, facts, goal);
            //Console.ReadKey(true);
        }

        private static void SolveForward(List<Rule> rules, List<char> facts, char goal)
        {
            rules.ForEach(r => r.used = false);

            var startingFacts = facts;
            facts = new List<char>(facts);
            while (!facts.Contains(goal))
            {
                if (rules.Count == 0)
                {
                    Console.WriteLine("Goal is unreachable");
                    return;
                }
                int index = 0;
                int count = rules.Count;
                for (; index < rules.Count; index++)
                {
                    Rule rule = rules[index];
                    if (rule.used)
                        continue;
                    if (facts.Contains(rule.product))
                    {
                        rule.used = true;
                    }
                    if (rule.recipe.All(i => facts.Contains(i)))
                    {
                        facts.Add(rule.product);
                        Console.WriteLine($"Added fact '{rule.product}' By using rule {rule.ToString()}");
                        rule.used = true;
                        break;
                    }
                }
                if (index == count)
                {
                    Console.WriteLine("Goal is unreachable, none of the rules moved forward");
                    return;
                }
            }
            Console.WriteLine($"Goal {goal} was reached");
        }

        private static void SolveBackward(List<Rule> rules, List<char> facts, char goal)
        {
            rules.ForEach(r => r.used = false);
            var startingFacts = facts;
            facts = new List<char>(facts);
            Stack<char> goalStack = new Stack<char>();
            goalStack.Push(goal);
            for (char currentgoal = goalStack.Pop();
                facts.Any(f => f == currentgoal);
                currentgoal = goalStack.Pop())
            {
                //char currentgoal = goalStack.Pop();
                foreach (var rule in rules)
                    if (rule.product == currentgoal)
                    {
                        rule.recipe.ForEach(a => goalStack.Push(a));
                        rule.used = true;
                        break;
                    }

                if(goalStack.Count == 0)
                {
                    Console.WriteLine("Goal is Unreachable");
                    return;
                }
            }
            Console.WriteLine("goal reached");
        }
    }
}
