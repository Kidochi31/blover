# The Zlover Grammar

Zlover is an untyped refinement byte code designed to check programs written in blover.


# Program Structure
A program is made of one or more files.
`Program -> File+ ;`

A file is made up of zero or more declarations
`File -> Declaration* ;`

# Declarations
A declaration may declare a struct, type, refinement, or function.
`Declaration -> FunctionDefinition | VariableDeclaration ;`

# Functions
A function must have a name, and can contain a pre, post, and body block of statements
`FunctionDefinition -> 'fun' Variable Block ;`
`VariableDeclaration -> 'dec' Variable ;`

# Control Structures


# Statements
`Statement -> DeclarationStatement | GuaranteeStatement | AssignmentStatement ;`

`DeclarationStatement -> 'dec' Variable;`

`AssignmentStatement -> AssignVariable | AssignInt | AssignBool | CallStatement | VerificationStatement ;`
`AssignVariable -> Variable '=' Variable Terminator ;`
`AssignInt -> Variable '=' 'int' Integer Terminator ;`
`AssignBool -> Variable '=' 'bool' Boolean Terminator ;`
`CallStatement -> Variable '=' 'call' Integer Variable '(' CallArgumentList? ')' Terminator ;`
`VerificationStatement -> Variable '=' 'verify' Variable '(' CallArgumentList? ')' Terminator ;`
`CallArgumentList -> Variable (',' Variable)* ;`

`GuaranteeStatement -> AssumptionStatement | AssertionStatement ;`
`AssumptionStatement -> 'assume' Variable Terminator ;`
`AssertionStatement -> 'assert' Variable Terminator ;`


# Miscellaneous
`NewLines -> NewLine+ ;`
`Block -> '{' Newlines Statement* NewLines? '}' NewLines ; `

# Keywords
`Keyword -> 'int' | 'bool' | 'assume' | 'assert' | 'dec' | 'fun' | 'call' | 'verify' ;` 