using System.Text.Json.Serialization;

namespace DScratch.Transactions;

[JsonDerivedType(typeof(InsertTextDiff))]
[JsonDerivedType(typeof(DeleteTextDiff))]
[JsonDerivedType(typeof(InsertElementInlineDiff))]
[JsonDerivedType(typeof(InsertElementBlockDiff))]
[JsonDerivedType(typeof(DeleteElementDiff))]
[JsonDerivedType(typeof(MoveInlineDiff))]
[JsonDerivedType(typeof(MoveBlockDiff))]
// ReSharper disable NotAccessedPositionalProperty.Global / is deserialized to be used in JS.
public abstract record StepDiff(string Type)
{
    /// <summary>
    /// Insert text in the DOM.
    /// </summary>
    /// <param name="Parent">Parent Node of the text</param>
    /// <param name="Offset">Offset of characters</param>
    /// <param name="Text">Text to insert</param>
    public record InsertTextDiff(string[] Parent, int Offset, string Text) : StepDiff("insertText");
    
    /// <summary>
    /// Deletes characters in the DOM.
    /// </summary>
    /// <param name="Parent">Parent Node of the text</param>
    /// <param name="Offset">Offset of character</param>
    /// <param name="Length">Amount of characters to delete</param>
    public record DeleteTextDiff(string[] Parent, int Offset, int Length) : StepDiff("deleteText");

    /// <summary>
    /// Inserts a Node as an DOM inline element.
    /// </summary>
    /// <param name="Parent">parent of the new element</param>
    /// <param name="Offset">Offset of characters to insert the element</param>
    /// <param name="TagName">The HTML Tag for the element to insert</param>
    /// <param name="NewNodeId">The id of the Node</param>
    public record InsertElementInlineDiff(string[] Parent, int Offset, string TagName, string NewNodeId) : StepDiff("insertElementInline");
    
    /// <summary>
    /// Inserts a Node as an DOM block element.
    /// </summary>
    /// <param name="Parent">parent of the new element</param>
    /// <param name="PreviousSibling">
    /// The previous sibling this element should be attached to. The new element will be the direct after the sibling.
    /// If null, it will be the first child of the parent.
    /// </param>
    /// <param name="TagName">The HTML Tag for the element to insert</param>
    /// <param name="NewNodeId">The id of the Node</param>
    public record InsertElementBlockDiff(string[] Parent, string[]? PreviousSibling, string TagName, string NewNodeId) : StepDiff("insertElementBlock");

    /// <summary>
    /// Deletes an element from the DOM.
    /// </summary>
    /// <remarks>Also deletes all child elements from the dom</remarks>
    /// <param name="Path">The path of the element to delete</param>
    public record DeleteElementDiff(string[] Path) : StepDiff("deleteElement");
    
    /// <summary>
    /// Removes the target node from the DOM and reinserts it as a child of target parent at the given offset
    /// </summary>
    /// <param name="TargetNodePath">The target to move</param>
    /// <param name="TargetParentPath">The new parent to target</param>
    /// <param name="TargetOffset">the offset of characters of the parent the element should be reinserted</param>
    public record MoveInlineDiff(string[] TargetNodePath, string[] TargetParentPath, int TargetOffset) : StepDiff("moveInline");
    
    /// <summary>
    /// Removes the target node from the DOM and reinserts it as a child of target parent at the given offset
    /// </summary>
    /// <param name="TargetNodePath">The target to move</param>
    /// <param name="TargetParentPath">The new parent to target</param>
    /// <param name="PreviousSibling">
    /// The previous sibling this element should be attached to. The moved element will be the direct after the sibling.
    /// If null, it will be the first child of the parent.
    /// </param>
    public record MoveBlockDiff(string[] TargetNodePath, string[] TargetParentPath, string[]? PreviousSibling) : StepDiff("moveBlock");
}