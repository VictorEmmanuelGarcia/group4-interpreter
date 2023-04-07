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
        public override object? VisitProgramStructure([NotNull] CodeParser.ProgramStructureContext context)
        {
            string code = context.GetText().Trim();
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE"))
            {
                //Visit each statement in the code
                foreach (var linesContext in context.programLines())
                {
                    VisitProgramLines(linesContext);
                }
                Console.WriteLine("\r\nCode is VALID");
            }
            else
            {
                Console.WriteLine("Code must begin with 'BEGIN CODE' and end with 'END CODE'");
            }

            return null;
        }

        public override object? VisitProgramLines([NotNull] CodeParser.ProgramLinesContext context)
        {
            if (context.variableInitialization() != null)
            {
                // Visit the variableInitialization context
                return VisitVariableInitialization(context.variableInitialization());
            }
            else if (context.variable() != null)
            {
                // Visit the variable context
                return VisitVariable(context.variable());
            }
            else if (context.assignmentOperator() != null)
            {
                // Visit the assignmentOperator context
                return VisitAssignmentOperator(context.assignmentOperator());
            }
            else if (context.display() != null)
            {
                // Visit the display context
               return VisitDisplay(context.display());
            }
            else if (context.scanFunction() != null)
            {
                // Visit the scanFunction context
                return VisitScanFunction(context.scanFunction());
            }
            else
            {
                throw new Exception("Unknown program line");
            }
           
        }

        public override object? VisitVariableInitialization([NotNull] CodeParser.VariableInitializationContext context)
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
                return null;
            }

            var varNames = context.IDENTIFIERS().Select(x => x.GetText()).ToArray();
            object? varValue = null;
            if (context.expression() != null)
            {
                varValue = Visit(context.expression());
            }

            foreach (var varName in varNames)
            {
                if (Variables.ContainsKey(varName))
                {
                    Console.WriteLine($"Variable '{varName}' is already defined!");
                }
                else
                {
                    var convertedValue = varValue;
                    if (varValue != null && type != varValue.GetType())
                    {
                        convertedValue = TypeDescriptor.GetConverter(type).ConvertFrom(varValue);
                    }
                    Variables[varName] = convertedValue;
                }
            }

            return null;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var dataTypeObj = VisitProgramDataTypes(context.programDataTypes());
            if (dataTypeObj is null)
            {
                throw new Exception("Invalid data type");
            }

            var dataType = (Type)dataTypeObj;
            var variableName = context.IDENTIFIERS().GetText();
            var variableValue = VisitExpression(context.expression());

            var varValueWithType = Convert.ChangeType(variableValue, dataType);
            Variables[variableName] = varValueWithType;

            return varValueWithType;
        }

        public override object? VisitAssignmentOperator([NotNull] CodeParser.AssignmentOperatorContext context)
        {
            var variableName = context.IDENTIFIERS().GetText();
            var variableValue = Visit(context.expression());

            return Variables[variableName] = variableValue;
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
                case "STRING":
                    return typeof(string);
                case "CHAR":
                    return typeof(char);
                case "BOOL":
                    return typeof(bool);
                default:
                    throw new Exception ("Invalid DATA TYPE!");
            }
        }
        public override object VisitIfCondition([NotNull] CodeParser.IfConditionContext context)
        {
            return base.VisitIfCondition(context);
        }

        public override object? VisitDisplay([NotNull] CodeParser.DisplayContext context)
        {
            var exp =  Visit(context.expression());
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

    }
}
