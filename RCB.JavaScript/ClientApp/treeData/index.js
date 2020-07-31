import _tree_3_11 from "./tree_3_11.json";

class TreeData {
    _tree;
    _keystones;

    constructor(tree) {
        this._tree = tree;
        this._keystones = Object.values(this._tree.nodes).filter((node) => {
            return node.isKeystone;
        });
    }

    getNode(id) {
        return this._tree.nodes[id];
    }

    getKeystones() {
        return this._keystones;
    }

    getTree() {
        return this._tree;
    }

    getRaw() {
        return this._tree;
    }
}

export const tree_3_11 = new TreeData(_tree_3_11);
export const lastest = tree_3_11;

export default lastest;
