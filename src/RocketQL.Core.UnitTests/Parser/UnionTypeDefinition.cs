﻿namespace RocketQL.Core.UnitTests.Parser;

public class UnionTypeDefinition
{
    [Theory]
    [InlineData("union foo = bar")]
    [InlineData("union foo = | bar")]
    public void OneMember(string schema)
    {
        var t = new Core.Parser(schema);
        var documentNode = t.Parse();

        var union = documentNode.NotNull().UnionTypeDefinitions.NotNull().One();
        Assert.Equal(string.Empty, union.Description);
        Assert.Equal("foo", union.Name);
        var member = union.MemberTypes.NotNull().One();
        Assert.Equal("bar", member);
        union.Directives.IsNull();
    }

    [Theory]
    [InlineData("union foo = bar | fizz")]
    [InlineData("union foo = | bar | fizz")]
    public void TwoMembers(string schema)
    {
        var t = new Core.Parser(schema);
        var documentNode = t.Parse();

        var union = documentNode.NotNull().UnionTypeDefinitions.NotNull().One();
        Assert.Equal(string.Empty, union.Description);
        Assert.Equal("foo", union.Name);
        union.MemberTypes.NotNull().Count(2);
        Assert.Equal("bar", union.MemberTypes[0]);
        Assert.Equal("fizz", union.MemberTypes[1]);
        union.Directives.IsNull();
    }

    [Theory]
    [InlineData("union foo = bar | fizz | hello")]
    [InlineData("union foo = | bar | fizz | hello")]
    public void ThreeMembers(string schema)
    {
        var t = new Core.Parser(schema);
        var documentNode = t.Parse();

        var union = documentNode.NotNull().UnionTypeDefinitions.NotNull().One();
        Assert.Equal(string.Empty, union.Description);
        Assert.Equal("foo", union.Name);
        union.MemberTypes.NotNull().Count(3);
        Assert.Equal("bar", union.MemberTypes[0]);
        Assert.Equal("fizz", union.MemberTypes[1]);
        Assert.Equal("hello", union.MemberTypes[2]);
        union.Directives.IsNull();
    }

    [Theory]
    [InlineData("\"bar\" union foo = bar")]
    [InlineData("\"\"\"bar\"\"\" union foo = bar")]
    public void Description(string schema)
    {
        var t = new Core.Parser(schema);
        var documentNode = t.Parse();

        var union = documentNode.NotNull().UnionTypeDefinitions.NotNull().One();
        Assert.Equal("bar", union.Description);
        Assert.Equal("foo", union.Name);
        var member = union.MemberTypes.NotNull().One();
        Assert.Equal("bar", member);
        union.Directives.IsNull();
    }

    [Fact]
    public void Directive()
    {
        var t = new Core.Parser("union foo @bar = fizz");
        var documentNode = t.Parse();

        var union = documentNode.NotNull().UnionTypeDefinitions.NotNull().One();
        Assert.Equal(string.Empty, union.Description);
        Assert.Equal("foo", union.Name);
        var directive = union.Directives.NotNull().One();
        Assert.Equal("bar", directive.Name);
        var member = union.MemberTypes.NotNull().One();
        Assert.Equal("fizz", member);
    }

    [Theory]
    [InlineData("scalar")]
    [InlineData("scalar foo @")]
    public void UnexpectedEndOfFile(string text)
    {
        var t = new Core.Parser(text);
        try
        {
            var documentNode = t.Parse();
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



