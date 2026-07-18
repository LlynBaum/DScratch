# Setup

The Project has 3 Components:
- ASP.NET Host (with Service-Side-Prerender)
- WASM C#
- TS alongside WASM for direct access to Browser APIs

# General flow of actions

A simple chain of actions done for every event in the browser looks like this:

1. Browser Input Event
2. TS caputres event
3. TS sends Event and Information about Cursor Position to WASM C# Layer
4. C# Invoke Event Handler
5. handler performs changes on the Document Tree in form of a transaction
6. Transaction generates a list of Diffs
7. Transaction send to Browser
8. TS manipulates dom

The goal is to let C# do all the heavy work and TS just blindly follows the Instruction that are given to it by C#.
This architecture has the following reasons:
- Interactive WASM rerender will break cursor Positions and is less flexible. So updating the DOM via WASM Interactive is not a valid option
- TS can hanlde Cursort Positions and dom manipulation much better
- C# is faster, has type safty and generely better for heavy work

Also an event can start in the C# Layer. The MenuBar at the Top is WASM interactive. In that case:

1. WASM Callback in C# on click is called
2. C# asks TS for the Cursor Position
3. Dispatches a command to performe changes in the CRDT Tree
4. Transaction generates a list of Diffs
7. Transaction send to Browser
8. TS manipulates dom

# Browser

The browser uses a contendeditable div to allow the user to type.
The brwoser part is writen in TypeScript.

## Input Handling

The TS part captures all input events on the browser via "beforeInput" and instantly does preventDefault(). Since different browser implemented contendeditable differently I'll do it myself. Then the event is pushed to the C# layer.

## Transaction

Transactions are a list of changes done to the document by the C# layer. The TS part will manipulate the DOM based on those Diffs. It only has very few types of Diff:

- InsertText
- DeleteText
- InsertInlineElement
- MoveInlineElement
- InsertBlockElement
- MoveBlockElement
- DeleteElement

Transactions also can contain metadata. Currently it only supports metadata for the CursorPosition.

### Cursor Position

When the C# Layer provides a Cursor Position the TS layer checks if the user hasn't moved it's cursor in the mean time, if so it sets the cursor to the given position.
This is not fully robust for all edge cases but does it's job for now.

# WASM C# Layer

Here is done all the work. C# has a internal representation of the document, using CRDT. C# can make complex changes on the Tree, but all those changes will be translated into a list of the Diff Types the TS Layer can handle.

The C# Layer does things like:
- Edit Text
- Insert new Elements
- Calculate Cursor Positions
- Format Text or other things

# ASP.NET Host

The Host does nearly nothing, it has only the following Tasks:

- Routing
- Serve WASM files and Static Assets 
- Server Side Prerendering
- Middleman for opening Peer-to-Peer connections (not implemented yet)