import {EditorState, Transaction} from "prosemirror-state";
import {Node, Mark} from "prosemirror-model"

export interface PmStep {
    name: string;
    args: any;
}

export const dispatchCSharp = (state: EditorState, dispatch: (tr: Transaction) => void, steps: PmStep[]) => {
    const tr = state.tr;

    steps.forEach(step => {
        switch (step.name) {
            case "replace":
                const slice = sliceFrom(step.args.slice);
                tr.replace(step.args.from ?? state.selection.from, step.args.to, slice);
                break;
            case "replaceWith": {
                const nodes = createNodes(step.args.nodes);
                tr.replaceWith(step.args.from, step.args.to, nodes);
                break;
            }
            case "delete": {
                tr.delete(step.args.from, step.args.to);
                break;
            }
            case "insert": {
                const content = createNodes(step.args.content);
                tr.insert(step.args.pos, content);
                break;
            }
            case "replaceRange": {
                const slice = sliceFrom(step.args.slice);
                tr.replaceRange(step.args.from, step.args.to, slice!);
                break;
            }
            case "replaceRangeWith": {
                const node = createNode(step.args.node);
                tr.replaceRangeWith(step.args.from, step.args.to, node);
                break;
            }
            case "deleteRange": {
                tr.deleteRange(step.args.from, step.args.to);
                break;
            }
            case "addMark": {
                const mark = createMark(step.args.mark);
                tr.addMark(step.args.from, step.args.to, mark);
                break;
            }
        }
    });

    if (tr.docChanged || tr.selectionSet) {
        dispatch(tr);
    }

    return;

    function createNode(node: any) : Node {
        return state.schema.nodes[node.name].create(node.args)
    }
    
    function createNodes(nodes: any[]) : Node[] {
        return nodes.map(createNode)
    }
    
    function createMark(mark: any): Mark {
        return state.schema.mark(mark.name, mark.args);
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
