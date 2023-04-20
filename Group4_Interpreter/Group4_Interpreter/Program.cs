using Antlr4.Runtime;
using Group4_Interpreter;
using Group4_Interpreter.Interpret;
using Group4_Interpreter.Visit;
using Interpreter.ErrorHandle;
using System.CodeDom.Compiler;

var fileName = "Interpret\\test.code";

var fileContents = File.ReadAllText(fileName);

var inputStream = new AntlrInputStream(fileContents);

var codeLexer = new CodeLexer(inputStream);
var commonTokenStream = new CommonTokenStream(codeLexer);
var codeParser = new CodeParser(commonTokenStream);

var errorListener = new ErrorListener();
codeParser.AddErrorListener(errorListener);

var codeContext = codeParser.programStructure();
var visitor = new Visitor();
visitor.Visit(codeContext);