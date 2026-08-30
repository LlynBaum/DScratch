export function createEditorFixture(options: { pageCount?: number; paragraphsPerPage?: number } = {}) {
    const pageCount = options.pageCount ?? 1;
    const paragraphsPerPage = options.paragraphsPerPage ?? 1;

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

export function insertText(parentId: string, id: string, text: string) {
    const parent = document.querySelector(`[data-dnode-id='${parentId}']`);
    parent!.innerHTML += `
        <span data-dnode-id="${id}">${text}</span>
    `;
}

export function createSplittedParagraph(pageIndex: number, nodeId: string) {
    const page = document.querySelector<HTMLElement>(`[data-page-index="${pageIndex}"]`);
    
    page!.innerHTML += `
    <p data-dnode-id="${nodeId}" data-split-part="1">
        <span data-dnode-id="${nodeId}-text">jlökgfjslkfjasölkj dfölkaj sdlkfj aölksdj flök ajsdfölkj aölskdfj ölaksjdfölk jasöldkfj aölksjdf ölkajsödlfk jasölkdj fölkasj dfölkaj slök fjlköasdj flkasj dlfkjasölkdfj ölkasdjf lökasjdlökfjaslökd fjlökas jflkj öjlöj</span> 
    </p>
    `;
    
    page?.insertAdjacentHTML("afterend", `
    <div class="page" data-page-index="${pageIndex + 1}">
      <div data-dnode-id="Root" contenteditable>
        <p data-dnode-id="${nodeId}" data-split-part="2">
            <span data-dnode-id="${nodeId}-text"> dwww</span> 
        </p>
      </div>
    </div>
    `);
}
