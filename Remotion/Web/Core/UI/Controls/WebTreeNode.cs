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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.UI.Controls
{
  /// <summary> A node for the <see cref="WebTreeView"/>. </summary>
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class WebTreeNode : IControlItem
  {
    /// <summary> The control to which this object belongs. </summary>
    private IControl _ownerControl;

    private string _itemID = string.Empty;
    private string _text = string.Empty;
    private string _toolTip = string.Empty;
    private IconInfo _icon;
    private string _menuID = string.Empty;

    private readonly WebTreeNodeCollection _children;
    private WebTreeView _treeView;
    private WebTreeNode _parentNode;
    private bool _isExpanded;
    private bool _isEvaluated;
    private bool _isSelected;
    private int _selectDesired;

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeNode (string itemID, string text, string toolTip, IconInfo icon)
    {
      ValidateItemId (itemID);
      _itemID = itemID;
      _text = text ?? string.Empty;
      _toolTip = toolTip ?? string.Empty;
      _icon = icon;
      _children = new WebTreeNodeCollection (null);
      _children.SetParent (null, this);
    }

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeNode (string itemID, string text, IconInfo icon)
        : this (itemID, text, string.Empty, icon)
    {
    }

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeNode (string itemID, string text, string iconUrl)
        : this (itemID, text, new IconInfo (iconUrl, Unit.Empty, Unit.Empty))
    {
    }

    /// <summary> Initalizes a new instance. </summary>
    public WebTreeNode (string itemID, string text)
        : this (itemID, text, string.Empty)
    {
    }

    /// <summary> Initalizes a new instance. For VS.NET Designer use only. </summary>
    /// <exclude/>
    [EditorBrowsable (EditorBrowsableState.Never)]
    public WebTreeNode ()
    {
      _children = new WebTreeNodeCollection (null);
      _children.SetParent (null, this);
    }

    //  /// <summary> Collapses the current node. </summary>
    //  public void Collapse()
    //  {
    //    IsExpanded = false;
    //  }
    //
    //  /// <summary> Collapses the current node and all child nodes. </summary>
    //  public void CollapseAll()
    //  {
    //    Collapse();
    //    _children.SetExpansion (false);
    //  }
    //  
    //  /// <summary> Expands the current node. </summary>
    //  public void Expand()
    //  {
    //    IsExpanded = true;
    //  }
    //  
    //  /// <summary> Expands the current node and all child nodes. </summary>
    //  public void ExpandAll()
    //  {
    //    Expand();
    //    _children.SetExpansion (true);
    //  }

    /// <summary> Evaluates the current node. </summary>
    public void Evaluate ()
    {
      if (_treeView != null)
        _treeView.EvaluateTreeNodeInternal (this);
    }

    /// <summary> Evaluates the current node's children. </summary>
    public void EvaluateChildren ()
    {
      for (int i = 0; i < Children.Count; i++)
        Children[i].Evaluate();
    }

    /// <summary> Evaluates and expands the current node. </summary>
    public void EvaluateExpand ()
    {
      Evaluate();
      IsExpanded = true;
    }

    /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
    protected virtual void OnOwnerControlChanged ()
    {
    }

    /// <summary> Sets this node's <see cref="WebTreeView"/> and parent <see cref="WebTreeNode"/>. </summary>
    protected internal void SetParent (WebTreeView treeView, WebTreeNode parentNode)
    {
      _treeView = treeView;
      if (_selectDesired == 1)
      {
        _selectDesired = 0;
        IsSelected = true;
      }
      else if (_selectDesired == -1)
      {
        _selectDesired = 0;
        IsSelected = false;
      }
      _parentNode = parentNode;
      _children.SetParent (_treeView, this);

      if (_treeView != null)
        _treeView.EnsureTreeNodeMenuInitialized (this);
    }

    /// <summary> Sets the node's selection state. </summary>
    protected internal void SetSelected (bool value)
    {
      _isSelected = value;
      if (_treeView == null)
        _selectDesired = value ? 1 : -1;
    }

    public override string ToString ()
    {
      string displayName = ItemID;
      if (string.IsNullOrEmpty (displayName))
        displayName = Text;
      if (string.IsNullOrEmpty (displayName))
        return DisplayedTypeName;
      else
        return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets or sets the ID of this node. </summary>
    /// <remarks> Must be unique within the collection of tree nodes. Must not be <see langword="null"/> or emtpy. </remarks>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Description ("The ID of this node.")]
    //No Default value
    [NotifyParentProperty (true)]
    [ParenthesizePropertyName (true)]
    public virtual string ItemID
    {
      get { return _itemID; }
      set
      {
        ValidateItemId(value);
        _itemID = value;
      }
    }

    private void ValidateItemId (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);
      if (! string.IsNullOrEmpty (value))
      {
        WebTreeNodeCollection nodes = null;
        if (ParentNode != null)
          nodes = ParentNode.Children;
        else if (TreeView != null)
          nodes = TreeView.Nodes;
        if (nodes != null)
        {
          if (nodes.Find (value) != null)
            throw new ArgumentException ("The collection already contains a node with ItemID '" + value + "'.", "value");
        }
      }
    }

    /// <summary> Gets or sets the text displayed in this node. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text displayed in this node.")]
    //No Default value
    [NotifyParentProperty (true)]
    public virtual string Text
    {
      get { return _text; }
      set
      {
        _text = value ?? string.Empty;
      }
    }

    /// <summary> Gets or sets the tool-tip displayed in this node. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The tool-tip displayed in this node.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string ToolTip
    {
      get { return _toolTip; }
      set
      {
        _toolTip = value ?? string.Empty;
      }
    }

    /// <summary> Gets or sets the icon displayed in this tree node. </summary>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The icon displayed in this tree node.")]
    [NotifyParentProperty (true)]
    public virtual IconInfo Icon
    {
      get { return _icon; }
      set { _icon = value; }
    }

    private bool ShouldSerializeIcon ()
    {
      return IconInfo.ShouldSerialize (_icon);
    }

    private void ResetIcon ()
    {
      _icon.Reset();
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public string MenuID
    {
      get { return _menuID; }
      set { _menuID = value; }
    }

    /// <summary> 
    ///   Gets the child nodes of for node. The node has to be evaluated before the actual child nodes can be accessed.
    /// </summary>
    [PersistenceMode (PersistenceMode.InnerProperty)]
    [ListBindable (false)]
    [MergableProperty (false)]
    //  Default category
    [Description ("The child nodes contained in this tree node.")]
    [DefaultValue ((string) null)]
    public virtual WebTreeNodeCollection Children
    {
      get { return _children; }
    }

    /// <summary> Gets the <see cref="WebTreeView"/> to which this node belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public WebTreeView TreeView
    {
      get { return _treeView; }
    }

    /// <summary> Gets the parent <see cref="WebTreeNode"/> of this node. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public WebTreeNode ParentNode
    {
      get { return _parentNode; }
    }

    /// <summary> Gets or sets a flag that determines whether this node is expanded. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsExpanded
    {
      get { return _isExpanded; }
      set { _isExpanded = value; }
    }

    /// <summary> Gets or sets a flag that determines whether this node's child collection has been populated. </summary>
    /// <remarks> Does not evaluate the tree node. </remarks>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsEvaluated
    {
      get { return _isEvaluated; }
      set { _isEvaluated = value; }
    }

    /// <summary> Gets or sets a flag that determines whether this node is the selected node of the tree view. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsSelected
    {
      get { return _isSelected; }
      set
      {
        SetSelected (value);
        if (_treeView != null)
        {
          if (value)
            _treeView.SetSelectedNode (this);
          else if (this == _treeView.SelectedNode)
            _treeView.SetSelectedNode (null);
        }
      }
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected virtual string DisplayedTypeName
    {
      get { return "Node"; }
    }

    /// <summary> Gets or sets the control to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IControl OwnerControl
    {
      get { return OwnerControlImplementation; }
      set { OwnerControlImplementation = value; }
    }

    protected virtual IControl OwnerControlImplementation
    {
      get { return _ownerControl; }
      set
      {
        if (_ownerControl != value)
        {
          _ownerControl = value;
          if (_children != null)
            _children.OwnerControl = value;
          OnOwnerControlChanged();
        }
      }
    }

    public virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      string key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (! string.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      if (Icon != null)
        Icon.LoadResources (resourceManager);
    }
  }
}
