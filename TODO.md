# Current

- E2E Tests for Links
  - Add Link on exiting Text
  - AddLink Button disabled when link empty (selection)
  - Remove Link
  - Add Link on multi line
- CTRL hold to click a link
- Option to choose target for link. Edit and Add
- Option to edit link url
- Insert Link Cursor Position doesn't work?

## Weird Bugs

- Popover closing instantly when using mouse clicks
- The first time opening the popover for adding link, the highlight does not work

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
  - CTRL + K for Links
- [ ] History with Undo and Redo features
- [ ] Auto Heading Numbering
  - This needs support for read-only text that is being displayed
  - [ ] Heading Ref Links
    - Each Heading has as ID the Heading Number
    - Links are just href="#number" in the DOM
    - Auto Update all Links that point to Headings when Numbers change
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
- [ ] Images
- [ ] Tables
- [ ] Copy & Paste

# Side Quests

- Color picker is not that nice to use. The cursor is not set anymore when clicking away. So you have to remember where you were

# Improvements?

- Command Handler get from DI via the Command without direct references is a bit, meh. Sure it allows me currently to always prepare the base stuff for all command handlers beforehand. But I could also make a base class instead
- HTML Tag on the Node itself is not so nice. Would be better if this is purely done by the rendering part.
