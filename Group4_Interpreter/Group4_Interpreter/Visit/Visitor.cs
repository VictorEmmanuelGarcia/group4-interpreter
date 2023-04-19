using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Group4_Interpreter.Interpret;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group4_Interpreter.Visit
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        public Dictionary<string, object?> Variables { get; } = new Dictionary<string, object?>();
        public override object? VisitVariableInitialization(CodeParser.VariableInitializationContext context)
        {
            // Map string type names to corresponding Type objects
            var typeMap = new Dictionary<string, Type>()
            {
                { "INT", typeof(int) },
                { "FLOAT", typeof(float) },
                { "BOOL", typeof(bool) },
                { "CHAR", typeof(char) },
                { "STRING", typeof(string) }
            };

            var typeStr = context.programDataTypes().GetText();
            if (!typeMap.TryGetValue(typeStr, out var type))
            {
                Console.WriteLine($"Invalid variable type '{typeStr}'");
                Environment.Exit(1);
            }
            var varNames = context.IDENTIFIERS();

            var contextstring = context.GetText().Replace(typeStr, "");

            var contextParts = contextstring.Split(',');
            var exp = context.expression();
            int expressionCounter = 0;

            for (int x = 0; x < contextParts.Length; x++)
            {
                if (Variables.ContainsKey(varNames[x].GetText()))
                {
                    Console.WriteLine($"{varNames[x].GetText()} is already defined!");
                    Environment.Exit(1);
                }

                if (contextParts[x].Contains('='))
                {
                    if (expressionCounter < exp.Length)
                    {
                        var expr = Visit(exp[expressionCounter]);
                        var convertedValue = expr;
                        if (expr != null && type != expr.GetType())
                        {
                            convertedValue = TypeDescriptor.GetConverter(type).ConvertFrom(expr);
                        }

                        Variables[varNames[x].GetText()] = convertedValue;
                        expressionCounter++;
                    }
                }
                else
                {
                    Variables[varNames[x].GetText()] = null;
                }
            }
            return null;
        }

        public override object? VisitAssignmentOperator([NotNull] CodeParser.AssignmentOperatorContext context)
        {
            var variableName = context.IDENTIFIERS().GetText();
            var variableValue = Visit(context.expression());

            return Variables[variableName] = variableValue;
        }
        public override object? VisitAssignmentStatement([NotNull] CodeParser.AssignmentStatementContext context)
        {
            var identifier = context.IDENTIFIERS();
            foreach (var a in identifier)
            {
                var expression = context.expression().Accept(this);
                Variables[a.GetText()] = expression;
            }
            return new object();
        }

        public override object? VisitConstantValueExpression([NotNull] CodeParser.ConstantValueExpressionContext context)
        {
            if (context.constantValues().INTEGER_VALUES() is { } a)
            {
                return int.Parse(a.GetText());
            }
            if (context.constantValues().FLOAT_VALUES() is { } b)
            {
                return float.Parse(b.GetText());
            }
            if (context.constantValues().CHARACTER_VALUES() is { } c)
            {
                return char.Parse(c.GetText().Substring(1, 1));
            }
            if (context.constantValues().BOOLEAN_VALUES() is { } d)
            {
                return bool.Parse(d.GetText());
            }
            if (context.constantValues().STRING_VALUES() is { } e)
            {
                return e.GetText()[1..^1];
            }
            return null;
        }

        public override object? VisitIdentifierExpression([NotNull] CodeParser.IdentifierExpressionContext context)
        {
            var varName = context.IDENTIFIERS().GetText();
            return Variables[varName];
        }

        public override object? VisitProgramDataTypes([NotNull] CodeParser.ProgramDataTypesContext context)
        {
            switch (context.GetText())
            {
                case "INT":
                    return typeof(int);
                case "FLOAT":
                    return typeof(float);
                case "CHAR":
                    return typeof(char);
                case "STRING":
                    return typeof(string);
                case "BOOL":
                    return typeof(bool);
                default:
                    throw new Exception("Invalid DATA TYPE!");
            }
        }
        public override object VisitIfCondition([NotNull] CodeParser.IfConditionContext context)
        {
            return base.VisitIfCondition(context);
        }

            return null;
        }
        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            var exp = Visit(context.expression());
            if (exp is bool a)
            {
                exp = a.ToString().ToUpper();
            }
            Console.Write(exp);
            return null;
        }
        public override object? VisitUnaryExpression([NotNull] CodeParser.UnaryExpressionContext context)
        {
            var value = Visit(context.expression());
            switch (context.unary_operator().GetText())
            {
                case "+":
                    return value;
                case "-":
                    if (value is int intValue)
                    {
                        return -intValue;
                    }
                    else if (value is float floatValue)
                    {
                        return -floatValue;
                    }
                    else
                    {
                        throw new Exception($"Invalid unary operator '-' for type {value?.GetType().Name}");
                    }
                default:
                    throw new Exception($"Invalid unary operator {context.unary_operator().GetText()}");
            }
        }
        public override object? VisitConcatExpression([NotNull] CodeParser.ConcatExpressionContext context)
        {
            var varLeft = Visit(context.expression(0));
            var varRight = Visit(context.expression(1));
            if (varLeft is bool a)
            {
                varLeft = a.ToString().ToUpper();
            }
            if (varRight is bool b)
            {
                varRight = b.ToString().ToUpper();
            }
            return $"{varLeft}{varRight}";
        }
        public override object? VisitEscapeCodeExpression([NotNull] CodeParser.EscapeCodeExpressionContext context)
        {
            return context.ESCAPECODE().GetText()[1];
        }
        public override object? VisitNewLineExpression([NotNull] CodeParser.NewLineExpressionContext context)
        {
            return "\n";
        }
        public override object? VisitScanFunction([NotNull] CodeParser.ScanFunctionContext context)
        {
            foreach (var id in context.IDENTIFIERS().Select(x => x.GetText()).ToArray())
            {
                Console.Write($"Input the corresponding value for the declared variable {id}: ");
                var input = Console.ReadLine();

                if (int.TryParse(input, out int intValue))
                {
                    Variables[id] = intValue;
                }
                else if (float.TryParse(input, out float floatValue))
                {
                    Variables[id] = floatValue;
                }
                else if (char.TryParse(input, out char charValue))
                {
                    Variables[id] = charValue;
                }
                else if (bool.TryParse(input, out bool boolValue))
                {
                    Variables[id] = boolValue;
                }
                else if (input != null)
                {
                    Variables[id] = input;
                }
                else
                {
                    throw new ArgumentException($"Invalid input for variable {id}");
                }
            }
            return null;
        }
        public override object? VisitParenthesisExpression([NotNull] CodeParser.ParenthesisExpressionContext context)
        {
            return Visit(context.expression());
        }
        public override object? VisitMultDivModExpression([NotNull] CodeParser.MultDivModExpressionContext context)
        {
            var leftValue = Visit(context.expression(0));
            var rightValue = Visit(context.expression(1));

            if (leftValue == null || rightValue == null)
            {
                throw new ArgumentNullException("Operand/s cannot be null.");
            }
            else if (leftValue is int leftIntValue && rightValue is int rightIntValue)
            {
                if (context.multDivModOperators().GetText() == "*")
                {
                    return leftIntValue * rightIntValue;
                }
                else if (context.multDivModOperators().GetText() == "/")
                {
                    return leftIntValue / rightIntValue;
                }
                else if (context.multDivModOperators().GetText() == "%")
                {
                    return leftIntValue % rightIntValue;
                }
            }
            else if (leftValue is float leftFloatValue && rightValue is float rightFloatValue)
            {
                if (context.multDivModOperators().GetText() == "*")
                {
                    return leftFloatValue * rightFloatValue;
                }
                else if (context.multDivModOperators().GetText() == "/")
                {
                    return leftFloatValue / rightFloatValue;
                }
                else if (context.multDivModOperators().GetText() == "%")
                {
                    return leftFloatValue % rightFloatValue;
                }
            }
            else if (leftValue is int leftIntValue2 && rightValue is float rightFloatValue2)
            {
                if (context.multDivModOperators().GetText() == "*")
                {
                    return leftIntValue2 * rightFloatValue2;
                }
                else if (context.multDivModOperators().GetText() == "/")
                {
                    return leftIntValue2 / rightFloatValue2;
                }
                else if (context.multDivModOperators().GetText() == "%")
                {
                    return leftIntValue2 % rightFloatValue2;
                }
            }
            else if (leftValue is float leftFloatValue2 && rightValue is int rightIntValue2)
            {
                if (context.multDivModOperators().GetText() == "*")
                {
                    return leftFloatValue2 * rightIntValue2;
                }
                else if (context.multDivModOperators().GetText() == "/")
                {
                    return leftFloatValue2 / rightIntValue2;
                }
                else if (context.multDivModOperators().GetText() == "%")
                {
                    return leftFloatValue2 % rightIntValue2;
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid operand types: " + leftValue?.GetType().Name + " and " + rightValue?.GetType().Name);
            }
            throw new InvalidOperationException("Invalid operator or operand types: " + context.multDivModOperators().GetText());
        }
        public override object? VisitAddSubExpression([NotNull] CodeParser.AddSubExpressionContext context)
        {
            var leftValue = Visit(context.expression(0));
            var rightValue = Visit(context.expression(1));

            if (leftValue == null || rightValue == null)
            {
                throw new ArgumentNullException("Operand/s cannot be null.");
            }
            else if (leftValue is int leftIntValue && rightValue is int rightIntValue)
            {
                if (context.addSubOperators().GetText() == "+")
                {
                    return leftIntValue + rightIntValue;
                }
                else if (context.addSubOperators().GetText() == "-")
                {
                    return leftIntValue - rightIntValue;
                }
            }
            else if (leftValue is float leftFloatValue && rightValue is float rightFloatValue)
            {
                if (context.addSubOperators().GetText() == "+")
                {
                    return leftFloatValue + rightFloatValue;
                }
                else if (context.addSubOperators().GetText() == "-")
                {
                    return leftFloatValue - rightFloatValue;
                }
            }
            else if (leftValue is int leftIntValue2 && rightValue is float rightFloatValue2)
            {
                if (context.addSubOperators().GetText() == "+")
                {
                    return leftIntValue2 + rightFloatValue2;
                }
                else if (context.addSubOperators().GetText() == "-")
                {
                    return leftIntValue2 - rightFloatValue2;
                }
            }
            else if (leftValue is float leftFloatValue2 && rightValue is int rightIntValue2)
            {
                if (context.addSubOperators().GetText() == "+")
                {
                    return leftFloatValue2 + rightIntValue2;
                }
                else if (context.addSubOperators().GetText() == "-")
                {
                    return leftFloatValue2 - rightIntValue2;
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid operand types: " + leftValue?.GetType().Name + " and " + rightValue?.GetType().Name);
            }
            throw new InvalidOperationException("Invalid operator or operand types: " + context.addSubOperators().GetText());
        }
        public override object? VisitComparisonExpression([NotNull] CodeParser.ComparisonExpressionContext context)
        {
            var leftValue = Visit(context.expression(0));
            var rightValue = Visit(context.expression(1));

            if (leftValue == null || rightValue == null)
            {
                throw new ArgumentNullException("Operand/s cannot be null.");
            }
            else if (leftValue is int leftIntValue && rightValue is int rightIntValue)
            {
                if (context.comparisonOperators().GetText() == ">")
                {
                    return leftIntValue > rightIntValue;
                }
                else if (context.comparisonOperators().GetText() == "<")
                {
                    return leftIntValue < rightIntValue;
                }
                else if (context.comparisonOperators().GetText() == ">=")
                {
                    return leftIntValue >= rightIntValue;
                }
                else if (context.comparisonOperators().GetText() == "<=")
                {
                    return leftIntValue <= rightIntValue;
                }
                else if (context.comparisonOperators().GetText() == "==")
                {
                    return leftIntValue == rightIntValue;
                }
                else if (context.comparisonOperators().GetText() == "<>")
                {
                    return leftIntValue != rightIntValue;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else if (leftValue is float leftFloatValue && rightValue is float rightFloatValue)
            {
                if (context.comparisonOperators().GetText() == ">")
                {
                    return leftFloatValue > rightFloatValue;
                }
                else if (context.comparisonOperators().GetText() == "<")
                {
                    return leftFloatValue < rightFloatValue;
                }
                else if (context.comparisonOperators().GetText() == ">=")
                {
                    return leftFloatValue >= rightFloatValue;
                }
                else if (context.comparisonOperators().GetText() == "<=")
                {
                    return leftFloatValue <= rightFloatValue;
                }
                else if (context.comparisonOperators().GetText() == "==")
                {
                    return leftFloatValue == rightFloatValue;
                }
                else if (context.comparisonOperators().GetText() == "<>")
                {
                    return leftFloatValue != rightFloatValue;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else if (leftValue is int leftIntValue2 && rightValue is float rightFloatValue2)
            {
                if (context.comparisonOperators().GetText() == ">")
                {
                    return leftIntValue2 > rightFloatValue2;
                }
                else if (context.comparisonOperators().GetText() == "<")
                {
                    return leftIntValue2 < rightFloatValue2;
                }
                else if (context.comparisonOperators().GetText() == ">=")
                {
                    return leftIntValue2 >= rightFloatValue2;
                }
                else if (context.comparisonOperators().GetText() == "<=")
                {
                    return leftIntValue2 <= rightFloatValue2;
                }
                else if (context.comparisonOperators().GetText() == "==")
                {
                    return leftIntValue2 == rightFloatValue2;
                }
                else if (context.comparisonOperators().GetText() == "<>")
                {
                    return leftIntValue2 != rightFloatValue2;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else if (leftValue is float leftFloatValue2 && rightValue is int rightIntValue2)
            {
                if (context.comparisonOperators().GetText() == ">")
                {
                    return leftFloatValue2 > rightIntValue2;
                }
                else if (context.comparisonOperators().GetText() == "<")
                {
                    return leftFloatValue2 < rightIntValue2;
                }
                else if (context.comparisonOperators().GetText() == ">=")
                {
                    return leftFloatValue2 >= rightIntValue2;
                }
                else if (context.comparisonOperators().GetText() == "<=")
                {
                    return leftFloatValue2 <= rightIntValue2;
                }
                else if (context.comparisonOperators().GetText() == "==")
                {
                    return leftFloatValue2 == rightIntValue2;
                }
                else if (context.comparisonOperators().GetText() == "<>")
                {
                    return leftFloatValue2 != rightIntValue2;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else if (leftValue is bool leftBoolValue && rightValue is bool rightBoolValue)
            {
                if (context.comparisonOperators().GetText() == "==")
                {
                    return leftBoolValue == rightBoolValue;
                }
                else if (context.comparisonOperators().GetText() == "<>")
                {
                    return leftBoolValue != rightBoolValue;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid operand types: " + leftValue?.GetType().Name + " and " + rightValue?.GetType().Name);
            }
        }
        public override object? VisitLogicalExpression([NotNull] CodeParser.LogicalExpressionContext context)
        {
            var leftValue = Visit(context.expression(0));
            var rightValue = Visit(context.expression(1));

            if (leftValue == null || rightValue == null)
            {
                throw new ArgumentNullException("Operand/s cannot be null.");
            }
            else if (leftValue is bool leftBoolValue && rightValue is bool rightBoolValue)
            {
                if (context.logicalOperators().GetText() == "AND")
                {
                    return leftBoolValue && rightBoolValue;
                }
                else if (context.logicalOperators().GetText() == "OR")
                {
                    return leftBoolValue || rightBoolValue;
                }
                else if (context.logicalOperators().GetText() == "NOT")
                {
                    return !leftBoolValue;
                }
                else
                {
                    throw new Exception("Unknown operator");
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid operand types: " + leftValue?.GetType().Name + " and " + rightValue?.GetType().Name);
            }
        }

        public override object? VisitIfStatement([NotNull] CodeParser.IfStatementContext context)
        {
            CodeParser.ConditionBlockContext[] conditions = context.conditionBlock();

            bool evaluatedBlock = false;

            foreach (CodeParser.ConditionBlockContext condition in conditions)
            {
                var evaluated = Visit(condition.expression());

                if (bool.Parse(evaluated.ToString()!) == true)
                {
                    evaluatedBlock = true;
                    Visit(condition.ifBlock());
                    break;
                }
            }

            if (!evaluatedBlock && context.ifBlock() != null)
            {
                Visit(context.ifBlock());
            }

            return new object();
        }
        public override object VisitWhileStatement([NotNull] CodeParser.WhileStatementContext context)
        {
            var value = Visit(context.expression());
            int currIterations = 0;
            int maxIterations = 1000;

            while (bool.Parse(value.ToString()!) == true)
            {
                currIterations++;
                if (currIterations > maxIterations)
                {
                    Console.WriteLine();
                    Console.Write("Possible infinite loop detected!");
                    Environment.Exit(400);
                }

                Visit(context.whileBlock());

                value = Visit(context.expression());
            }

            return new object();
        }

    }
}