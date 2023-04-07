﻿using System.Collections.Generic;

namespace RocketQL.Core;

public ref struct Parser
{
    private Tokenizer _tokenizer;
    private string? _description = null;

    public Parser(ReadOnlySpan<char> text)
    {
        _tokenizer = new Tokenizer(text);
    }

    public DocumentNode Parse()
    {
        OperationNodeList _operations = new();
        SchemaNodeList _schemas = new();
        ScalarTypeDefinitionNodeList _scalarTypeDefinitions = new();
        ObjectTypeDefinitionNodeList _objectTypeDefinitions = new();
        InterfaceTypeDefinitionNodeList _interfaceTypeDefinitions = new();
        UnionTypeDefinitionNodeList _unionTypeDefinitions = new();
        EnumTypeDefinitionNodeList _enumTypeDefinitions = new();
        InputObjectTypeDefinitionNodeList _inputObjectTypeDefinitions = new();
        DirectiveDefinitionNodeList _directiveDefinitions = new();

        // Move to the first real token
        _tokenizer.Next();

        // Keep processing until we reach the end of the tokens or throw because of an exception
        while(_tokenizer.TokenKind != TokenKind.EndOfText)
        {
            switch(_tokenizer.TokenKind) 
            {
                case TokenKind.StringValue:
                    _description = _tokenizer.TokenString;
                    _tokenizer.Next();
                    break;
                case TokenKind.Name:
                    switch (_tokenizer.TokenValue)
                    {
                        case "query":
                        case "mutation":
                        case "subscription":
                            _operations.Add(ParseOperation());
                            break;
                        case "schema":
                            _schemas.Add(ParseSchema());
                            break;
                        case "scalar":
                            _scalarTypeDefinitions.Add(ParseScalarTypeDefinition());
                            break;
                        case "type":
                            _objectTypeDefinitions.Add(ParseObjectTypeDefinition());
                            break;
                        case "interface":
                            _interfaceTypeDefinitions.Add(ParseInterfaceTypeDefinition());
                            break;
                        case "union":
                            _unionTypeDefinitions.Add(ParseUnionTypeDefinition());
                            break;
                        case "enum":
                            _enumTypeDefinitions.Add(ParseEnumTypeDefinition());
                            break;
                        case "input":
                            _inputObjectTypeDefinitions.Add(ParseInputObjectTypeDefinition());
                            break;
                        case "directive":
                            _directiveDefinitions.Add(ParseDirectiveDefinition());
                            break;
                        default:
                            throw SyntaxException.UnrecognizedKeyword(_tokenizer.Location, _tokenizer.TokenValue);
                    }
                    break;
                default:
                    throw SyntaxException.UnrecognizedToken(_tokenizer.Location, _tokenizer.TokenKind);
            }
        }

        return new DocumentNode(_operations,
                                _schemas,
                                _scalarTypeDefinitions,
                                _objectTypeDefinitions,
                                _interfaceTypeDefinitions,
                                _unionTypeDefinitions,
                                _enumTypeDefinitions,
                                _inputObjectTypeDefinitions,
                                _directiveDefinitions);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OperationNode ParseOperation()
    {
        OperationType operationType = OperationTypeFromTokenValue();
        MandatoryNext();

        string name = string.Empty;
        if (_tokenizer.TokenKind == TokenKind.Name)
        {
            name = _tokenizer.TokenValue;
            MandatoryNext();
        }

        var variables = ParseVariablesOptionalDefinition();
        var directives = ParseDirectivesOptional();
        var selectionSet = ParseSelectionSet();

        return new OperationNode(operationType, name, variables, directives, selectionSet);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SchemaNode ParseSchema()
    {
        MandatoryNext();
        var directives = ParseDirectivesOptional();
        MandatoryTokenNext(TokenKind.LeftCurlyBracket);

        OperationTypeDefinitionNodeList list = new();

        do
        {
            MandatoryToken(TokenKind.Name);
            OperationType operationType = OperationTypeFromTokenValue();
            MandatoryNextToken(TokenKind.Colon);
            MandatoryNextToken(TokenKind.Name);
            string namedType = _tokenizer.TokenValue;
            MandatoryNext();

            list.Add(new OperationTypeDefinitionNode(operationType, namedType));

        } while (_tokenizer.TokenKind != TokenKind.RightCurlyBracket);
        
        _tokenizer.Next();
        return new SchemaNode(UseTopLevelDescription(), directives, list);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ScalarTypeDefinitionNode ParseScalarTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var directives = ParseDirectivesOptional();

        return new ScalarTypeDefinitionNode(UseTopLevelDescription(), name, directives);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ObjectTypeDefinitionNode ParseObjectTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var implementsInterfaces = ParseImplementsInterfacesOptional();
        var directives = ParseDirectivesOptional();
        var fields = ParseFieldsOptionalDefinition();

        return new ObjectTypeDefinitionNode(UseTopLevelDescription(), name, implementsInterfaces, directives, fields);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InterfaceTypeDefinitionNode ParseInterfaceTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var implementsInterfaces = ParseImplementsInterfacesOptional();
        var directives = ParseDirectivesOptional();
        var fields = ParseFieldsOptionalDefinition();

        return new InterfaceTypeDefinitionNode(UseTopLevelDescription(), name, implementsInterfaces, directives, fields);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private UnionTypeDefinitionNode ParseUnionTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var directives = ParseDirectivesOptional();
        var memberTypes = ParseMemberTypesOptional();

        return new UnionTypeDefinitionNode(UseTopLevelDescription(), name, directives, memberTypes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EnumTypeDefinitionNode ParseEnumTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var directives = ParseDirectivesOptional();
        var enumValueTypes = ParseEnumValueTypesOptional();

        return new EnumTypeDefinitionNode(UseTopLevelDescription(), name, directives, enumValueTypes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InputObjectTypeDefinitionNode ParseInputObjectTypeDefinition()
    {
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        _tokenizer.Next();
        var directives = ParseDirectivesOptional();
        MandatoryTokenNext(TokenKind.LeftCurlyBracket);
        var inputFields = ParseInputValueListDefinition();
        MandatoryToken(TokenKind.RightCurlyBracket);
        _tokenizer.Next();

        return new InputObjectTypeDefinitionNode(UseTopLevelDescription(), name, directives, inputFields);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private DirectiveDefinitionNode ParseDirectiveDefinition()
    {
        MandatoryNextToken(TokenKind.At);
        MandatoryNextToken(TokenKind.Name);
        string name = _tokenizer.TokenValue;
        MandatoryNext();
        var arguments = ParseArgumentsOptionalDefinition();
        var repeatable = OptionalKeyword("repeatable");
        MandatoryKeyword("on");
        var directiveLocations = ParseDirectiveLocations();

        return new DirectiveDefinitionNode(UseTopLevelDescription(), name, arguments, repeatable, directiveLocations);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NameList ParseImplementsInterfacesOptional()
    {
        NameList list = new();

        if (OptionalKeyword("implements"))
        {
            OptionalToken(TokenKind.Ampersand);
            MandatoryToken(TokenKind.Name);
            list.Add(_tokenizer.TokenValue);
            _tokenizer.Next();

            while(_tokenizer.TokenKind == TokenKind.Ampersand)
            {
                MandatoryNextToken(TokenKind.Name);
                list.Add(_tokenizer.TokenValue);
                _tokenizer.Next();
            }
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private VariableDefinitionNodeList ParseVariablesOptionalDefinition()
    {
        if (_tokenizer.TokenKind == TokenKind.LeftParenthesis)
        {
            MandatoryNext();
            var arguments = ParseVariableslDefinition();
            MandatoryTokenNext(TokenKind.RightParenthesis);
            return arguments;
        }

        return new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private VariableDefinitionNodeList ParseVariableslDefinition()
    {
        VariableDefinitionNodeList list = new();

        do
        {
            MandatoryTokenNext(TokenKind.Dollar);
            MandatoryToken(TokenKind.Name);
            string name = _tokenizer.TokenValue;
            MandatoryNextToken(TokenKind.Colon);
            MandatoryNext();
            var type = ParseType();
            var defaultValue = ParseDefaultValueOptional();
            var directives = ParseDirectivesOptional();
            list.Add(new VariableDefinitionNode(name, type, defaultValue, directives));

        } while (_tokenizer.TokenKind == TokenKind.Dollar);

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SelectionNodeList ParseSelectionSetOptional()
    {
        if (_tokenizer.TokenKind == TokenKind.LeftCurlyBracket)
            return ParseSelectionSet();

        return new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private SelectionNodeList ParseSelectionSet()
    {
        SelectionNodeList list = new();

        MandatoryTokenNext(TokenKind.LeftCurlyBracket);

        do
        {
            switch (_tokenizer.TokenKind)
            {
                case TokenKind.Name:
                    {
                        MandatoryToken(TokenKind.Name);
                        string alias = string.Empty;
                        string name = _tokenizer.TokenValue;
                        MandatoryNext();

                        if (_tokenizer.TokenKind == TokenKind.Colon)
                        {
                            MandatoryNextToken(TokenKind.Name);
                            alias = name;
                            name = _tokenizer.TokenValue;
                            MandatoryNext();
                        }

                        var arguments = ParseArgumentsOptional(constant: false);
                        var directives = ParseDirectivesOptional();
                        var selectionSet = ParseSelectionSetOptional();
                        list.Add(new FieldSelectionNode(alias, name, arguments, directives, selectionSet));
                    }
                    break;
                case TokenKind.Spread:
                    {
                        // TODO
                    }
                    break;
                default:
                    throw SyntaxException.SelectionSetInvalidToken(_tokenizer.Location, _tokenizer.TokenKind);

            }
        } while (_tokenizer.TokenKind != TokenKind.RightCurlyBracket);

        _tokenizer.Next();
        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private FieldDefinitionNodeList ParseFieldsOptionalDefinition()
    {
        FieldDefinitionNodeList list = new();

        if (_tokenizer.TokenKind == TokenKind.LeftCurlyBracket)
        {
            MandatoryNext();

            do
            {
                var description = OptionalString();
                MandatoryToken(TokenKind.Name);
                string name = _tokenizer.TokenValue;
                MandatoryNext();
                var arguments = ParseArgumentsOptionalDefinition();
                MandatoryTokenNext(TokenKind.Colon);
                var type = ParseType();
                var directives = ParseDirectivesOptional();
                list.Add(new FieldDefinitionNode(description ?? string.Empty, name, arguments, type, directives));

            } while (_tokenizer.TokenKind != TokenKind.RightCurlyBracket);

            _tokenizer.Next();
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EnumValueDefinitionList ParseEnumValueTypesOptional()
    {
        EnumValueDefinitionList list = new();

        if (_tokenizer.TokenKind == TokenKind.LeftCurlyBracket)
        {
            MandatoryNext();

            do
            {
                var description = OptionalString();
                MandatoryToken(TokenKind.Name);
                string name = _tokenizer.TokenValue;
                MandatoryNext();
                var directives = ParseDirectivesOptional();
                list.Add(new EnumValueDefinition(description ?? string.Empty, name, directives));

            } while (_tokenizer.TokenKind != TokenKind.RightCurlyBracket);

            _tokenizer.Next();
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private NameList ParseMemberTypesOptional()
    {
        NameList list = new();

        if (_tokenizer.TokenKind == TokenKind.Equals)
        {
            MandatoryNext();
            OptionalToken(TokenKind.Vertical);
            MandatoryToken(TokenKind.Name);
            list.Add(_tokenizer.TokenValue);
            _tokenizer.Next();

            while (_tokenizer.TokenKind == TokenKind.Vertical)
            {
                MandatoryNextToken(TokenKind.Name);
                list.Add(_tokenizer.TokenValue);
                _tokenizer.Next();
            }
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InputValueDefinitionNodeList ParseArgumentsOptionalDefinition()
    {
        if (_tokenizer.TokenKind == TokenKind.LeftParenthesis)
        {
            MandatoryNext();
            var arguments = ParseInputValueListDefinition();
            MandatoryTokenNext(TokenKind.RightParenthesis);
            return arguments;
        }

        return new();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InputValueDefinitionNodeList ParseInputValueListDefinition()
    {
        InputValueDefinitionNodeList list = new();

        do
        {
            var description = OptionalString();
            MandatoryToken(TokenKind.Name);
            string name = _tokenizer.TokenValue;
            MandatoryNextToken(TokenKind.Colon);
            MandatoryNext();
            var type = ParseType();
            var defaultValue = ParseDefaultValueOptional();
            var directives = ParseDirectivesOptional();
            list.Add(new InputValueDefinitionNode(description ?? string.Empty, name, type, defaultValue, directives));

        } while ((_tokenizer.TokenKind == TokenKind.Name) || (_tokenizer.TokenKind == TokenKind.StringValue));

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueNode? ParseDefaultValueOptional()
    {
        if (_tokenizer.TokenKind != TokenKind.Equals)
            return null;

        MandatoryNext();
        return ParseValue(constant: true);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueNode ParseValue(bool constant)
    {
        ValueNode node;

        switch (_tokenizer.TokenKind)
        {
            case TokenKind.Dollar:
                {
                    if (constant)
                        throw SyntaxException.TokenNotAllowedHere(_tokenizer.Location, _tokenizer.TokenKind);

                    MandatoryNextToken(TokenKind.Name);
                    node = new VariableValueNode(_tokenizer.TokenValue);
                }
                break;
            case TokenKind.IntValue:
                node = new IntValueNode(_tokenizer.TokenValue);
                break;
            case TokenKind.FloatValue:
                node = new FloatValueNode(_tokenizer.TokenValue);
                break;
            case TokenKind.StringValue:
                node = new StringValueNode(_tokenizer.TokenString);
                break;
            case TokenKind.Name:
                {
                    var name = _tokenizer.TokenValue;
                    node = name switch
                    {
                        "true" => new BooleanValueNode(true),
                        "false" => new BooleanValueNode(false),
                        "null" => new NullValueNode(),
                        _ => new EnumValueNode(name),
                    };
                }
                break;
            case TokenKind.LeftSquareBracket:
                {
                    ValueNodeList list = new();

                    MandatoryNext();
                    while (_tokenizer.TokenKind != TokenKind.RightSquareBracket)
                        list.Add(ParseValue(constant: constant));

                    node = new ListValueNode(list);
                }
                break;
            case TokenKind.LeftCurlyBracket:
                {
                    ObjectFieldNodeList list = new();

                    MandatoryNext();
                    while (_tokenizer.TokenKind != TokenKind.RightCurlyBracket)
                    {
                        MandatoryToken(TokenKind.Name);
                        string name = _tokenizer.TokenValue;
                        MandatoryNextToken(TokenKind.Colon);
                        MandatoryNext();
                        list.Add(new ObjectFieldNode(name, ParseValue(constant: constant)));
                    }

                    node = new ObjectValueNode(list);
                }
                break;
            default:
                throw SyntaxException.TokenNotAllowedHere(_tokenizer.Location, _tokenizer.TokenKind);
        }

        MandatoryNext();
        return node;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private DirectiveNodeList ParseDirectivesOptional()
    {
        DirectiveNodeList list = new();

        while(_tokenizer.TokenKind == TokenKind.At)
        {
            MandatoryNextToken(TokenKind.Name);
            string name = _tokenizer.TokenValue;
            _tokenizer.Next();
            var arguments = ParseArgumentsOptional(true);
            list.Add(new DirectiveNode(name, arguments));
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ObjectFieldNodeList ParseArgumentsOptional(bool constant)
    {
        ObjectFieldNodeList list = new();

        if (_tokenizer.TokenKind == TokenKind.LeftParenthesis)
        {
            MandatoryNext();
            while (_tokenizer.TokenKind != TokenKind.RightParenthesis)
            {
                MandatoryToken(TokenKind.Name);
                string name = _tokenizer.TokenValue;
                MandatoryNextToken(TokenKind.Colon);
                MandatoryNext();
                list.Add(new ObjectFieldNode(name, ParseValue(constant: constant)));
            }

            MandatoryNext();
        }

        return list;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private TypeNode ParseType()
    {
        switch(_tokenizer.TokenKind) 
        {
            case TokenKind.Name:
                {
                    var name = _tokenizer.TokenValue;
                    MandatoryNext();
                    var nonNull = OptionalToken(TokenKind.Exclamation);
                    return new TypeNameNode(name, nonNull);
                }
            case TokenKind.LeftSquareBracket:
                {
                    MandatoryNext();
                    var listType = ParseType();
                    MandatoryTokenNext(TokenKind.RightSquareBracket);
                    var nonNull = OptionalToken(TokenKind.Exclamation);
                    return new TypeListNode(listType, nonNull);
                }
            default:
                throw SyntaxException.TypeMustBeNameOrList(_tokenizer.Location, _tokenizer.TokenKind);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private DirectiveLocations ParseDirectiveLocations()
    {
        MandatoryNextToken(TokenKind.Name);
        DirectiveLocations directiveLocations = StringToDriveLocations(_tokenizer.TokenValue);

        if (_tokenizer.Next())
        {
            while (_tokenizer.TokenKind == TokenKind.Vertical)
            {
                MandatoryNextToken(TokenKind.Name);
                directiveLocations |= StringToDriveLocations(_tokenizer.TokenValue);
                _tokenizer.Next();
            }
        }

        return directiveLocations;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private DirectiveLocations StringToDriveLocations(string location)
    {
        return location switch
        {
            // Executable Directive Locations
            "QUERY" => DirectiveLocations.QUERY,
            "MUTATION" => DirectiveLocations.MUTATION,
            "SUBSCRIPTION" => DirectiveLocations.SUBSCRIPTION,
            "FIELD" => DirectiveLocations.FIELD,
            "FRAGMENT_DEFINITION" => DirectiveLocations.FRAGMENT_DEFINITION,
            "FRAGMENT_SPREAD" => DirectiveLocations.FRAGMENT_SPREAD,
            "INLINE_FRAGMENT" => DirectiveLocations.INLINE_FRAGMENT,
            "VARIABLE_DEFINITION" => DirectiveLocations.VARIABLE_DEFINITION,

            // Type System Directive Locations
            "SCHEMA" => DirectiveLocations.SCHEMA,
            "SCALAR" => DirectiveLocations.SCALAR,
            "OBJECT" => DirectiveLocations.OBJECT,
            "FIELD_DEFINITION" => DirectiveLocations.FIELD_DEFINITION,
            "ARGUMENT_DEFINITION" => DirectiveLocations.ARGUMENT_DEFINITION,
            "INTERFACE" => DirectiveLocations.INTERFACE,
            "UNION" => DirectiveLocations.UNION,
            "ENUM" => DirectiveLocations.ENUM,
            "ENUM_VALUE" => DirectiveLocations.ENUM_VALUE,
            "INPUT_OBJECT" => DirectiveLocations.INPUT_OBJECT,
            "INPUT_FIELD_DEFINITION" => DirectiveLocations.INPUT_FIELD_DEFINITION,

            _ => throw SyntaxException.ExpectedDirectiveLocationNotFound(_tokenizer.Location, location)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string UseTopLevelDescription()
    {
        string? description = _description;
        _description = null;
        return description ?? String.Empty;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private OperationType OperationTypeFromTokenValue()
    {
        return _tokenizer.TokenValue switch
        {
            "query" => OperationType.QUERY,
            "mutation" => OperationType.MUTATION,
            "subscription" => OperationType.SUBSCRIPTION,
            _ => throw SyntaxException.UnrecognizedOperationType(_tokenizer.Location, _tokenizer.TokenValue)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MandatoryNext()
    {
        if ((_tokenizer.TokenKind == TokenKind.EndOfText) || !_tokenizer.Next())
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MandatoryToken(TokenKind token) 
    {
        if (_tokenizer.TokenKind == TokenKind.EndOfText)
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);

        if (_tokenizer.TokenKind != token)
            throw SyntaxException.ExpectedTokenNotFound(_tokenizer.Location, token, _tokenizer.TokenKind);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MandatoryNextToken(TokenKind token)
    {
        if ((_tokenizer.TokenKind == TokenKind.EndOfText) || !_tokenizer.Next())
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);

        if (_tokenizer.TokenKind != token)
            throw SyntaxException.ExpectedTokenNotFound(_tokenizer.Location, token, _tokenizer.TokenKind);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MandatoryTokenNext(TokenKind token)
    {
        if (_tokenizer.TokenKind == TokenKind.EndOfText)
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);

        if (_tokenizer.TokenKind != token)
            throw SyntaxException.ExpectedTokenNotFound(_tokenizer.Location, token, _tokenizer.TokenKind);

        if (!_tokenizer.Next())
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void MandatoryKeyword(string keyword)
    {
        if (_tokenizer.TokenKind == TokenKind.EndOfText)
            throw SyntaxException.UnexpectedEndOfFile(_tokenizer.Location);

        if (_tokenizer.TokenKind != TokenKind.Name)
            throw SyntaxException.ExpectedTokenNotFound(_tokenizer.Location, TokenKind.Name, _tokenizer.TokenKind);

        if (_tokenizer.TokenValue != keyword)
            throw SyntaxException.ExpectedKeywordNotFound(_tokenizer.Location, "on", _tokenizer.TokenValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool OptionalToken(TokenKind tokenKind)
    {
        if (_tokenizer.TokenKind == tokenKind)
        {
            _tokenizer.Next();
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool OptionalKeyword(string keyword)
    {
        if ((_tokenizer.TokenKind == TokenKind.Name) && (_tokenizer.TokenValue == keyword))
        {
            _tokenizer.Next();
            return true;
        }

        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string? OptionalString()
    {
        if (_tokenizer.TokenKind == TokenKind.StringValue)
        {
            string ret = _tokenizer.TokenString;
            _tokenizer.Next();
            return ret;
        }

        return null;
    }
}
