export interface MetadataEntry {
    metadataDelete: "onSelectionChange" | "onTyping";
    data: any; // TODO: maybe make interfaces for type safety
}

export class MetadataController {
    private typingEntries: Array<MetadataEntry>;
    private selectionEntries: Array<MetadataEntry>;

    constructor() {
        this.typingEntries = [];
        this.selectionEntries = [];
    }

    public getActive(id: string) {
        return {
            fromSelection: this.selectionEntries,
            fromId: this.typingEntries
        };
    }

    public add(entry: MetadataEntry) {
        if(entry.metadataDelete === "onTyping") {
            this.typingEntries.push(entry);
        } else if (entry.metadataDelete === "onSelectionChange") {
            this.selectionEntries.push(entry);
        }
    }

    public discard(entries: Array<MetadataEntry>) {
        this.typingEntries = this.typingEntries.filter(e => !entries.includes(e));
    }

    public discardOnSelectionChange() {
        this.selectionEntries = [];
    }
}

export const metadataController: MetadataController = new MetadataController();

export function initMetadataController() {
    window.editor.node?.addEventListener("selectionchange", metadataController.discardOnSelectionChange);
}