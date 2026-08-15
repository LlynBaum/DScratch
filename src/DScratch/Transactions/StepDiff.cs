using System.Text.Json.Serialization;

namespace DScratch.Transactions;

[JsonDerivedType(typeof(InsertTextDiff))]
[JsonDerivedType(typeof(DeleteTextDiff))]
[JsonDerivedType(typeof(InsertElementDiff))]
[JsonDerivedType(typeof(DeleteElementDiff))]
[JsonDerivedType(typeof(MoveDiff))]
[JsonDerivedType(typeof(UpdateMarksDiff))]
[JsonDerivedType(typeof(UpdateAttributesDiff))]
// ReSharper disable NotAccessedPositionalProperty.Global / is deserialized to be used in JS.
public abstract record StepDiff(string Type)
{
    public const string InsertText = "insertText";
    public const string DeleteText = "deleteText";
    public const string InsertElement = "insertElement";
    public const string DeleteElement = "deleteElement";
    public const string Move = "move";
    public const string UpdateMarks = "updateMarks";
    public const string UpdateAttributes = "updateAttributes";

    public record InsertTextDiff(string ParentId, int Offset, string Text) : StepDiff(InsertText);

    public record DeleteTextDiff(string ParentId, int Offset, int Length) : StepDiff(DeleteText);

    public record InsertElementDiff(
        string ParentId,
        string? PreviousSiblingId,
        string TagName,
        string NewNodeId,
        IReadOnlyDictionary<string, string>? Attributes = null)
        : StepDiff(InsertElement);

    public record DeleteElementDiff(string TargetId) : StepDiff(DeleteElement);

    public record MoveDiff(string TargetNodeId, string TargetParentId, string? PreviousSiblingId) : StepDiff(Move);

    public record UpdateMarksDiff(string NodeId, Dictionary<string, string> Marks) : StepDiff(UpdateMarks);

    public record UpdateAttributesDiff(string NodeId, Dictionary<string, string> Attributes)
        : StepDiff(UpdateAttributes);
}