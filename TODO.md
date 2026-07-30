# Features

- [x] Marks
  - [x] Bold
  - [x] Italic
  - [x] Color
- [ ] Make marks continues when Typing
  - [x] Typing
  - [x] Pending Marks
  - [x] Marks for empty blocks
  - [ ] carry over mark to new block
- [x] MenuBar shows active marks
  - Also add this to the debug viewer.
- [ ] Inline Elements
  - For now just Links 
  - Later maybe like Chapter References and co.
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

- Color picker is not that nice to use. The cursor is not set anymore when clicking away. So you have to remember where you were

# Improvements?

- Command Handler get from DI via the Command without direct references is a bit, meh. Sure it allows me currently to always prepare the base stuff for all command handlers beforehand. But I could also make a base class instead
