﻿namespace RocketQL.Core.UnitTests.SchemaDeserialize;

public class ScalarTypeDefinition : UnitTestBase
{
    [Fact]
    public void Minimum()
    {
        var documentNode = Serialization.SchemaDeserialize("scalar foo");

        var scalar = documentNode.NotNull().ScalarTypes.NotNull().One();
        Assert.Equal(string.Empty, scalar.Description);
        Assert.Equal("foo", scalar.Name);
        scalar.Directives.NotNull().Count(0);
    }

    [Theory]
    [InlineData("\"bar\" scalar foo")]
    [InlineData("\"\"\"bar\"\"\" scalar foo")]
    public void Description(string schema)
    {
        var documentNode = Serialization.SchemaDeserialize(schema);

        var scalar = documentNode.NotNull().ScalarTypes.NotNull().One();
        Assert.Equal("bar", scalar.Description);
        Assert.Equal("foo", scalar.Name);
        scalar.Directives.NotNull().Count(0);
    }

    [Fact]
    public void Directive()
    {
        var documentNode = Serialization.SchemaDeserialize("scalar foo @bar");

        var scalar = documentNode.NotNull().ScalarTypes.NotNull().One();
        Assert.Equal(string.Empty, scalar.Description);
        Assert.Equal("foo", scalar.Name);
        var directive = scalar.Directives.NotNull().One();
        Assert.Equal("bar", directive.Name);
    }

    [Theory]
    [InlineData("scalar")]
    [InlineData("scalar foo @")]
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



