export function createEditorFixture(options: { pageCount?: number; paragraphsPerPage?: number } = {}) {
    const pageCount = options.pageCount ?? 1;
    const paragraphsPerPage = options.paragraphsPerPage ?? 0;

    document.body.innerHTML = `
        <div id="doc-editor">
          ${Array.from({length: pageCount}, (_, i) => `
            <div class="page" data-page-index="${i + 1}">
              <div data-dnode-id="Root" contenteditable>
                ${Array.from({length: paragraphsPerPage}, (_, p) => `
                  <p data-dnode-id="p-${i + 1}-${p + 1}">Paragraph ${p + 1}</p>
                `).join('')}
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
