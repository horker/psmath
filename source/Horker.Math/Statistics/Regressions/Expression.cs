using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.Math
{
    namespace Formula
    {
        public class Node
        {
            private string _expr;

            public Node()
            {
            }
        }

        public class Leaf : Node
        {
        }

        public class Stem : Node
        {
            public Node Left;
            public Node Right;

            public Stem(Node left, Node right)
            {
                Left = left;
                Right = right;
            }
        }

        public class Variable : Leaf
        {
            public string Name;
        }

        public class Immediate : Leaf
        {
            public double Value;

            public Immediate(double value)
            {
                Value = value;
            }
        }

        public class Plus : Stem
        {
            public Plus(Node left, Node right)
                : base(left, right)
            {
            }

        }

        public class Exp : Stem
        {
            public Exp(Node baseValue, Immediate exponent)
                : base(baseValue, exponent)
            {
            }

        }

        public class Formula : Stem
        {
            public Formula(Node lhs, Node rhs)
                : base(lhs, rhs)
            {
            }
        }

        public class FormulaParser
        {
            public FormulaParser()
            {
            }

            public Formula Parse(string formulaString)
            {
                return null;
            }
        }
    }
}