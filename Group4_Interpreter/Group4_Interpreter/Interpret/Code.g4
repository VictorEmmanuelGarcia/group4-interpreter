﻿grammar Code;

programStructure: BEGIN_CODE NEWLINE programLines* NEWLINE END_CODE ;

programLines
    : variableInitialization
	| variable
    | assignmentOperator
	| methodCall
    | ifCondition 
    | whileLoop
    | display
    | scanFunction
	| COMMENTS
    ;

variableInitialization: programDataTypes IDENTIFIERS (',' IDENTIFIERS)* ('=' expression)? NEWLINE?;
variable: programDataTypes IDENTIFIERS ('=' expression)? NEWLINE?;
assignmentOperator: IDENTIFIERS '=' expression NEWLINE?;

beginBlocks: (BEGIN_CODE | BEGIN_IF | BEGIN_WHILE);

BEGIN_CODE: 'BEGIN CODE' ;
END_CODE: 'END CODE' ;

BEGIN_IF: 'BEGIN' 'IF' ;
END_IF: 'END' 'IF' ;
ifCondition: 'IF' '('expression')' BEGIN_IF beginBlocks END_IF elseIfCondition? ;
elseIfCondition: 'ELSE' (BEGIN_IF beginBlocks END_IF) | ifCondition ;

WHILE: 'WHILE' ;
BEGIN_WHILE: 'BEGIN' 'WHILE' ;
END_WHILE: 'END' 'WHILE' ;
whileLoop: WHILE '(' expression ')' BEGIN_WHILE beginBlocks* END_WHILE ;

programDataTypes: INT | FLOAT | BOOL | CHAR ;
INT: 'INT' ;
FLOAT: 'FLOAT';
CHAR: 'CHAR';
BOOL: 'BOOL';

constantValues: INTEGER_VALUES | FLOAT_VALUES | CHARACTER_VALUES | BOOLEAN_VALUES | STRING_VALUES ;
INTEGER_VALUES: [0-9]+ ;
FLOAT_VALUES: [0-9]+ '.' [0-9]+ ;
CHARACTER_VALUES: '\'' ~[\r\n\'] '\'' ;
BOOLEAN_VALUES:  '\"TRUE\"' | '\"FALSE\"' ;
STRING_VALUES: ('"' ~'"'* '"') | ('\'' ~'\''* '\'') ;

expression
    : constantValues                                            #constantValueExpression
    | IDENTIFIERS                                               #identifierExpression
    | COMMENTS                                                  #commentExpression
    | methodCall                                                #methodCallExpression
    | '(' expression ')'                                        #parenthesisExpression
    | 'NOT' expression                                          #notExpression
    | expression multDivModOperators expression                 #multDivModExpression
    | expression addSubConcatenatorOperators expression         #addSubConcatenatorExpression
    | expression comparisonOperators expression                 #comparisonExpression
    | expression logicalOperators expression                    #logicalExpression
    | escapeCodeOpen expression escapeCodeClose                 #escapeCodeExpression
    ; 

multDivModOperators: '*' | '/' | '%' ;
addSubConcatenatorOperators: '+' | '-' | '&' ;
comparisonOperators: '==' | '<>' | '>' | '<' | '>=' | '<='  ;
logicalOperators: LOGICAL_OPERATORS ;
escapeCodeOpen: '[' ;
escapeCodeClose: ']' ;

LOGICAL_OPERATORS: 'AND' | 'OR' | 'NOT' ;

// for DISPLAY: and SCAN:
methodCall: IDENTIFIERS ':' (expression (',' expression)*)? ;
display: NEWLINE? 'DISPLAY' ':' expression* ;

// Not working
SCAN: 'SCAN:';
scanFunction: SCAN IDENTIFIERS (',' IDENTIFIERS)* ;

IDENTIFIERS: [a-zA-Z_][a-zA-Z0-9_]* ;
COMMENTS: '#' ~[\r\n]* -> skip ;
WHITESPACES: [ \t\r]+ -> skip ;
NEWLINE: '\r'? '\n'| '\r';