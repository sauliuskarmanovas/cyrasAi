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
            Stack<List<Rule>> ruleState = new Stack<List<Rule>>();
            ruleState.Push(new List<Rule>());
            Stack<List<char>> goalState = new Stack<List<char>>();
            goalState.Push(new List<char> { goal });
            var currentRuleState = ruleState.Pop();
            var currentGoalState = goalState.Pop();
            try
            {
                while (currentGoalState.Count != 0)
                {
                    var newruleState = new List<List<Rule>>();
                    var newgoalState = new List<List<char>>();
                    foreach (var rule in rules)
                    {
                        if (currentRuleState.Contains(rule))
                        {
                            for (int i = 0; i < currentRuleState.Count; i++)
                                Console.Write($" ");
                            Console.WriteLine($"Skipping R{rules.IndexOf(rule)+1}. Already used.");
                        }
                        else
                        {
                            if (!currentGoalState.Contains(rule.product))
                            {
                                for (int i = 0; i < currentRuleState.Count; i++)
                                    Console.Write($" ");
                                Console.WriteLine($"Skipping R{rules.IndexOf(rule) + 1}. Goals don't contain rule product.");
                            }
                            else
                            {
                                var addGoal = new List<char>(currentGoalState);
                                var addRule = new List<Rule>(currentRuleState);
                                addGoal.Remove(rule.product);
                                if (rule.recipe.All(r => facts.Contains(r)))
                                {
                                    //closed end
                                    for (int i = 0; i < currentRuleState.Count; i++)
                                        Console.Write($" ");
                                    Console.WriteLine($"Using R{rules.IndexOf(rule) + 1}. No new goals.");
                                }
                                else
                                {
                                    addRule.Add(rule);
                                    for (int i = 0; i < currentRuleState.Count; i++)
                                        Console.Write($" ");
                                    Console.Write($"Using R{rules.IndexOf(rule) + 1}. Adding goals: ");
                                    foreach (var rec in rule.recipe.Where(r => !facts.Contains(r)))
                                    {
                                        addGoal.Add(rec);
                                        Console.Write($"{rec} ");
                                    }
                                    Console.WriteLine();
                                }
                                newruleState.Add(addRule);
                                newgoalState.Add(addGoal);
                            }
                        }
                    }
                    foreach (var r in newruleState.AsEnumerable().Reverse())
                        ruleState.Push(r);
                    foreach (var g in newgoalState.AsEnumerable().Reverse())
                        goalState.Push(g);

                    currentRuleState = ruleState.Pop();
                    currentGoalState = goalState.Pop();
                }
            } catch (InvalidOperationException)
            {
                Console.WriteLine("Goal is unreachable, none of the rules moved forward");

            }
            Console.Write($"Goal '{goal}' reached.");
            foreach (var r in currentRuleState)
                Console.Write($" R{rules.IndexOf(r) + 1}");
            Console.WriteLine();
        }
    }
}
