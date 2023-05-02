# Group 4 Interpreter
## Code Interpreter using C# and ANTLR

This project is an implementation of an interpreter using C# and ANTLR. It provides a basic shell for executing a small language, which allows arithmetic expressions and control flow statements such as if statements.

The interpreter uses ANTLR to generate a lexer and parser based on a grammar specified in an ANTLR grammar file. The grammar defines the syntax of the language being interpreted.

### This project has the following components:

* `Code.g4:` This is the ANTLR grammar file that defines the syntax of the language. It includes rules for arithmetic expressions and control flow statements.
* `Visitor.cs:` This is the main interpreter class. It uses the lexer and parser to parse input code and execute it.
* `Program.cs:` This is the entry point of the program. It reads input from the test.code and passes it to the Visitor class.
* `ErrorListener.cs:` This is a class that is used as a listener for the Antlr-generated parser to handle errors and report them to the user in a meaningful way.

## How to Run:

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Build the solution.
5. Input code in test.code
4. Run the program.

## Examples:

`To execute a simple arithmetic expression:`

```
BEGIN CODE
INT a
a = 5 + 2
DISPLAY: a
END CODE
```

Output: 7
<br/>
<br/>

`To execute an if statement:`
```
BEGIN CODE
a = 5
IF(a > 3)
BEGIN IF
DISPLAY: "a is greater than 3"
END IF
ELSE
BEGIN IF
DISPLAY: "a is less than or equal to 3"
END IF
END IF
END CODE
```

Output: "a is greater than 3"
<br/>
<br/>

## Limitations:

* The language being interpreted is very basic, and does not support more advanced features such as functions or object-oriented programming.
* The error handling is minimal, and may not provide very informative error messages in some cases.

## Contributors
* Clamor, Jonas Angelo
* Cuizon, Shane Denney
* Garcia, Victor Emmanuel