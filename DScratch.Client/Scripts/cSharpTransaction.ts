import {EditorState, Transaction} from "prosemirror-state";
import {Node} from "prosemirror-model"

export interface PmStep {
    name: string;
    args: any;
}

export const dispatchCSharp = (state: EditorState, dispatch: (tr: Transaction) => void, steps: PmStep[]) => {
    let tr = state.tr;

    steps.forEach(step => {
        switch (step.name) {
            case "replace":
                tr = tr.replace(step.args.from ?? state.selection.from, step.args.to);
                break;
            case "replaceWith": {
                const nodes: readonly Node[] = step.args.nodes.map((n: any) => state.schema.nodes[n.name].create(n.attrs))
                tr = tr.replaceWith(step.args.from, step.args.to, nodes);
                break;
            }
        }
    });

    if (tr.docChanged || tr.selectionSet) {
        dispatch(tr);
    }
}