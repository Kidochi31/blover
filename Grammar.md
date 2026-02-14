# The Blover Grammar

Blover is a refinement type byte code designed to type check programs written in plover.


# Program Structure
A program is made of one or more files.
`Program -> File+ ;`

A file is made up of zero or more declarations
`File -> Declaration* ;`

# Declarations
A declaration may declare a struct, type, refinement, or function.
`Declaration -> StructDec | TypeDec | RefinementDec | FunctionDec ;`

# Functions
A function must have a name, and comma-separated function parameters which are typed
`FunctionDec -> 'fun' Variable '(' FunctionParameterStatementList ')' '{' FunctionBody '}' ;`
`FunctionParameterStatementList -> Statement* ;`


# Function Body
`FunctionBody -> Statement* ;`

# Control Structures


# Statements
`Statement -> DeclarationStatement | RefAssignmentStatement | GuaranteeStatement | ConfirmationStatement`
`           | ParameterStatement | FunctionAssertionStatement | MutStatement | ReturnStatement`
`           | TypeDefinition | ControlBlock ;`

`DeclarationStatement -> Variable ':' TypeVariable Terminator ;`

`AssignmentStatement -> AssignVariable | AssignInt | AssignBool | CallStatement ;`
`AssignVariable -> Variable '=' Variable Terminator ;`
`AssignInt -> Variable '=' 'int' Integer Terminator ;`
`AssignBool -> Variable '=' 'bool' Boolean Terminator ;`
`CallStatement -> Variable '(' CallArgumentList? ')' Terminator ;`
`CallArgumentList -> CallArgument (',' CallArgument)* ;`
`CallArgument -> Variable | 'out' Variable ;`

`RefAssignmentStatement -> RefAssignVariable | RefAssignInt | RefAssignBool | RefCallStatement ;`
`RefAssignVariable -> 'ref' Variable '=' Variable Terminator ;`
`RefAssignInt -> 'ref' Variable '=' 'int' Integer Terminator ;`
`RefAssignBool -> 'ref' Variable '=' 'bool' Boolean Terminator ;`
`RefCallStatement -> 'ref' Variable '(' CallArgumentList? ')' Terminator ;`

`GuaranteeStatement -> AssumptionStatement | AssertionStatement ;`
`AssumptionStatement -> 'assume' Variable Terminator ;`
`AssertionStatement -> 'assert' Variable Terminator ;`


`ConfirmationStatement -> 'confirm' Variable Terminator ;`

`ParameterStatement -> InParameterStatement | OutParameterStatement ;`
`InParameterStatement -> 'in' 'param' Variable ':' TypeVariable Terminator ;`
`OutParameterStatement -> 'out' 'param' Variable ':' TypeVariable Terminator ;`

`FunctionAssertionStatement -> PreConditionStatement | PostConditionStatement ;`
`PreConditionStatement -> 'pre' Variable Terminator ;`
`PostConditionStatement -> 'post' Variable Terminator ;`

`MutStatement -> 'mut' Variable ('if' Variable)? Terminator ;`

`ReturnStatement -> 'ret' (Variable)? Terminator ;`

`TypeDefinition -> TypeRefinement ;`
`TypeRefinement -> 'type' TypeVariable '=' 'refine' TypeVariable Variable NewLines?`
`                   '{' NewLines Statement* NewLines? '}' NewLines ;`

`ControlBlock -> SequenceBlock | IfBlock | WhileBlock;`
`SequenceBlock -> '{' Statement* '}' ;`
`IfBlock -> 'if' Variable '{' Statement* '}' ('else' '{' Statement* '}')? ;` 

# Miscellaneous
`NewLines -> NewLine+ ;`

# Keywords
`Keyword -> 'int' | 'bool' | 'in' | 'out' | 'ref' | 'assume' | 'assert' | 'confirm' | 'param' | 'mut' | 'ret' | 'type' | 'refine' | 'if' ;` 