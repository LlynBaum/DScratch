# DScratch: Final Pagination Architecture Specification

## 1. Core Architectural Principle

**Strict Separation of Concerns:**

* **C# WASM Backend (The Document Authority):** Owns the single source of truth (the continuous CRDT logical tree), transactional updates, history, and peer-to-peer synchronization. It has **zero awareness** of pages, pixels, line wrapping, or font metrics.
* **TypeScript Frontend (The Visual Stabilizer):** Receives atomic `StepDiff` instructions, updates the DOM, and manages the visual pagination flow entirely within the browser’s native rendering engine.

```
[User Input] ──> [TS: beforeinput (preventDefault)]
                        │
                        ▼
                 [C# WASM: CRDT Transaction]
                        │ (Emits StepDiffs + Cursor Metadata)
                        ▼
                 [TS: Apply StepDiffs to DOM]
                        │
                        ▼
                 [TS: Page Stabilization Routine (Greedy DOM Flow)]
                        │
                        ▼
                 [TS: Restore Browser Selection / Cursor]

```

---

## 2. DOM Structure & Visual Page Model

The document is represented in the DOM as a sequential stack of fixed-dimension page containers:

```html
<div id="dscratch-viewport">
  <div class="page" data-page-index="1">
    <div class="page-content">
      <p data-node-id="p-1">First paragraph...</p>
      <p data-node-id="p-2" data-split-part="1">Beginning of paragraph that overflows...</p>
    </div>
  </div>

  <div class="page" data-page-index="2">
    <div class="page-content">
      <p data-node-id="p-2" data-split-part="2">...continuation of paragraph on page 2.</p>
      <p data-node-id="p-3">Next paragraph...</p>
    </div>
  </div>
</div>

```

---

## 3. The TypeScript Page Stabilization Routine (Greedy Flow)

After applying any batch of `StepDiff` mutations, TypeScript executes a **local page stabilization cycle**:

1. **Target Page Identification:** Identify the earliest page affected by the current transaction.
2. **Downward Sweep (Overflow Check):**
* Check if `.page-content` height exceeds the fixed printable height (Page Height minus Margins/Header/Footer).
* If an element overflows:
* **Block Element (Image / Table Row / Heading):** Shift the entire element to the top of the next page.
* **Text Paragraph:** Use the browser's native `Range` API to find the exact character boundary where the text crosses the bottom margin. Split the `<p>` into two DOM nodes stamped with `data-split-part="1"` and `data-split-part="2"`. Move Part 2 to the top of the next page.




3. **Upward Sweep (Underflow / Gap Check):**
* If space becomes available on the current page due to text deletion, pull elements or paragraph fragments from the top of the next page upwards.


4. **Early Circuit Breaker:** Stop the cascade as soon as a page settles without overflowing or underflowing. Unaffected subsequent pages are untouched.

---

## 4. Cursor & Selection Handling with Split Nodes

Because a single logical `TextNode` in C# may be split into multiple visual DOM nodes (`part 1`, `part 2`), TypeScript handles the coordinate translation:

* **Selection Reader (`getSelection`):**
  When the user places the cursor in `data-split-part="2"`, TypeScript adds the character length of `data-split-part="1"` to the local offset. It passes the **unified logical character offset** back to C#.
* **Selection Setter:**
  When C# sends a cursor position target for a split node, TypeScript iterates through the parts, determines which visual fragment contains the offset, and sets the native browser `Selection` range accordingly.

---

## 5. Cross-Browser Parity & Visual Determinism

To ensure different operating systems (Windows, macOS, Linux) render lines and page breaks with near-identical parity without custom font parsers:

### CSS Typography Constraints

```css
.dscratch-viewport {
  font-family: 'EditorFont', sans-serif;
  font-size: 16px;
  line-height: 24px; /* Explicit absolute line-height (1.5x rule) */
  
  /* Strip OS-specific font-shaping variance */
  font-kerning: none;
  font-variant-ligatures: none;
  text-rendering: geometricPrecision;
}

```

### Canvas Zooming

Page zooming is handled via GPU CSS transforms rather than altering font sizes or relative units:

```css
.dscratch-viewport {
  transform: scale(var(--zoom-level));
  transform-origin: top center;
}

```

This prevents sub-pixel rounding drift during zoom adjustments.

---

## 6. Table of Contents & Metadata Queries

* **Decoupled Architecture:** C# does not maintain or track page numbers during standard editing transactions.
* **On-Demand JS Queries:** When generating or refreshing a Table of Contents (ToC):
1. C# requests page numbers for a list of heading `NodeId`s.
2. A lightweight JS helper queries the DOM (`document.querySelector('[data-node-id="..."]')`), checks the closest parent `.page`, and returns a flat mapping back to C#.
3. C# writes the page numbers into the ToC CRDT tree.


* **Feedback Loop Protection:** ToC generation is **debounced** to ensure an update does not trigger an infinite cascading shift if new ToC lines push headings onto subsequent pages.

---

## 7. Extensibility Model for Future Features

| Feature | C# CRDT Representation | TypeScript DOM Pagination Handling |
| --- | --- | --- |
| **Hard Page Break (`Ctrl+Enter`)** | `PageBreakNode` (Empty landmark) | Forces all subsequent siblings immediately into a new `.page` container. |
| **Images** | `ImageNode(src, width, height)` | Bounding box treated as an atomic unit. Pushed to next page if exceeding bottom margin. |
| **Tables** | `TableNode` -> `TableRowNode` -> `TableCellNode` | Sliced across pages at table row boundaries. JS automatically clones the `<thead>` onto the new page. |