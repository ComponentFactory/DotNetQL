﻿namespace RocketQL.Core.Exceptions;

public class ValidationException(Location location, string message) : RocketException(location, message)
{
    public static ValidationException UnrecognizedType(Location location, string name) => new(location, $"Unrecognized type '{name}' encountered.");
    public static ValidationException UnrecognizedType(SyntaxNode node) => new(node.Location, $"Unrecognized type '{node.GetType()}' encountered.");
    public static ValidationException UnrecognizedType(SchemaNode node) => new(node.Location, $"Unrecognized type '{node.GetType()}' encountered.");
    public static ValidationException SchemaDefinitionAlreadyDefined(Location location) => new(location, $"Schema definition is already defined.");
    public static ValidationException SchemaDefinitionEmpty(Location location) => new(location, "Schema definition does not define any operations.");
    public static ValidationException SchemaOperationAlreadyDefined(Location location, OperationType operation) => new(location, $"Schema definition already defines the {operation} operation.");
    public static ValidationException SchemaDefinitionMissingQuery(Location location) => new(location, $"Schema definition missing mandatory Query operation.");
    public static ValidationException SchemaOperationsNotUnique(Location location, string op1, string op2, string type) => new(location, $"Schema {op1} and {op2} operations cannot have same '{type}' type.");
    public static ValidationException TypeNotDefinedForSchemaOperation(Location location, OperationType operation, string type) => new(location, $"Type '{type}' not defined for the schema operation {operation}.");
    public static ValidationException OperationTypeAlreadyDefined(Location location, OperationType operationType) => new(location, $"Type already defined for '{operationType}' operation.");
    public static ValidationException NameDoubleUnderscore(SchemaNode node) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' not allowed to start with two underscores.");
    public static ValidationException NameAlreadyDefined(Location location, string target, string name) => new(location, $"{target} '{name}' is already defined.");
    public static ValidationException EnumValueAlreadyDefined(Location location, string enumValue, string enumTypeName) => new(location, $"Enum '{enumTypeName}' has duplicate definition of value '{enumValue}'.");    
    public static ValidationException ListEntryDuplicateName(Location location, string list, string entryName, string entryType, string name) => new(location, $"{list} '{entryName}' has duplicate {entryType} '{name}'.");
    public static ValidationException ListEntryDuplicateName(Location location, string type, string typeName, string list, string entryName, string entryType, string name) => new(location, $"{type} '{typeName}' has {list.ToLower()} '{entryName}' with duplicate {entryType} '{name}'.");
    public static ValidationException ListEntryDoubleUnderscore(Location location, string list, string entryName, string entryType, string name) => new(location, $"{list} '{entryName}' has {entryType.ToLower()} '{name}' not allowed to start with two underscores.");
    public static ValidationException ListEntryDoubleUnderscore(Location location, string type,  string typeName, string list, string entryName, string entryType, string name) => new(location, $"{type} '{typeName}' has {list.ToLower()} '{entryName}' with {entryType.ToLower()} '{name}' not allowed to start with two underscores.");
    public static ValidationException UndefinedTypeForListEntry(Location location, string type, string listNodeElement, string listNodeName, SchemaNode parentNode) => new(location, $"Undefined type '{type}' for {listNodeElement.ToLower()} '{listNodeName}' of {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException UndefinedDirective(SchemaNode node, SchemaNode parentNode) => new(node.Location, $"Undefined directive '{node.OutputName}' defined on {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException UndefinedDirective(SchemaNode node, string parentNodeElement, string parentNodeName, SchemaNode grandParentNode) => new(node.Location, $"Undefined directive '{node.OutputName}' defined on {parentNodeElement.ToLower()} '{parentNodeName}' of {grandParentNode.OutputElement.ToLower()} '{grandParentNode.OutputName}'.");
    public static ValidationException UndefinedInterface(SchemaNode node, SchemaNode parentNode) => new(node.Location, $"Undefined interface '{node.OutputName}' defined on {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException InterfaceCannotImplmentOwnInterface(SchemaNode node) => new(node.Location, $"Interface '{node.OutputName}' cannnot implement itself.");
    public static ValidationException UndefinedMemberType(SchemaNode node, SchemaNode parentNode) => new(node.Location, $"Undefined member type '{node.OutputName}' defined on {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException TypeIsNotAnInterface(SchemaNode node, SchemaNode parentNode, SchemaNode actualNode) => new(node.Location, $"Cannot implement interface '{node.OutputName}' defined on {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}' because it is a '{actualNode.OutputElement.ToLower()}'.");
    public static ValidationException TypeIsNotAnObject(SchemaNode node, SchemaNode parentNode, SchemaNode actualNode, string article) => new(node.Location, $"Cannot reference member type '{node.OutputName}' defined on {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}' because it is {article} {actualNode.OutputElement.ToLower()}.");
    public static ValidationException TypeIsNotAnOutputType(SchemaNode node, SchemaNode parentNode, string errorType) => new(node.Location, $"{parentNode.OutputElement} '{parentNode.OutputName}' has {node.OutputElement.ToLower()} '{node.OutputName}' with type '{errorType}' that is not an output type.");
    public static ValidationException TypeIsNotAnInputType(SchemaNode node, SchemaNode parentNode, SchemaNode argumentNode, string errorType) => new(node.Location, $"{parentNode.OutputElement} '{parentNode.OutputName}' has {node.OutputElement.ToLower()} '{node.OutputName}' with argument '{argumentNode.OutputName}' of type '{errorType}' that is not an input type.");
    public static ValidationException TypeIsNotAnInputType(SchemaNode node, SchemaNode parentNode, string errorType) => new(node.Location, $"{parentNode.OutputElement} '{parentNode.OutputName}' has {node.OutputElement.ToLower()} '{node.OutputName}' of type '{errorType}' that is not an input type.");
    public static ValidationException NonNullArgumentCannotBeDeprecated(SchemaNode node, SchemaNode parentNode, SchemaNode argumentNode) => new(node.Location, $"Cannot use @deprecated directive on non-null {argumentNode.OutputElement.ToLower()} '{argumentNode.OutputName}' of {node.OutputElement.ToLower()} '{node.OutputName}' of {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException NonNullFieldCannotBeDeprecated(SchemaNode node, SchemaNode parentNode) => new(node.Location, $"Cannot use @deprecated directive on non-null {node.OutputElement.ToLower()} '{node.OutputName}' of {parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException TypeMissingImplements(SchemaNode node, string implementsName, string interfaceName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' is missing implements '{implementsName}' because it is declared on interface '{interfaceName}'.");
    public static ValidationException TypeMissingFieldFromInterface(SchemaNode node, string fieldName, string interfaceName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' is missing field '{fieldName}' declared on interface '{interfaceName}'.");
    public static ValidationException TypeMissingFieldArgumentFromInterface(SchemaNode node, string fieldName, string interfaceName, string argumentName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' field '{fieldName}' is missing argument '{argumentName}' declared on interface '{interfaceName}'.");
    public static ValidationException TypeFieldArgumentTypeFromInterface(SchemaNode node, string fieldName, string interfaceName, string argumentName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' field '{fieldName}' argument '{argumentName}' has different type to the declared interface '{interfaceName}'.");
    public static ValidationException TypeFieldArgumentNonNullFromInterface(SchemaNode node, string fieldName, string interfaceName, string argumentName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' field '{fieldName}' argument '{argumentName}' cannot be non-null type because not declared on interface '{interfaceName}'.");
    public static ValidationException TypeFieldReturnNotCompatibleFromInterface(SchemaNode node, string fieldName, string interfaceName) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' field '{fieldName}' return type not a sub-type of matching field on interface '{interfaceName}'.");
    public static ValidationException InputObjectCircularReference(SchemaNode node) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' has circular reference requiring a non-null value.");
    public static ValidationException DirectiveCircularReference(SchemaNode node) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' has circular reference to itself.");
    public static ValidationException DirectiveNotAllowedLocation(SchemaNode node, SchemaNode parentNode, params SchemaNode?[] extraNodes) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' is not specified for use on {ExpandNodesOf(extraNodes)}{parentNode.OutputElement.ToLower()} '{parentNode.OutputName}' location.");
    public static ValidationException DirectiveNotRepeatable(SchemaNode node, SchemaNode parentNode, params SchemaNode?[] extraNodes) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' is not repeatable but has been applied multiple times on {ExpandNodesOf(extraNodes)}{parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException DirectiveArgumentNotDefined(SchemaNode node, string argumentName, SchemaNode parentNode, params SchemaNode?[] extraNodes) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' does not define argument '{argumentName}' provided on {ExpandNodesOf(extraNodes)}{parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException DirectiveMandatoryArgumentMissing(SchemaNode node, string argumentName, SchemaNode parentNode, params SchemaNode?[] extraNodes) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' has mandatory argument '{argumentName}' missing on {ExpandNodesOf(extraNodes)}{parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");
    public static ValidationException DirectiveMandatoryArgumentNull(SchemaNode node, string argumentName, SchemaNode parentNode, params SchemaNode?[] extraNodes) => new(node.Location, $"{node.OutputElement} '{node.OutputName}' has mandatory argument '{argumentName}' that is specified as null on {ExpandNodesOf(extraNodes)}{parentNode.OutputElement.ToLower()} '{parentNode.OutputName}'.");

    private static string ExpandNodesOf(SchemaNode?[] extraNodes)
    {
        StringBuilder sb = new StringBuilder();

        foreach (var extraNode in extraNodes.Reverse())
            if (extraNode is not null)
                sb.Append($"{extraNode.OutputElement.ToLower()} '{extraNode.OutputName}' of ");

        return sb.ToString();
    }
}


