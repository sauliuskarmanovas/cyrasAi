using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.IO;


namespace Forward
{
    class Program
    {
        static StreamWriter wr;
        static Dictionary<char,int> goalsNodeId;
        static Dictionary<int,int> rulesNodeId;

        static List<int> pa = new List<int>();
        static void Main(string[] args)
        {
            Console.WriteLine("Enter file name:");
            List<Rule> rules = new List<Rule>();
            List<char> facts; 
            char goal;

            using (var reader = new StreamReader(Console.ReadLine())) 
            {
                try
                {
                    goal = reader.ReadLine()[0];
                    facts = new List<char>(reader.ReadLine());
                    while (!reader.EndOfStream)
                    {
                        var c = reader.ReadLine().Split(' ');
                        if(c.Count() != 2)
                        {
                            Console.WriteLine("Invalid rule format");
                            return;
                        }
                        rules.Add(new Rule { product = c.Last()[0], recipe = new List<char>(c.First()) });
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error reading the file");
                    return;
                }
            }

            Console.WriteLine($"Tikslas: {goal}");

            Console.WriteLine($"Faktai: ");
            facts.ForEach(p => Console.Write($"{p} "));
            Console.WriteLine();

            Console.WriteLine($"Taisyklės: ");

            rules.ForEach(p => Console.WriteLine($"\tR{rules.IndexOf(p)+1}:\t {p}"));
            Console.WriteLine();

            SolveForward(rules, facts, goal);
            Console.WriteLine();
            Console.WriteLine();

            goalsNodeId = new Dictionary<char, int>();
            rules.ForEach(r => { goalsNodeId[r.product] = 0; r.recipe.ForEach(rr => goalsNodeId[rr] = 0); });
            goalsNodeId[goal]= 0;
            facts.ForEach(r => goalsNodeId[r] = 0);

            rulesNodeId = new Dictionary<int, int>();

            for (int i = 0; i < rules.Count; i++)
            {
                rulesNodeId[i]= 0;
            }

            using (wr = new StreamWriter("gr.txt"))
            {
                wr.Write(@"digraph a{");
                if(Prove(rules, facts, goal))
                {
                    Console.WriteLine("Goal was proved");
                    Console.Write($"Rules: ");
                    pa.ForEach(p => Console.Write($"R{p} "));
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("Goal can't be proved");
                }
                wr.Write(@"}");

            }
            //Console.ReadKey(true);

            System.Diagnostics.Process.Start("dot", $@"-Tpng ""{Directory.GetCurrentDirectory()}\gr.txt"" -o ""{Directory.GetCurrentDirectory()}\gr.png""");

            System.Threading.Thread.Sleep(1000);
            var pic = new PictureBox() { ImageLocation = @"gr.png", Dock = DockStyle.Fill };
            var f = new Form() { };
            pic.Load();
            f.Controls.Add(pic);
            f.Width = pic.Image.Width + 50; f.Height = pic.Image.Height + 50;
            
            f.ShowDialog();

            return;
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
        

        private static bool Prove(List<Rule> rules, List<char> facts, char goal, string dirtyGoals = "", List<char> provedFacts = null, int depth = 0)
        {
            var goalID = $"{goal}_{goalsNodeId[goal]++}";
            wr.WriteLine($"{goalID} [dir=back]");
            wr.WriteLine($"{ goalID} [label=\"{goal}\" shape=square]");


            provedFacts = provedFacts ?? new List<char>();
            if (dirtyGoals.Contains(goal))
            {
                Console.WriteLine(new string(' ', depth)+"Cycle detected");
                wr.WriteLine($"{ goalID} [fillcolor=\"#ffff00\" style=filled]");
                return false;
            }
            if (facts.Contains(goal))
            {
                Console.WriteLine(new string(' ', depth)+$"Original facts contain goal {goal}");
                wr.WriteLine($"{ goalID} [fillcolor=\"#00ff00\" style=filled]");
                return true;
            }
            if (provedFacts.Contains(goal)) {
                Console.WriteLine(new string(' ', depth)+$"Proved facts contain goal {goal}");
                return true;
            }
            var selectedRule = rules.FirstOrDefault(rule => {
                var ruleID = $"R{ rules.IndexOf(rule) + 1}_{rulesNodeId[rules.IndexOf(rule)]++}";
                wr.WriteLine($"{goalID} -> {ruleID} [dir=back]");
                wr.WriteLine($"{ruleID} [label=\"R{rules.IndexOf(rule) + 1}\" shape=circle]");


                Console.WriteLine(new string(' ', depth)+(rule.product == goal ? "Trying" : "Skipping") + $" rule R{rules.IndexOf(rule) + 1}");


                var result = rule.product == goal && rule.recipe.All(r => {
                    wr.Write($"{ruleID} -> ");
                    return Prove(rules, facts, r, dirtyGoals + goal, provedFacts, depth + 1);
                    });
                if (!result)
                    wr.WriteLine($"{ruleID} [fillcolor=red style=filled]");
                else
                    pa.Add(rules.IndexOf(rule) + 1);
                return result;

                });
            if (selectedRule == null)
            {
                Console.WriteLine(new string(' ', depth) + $"All rules exhausted FAIL");
                wr.WriteLine($"{ goalID} [fillcolor=red style=filled]");
                return false;
            }
            Console.WriteLine(new string(' ', depth)+$"Proved new fact {goal}");
            provedFacts.Add(selectedRule.product);
            return true;
        }
    }

}
