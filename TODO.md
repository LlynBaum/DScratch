# Current

- [ ] E2E Tests
- [x] Transaction collects modified nodes
- [ ] Greedy Flow 
  - [ ] Move page down if overflow 
    - [x] Use `Range.extractContent()`
    - [ ] Move text to existing next page & evt to split paragraph
    - [ ] on overflow queue next page for greedyFlow
    - [ ] make split-part not binary, it can go from part 1 - infinity. A block can span over more than 2 pages
    - [ ] When moving stuff to next page, check for empty block that might can be removed from the DOM and are no longer split
    - [ ] check overflow not only for last element, look for the first element that overflows
  - [ ] Move stuff up if underflow
- [ ] Page Number API for C#
  - [x] TS part
  - [ ] implement C# side 
- [ ] Hard Page Break (CTRL + Enter, UI)
- [ ] Transaction focus set should scroll focus into view

## Random stuff

- First check for Hard Page Breaks, if found move everything after. Else check ofr Over or Underflow, push or pull content and go to next page
- Pulling things up when space between last node and page bottom is larger than 0 (maybe optimization with line height?)
- when pulling stuff up, first check the start of a node, if not enough space, skip. If enough space check end of node.
  If enough space, same for next node. Else binary search within. Speeds things probably a lot up.


TextNodes are currently not CRDT ready... if oyu merge nodes and the take one by its id, and you get a node with a different id
because the given id was in the range of that Node, it should take that into account and adjust the given offset.
Else the offset is wrong, because it was calculated based on a node that started at another id.

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
- [x] Inline Elements
  - For now just Links 
- [ ] Pages
  - [ ] Extend Layout Engine to automatically make pages
  - [ ] Add custom page Break UI & CTRL + Enter
  - [ ] New Line without new Paragraph (currently backend can't hanlde so browser just does stuff and backend breaks)
- [ ] Save Files and Load them
  - [ ] Starting Page
    - Create new Document
    - Load Document
  - [ ] Save file
- [ ] Shortcut Keys
  - Keybinds like CTRL + B for Bold
  - CTRL + K for Links
- [ ] History with Undo and Redo features
- [ ] Text Alignment
  - Left, Right
  - Middle
  - Justify
- [ ] Text Size
  - CSS LineHeight = TextSize * MagicNumber
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
- [ ] Auto Heading Numbering
  - This needs support for read-only text that is being displayed
  - [ ] Heading Ref Links
    - Each Heading has as ID the Heading Number
    - Links are just href="#number" in the DOM
    - Auto Update all Links that point to Headings when Numbers change
- [ ] Header & Footer
  - Maybe opt out option
- [ ] Auto Page Numbering
- [ ] Chapter References and co.
- [ ] Default Formating for Block type settings
  - Things like text spacing and so on are done here. Do not support as mark for now
  - Those are Document Settings, not CRDT. So it is a global setting you can choose without effecting the tree.
  - Those settings are implicitly included in the rendering of marks. So will go over the UpdateMarkDiff, but are not on the nodes itself
- [ ] Auto Gen "Table of Contents"
  - Updates only on Printing, Preview or explicitly told by user
- [ ] Images
- [ ] Tables
- [ ] Copy & Paste
- [ ] PDF Export
- [ ] Zoom into Document
  - via CSS Transform

# Side Quests

- Color picker is not that nice to use. The cursor is not set anymore when clicking away. So you have to remember where you were
- Make Popovers better, they are in middle of page when targeting a paragraph
- why does selection with the mouse not work that great? Is that just a problem of the browser?
- Inserting a node is not completely safe. What if you try to insert with same origin and rightOrigin? Well it will break. So this should be a safe action. Best if the InsertChild in the DNode can do it on their own.

# Improvements?

- Command Handler get from DI via the Command without direct references is a bit, meh. Sure it allows me currently to always prepare the base stuff for all command handlers beforehand. But I could also make a base class instead
- HTML Tag on the Node itself is not so nice. Would be better if this is purely done by the rendering part.
