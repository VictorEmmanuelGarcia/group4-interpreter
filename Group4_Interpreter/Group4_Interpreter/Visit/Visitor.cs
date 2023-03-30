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
                Console.WriteLine("Code is VALID");
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
            else if (context.methodCall() != null)
            {
                // Visit the methodCall context
                return VisitMethodCall(context.methodCall());
            }
            else if (context.ifCondition() != null)
            {
                // Visit the ifCondition context
                return VisitIfCondition(context.ifCondition());
            }
            else if (context.whileLoop() != null)
            {
                // Visit the whileLoop context
                return VisitWhileLoop(context.whileLoop());
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
            var type = context.programDataTypes().GetText();
            var varName = context.IDENTIFIERS().Select(x => x.GetText()).ToArray();

            var varValue = Visit(context.expression());

            for (int i = 0; i < varName.Length; i++)
            {
                if (Variables.ContainsKey(varName[i]))
                {
                    Console.WriteLine($"Variable '{varName}' is already defined!");
                }
                else
                {
                    if (type.Equals("INT"))
                    {
                        if (int.TryParse(varValue.ToString(), out int intValue))
                        {
                            Variables[varName[i]] = intValue;
                        }
                        else
                        {
                            int value;
                            bool success = int.TryParse(varValue.ToString(), out value);
                            if (!success)
                            {
                                Console.WriteLine($"Invalid value for integer variable '{varName}'");
                            }
                        }
                    }
                    else if (type.Equals("FLOAT"))
                    {
                        if (float.TryParse(varValue.ToString(), out float floatValue))
                            return Variables[varName[i]] = floatValue;
                        else
                            Console.WriteLine($"Invalid value for float variable '{varName}'");
                    }
                    else if (type.Equals("BOOL"))
                    {
                        if (bool.TryParse(varValue.ToString(), out bool boolValue))
                            return Variables[varName[i]] = boolValue;
                        else
                            Console.WriteLine($"Invalid value for boolean variable '{varName}'");
                    }
                    else if (type.Equals("CHAR"))
                    {
                        var charValue = varValue.ToString();
                        if (charValue?.Length == 3 && charValue[i] == '\'' && charValue[2] == '\'')
                            return Variables[varName[i]] = charValue[1];
                        else
                            Console.WriteLine($"Invalid value for character variable '{varName}'");
                    }
                    else if (type.Equals("STRING"))
                    {
                        return Variables[varName[i]] = varValue.ToString();
                    }
                    else
                    {
                        Console.WriteLine($"Invalid variable type '{type}'");
                    }
                }

            }

            return null;
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var varDataType = VisitProgramDataTypes(context.programDataTypes());
            var variableName = context.IDENTIFIERS().GetText();
            var varValue = VisitExpression(context.expression());

                var convert = TypeDescriptor.GetConverter(varValue.GetType());
                //var varValueWithType = convert.ConvertFrom(varValue?.ToString(, varDataType.GetType() ?? "");
                return Variables[variableName] = varValue;
        }

        public override object? VisitAssignmentOperator([NotNull] CodeParser.AssignmentOperatorContext context)
        {
            var variableName = context.IDENTIFIERS().GetText();
            var variableValue = Visit(context.expression());

            return Variables[variableName] = variableValue;
            //kuwang ug error handling pa
        }

        public override object VisitBeginBlocks([NotNull] CodeParser.BeginBlocksContext context)
        {
            return base.VisitBeginBlocks(context);
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
            foreach (var variable in Variables)
            {
                Console.WriteLine("{0}", variable.Value);
            }
            Console.WriteLine();
            return null;
        }
        

    }
}
