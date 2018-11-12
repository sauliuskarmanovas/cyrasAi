using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forward
{
    public class Rule
    {
        public State used = State.Open;
        public char product;
        public List<char> recipe = new List<char>();
        public override string ToString()
        {
            string s = "";
            foreach (var item in recipe)
            {
                s += $"'{item}'->";
            }
            return s += $"'{product}'";
        }
    }public enum State
    {
        Open, Used, Cons
    }
}
