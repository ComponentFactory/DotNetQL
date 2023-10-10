﻿namespace RocketQL.Core.Nodes;

public static class CloneNodes
{
    public static ValidatedSchemaNode Clone(this ValidatedSchemaNode node)
    {
        return new ValidatedSchemaNode(new Dictionary<string, DirectiveDefinitionNode>(node.Directives));
    }
}
