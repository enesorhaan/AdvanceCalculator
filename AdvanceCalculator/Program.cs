using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdvanceCalculator
{

    class Program
    {
        static void Main()
        {
            Console.WriteLine("Hesap Makinesine Hoş Geldiniz!");
            Console.WriteLine("Çıkmak için 'exit' yazabilirsiniz.");

            while (true)
            {
                Console.Write("İfadeyi girin: ");
                string expression = Console.ReadLine();

                if (expression.ToLower() == "exit")
                {
                    Console.WriteLine("Hesap makinesi kapatılıyor...");
                    Console.ReadLine();
                    break;
                }

                try
                {
                    double result = EvaluateExpression(expression);
                    Console.WriteLine("Sonuç: " + result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
        }

        static List<String> expressionOrganizer(List<String> inputList)
        {

            for (int i = 0; i < inputList.Count(); i++)
            {
                String value = inputList[i];

                // Check if the value is a function (starts with "sin", "cos", "tan", or "pow")
                if (value.StartsWith("sin") || value.StartsWith("cos") || value.StartsWith("tan") || value.StartsWith("pow"))
                {
                    // Calculate the function value using the functionCalculator method
                    String result = FunctionCalculator(value);

                    // Replace the function with its result in the list
                    inputList[i] = result;
                }

            }
            return inputList;
        }

        static double EvaluateExpression(string expression)
        {
            expression = expression.Replace(" ", "");
            List<string> tokens = TokenizeExpression(expression);
            List<String> updatedTokens = expressionOrganizer(tokens);

            Stack<double> values = new Stack<double>();
            Stack<string> operators = new Stack<string>();

            foreach (string token in tokens)
            {
                if (double.TryParse(token, out double number))
                {
                    values.Push(number);
                }
                else if (IsOperator(token))
                {
                    while (operators.Count > 0 && IsOperator(operators.Peek()) && GetPrecedence(token) <= GetPrecedence(operators.Peek()))
                    {
                        values.Push(ApplyOperation(operators.Pop(), values.Pop(), values.Pop()));
                    }
                    operators.Push(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        values.Push(ApplyOperation(operators.Pop(), values.Pop(), values.Pop()));
                    }
                    operators.Pop();
                }
                else
                {
                    throw new InvalidOperationException("Geçersiz ifade: " + token);
                }
            }

            while (operators.Count > 0)
            {
                values.Push(ApplyOperation(operators.Pop(), values.Pop(), values.Pop()));
            }

            return values.Pop();
        }


        //****************

        static string FunctionCalculator(string value)
        {
            string func = value.ToLower();
            if (func.StartsWith("sin") || func.StartsWith("cos") || func.StartsWith("tan"))
            {
                string funcName = func.Substring(0, 3);
                int valInsideP = 0;
                int startIndex = func.IndexOf('(');
                int endIndex = func.IndexOf(')');

                if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                {
                    // Extract the value between parentheses
                    string valueInsideParentheses = func.Substring(startIndex + 1, endIndex - startIndex - 1);
                    valInsideP = int.Parse(valueInsideParentheses);
                    Console.WriteLine("Value inside parentheses: " + valueInsideParentheses);
                }
                else
                {
                    Console.WriteLine("No value found inside 'sin' parentheses.");
                }

                switch (funcName)
                {
                    case "sin":
                        return Math.Sin(valInsideP).ToString();
                    case "cos":
                        return Math.Cos(valInsideP).ToString();
                    case "tan":
                        return Math.Tan(valInsideP).ToString();
                }
            }
            else if (func.StartsWith("pow"))
            {
                int startIndex = func.IndexOf('(');
                int endIndex = func.IndexOf(')');

                if (startIndex != -1 && endIndex != -1 && endIndex > startIndex)
                {
                    // Extract the content between parentheses
                    string contentInsideParentheses = func.Substring(startIndex + 1, endIndex - startIndex - 1);

                    // Split the content by comma to get the two values
                    string[] values = contentInsideParentheses.Split(',');

                    if (values.Length == 2)
                    {
                        try
                        {
                            int value1 = int.Parse(values[0].Trim());
                            int value2 = int.Parse(values[1].Trim());
                            return Math.Pow(value1, value2).ToString();
                        }
                        catch (FormatException e)
                        {
                            Console.WriteLine("Invalid integer values inside pow.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid format inside pow.");
                    }
                }
                else
                {
                    Console.WriteLine("No pow function found.");
                }
            }

            return "0";
        }

        //****************

        static List<string> TokenizeExpression(string expression)
        {
            List<string> tokens = new List<string>();
            string[] delimiters = { "+", "-", "*", "/" };

            foreach (string delimiter in delimiters)
            {
                expression = expression.Replace(delimiter, $" {delimiter} ");
            }

            tokens = new List<string>(expression.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries));
            return tokens;
        }

        static bool IsOperator(string token)
        {
            return token == "+" || token == "-" || token == "*" || token == "/" ||
                   token == "pow" || token == "sin" || token == "cos" || token == "tan";
        }

        static double ApplyOperation(string operation, double b, double a)
        {
            switch (operation)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return a / b;
                case "pow": return Math.Pow(a, b);
                case "sin": return Math.Sin(a);
                case "cos": return Math.Cos(a);
                case "tan": return Math.Tan(a);
                default: throw new InvalidOperationException("Geçersiz operatör: " + operation);
            }
        }

        static int GetPrecedence(string operation)
        {
            switch (operation)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                case "pow":
                case "sin":
                case "cos":
                case "tan":
                    return 2;
                default:
                    return 0;
            }
        }
    }
}
