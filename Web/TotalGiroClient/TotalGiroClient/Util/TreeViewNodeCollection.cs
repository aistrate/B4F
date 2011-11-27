using System;
using System.Collections.Generic;
using System.Linq;

namespace B4F.TotalGiro.Client.Web.Util
{
    [Serializable]
    public class TreeViewNodeCollection : List<TreeViewNode>
    {
        /// <summary>
        /// The input TreeViewNodes only need to have NodeKey and ParentNodeKey set.
        /// </summary>
        public TreeViewNodeCollection(IEnumerable<TreeViewNode> nodeCollection)
        {
            Root = new TreeViewNode(0, -1);
            Add(Root);
            AddRange(nodeCollection);

            foreach (TreeViewNode node in this)
                node.ChildNodes = this.Where(n => n.ParentNodeKey == node.NodeKey).ToArray();

            Root.Depth = -1;

            Root.Expanded = true;
            UpdateVisible();
        }

        public TreeViewNode Root { get; private set; }

        public TreeViewNode GetNode(int nodeKey)
        {
            return this.FirstOrDefault(n => n.NodeKey == nodeKey);
        }

        public void ToggleExpanded(int nodeKey)
        {
            TreeViewNode treeViewNode = GetNode(nodeKey);
            if (treeViewNode != null)
            {
                treeViewNode.Expanded = !treeViewNode.Expanded;
                UpdateVisible();
            }
            else
                throw new ApplicationException(string.Format("TreeViewNodeCollection: Could not find node with key {0}.", nodeKey));
        }

        public TreeViewNodeCollection UpdateVisible()
        {
            Root.Expanded = true;
            Root.Visible = true;
            updateVisible(Root, true);
            Root.Visible = false;

            return this;
        }

        private void updateVisible(TreeViewNode node, bool isParentVisibleAndExpanded)
        {
            node.Visible = isParentVisibleAndExpanded;

            if (node.HasChildren)
                foreach (TreeViewNode child in node.ChildNodes)
                    updateVisible(child, isParentVisibleAndExpanded && node.Visible && node.Expanded);
        }

        public TreeViewNodeCollection CollapseAll()
        {
            foreach (TreeViewNode node in this)
                node.Expanded = false;
            Root.Expanded = true;

            UpdateVisible();
            return this;
        }

        /// <summary>
        /// The parameter specifies the deepest visible node depth (the shallowest being zero, always visible).
        /// </summary>
        public TreeViewNodeCollection ExpandToDepth(int depth)
        {
            depth = Math.Max(0, depth);
            expandToDepth(Root, depth);

            UpdateVisible();
            return this;
        }

        private void expandToDepth(TreeViewNode treeViewNode, int depth)
        {
            if (treeViewNode.HasChildren && treeViewNode.Depth <= depth - 1)
            {
                treeViewNode.Expanded = true;

                if (treeViewNode.Depth <= depth - 2)
                    foreach (TreeViewNode child in treeViewNode.ChildNodes.Where(c => c.HasChildren))
                        expandToDepth(child, depth);
            }
        }
    } 
}
