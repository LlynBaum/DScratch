namespace DScratch.Client.JsBridge;

/// <summary>
/// Represents a Slice of the document.
/// </summary>
/// <param name="Start">Index of the first Token</param>
/// <param name="End">Index of the last Token</param>
/// <param name="IncludeParents">
/// defines whether the slice is a "fragment" or a "full hierarchy."
/// false (Default): Extracts only the specific content between from and to.
/// true: Wraps the content in its full ancestor node structure (up to the Doc node).
/// </param>
/// <remarks>See more for Indexing https://prosemirror.net/docs/guide/#doc.indexing</remarks>
public record PmSlice(int Start, int? End = null, bool IncludeParents = false);