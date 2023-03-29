using Antlr4.Runtime.Misc;
using Group4_Interpreter.Interpret;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group4_Interpreter.Visit
{
    public class Visitor : CodeBaseVisitor<object?>
    {
        public Dictionary<string, object?> Variables { get; } = new();
        public override object? VisitProgramStructure([NotNull] CodeParser.ProgramStructureContext context)
        {
            string code = context.GetText().Trim();
            if (code.StartsWith("BEGIN CODE") && code.EndsWith("END CODE"))
            {
                // Visit each statement in the code
                foreach (var linesContext in context.programLines())
                {
                    VisitProgramLines(linesContext);
                }
                Console.WriteLine("Code is VALID");
            }
            else
            {
                Console.WriteLine("Code must start with 'BEGIN CODE' and end with 'END CODE'.");
            }

            return null;
        }

        public override object? VisitProgramLines([NotNull] CodeParser.ProgramLinesContext context)
        {
            if (context.variableInitialization() != null)
            {
                // Visit the variableInitialization context
                VisitVariableInitialization(context.variableInitialization());
            }
            else if (context.assignmentOperator() != null)
            {
                // Visit the assignmentOperator context
                VisitAssignmentOperator(context.assignmentOperator());
            }
            else if (context.methodCall() != null)
            {
                // Visit the methodCall context
                VisitMethodCall(context.methodCall());
            }
            else if (context.ifCondition() != null)
            {
                // Visit the ifCondition context
                VisitIfCondition(context.ifCondition());
            }
            else if (context.whileLoop() != null)
            {
                // Visit the whileLoop context
                VisitWhileLoop(context.whileLoop());
            }
            else if (context.display() != null)
            {
                // Visit the display context
                VisitDisplay(context.display());
            }
            else if (context.scanFunction() != null)
            {
                // Visit the scanFunction context
                VisitScanFunction(context.scanFunction());
            }

            return null;
        }

        public override object? VisitVariableInitialization([NotNull] CodeParser.VariableInitializationContext context)
        {
            var dataType = context.programDataTypes().GetText();
            var variableName = context.IDENTIFIERS().Select(id => id.GetText()).ToArray();
            var variableValue = Visit(context.expression());
            foreach(var name in variableName)
            {
                
            }
            return base.VisitVariableInitialization(context);
        }

        public override object? VisitVariable([NotNull] CodeParser.VariableContext context)
        {
            var dataType = context.programDataTypes().GetText();
            var variableName = context.IDENTIFIERS().GetText();
            var variableValue = Visit(context.expression());
            
            return null;
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
                break;
            }
            Console.WriteLine();
            return null;
        }
        

    }
}
