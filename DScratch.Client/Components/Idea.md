# Insert Text

JavaScript tracks changes with onInput. It knows the index of the character the new text started to be inserted.

JS Sends the Paragraph Info (a data-attribute with id or via dom structure) with the index of where curser started and text.
C# finds via the PInfo the Paragraph Node and via index it finds the CharNode where it has to go. I create the nodes and adds them.
So We do not have to rerender the doc via WASM and no Focus problems. C# still can update everything.

This should work, even with Colab features later. Since the changes from remote will be merged after it finds the Nodes,
where it has to go.


# Detect new Paragrapth


IDK yet, still have to add Node Ids as well xD