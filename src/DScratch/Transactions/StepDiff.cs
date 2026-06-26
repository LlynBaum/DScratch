using System.Text.Json.Serialization;

namespace DScratch.Transactions;

[JsonDerivedType(typeof(InsertTextDiff))]
[JsonDerivedType(typeof(DeleteTextDiff))]
[JsonDerivedType(typeof(InsertElementDiff))]
[JsonDerivedType(typeof(DeleteElementDiff))]
[JsonDerivedType(typeof(MoveDiff))]
// ReSharper disable NotAccessedPositionalProperty.Global / is deserialized to be used in JS.
public abstract record StepDiff(string Type)
{
    public const string InsertText = "insertText";
    public const string DeleteText = "deleteText";
    public const string InsertElement = "insertElement";
    public const string DeleteElement = "deleteElement";
    public const string Move = "move";
    
    /// <summary>
    /// Insert text in the DOM.
    /// </summary>
    /// <param name="ParentId">Parent Node of the text</param>
    /// <param name="Offset">Offset of characters</param>
    /// <param name="Text">Text to insert</param>
    public record InsertTextDiff(string ParentId, int Offset, string Text) : StepDiff(InsertText);

    /// <summary>
    /// Deletes characters in the DOM.
    /// </summary>
    /// <param name="ParentId">Parent Node of the text</param>
    /// <param name="Offset">Offset of character</param>
    /// <param name="Length">Amount of characters to delete</param>
    /// <remarks>Attempts to delete the text wrapping element if it is empty.</remarks>
    public record DeleteTextDiff(string ParentId, int Offset, int Length) : StepDiff(DeleteText);

    /// <summary>
    /// Inserts a Node as an DOM block element.
    /// </summary>
    /// <param name="ParentId">parent of the new element</param>
    /// <param name="PreviousSiblingId">
    /// The previous sibling this element should be attached to. The new element will be the direct after the sibling.
    /// If null, it will be the first child of the parent.
    /// </param>
    /// <param name="TagName">The HTML Tag for the element to insert</param>
    /// <param name="NewNodeId">The id of the Node</param>
    public record InsertElementDiff(string ParentId, string? PreviousSiblingId, string TagName, string NewNodeId) : StepDiff(InsertElement);

    /// <summary>
    /// Deletes an element from the DOM.
    /// </summary>
    /// <remarks>Also deletes all child elements from the dom</remarks>
    /// <param name="TargetId">The path of the element to delete</param>
    public record DeleteElementDiff(string TargetId) : StepDiff(DeleteElement);

    /// <summary>
    /// Removes the target node from the DOM and reinserts it as a child of target parent at the given offset
    /// </summary>
    /// <param name="TargetNodeId">The target to move</param>
    /// <param name="TargetParentId">The new parent to target</param>
    /// <param name="PreviousSiblingId">
    /// The previous sibling this element should be attached to. The moved element will be the direct after the sibling.
    /// If null, it will be the first child of the parent.
    /// </param>
    public record MoveDiff(string TargetNodeId, string TargetParentId, string? PreviousSiblingId) : StepDiff(Move);
}