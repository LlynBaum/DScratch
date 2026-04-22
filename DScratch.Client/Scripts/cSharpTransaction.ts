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
                const slice = sliceFrom(step.args.slice);
                
                tr = tr.replace(step.args.from ?? state.selection.from, step.args.to, slice);
                break;
            case "replaceWith": {
                const nodes = createNodes(step.args.nodes);
                tr = tr.replaceWith(step.args.from, step.args.to, nodes);
                break;
            }
            case "delete": {
                tr = tr.delete(step.args.from, step.args.to);
                break;
            }
            case "insert": {
                const content = createNodes(step.args.content);
                tr = tr.insert(step.args.pos, content);
                break;
            }
            case "replaceRange": {
                const slice = sliceFrom(step.args.slice);
                tr = tr.replaceRange(step.args.from, step.args.to, slice!);
                break;
            }
            case "replaceRangeWith": {
                const node = createNode(step.args.node);
                tr = tr.replaceRangeWith(step.args.from, step.args.to, node);
                break;
            }
            case "deleteRange": {
                tr = tr.deleteRange(step.args.from, step.args.to);
                break;
            }
        }
    });

    if (tr.docChanged || tr.selectionSet) {
        dispatch(tr);
    }

    return;

    function createNode(node: any) : Node {
        return state.schema.nodes[node.name].create(node.attrs)
    }
    
    function createNodes(nodes: any[]) : Node[] {
        return nodes.map(createNode)
    }
    
    function sliceFrom(args: any) {
        if(!args){
            return undefined;
        }
        
        return state.doc.slice(
            args.slice.Start,
            args.slice.End,
            args.slice.IncludeParents);
    }
}
