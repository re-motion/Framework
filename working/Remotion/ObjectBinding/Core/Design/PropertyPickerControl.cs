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
  public class PropertyPickerControl : EditorControlBase
  {
    private Label FilterLabel;
    private TextBox FilterField;
    private Button SelectButton;

    private IBusinessObjectBoundControl _control;
    private string _value;
    private ListBox PropertiesList;

    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private Container components = null;

    public PropertyPickerControl (IBusinessObjectBoundControl control, IServiceProvider provider, IWindowsFormsEditorService editorService)
      : base (provider, editorService)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (control.DataSource == null)
        throw new InvalidOperationException ("Cannot use PropertyPickerControl for controls without DataSource.");

      // This call is required by the Windows.Forms Form Designer.
      InitializeComponent();

      _control = control;

      FillList();
    }

    private void FillList ()
    {
      PropertiesList.Items.Clear();

      if (_control.DataSource != null && _control.DataSource.BusinessObjectClass != null)
      {
        string filter = FilterField.Text.ToLower().Trim();
        IBusinessObjectProperty[] properties = _control.DataSource.BusinessObjectClass.GetPropertyDefinitions();

        foreach (IBusinessObjectProperty property in properties)
        {
          if (filter.Length == 0
              || (property.Identifier != null && property.Identifier.ToLower().IndexOf (filter) >= 0))
          {
            if (_control.SupportsProperty (property))
              PropertiesList.Items.Add (property.Identifier);
          }
        }

        PropertiesList.Sorted = true;
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
      this.FilterLabel = new System.Windows.Forms.Label();
      this.FilterField = new System.Windows.Forms.TextBox();
      this.SelectButton = new System.Windows.Forms.Button();
      this.PropertiesList = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
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
      // SelectButton
      // 
      this.SelectButton.Anchor =
          ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SelectButton.Location = new System.Drawing.Point (8, 239);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size (64, 24);
      this.SelectButton.TabIndex = 3;
      this.SelectButton.Text = "&Select";
      this.SelectButton.Click += new System.EventHandler (this.SelectButton_Click);
      // 
      // PropertiesList
      // 
      this.PropertiesList.Anchor =
          ((System.Windows.Forms.AnchorStyles) ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                                                  | System.Windows.Forms.AnchorStyles.Left)
                                                 | System.Windows.Forms.AnchorStyles.Right)));
      this.PropertiesList.Location = new System.Drawing.Point (8, 32);
      this.PropertiesList.Name = "PropertiesList";
      this.PropertiesList.Size = new System.Drawing.Size (284, 199);
      this.PropertiesList.TabIndex = 5;
      this.PropertiesList.DoubleClick += new System.EventHandler (this.SelectButton_Click);
      // 
      // PropertyPickerControl
      // 
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add (this.FilterLabel);
      this.Controls.Add (this.PropertiesList);
      this.Controls.Add (this.SelectButton);
      this.Controls.Add (this.FilterField);
      this.Name = "PropertyPickerControl";
      this.Size = new System.Drawing.Size (300, 271);
      this.Load += new System.EventHandler (this.PropertyPathPicker_Load);
      this.ResumeLayout (false);
    }

    #endregion

    private void SelectButton_Click (object sender, EventArgs e)
    {
      _value = (string) PropertiesList.SelectedItem;
      if (EditorService != null)
        EditorService.CloseDropDown();
    }

    private void FilterField_TextChanged (object sender, EventArgs e)
    {
      FillList();
    }

    private void PropertyPathPicker_Load (object sender, EventArgs e)
    {
    }

    public override object Value
    {
      get { return _value; }

      set
      {
        if (value == null)
          _value = string.Empty;
        else
          _value = ((string)value).Trim();

        if (_value.Length > 0)
        {
          for (int i = 0; i < PropertiesList.Items.Count; ++i)
          {
            string item = (string) PropertiesList.Items[i];
            if (item == (string)value)
              PropertiesList.SelectedIndex = i;
          }
        }
      }
    }
  }
}
