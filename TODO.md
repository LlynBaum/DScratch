# TODOs

- [ ] Layout Engine **MVP**
  - Get to work what is currently working
- [ ] Marks
  - [ ] Bold
  - [ ] Italic
  - [ ] Color
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

## Layout Engine

We have the modified node, then get the Pages. 

For Inserting mark parent as Modified. For moving mark the moved node and the new Parent as modified.

Then rerender the found Pages. While calculating the Layout, set the current Page to all visited DNodes.

Basically what now has to happen is, take the Last DNode of the First **NOT** modified Page.
Then if Node wraps over to next Page use it else go to the next node in the Tree. Then start layouting.
Here just build a little Tree with the ElementNodes and after that yeet it through a razor renderer and all that.

*Note:* The LastNode of a Page always has to be a BlockNode.

Pages are only prep Work, so no need to add new Pages and all that. For now all on one page.