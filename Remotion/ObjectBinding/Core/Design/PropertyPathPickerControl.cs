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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Design
{
  public class PropertyPathPickerControl : EditorControlBase
  {
    private IBusinessObjectClassSource _classSource;
    private string _value;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private Container components = null;

    private TextBox FilterField;
    private Label FilterLabel;
    private Button SelectButton;
    private CheckBox ClassFilterCheck;
    private TreeView PathTree;

    public PropertyPathPickerControl (IBusinessObjectClassSource classSource, IServiceProvider provider, IWindowsFormsEditorService editorService)
      : base (provider, editorService)
    {
      ArgumentUtility.CheckNotNull ("classSource", classSource);

      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      _classSource = classSource;

      if (_classSource != null && _classSource.BusinessObjectClass != null)
        PathTree.PathSeparator = classSource.BusinessObjectClass.BusinessObjectProvider.GetPropertyPathSeparator().ToString();

      FillTree();
    }

    private void FillTree ()
    {
      Cursor oldCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
      PathTree.BeginUpdate();

      PathTree.Nodes.Clear();
      string filter = FilterField.Text.ToLower().Trim();

      if (_classSource.BusinessObjectClass == null)
        return;
      IBusinessObjectProperty[] properties = _classSource.BusinessObjectClass.GetPropertyDefinitions();

      foreach (IBusinessObjectProperty property in properties)
      {
        if (filter.Length == 0
            || (property.Identifier != null && property.Identifier.ToLower().IndexOf (filter) >= 0))
        {
          AddProperty (PathTree.Nodes, property, true);
        }
      }

      PathTree.EndUpdate();
      Cursor.Current = oldCursor;
    }

    /// <summary>
    ///   Adds a <see cref="IBusinessObjectProperty"/> to the <see cref="TreeNodeCollection"/>.
    /// </summary>
    /// <param name="nodes">
    ///   The <see cref="TreeNodeCollection"/> to which the <paramref name="property"/> will be added.
    /// </param>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> to add. </param>
    /// <param name="includeReferenceClassProperties"> 
    ///   <see langword="true"/> to call <see cref="AddReferenceClassProperties"/> if the 
    ///   <paramref name="property"/> is of type <see cref="IBusinessObjectReferenceProperty"/>.
    /// </param>
    private void AddProperty (
        TreeNodeCollection nodes,
        IBusinessObjectProperty property,
        bool includeReferenceClassProperties)
    {
      TreeNode node = new TreeNode (property.Identifier);
      node.Tag = property;
      nodes.Add (node);

      if (! includeReferenceClassProperties)
        return;

      IBusinessObjectReferenceProperty referenceProperty = property as IBusinessObjectReferenceProperty;
      if (referenceProperty != null)
        AddReferenceClassProperties (node.Nodes, referenceProperty.ReferenceClass);
    }

    /// <summary>
    ///   Adds the <see cref="IBusinessObjectProperty"/> objects found in 
    ///   <paramref name="businessObjectClass"/> to the <see cref="TreeNodeCollection"/>.
    /// </summary>
    /// <param name="nodes">
    ///   The <see cref="TreeNodeCollection"/> to which the <see cref="IBusinessObjectProperty"/>
    ///   objects of the <paramref name="businessObjectClass"/> will be added.
    /// </param>
    /// <param name="businessObjectClass">
    ///   The <see cref="IBusinessObjectClass"/> to use as a property source.
    /// </param>
    private void AddReferenceClassProperties (
        TreeNodeCollection nodes,
        IBusinessObjectClass businessObjectClass)
    {
      IBusinessObjectProperty[] properties = businessObjectClass.GetPropertyDefinitions();

      foreach (IBusinessObjectProperty property in properties)
        AddProperty (nodes, property, false);
    }

    /// <summary>
    ///   Populates the <see cref="TreeNode.Nodes"/> collection for each <see cref="TreeNode"/>
    ///   in this branch representing a <see cref="IBusinessObjectReferenceProperty"/>.
    /// </summary>
    /// <remarks>
    ///   Adds only nodes where the collection is still empty.
    /// </remarks>
    /// <param name="node"> The branch whose nodes will be populated. </param>
    private void PopulateBranch (TreeNode node)
    {
      foreach (TreeNode childNode in node.Nodes)
      {
        //  Has this Node already been populated?
        if (childNode.Nodes.Count > 0)
          continue;

        //  Is the Property this node represents a ReferenceProperty?
        IBusinessObjectReferenceProperty referenceProperty =
            childNode.Tag as IBusinessObjectReferenceProperty;
        if (referenceProperty != null)
          AddReferenceClassProperties (childNode.Nodes, referenceProperty.ReferenceClass);
      }
    }

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose (bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose (disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent ()
    {
      this.PathTree = new System.Windows.Forms.TreeView();
      this.FilterField = new System.Windows.Forms.TextBox();
      this.FilterLabel = new System.Windows.Forms.Label();
      this.SelectButton = new System.Windows.Forms.Button();
      this.ClassFilterCheck = new System.Windows.Forms.CheckBox();
      this.SuspendLayout();
      // 
      // PathTree
      // 
      this.PathTree.Anchor =
          ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                  | System.Windows.Forms.AnchorStyles.Left)
                                                 | System.Windows.Forms.AnchorStyles.Right)));
      this.PathTree.HideSelection = false;
      this.PathTree.ImageIndex = -1;
      this.PathTree.Location = new System.Drawing.Point (8, 32);
      this.PathTree.Name = "PathTree";
      this.PathTree.PathSeparator = ".";
      this.PathTree.SelectedImageIndex = -1;
      this.PathTree.Size = new System.Drawing.Size (284, 199);
      this.PathTree.Sorted = true;
      this.PathTree.TabIndex = 4;
      this.PathTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler (this.PathTree_BeforeExpand);
      // 
      // FilterField
      // 
      this.FilterField.Anchor =
          ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                                                 | System.Windows.Forms.AnchorStyles.Right)));
      this.FilterField.Location = new System.Drawing.Point (49, 8);
      this.FilterField.Name = "FilterField";
      this.FilterField.Size = new System.Drawing.Size (243, 20);
      this.FilterField.TabIndex = 2;
      this.FilterField.Text = "";
      this.FilterField.TextChanged += new System.EventHandler (this.FilterField_TextChanged);
      // 
      // FilterLabel
      // 
      this.FilterLabel.AutoSize = true;
      this.FilterLabel.Location = new System.Drawing.Point (8, 11);
      this.FilterLabel.Name = "FilterLabel";
      this.FilterLabel.Size = new System.Drawing.Size (33, 16);
      this.FilterLabel.TabIndex = 1;
      this.FilterLabel.Text = "&Filter:";
      // 
      // SelectButton
      // 
      this.SelectButton.Anchor =
          ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SelectButton.Location = new System.Drawing.Point (8, 239);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size (64, 24);
      this.SelectButton.TabIndex = 5;
      this.SelectButton.Text = "&Select";
      this.SelectButton.Click += new System.EventHandler (this.SelectButton_Click);
      // 
      // ClassFilterCheck
      // 
      this.ClassFilterCheck.Anchor =
          ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                                                 | System.Windows.Forms.AnchorStyles.Right)));
      this.ClassFilterCheck.Checked = true;
      this.ClassFilterCheck.CheckState = System.Windows.Forms.CheckState.Checked;
      this.ClassFilterCheck.Location = new System.Drawing.Point (80, 243);
      this.ClassFilterCheck.Name = "ClassFilterCheck";
      this.ClassFilterCheck.Size = new System.Drawing.Size (212, 16);
      this.ClassFilterCheck.TabIndex = 4;
      this.ClassFilterCheck.Text = "&Properties of class {0} only";
      this.ClassFilterCheck.Visible = false;
      this.ClassFilterCheck.CheckedChanged += new System.EventHandler (this.ClassFilterCheck_CheckedChanged);
      // 
      // PropertyPathPickerControl
      // 
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add (this.ClassFilterCheck);
      this.Controls.Add (this.SelectButton);
      this.Controls.Add (this.FilterLabel);
      this.Controls.Add (this.FilterField);
      this.Controls.Add (this.PathTree);
      this.Name = "PropertyPathPickerControl";
      this.Size = new System.Drawing.Size (300, 271);
      this.ResumeLayout (false);
    }

    #endregion

    private void PathTree_BeforeExpand (object sender, TreeViewCancelEventArgs e)
    {
      Cursor oldCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
      PathTree.BeginUpdate();

      PopulateBranch (e.Node);

      PathTree.EndUpdate();
      Cursor.Current = oldCursor;
    }

    private void ClassFilterCheck_CheckedChanged (object sender, EventArgs e)
    {
    }

    private void SelectButton_Click (object sender, EventArgs e)
    {
      if (PathTree.SelectedNode != null)
      {
        _value = PathTree.SelectedNode.FullPath;
        if (EditorService != null)
          EditorService.CloseDropDown();
      }
    }

    private void FilterField_TextChanged (object sender, EventArgs e)
    {
      FillTree();
    }

    public override object Value
    {
      get { return _value; }

      set
      {
        Cursor oldCursor = Cursor.Current;
        Cursor.Current = Cursors.WaitCursor;
        PathTree.BeginUpdate();

        if (value == null)
          _value = string.Empty;
        else
          _value = ((string)value).Trim();

        if (_value.Length > 0)
        {
          if (_classSource.BusinessObjectClass == null)
            throw new InvalidOperationException ("Cannot set value because edited object has no object class set.");

          IBusinessObjectPropertyPath propertyPath = null;
          try
          {
            propertyPath = BusinessObjectPropertyPath.CreateStatic (_classSource.BusinessObjectClass, (string) value);
          }
          catch (ParseException)
          {
            //  PropertyPath invalid. 
            //  Do nothing since the editor's task is to create a valid propertypath in the first place.
          }

          if (propertyPath != null)
          {
            TreeNodeCollection nodes = PathTree.Nodes;

            foreach (IBusinessObjectProperty property in propertyPath.Properties)
            {
              if (nodes == null)
                break;
              TreeNode node = null;
              foreach (TreeNode childNode in nodes)
              {
                IBusinessObjectProperty nodeProperty = (IBusinessObjectProperty) childNode.Tag;
                if (nodeProperty.Identifier == property.Identifier)
                {
                  node = childNode;
                  break;
                }
              }
              if (node == null)
                break;
              PopulateBranch (node);
              PathTree.SelectedNode = node;
              nodes = node.Nodes;
            }
          }
        }

        PathTree.EndUpdate();
        Cursor.Current = oldCursor;
      }
    }
  }
}
