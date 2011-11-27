using System;

[Serializable]
public class TreeViewNode
{
    public TreeViewNode(int nodeKey, int parentNodeKey)
    {
        NodeKey = nodeKey;
        ParentNodeKey = parentNodeKey;

        Expanded = false;
        Visible = false;
    }

    public int NodeKey { get; private set; }
    public int ParentNodeKey { get; private set; }
    
    public TreeViewNode[] ChildNodes { get; set; }

    public bool HasChildren { get { return ChildNodes != null && ChildNodes.Length > 0; } }

    public bool Expanded { get; set; }
    public bool Visible { get; internal set; }

    public int Depth
    {
        get { return depth; }
        internal set
        {
            depth = value;
            foreach (TreeViewNode child in ChildNodes)
                child.Depth = value + 1;
        }
    }

    private int depth = 0;
}
