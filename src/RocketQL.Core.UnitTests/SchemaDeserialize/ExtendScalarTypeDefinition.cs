﻿namespace RocketQL.Core.UnitTests.SchemaDeserialize;

public class ExtendScalarTypeDefinition : UnitTestBase
{
    [Fact]
    public void Minimum()
    {
        var documentNode = Serialization.SchemaDeserialize("extend scalar foo");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxExtendScalarTypeDefinitionNode>(definition);
        var scalar = ((SyntaxExtendScalarTypeDefinitionNode)definition);
        Assert.Equal("foo", scalar.Name);
        scalar.Directives.NotNull().Count(0);
    }

    [Fact]
    public void Directive()
    {
        var documentNode = Serialization.SchemaDeserialize("extend scalar foo @bar");

        var definition = documentNode.NotNull().One();
        Assert.IsType<SyntaxExtendScalarTypeDefinitionNode>(definition);
        var scalar = ((SyntaxExtendScalarTypeDefinitionNode)definition);
        Assert.Equal("foo", scalar.Name);
        var directive = scalar.Directives.NotNull().One();
        Assert.Equal("@bar", directive.Name);
    }

    [Theory]
    [InlineData("extend")]
    [InlineData("extend scalar")]
    [InlineData("extend scalar foo @")]
    public void UnexpectedEndOfFile(string text)
    {
        try
        {
            var documentNode = Serialization.SchemaDeserialize(text);
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
}



