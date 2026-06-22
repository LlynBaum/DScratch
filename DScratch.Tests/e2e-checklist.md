# E2E Check List

## Write Text

1. Writing Text, adds character at cursor position, and cursor Position is after the new character.
2. Writing into empty paragraph works
3. Writing in...
   1. middle of Text
   2. start of Text
   3. end of Text

## Deleting Text

1. Delete Backwards - deletes 1 character left of Cursor
2. Delete Forward - deletes 1 character right of Cursor
3. Delete Word Backwards - deletes empty characters, then characters, until white space again. Cursor at expected position
4. Delete Word Forward - deletes empty characters, then characters, until white space again. Cursor at expected position

## Insert Paragraph

1. Pressing enter creates new paragraph after current paragraph
2. Pressing enter creates new paragraph after current heading
3. Pressing enter at end of paragraph inserts new after current
4. Pressing enter in middle of text, moves text on the right into new paragraph
5. Pressing enter at start, inserts paragraph before current

## Switch Block Type

1. Pressing Heading 1-6, switches Paragraph to heading, preserves Text and Cursor position
2. Pressing Paragraph, switches Heading to paragraph, preserves Text and Cursor position