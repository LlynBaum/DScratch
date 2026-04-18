using System.Collections.Generic;

namespace DScratch.SourceGenerator;

public class SchemaDefinitionPart
{
    public string Name { get; set; } = null!;

    public List<Spec> Nodes { get; set; } = null!;
}