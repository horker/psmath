using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horker.DataAnalysis
{
    namespace Expression
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

        public class Expression : Stem
        {
            public Expression(Node lhs, Node rhs)
                : base(lhs, rhs)
            {
            }
        }
    }

    public class ExpressionParser
    {
        public ExpressionParser()
        {
        }

        public Expression.Expression Parse(string expressionString)
        {
            return null;
        }
    }
}
