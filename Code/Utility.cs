using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SlagalicaPC
{
    public class Stack
    {
        public string[] stack;
        private int max;
        public int top;

        public bool IsEmpty()
        {
            if (top < 0) return true;
            return false;
        }

        public Stack(int max)
        {
            this.max = max;
            this.stack = new string[max];
            this.top = -1;
        }

        public void Push(string c)
        {
            if(top>=max)
            {
                MessageBox.Show("Stack Overflow!");
                return;
            }
            else
            {
                stack[++top] = c;
            }
        }

        public string Pop()
        {
            if (top < 0)
            {
                return "N";
            }
            else
            {
                return stack[top--];
            }

        }

        public string Peek()
        {
            if (top < 0)
            {
                return "N";
            }
            else
            {
                return stack[top];
            }
        }
    }
    public static class Utility
    {
        public static IEnumerable<T> GetChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in GetChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        //od stringa izraza pravi niz stringova gde je svaki broj ili operator jedan clan "123" "+" "100" "-" "20" =203
        public static string[] ToExpressionInput(string input) 
        {
            string[] x = new string[input.Length];
            int i = 0, cnt=0; 
            while(i<input.Length)
            {
                if(IsOperator(input[i].ToString()))
                {
                    x[cnt++] = input[i++].ToString();
                }
                else
                {
                    string temp = "";
                    while(i<input.Length && !IsOperator(input[i].ToString()))
                    {
                        temp += input[i++].ToString();
                    }
                    x[cnt++] = temp;
                }
            }
            return x;
        }

        private static bool IsOperator(string c)
        {
            if (c == "+" || c == "*" || c == "+" || c == "-" || c == "(" || c == ")" || c=="/")
                return true;
            return false;
        }
        private static int Priority(char c)
        {
            switch(c)
            {
                case '+': return 2;
                case '-': return 2;
                case '*': return 3;
                case '/': return 3;
                default: return -1;
            }
        }

        private static int Calculate(int op1, int op2, string opr)
        {

            try
            {
                if (opr == "*")
                    return op1 * op2;
                if (opr == "+")
                    return op1 + op2;
                if (opr == "/")
                {

                    if (op1 % op2 == 0)
                        return op1 / op2;
                    else
                        throw new Exception("cannot calculate");
                }
                if (opr == "-")
                    return op1 - op2;
            }
            catch
            {
                throw new Exception("cannot calculate");
            }

            throw new Exception("cannot calculate");
        }

        private static string[] IN2POST(string expr)
        {
            string[] input = ToExpressionInput(expr);
            Stack s = new Stack(1000);

            string[] postfix = new string[input.Length];
            int i = 0, cnt = 0;
            try
            {
                while (i < input.Length && input[i]!=null)
                {
                    if (!IsOperator(input[i]))
                    {
                        postfix[cnt++]=input[i];
                    }
                    else if(input[i]=="(")
                    {
                        s.Push(input[i]);
                    }
                    else if(input[i]==")")
                    {
                        while(s.Peek()!="N" && s.Peek() != "(")
                        {
                            string temp = s.Pop();
                            postfix[cnt++] = temp;
                        }
                        if (s.Peek() == "(")
                            s.Pop();
                    }
                    else
                    {
                        while (s.Peek() != "N"  && (Priority(s.Peek()[0]) >= Priority(input[i][0])))
                        {
                            string temp = s.Pop();
                            postfix[cnt++] = temp;
                        }
                        
                        s.Push(input[i]);
                        
                    }

                    i++;

                }
                while (s.Peek() != "N")
                {
                    string temp = s.Pop();
                    if (temp != "(")
                        postfix[cnt++] = temp;
                }
                return postfix;
            }
            catch
            {
                return null;
            }
        }

        public static double Evaluate(string expression)
        {
            

            Stack s = new Stack(1000);
            int i = 0;
            string[] postfix = IN2POST(expression);
            if(postfix==null)
            {
                throw new Exception();
            }
            try
            {
                while (i < postfix.Length)
                {
                    var x = postfix[i++];
                    if (x == null) break;
                    if (!IsOperator(x))
                    {
                        s.Push(x);
                    }
                    else
                    {
                        int op2 = Int32.Parse(s.Pop());
                        int op1 = Int32.Parse(s.Pop());

                        try
                        {
                            int res = Calculate(op1, op2, x);
                            s.Push(res.ToString());
                        }
                        catch(Exception ex)
                        {
                            throw ex;
                        }
                        //if (res == -999) return -999;
                        
                    }
                }
                return Double.Parse(s.Pop());
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static double Evaluate(string[] expression)
        {
            Stack s = new Stack(1000);
            int i = 0;
            string[] postfix = expression;
            if (postfix == null)
            {
                throw new Exception();
            }
            try
            {
                while (i < postfix.Length)
                {
                    var x = postfix[i++];
                    if (x == null) break;
                    if (!IsOperator(x))
                    {
                        s.Push(x);
                    }
                    else
                    {
                        int op2 = Int32.Parse(s.Pop());
                        int op1 = Int32.Parse(s.Pop());

                        try
                        {
                            int res = Calculate(op1, op2, x);
                            s.Push(res.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        //if (res == -999) return -999;

                    }
                }
                return Double.Parse(s.Pop());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

   
}

   

