type IMetadata = BlockMetadata | PositionMetadata;

export interface MetadataEntry<T extends IMetadata = IMetadata> {
    metadataDelete: "onSelectionChange" | "onTyping";
    data: T;
}

interface BlockMetadata {
    id: string;
    mark: any;
}

interface PositionMetadata {
    mark: any;
}

export class MetadataController {
    private typingEntries: Array<MetadataEntry<BlockMetadata>>;
    private selectionEntries: Array<MetadataEntry<PositionMetadata>>;

    constructor() {
        this.typingEntries = [];
        this.selectionEntries = [];
    }

    public getActive(id: string) {
        return {
            fromSelection: this.selectionEntries,
            fromId: this.typingEntries.filter(t => t.data.id === id)
        };
    }

    public add(entry: MetadataEntry, action: string) {
        if(entry.metadataDelete === "onTyping") {
            action === "remove"
            ? this.typingEntries.splice(this.typingEntries.findIndex(t => t.data === entry.data), 1)
            : this.typingEntries.push(entry as MetadataEntry<BlockMetadata>);
        } else if (entry.metadataDelete === "onSelectionChange") {
            action === "remove"
                ? this.selectionEntries.splice(this.selectionEntries.findIndex(t => t.data === entry.data), 1)
                : this.selectionEntries.push(entry as MetadataEntry<PositionMetadata>);
        }
    }

    public discard(entries: Array<MetadataEntry<BlockMetadata>>) {
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