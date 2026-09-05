export function createEditorFixture(options: { pageCount?: number; paragraphsPerPage?: number, includeText?: boolean } = {}) {
    const pageCount = options.pageCount ?? 1;
    const paragraphsPerPage = options.paragraphsPerPage ?? 1;
    const includeParagraphText = options.includeText ?? false;

    document.body.innerHTML = `
        <div id="doc-editor">
          ${Array.from({length: pageCount}, (_, i) => `
            <div class="page" data-page-index="${i + 1}">
              <div data-dnode-id="Root" contenteditable>
                ${Array.from({length: paragraphsPerPage}, (_, p) => `<p data-dnode-id="p-${i + 1}-${p + 1}">${includeParagraphText ? `<span data-dnode-id="p-${i + 1}-${p + 1}-text">Paragraph ${p + 1}</span>` : ''}</p>`).join('')}
              </div>
            </div>
          `).join('')}
        </div>
        <template id="page-template">
          <div class="page" data-page-index="-1">
            <div data-dnode-id="Root" contenteditable></div>
          </div>
        </template>
      `;

    const editorNode = document.getElementById("doc-editor")!;
    window.editor = {
        bridgeReference: null,
        initialize: () => { },
        applyTransaction: () => { },
        getEditorSelection: () => null,
        getPageNumbers: () => [],
        node: editorNode
    };

    return editorNode;
}

export function insertText(text: string, options: {
    parentId: string, 
    id: string,
    splitPart?: number;
}) {
    const parent = options.splitPart
        ? document.querySelector(`[data-split-part="${options.splitPart}"][data-dnode-id='${options.parentId}']`)
        : document.querySelector(`[data-dnode-id='${options.parentId}']`);
    
    parent!.innerHTML += `
        <span data-dnode-id="${options.id}">${text}</span>
    `;
}

export function createSplittedParagraph(pageIndex: number, nodeId: string) {
    const page = document.querySelector<HTMLElement>(`[data-page-index="${pageIndex}"]`);
    
    page!.firstElementChild!.innerHTML += `<p data-dnode-id="${nodeId}" data-split-part="1"></p>`;
    
    page?.insertAdjacentHTML("afterend", `
      <div class="page" data-page-index="${pageIndex + 1}">
        <div data-dnode-id="Root" contenteditable>
          <p data-dnode-id="${nodeId}" data-split-part="2"></p>
        </div>
      </div>
    `);
}
