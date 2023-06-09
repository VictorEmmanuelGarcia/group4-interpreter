﻿grammar Code;

programStructure: NEWLINE* BEGIN_CODE NEWLINE* variableInitialization* programLines* NEWLINE* END_CODE EOF;

programLines
    : assignmentOperator
    | assignmentStatement
    | ifStatement
    | whileStatement
    | switchstatement
    | display
    | scanFunction
	| COMMENTS
    ;

variableInitialization: NEWLINE* programDataTypes IDENTIFIERS ('=' expression)? (',' IDENTIFIERS ('=' expression)?)*;
assignmentOperator: IDENTIFIERS '=' expression NEWLINE*;
assignmentStatement: IDENTIFIERS ('=' IDENTIFIERS)* '=' expression NEWLINE* ;

BEGIN_CODE: 'BEGIN CODE' ;
END_CODE: 'END CODE' ;

BEGIN_IF: 'BEGIN IF' ;
END_IF: 'END IF';
block: programLines NEWLINE*;
ifStatement: IF conditionBlock (ELSE IF conditionBlock)* (ELSE ifBlock)?;
conditionBlock: expression NEWLINE* ifBlock;
ifBlock: NEWLINE* BEGIN_IF NEWLINE* block* END_IF NEWLINE*;

ELSE: 'ELSE';
IF: 'IF';
WHILE: 'WHILE';

BEGIN_WHILE: 'BEGIN WHILE' ;
END_WHILE: 'END WHILE' ;
whileStatement: WHILE expression whileBlock;
whileBlock: NEWLINE* BEGIN_WHILE NEWLINE* block* NEWLINE* END_WHILE NEWLINE*;

SWITCH: 'SWITCH';
CASE: 'CASE';
DEFAULT: 'DEFAULT';
switchstatement: SWITCH '(' expression ')' NEWLINE* '{' NEWLINE* caseBlock (caseBlock)* NEWLINE* (defaultBlock)? '}' NEWLINE*;
caseBlock: NEWLINE* CASE expression ':' NEWLINE* block* NEWLINE*;
defaultBlock: NEWLINE* DEFAULT ':' block* NEWLINE*;

programDataTypes: INT | FLOAT | BOOL | CHAR | STRING ;
INT: 'INT' ;
FLOAT: 'FLOAT';
CHAR: 'CHAR';
BOOL: 'BOOL';
STRING: 'STRING';

constantValues: INTEGER_VALUES | FLOAT_VALUES | CHARACTER_VALUES | BOOLEAN_VALUES | STRING_VALUES ;
INTEGER_VALUES: [0-9]+ ;
FLOAT_VALUES: [0-9]+ '.' [0-9]+ ;
CHARACTER_VALUES: '\'' ~[\r\n\'] '\'' ;
BOOLEAN_VALUES: '"TRUE"' | '"FALSE"' ;
STRING_VALUES: ('"' ~'"'* '"') | ('\'' ~'\''* '\'') ;

expression
    : unary_operator expression                                 #unaryExpression
    | expression multDivModOperators expression                 #multDivModExpression
    | expression addSubOperators expression                     #addSubExpression
    | expression comparisonOperators expression                 #comparisonExpression
    | expression logicalOperators expression                    #logicalExpression
	| '(' expression ')'                                        #parenthesisExpression
    | notOperator expression                                    #notExpression
    | ESCAPECODE                                                #escapeCodeExpression
    | NEWLINE                                                   #newLineExpression
    | expression concatVariable expression                      #concatExpression
	| constantValues                                            #constantValueExpression
    | IDENTIFIERS                                               #identifierExpression
    | COMMENTS                                                  #commentExpression
    | methodCall                                                #methodCallExpression
    ; 

multDivModOperators: '*' | '/' | '%' ;
addSubOperators: '+' | '-' ;
comparisonOperators: '==' | '<>' | '>' | '<' | '>=' | '<='  ;
concatVariable: '&' ;
logicalOperators: LOGICAL_OPERATORS ;
ESCAPECODE: '['.']' ;
notOperator: 'NOT' ;
LOGICAL_OPERATORS: 'AND' | 'OR' | 'NOT' ;

unary_operator: '+' | '-' ;

// for DISPLAY: and SCAN:
methodCall: IDENTIFIERS ':' (expression (',' expression)*)? ;
display: NEWLINE* 'DISPLAY' ':' expression NEWLINE* ;

// Not working
SCAN: 'SCAN:';
scanFunction: SCAN IDENTIFIERS (',' IDENTIFIERS)* ;

IDENTIFIERS: [a-zA-Z_][a-zA-Z0-9_]* ;
COMMENTS: '#' ~[\r\n]* -> skip ;
WHITESPACES: [ \t\r]+ -> skip ;
NEWLINE: '$';