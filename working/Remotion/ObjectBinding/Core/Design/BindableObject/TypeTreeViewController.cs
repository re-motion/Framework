// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  public class TypeTreeViewController : ControllerBase
  {
    public enum TreeViewIcons
    {
      Assembly,
      Namespace,
      Class = 2
    }

    private readonly TreeView _treeView;

    public TypeTreeViewController (TreeView treeView)
    {
      _treeView = treeView;
      _treeView.ImageList = CreateImageList (
          Tuple.Create ((Enum) TreeViewIcons.Assembly, "VSObject_Assembly.bmp"),
          Tuple.Create ((Enum) TreeViewIcons.Namespace, "VSObject_Namespace.bmp"),
          Tuple.Create ((Enum) TreeViewIcons.Class, "VSObject_Class.bmp"));
    }

    public TreeView TreeView
    {
      get { return _treeView; }
    }

    public void PopulateTreeNodes (List<Type> types, Type selectedType)
    {
      _treeView.BeginUpdate();
      _treeView.Nodes.Clear();

      foreach (Type type in types)
      {
        TreeNode assemblyNode = GetAssemblyNode (type, _treeView.Nodes);
        TreeNode namespaceNode = GetNamespaceNode (type, assemblyNode.Nodes);
        TreeNode typeNode = GetTypeNode (type, namespaceNode.Nodes);
        TrySelect (typeNode, selectedType);
      }

      ExpandTypeTreeView();
      _treeView.EndUpdate();
    }

    public Type GetSelectedType ()
    {
      if (_treeView.SelectedNode == null)
        return null;
      return _treeView.SelectedNode.Tag as Type;
    }

    private TreeNode GetAssemblyNode (Type type, TreeNodeCollection assemblyNodes)
    {
      AssemblyName assemblyName = type.Assembly.GetName();
      TreeNode assemblyNode = assemblyNodes[assemblyName.FullName];
      if (assemblyNode == null)
      {
        assemblyNode = new TreeNode();
        assemblyNode.Name = assemblyName.FullName;
        assemblyNode.Text = assemblyName.Name;
        assemblyNode.ToolTipText = assemblyName.FullName;
        assemblyNode.ImageKey = TreeViewIcons.Assembly.ToString();
        assemblyNode.SelectedImageKey = TreeViewIcons.Assembly.ToString();

        assemblyNodes.Add (assemblyNode);
      }

      return assemblyNode;
    }

    private TreeNode GetNamespaceNode (Type type, TreeNodeCollection namespaceNodes)
    {
      TreeNode namespaceNode = namespaceNodes[type.Namespace];
      if (namespaceNode == null)
      {
        namespaceNode = new TreeNode();
        namespaceNode.Name = type.Namespace;
        namespaceNode.Text = type.Namespace;
        namespaceNode.ImageKey = TreeViewIcons.Namespace.ToString();
        namespaceNode.SelectedImageKey = TreeViewIcons.Namespace.ToString();


        namespaceNodes.Add (namespaceNode);
      }

      return namespaceNode;
    }

    private TreeNode GetTypeNode (Type type, TreeNodeCollection typeNodes)
    {
      TreeNode typeNode = typeNodes[type.FullName];
      if (typeNode == null)
      {
        typeNode = new TreeNode();
        typeNode.Name = TypeUtility.GetPartialAssemblyQualifiedName (type);
        typeNode.Text = type.Name;
        typeNode.Tag = type;
        typeNode.ImageKey = TreeViewIcons.Class.ToString();
        typeNode.SelectedImageKey = TreeViewIcons.Class.ToString();

        typeNodes.Add (typeNode);
      }

      return typeNode;
    }

    private void TrySelect (TreeNode node, Type selectedType)
    {
      if (node.Tag is Type
          && selectedType != null
          && ((Type) node.Tag).FullName.Equals (selectedType.FullName, StringComparison.CurrentCultureIgnoreCase))
      {
        _treeView.SelectedNode = node;
        node.EnsureVisible();
      }
    }

    private void ExpandTypeTreeView ()
    {
      if (_treeView.Nodes.Count < 4)
      {
        bool expandAll = _treeView.GetNodeCount (true) < 21;
        foreach (TreeNode assemblyNode in _treeView.Nodes)
        {
          assemblyNode.Expand();
          if (expandAll || assemblyNode.Nodes.Count == 1)
            assemblyNode.ExpandAll();
        }
      }
    }
  }
}
