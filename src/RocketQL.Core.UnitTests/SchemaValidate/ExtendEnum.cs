﻿namespace RocketQL.Core.UnitTests.SchemaValidation;

public class ExtendEnum : UnitTestBase
{
    [Theory]
    [InlineData("""
                type Query { alpha: Int }
                extend enum foo { FIRST }
                """,                                            "Enum 'foo' cannot be extended because it is not defined.")]
    [InlineData("""
                type Query { alpha: Int }
                extend enum foo { SECOND }
                enum foo { FIRST }
                """,                                            "Enum 'foo' cannot be extended because it is not defined.")]
    [InlineData("""
                type Query { alpha: Int }
                directive @bar on ENUM
                enum foo @bar { FIRST }
                extend enum foo @bar
                """,                                            "Directive '@bar' is not repeatable but has been applied multiple times on enum 'foo'.")]
    [InlineData("""
                type Query { alpha: Int }
                directive @bar on ENUM_VALUE
                enum foo { FIRST @bar }
                extend enum foo { FIRST @bar }
                """,                                            "Directive '@bar' is not repeatable but has been applied multiple times on enum value 'FIRST' of enum 'foo'.")]
    [InlineData("""
                type Query { alpha: Int }
                enum foo { FIRST }
                extend enum foo { FIRST }
                """,                                            "Extend enum 'foo' for existing enum value 'FIRST' does not make any change.")]
    [InlineData("""
                type Query { alpha: Int }
                enum foo { FIRST }
                extend enum foo { SECOND SECOND }
                """,                                            "Extend enum 'foo' has duplicate definition of enum value 'SECOND'.")]
    public void ValidationSingleExceptions(string schemaText, string message)
    {
        SchemaValidationSingleException(schemaText, message);
    }

    [Fact]
    public void AddDirectiveToEnum()
    {
        var schema = new Schema();
        schema.Add("""
                   type Query { fizz: Int }
                   directive @bar on ENUM
                   enum foo { FIRST }
                   extend enum foo @bar                
                   """);
        schema.Validate();

        var foo = schema.Types["foo"] as EnumTypeDefinition;
        Assert.NotNull(foo);
        Assert.Equal("foo", foo.Name);
        Assert.Single(foo.EnumValues);
        var first = foo.EnumValues["FIRST"];
        Assert.NotNull(first);
        var directive = foo.Directives[0];
        Assert.NotNull(directive);
        Assert.Equal("@bar", directive.Name);
    }


    [Fact]
    public void AddDirectiveToEnumValue()
    {
        var schema = new Schema();
        schema.Add("""
                   type Query { fizz: Int }
                   directive @bar on ENUM_VALUE
                   enum foo { FIRST }
                   extend enum foo { FIRST @bar }              
                   """);
        schema.Validate();

        var foo = schema.Types["foo"] as EnumTypeDefinition;
        Assert.NotNull(foo);
        Assert.Equal("foo", foo.Name);
        Assert.Single(foo.EnumValues);
        var first = foo.EnumValues["FIRST"];
        Assert.NotNull(first);
        var directive = first.Directives[0];
        Assert.NotNull(directive);
        Assert.Equal("@bar", directive.Name);
    }

    [Fact]
    public void AddEnumValue()
    {
        var schema = new Schema();
        schema.Add("""
                   type Query { fizz: Int }
                   directive @bar on ENUM
                   enum foo { FIRST }
                   extend enum foo { SECOND }              
                   """);
        schema.Validate();

        var foo = schema.Types["foo"] as EnumTypeDefinition;
        Assert.NotNull(foo);
        Assert.Equal("foo", foo.Name);
        Assert.Equal(2, foo.EnumValues.Count);
        var first = foo.EnumValues["FIRST"];
        Assert.NotNull(first);
        var second = foo.EnumValues["SECOND"];
        Assert.NotNull(first);
    }

    [Fact]
    public void AddEnumValueWithDirective()
    {
        var schema = new Schema();
        schema.Add("""
                   type Query { fizz: Int }
                   directive @bar on ENUM_VALUE
                   enum foo { FIRST }
                   extend enum foo { SECOND @bar }              
                   """);
        schema.Validate();

        var foo = schema.Types["foo"] as EnumTypeDefinition;
        Assert.NotNull(foo);
        Assert.Equal("foo", foo.Name);
        Assert.Equal(2, foo.EnumValues.Count());
        var second = foo.EnumValues["SECOND"];
        Assert.NotNull(second);
        var directive = second.Directives[0];
        Assert.NotNull(directive);
        Assert.Equal("@bar", directive.Name);
    }
}

