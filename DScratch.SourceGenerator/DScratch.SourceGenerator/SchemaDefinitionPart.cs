using System.Collections.Generic;

namespace DScratch.SourceGenerator;

public class SchemaDefinitionPart
{
    public string Name { get; set; } = null!;

    public List<Node> Nodes { get; set; } = null!;
}