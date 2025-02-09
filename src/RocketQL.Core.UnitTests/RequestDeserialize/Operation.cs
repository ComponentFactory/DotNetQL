﻿namespace RocketQL.Core.UnitTests.RequestDeserialize;

public class Operation : UnitTestBase
{
    [Theory]
    [InlineData("query { foo }", OperationType.QUERY)]
    [InlineData("mutation { foo }", OperationType.MUTATION)]
    [InlineData("subscription { foo }", OperationType.SUBSCRIPTION)]
    public void OperationTypes(string schema, OperationType operationType)
    {
        var documentNode = Serialization.RequestDeserialize(schema);

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(operationType, operation.Operation);
        Assert.Equal("", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        field.Arguments.NotNull().Count(0);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void OperationName()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        field.Arguments.NotNull().Count(0);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void FieldAlias()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo : bar }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("foo", field.Alias);
        Assert.Equal("bar", field.Name);
        field.Arguments.NotNull().Count(0);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void FieldArgument()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo(bar: 3) }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        var argument = field.Arguments.NotNull().One();
        Assert.Equal("bar", argument.Name);
        Assert.Equal("3", argument.Value.IsType<IntValueNode>().Value);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void FieldArgumentUsingVariable()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo(bar: $var) }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        var argument = field.Arguments.NotNull().One();
        Assert.Equal("bar", argument.Name);
        Assert.Equal("$var", argument.Value.IsType<VariableValueNode>().Value);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void FieldDirective()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo @bar }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        field.Arguments.NotNull().Count(0);
        var directive = field.Directives.NotNull().One();
        Assert.Equal("@bar", directive.Name);
        field.SelectionSet.NotNull().Count(0);
    }

    [Fact]
    public void FieldSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo { bar } }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field1 = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field1.Alias);
        Assert.Equal("foo", field1.Name);
        field1.Arguments.NotNull().Count(0);
        field1.Directives.NotNull().Count(0);
        var field2 = (SyntaxFieldSelectionNode)field1.SelectionSet.NotNull().One();
        Assert.Equal("", field2.Alias);
        Assert.Equal("bar", field2.Name);
        field2.Arguments.NotNull().Count(0);
        field2.Directives.NotNull().Count(0);
    }

    [Fact]
    public void FieldAliasArgumentDirectiveSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo : bar(fizz: 3) @bar { foo2 : bar2(fizz2: 3) @bar2} }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field1 = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("foo", field1.Alias);
        Assert.Equal("bar", field1.Name);
        var argument1 = field1.Arguments.NotNull().One();
        Assert.Equal("fizz", argument1.Name);
        Assert.Equal("3", argument1.Value.IsType<IntValueNode>().Value);
        var directive1 = field1.Directives.NotNull().One();
        Assert.Equal("@bar", directive1.Name);
        var field2 = (SyntaxFieldSelectionNode)field1.SelectionSet.NotNull().One();
        Assert.Equal("foo2", field2.Alias);
        Assert.Equal("bar2", field2.Name);
        var argument2 = field2.Arguments.NotNull().One();
        Assert.Equal("fizz2", argument2.Name);
        Assert.Equal("3", argument2.Value.IsType<IntValueNode>().Value);
        var directive2 = field2.Directives.NotNull().One();
        Assert.Equal("@bar2", directive2.Name);
    }

    [Fact]
    public void FragmentSpread()
    {
        var documentNode = Serialization.RequestDeserialize("query name { ... bar }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var spread = (SyntaxFragmentSpreadSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("bar", spread.Name);
        spread.Directives.NotNull().Count(0);
    }

    [Fact]
    public void FragmentSpreadDirective()
    {
        var documentNode = Serialization.RequestDeserialize("query name { ... bar @foo }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var spread = (SyntaxFragmentSpreadSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("bar", spread.Name);
        var directive = spread.Directives.NotNull().One();
        Assert.Equal("@foo", directive.Name);
    }

    [Fact]
    public void InlineFragmentSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("query name { ... { bar } }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var inline = (SyntaxInlineFragmentSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", inline.TypeCondition);
        inline.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)inline.SelectionSet.NotNull().One();
        Assert.Equal("bar", field.Name);
    }

    [Fact]
    public void InlineFragmentOnTypeSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("query name { ... on foo { bar } }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var inline = (SyntaxInlineFragmentSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("foo", inline.TypeCondition);
        inline.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)inline.SelectionSet.NotNull().One();
        Assert.Equal("bar", field.Name);
    }

    [Fact]
    public void InlineFragmentOnTypeDirectiveSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("query name { ... on foo @fizz { bar } }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var inline = (SyntaxInlineFragmentSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("foo", inline.TypeCondition);
        var directive = inline.Directives.NotNull().One();
        Assert.Equal("@fizz", directive.Name);
        var field = (SyntaxFieldSelectionNode)inline.SelectionSet.NotNull().One();
        Assert.Equal("bar", field.Name);
    }

    [Fact]
    public void FieldFragmentSpreadInlineFragment()
    {
        var documentNode = Serialization.RequestDeserialize("query name { foo ... bar ... on fizz { buzz } }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        operation.SelectionSet.NotNull().Count(3);
        var field1 = (SyntaxFieldSelectionNode)operation.SelectionSet[0];
        Assert.Equal("", field1.Alias);
        Assert.Equal("foo", field1.Name);
        field1.Directives.NotNull().Count(0);
        var spread = (SyntaxFragmentSpreadSelectionNode)operation.SelectionSet[1];
        Assert.Equal("bar", spread.Name);
        var inline = (SyntaxInlineFragmentSelectionNode)operation.SelectionSet[2];
        Assert.Equal("fizz", inline.TypeCondition);
        inline.Directives.NotNull().Count(0);
        var field2 = (SyntaxFieldSelectionNode)inline.SelectionSet.NotNull().One();
        Assert.Equal("buzz", field2.Name);
    }

    [Fact]
    public void NamedOperationArgument()
    {
        var documentNode = Serialization.RequestDeserialize("query name($arg: Int) { foo }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("name", operation.Name);
        var variable = operation.VariableDefinitions.NotNull().One();
        Assert.Equal("$arg", variable.Name);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        field.Arguments.NotNull().Count(0);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }


    [Fact]
    public void BareSelectionSet()
    {
        var documentNode = Serialization.RequestDeserialize("{ foo }");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxOperationDefinitionNode>(definition);
        var operation = ((SyntaxOperationDefinitionNode)definition);
        Assert.Equal(OperationType.QUERY, operation.Operation);
        Assert.Equal("", operation.Name);
        operation.VariableDefinitions.NotNull().Count(0);
        operation.Directives.NotNull().Count(0);
        var field = (SyntaxFieldSelectionNode)operation.SelectionSet.NotNull().One();
        Assert.Equal("", field.Alias);
        Assert.Equal("foo", field.Name);
        field.Arguments.NotNull().Count(0);
        field.Directives.NotNull().Count(0);
        field.SelectionSet.NotNull().Count(0);
    }

    [Theory]
    [InlineData("query")]
    [InlineData("query {")]
    [InlineData("query { foo")]
    [InlineData("query { ... on")]
    [InlineData("query { ... on foo")]
    public void UnexpectedEndOfFile(string text)
    {
        try
        {
            var documentNode = Serialization.RequestDeserialize(text);
        }
        catch (SyntaxException ex)
        {
            Assert.Equal($"Unexpected end of file encountered.", ex.Message);
        }
        catch
        {
            Assert.Fail("Wrong exception");
        }
    }

    [Theory]
    [InlineData("query { ... foo")]
    public void SelectionSetInvalidToken(string text)
    {
        try
        {
            var documentNode = Serialization.RequestDeserialize(text);
        }
        catch (SyntaxException ex)
        {
            Assert.Equal($"Found token 'EndOfText' instead of either a name or spread operator inside the selection set.", ex.Message);
        }
        catch
        {
            Assert.Fail("Wrong exception");
        }
    }
}



