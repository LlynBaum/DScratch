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

1. update unit tests
   - correct Modified notifications and all that
   - CleanUp update Modified notic correctly
2. Add tests for layouting
   - Layouting creates correct tree and all that (unit test)
   - E2E Tests should still work (visually there shouldn't be any changes)
3. Test manually
4. Scroll?

### Scroll

DOM swapping migh break the current scrole so it has to be stored by JS then swap and then reset back. But do this only
in case the Browser can't figure it out on it's own.
