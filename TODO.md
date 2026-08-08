# Current

- getSelection should return null when selection not in editor. Maybe rename to getEditorSelection. C# should handle null selection.
- add links tests
- remove links

## Marks Toggle with inheriting

Marks are an Object with functions. They know on their own how they can be toggled. So instead of toggle removes/adds, it just calls that, or if not there add.
However, the toggle must also take inheriting into account. So maybe instead of that, there is a static class
that knows how to compute Marks. It just takes a MarkKey, Node and Action. Then it will change the Mark on the node.
It also takes into account the ComputedMarks and will therefor either just remove or add marks. Or instead of removing set things like `FontStyle = normal`.
Or instead of it setting it, it can just compute. It is basically a lookup table for the mark values for active and inactive.
You give the key and action, and it will check if it has to set the default value or the active value.
And yes this will end up with a shit ton of marks that are just `FontStyle: normal`, but whatever, it is simple and stupid.

# Features

- [x] Marks
  - [x] Bold
  - [x] Italic
  - [x] Color
- [x] Make marks continues when Typing
  - [x] Typing
  - [x] Pending Marks
  - [x] Marks for empty blocks
  - [x] carry over mark to new block
- [x] MenuBar shows active marks
  - Also add this to the debug viewer.
- [ ] Inline Elements
  - For now just Links 
  - Later maybe like Chapter References and co.
- [ ] Text Alignment
  - Left, Right
  - Middle
  - Justify
- [ ] Text Size
- [ ] Text Decoration
  - Strike
  - Underline
  - Underline squiggly
- [ ] Lists
  - Bullet point
  - numbered
- [ ] Format Helpers
  - Format Clear
  - Format Brush
- [ ] Shortcut Keys
  - Keybinds like CTRL + B for Bold
- [ ] History with Undo and Redo features
- [ ] Auto Heading Numbering
  - This needs support for read-only text that is being displayed
- [ ] Pages
  - [ ] Extend Layout Engine to automatically make pages
  - [ ] Add custom page Break UI & CTRL + Enter
- [ ] Header & Footer
  - Maybe opt out option
- [ ] Auto Page Numbering
- [ ] Default Formating for Block type settings
  - Things like text spacing and so on are done here. Do not support as mark for now
  - Those are Document Settings, not CRDT. So it is a global setting you can choose without effecting the tree.
  - Those settings are implicitly included in the rendering of marks. So will go over the UpdateMarkDiff, but are not on the nodes itself
- [ ] Auto Gen "Table of Contents"

# Side Quests

- Selection Update Event should check if selection is even in the editor before trying to look for nodes
- Color picker is not that nice to use. The cursor is not set anymore when clicking away. So you have to remember where you were

# Improvements?

- Command Handler get from DI via the Command without direct references is a bit, meh. Sure it allows me currently to always prepare the base stuff for all command handlers beforehand. But I could also make a base class instead
- HTML Tag on the Node itself is not so nice. Would be better if this is purely done by the rendering part.
