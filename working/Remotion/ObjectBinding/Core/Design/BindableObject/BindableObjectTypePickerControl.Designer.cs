using System.Windows.Forms;

namespace Remotion.ObjectBinding.Design.BindableObject
{
  partial class BindableObjectTypePickerControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private Label SearchLabel;
    private Button SelectButton;
    private TextBox SearchField;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose (bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose ();
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
      this.SearchLabel = new System.Windows.Forms.Label ();
      this.SelectButton = new System.Windows.Forms.Button ();
      this.SearchField = new System.Windows.Forms.TextBox ();
      this.IncludeGacCheckBox = new System.Windows.Forms.CheckBox ();
      this.TypeTreeView = new System.Windows.Forms.TreeView ();
      this.SearchButton = new System.Windows.Forms.Button ();
      this.SuspendLayout ();
      // 
      // SearchLabel
      // 
      this.SearchLabel.AutoSize = true;
      this.SearchLabel.Location = new System.Drawing.Point (9, 11);
      this.SearchLabel.Name = "SearchLabel";
      this.SearchLabel.Size = new System.Drawing.Size (44, 13);
      this.SearchLabel.TabIndex = 10;
      this.SearchLabel.Text = "&Search:";
      // 
      // SelectButton
      // 
      this.SelectButton.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.SelectButton.Location = new System.Drawing.Point (8, 370);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size (64, 24);
      this.SelectButton.TabIndex = 30;
      this.SelectButton.Text = "&Select";
      this.SelectButton.Click += new System.EventHandler (this.SelectButton_Click);
      // 
      // SearchField
      // 
      this.SearchField.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.SearchField.Location = new System.Drawing.Point (59, 8);
      this.SearchField.Name = "SearchField";
      this.SearchField.Size = new System.Drawing.Size (304, 20);
      this.SearchField.TabIndex = 11;
      this.SearchField.TextChanged += new System.EventHandler (this.FilterField_TextChanged);
      // 
      // IncludeGacCheckBox
      // 
      this.IncludeGacCheckBox.AutoSize = true;
      this.IncludeGacCheckBox.Location = new System.Drawing.Point (254, 375);
      this.IncludeGacCheckBox.Name = "IncludeGacCheckBox";
      this.IncludeGacCheckBox.Size = new System.Drawing.Size (141, 17);
      this.IncludeGacCheckBox.TabIndex = 31;
      this.IncludeGacCheckBox.Text = "Include &GAC Assemblies";
      this.IncludeGacCheckBox.UseVisualStyleBackColor = true;
      this.IncludeGacCheckBox.CheckedChanged += new System.EventHandler (this.IncludeGacCheckBox_CheckedChanged);
      // 
      // TypeTreeView
      // 
      this.TypeTreeView.HideSelection = false;
      this.TypeTreeView.Location = new System.Drawing.Point (8, 36);
      this.TypeTreeView.Name = "TypeTreeView";
      this.TypeTreeView.ShowNodeToolTips = true;
      this.TypeTreeView.Size = new System.Drawing.Size (384, 325);
      this.TypeTreeView.Sorted = true;
      this.TypeTreeView.TabIndex = 20;
      this.TypeTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler (this.TypeTreeView_AfterSelect);
      this.TypeTreeView.DoubleClick += new System.EventHandler (this.SelectButton_Click);
      // 
      // SearchButton
      // 
      this.SearchButton.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
      this.SearchButton.Location = new System.Drawing.Point (369, 8);
      this.SearchButton.Name = "SearchButton";
      this.SearchButton.Size = new System.Drawing.Size (23, 23);
      this.SearchButton.TabIndex = 12;
      this.SearchButton.UseVisualStyleBackColor = true;
      // 
      // BindableObjectTypePickerControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.Controls.Add (this.SearchButton);
      this.Controls.Add (this.IncludeGacCheckBox);
      this.Controls.Add (this.SearchLabel);
      this.Controls.Add (this.SelectButton);
      this.Controls.Add (this.SearchField);
      this.Controls.Add (this.TypeTreeView);
      this.Name = "BindableObjectTypePickerControl";
      this.Size = new System.Drawing.Size (400, 401);
      this.ResumeLayout (false);
      this.PerformLayout ();

    }

    #endregion

    private CheckBox IncludeGacCheckBox;
    private TreeView TypeTreeView;
    private Button SearchButton;
  }
}
