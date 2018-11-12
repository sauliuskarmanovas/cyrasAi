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
            rules.ForEach(r => r.used = State.Open);

            var startingFacts = facts;
            facts = new List<char>(facts);
            int iter = 0;
            List<Rule> path = new List<Rule>();
            while (!facts.Contains(goal))
            {
                Console.WriteLine($"{++iter} ITERACIJA");
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
                    if (rule.used != State.Open)
                    {
                        Console.WriteLine($"\tR{rules.IndexOf(rule)+1}:{rule} praleidžiama, nes pekelta flag{(int) rule.used}");
                        continue;
                    }
                    if (facts.Contains(rule.product))
                    {
                        rule.used = State.Cons;
                        Console.WriteLine($"\tR{rules.IndexOf(rule) + 1}:{rule} netaikoma, nes konsekventas faktuose. Pakeliama flag{(int)rule.used} ");
                        continue;
                    }
                    if (rule.recipe.All(i => facts.Contains(i)))
                    {
                        facts.Add(rule.product);
                        path.Add(rule);
                        rule.used = State.Used;
                        Console.Write($"\tR{rules.IndexOf(rule) + 1}:{rule} taikoma, Pakeliama flag{(int)rule.used} ");
                        startingFacts.ToList().ForEach(r => Console.Write($"{r} "));
                        Console.Write("ir ");

                        facts.Where(r => !startingFacts.Contains(r)).ToList().ForEach(r => Console.Write($"{r} "));
                        Console.WriteLine();


                        break;
                    }
                    else
                    {
                        Console.Write($"\tR{rules.IndexOf(rule) + 1}:{rule} netaikoma, nes trūksta ");
                        rule.recipe.Where(r => !facts.Contains(r)).ToList().ForEach(r => Console.Write($"{r} "));
                        Console.WriteLine();
                    }
                }
                if (index == count)
                {
                    Console.WriteLine("Tikslas nepasiekiamas");
                    return;
                }
            }
            Console.WriteLine($"Tikslas {goal} gautas");
            Console.Write($"Kelias ");
            path.ForEach(p => Console.Write($"R{rules.IndexOf(p)+1} "));
            Console.WriteLine();

        }

        private static void SolveBackward(List<Rule> rules, List<char> facts, char goal)
        {
            rules.ForEach(r => r.used = State.Open);
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
                        rule.used = State.Used;
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
