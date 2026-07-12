# TODOs

- [ ] Marks
  - [ ] Bold
  - [ ] Italic
  - [ ] Color
- [ ] Make marks continues when Typing
  - Typing
  - New Block, what then?
- [ ] Inline Elements
  - For now just Links 
  - Later maybe like Chapter References and co.
- [ ] Shortcut Keys
  - Keybinds like CCTRL + B for Bold
- [ ] History with Undo and Redu features
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
- [ ] Auto Gen "Table of Contents"

# Continues Marks

This could work automatically with inserting text with the merging. However, not all the time,
so best would be to check the previous node for a continues mark, and copy it over.

Maybe Marks can even be continues or not, so like a flag. But idk for now not needed.

When pressing "Bold" or so in the UI with no selection, user would expect that when start typing this marks will be added.
However I don't actually do anything with no selection and toggling mark. 
Sooooo, what we could to is just remember that the user pressed bold, for as long as the user does not move it's cursor.
If start typing the first char add mark, rest will work out.


Then there is also the problem with new Blocks, they should keep that mark as well.