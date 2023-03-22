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
using System.Diagnostics.CodeAnalysis;
using System.Web.UI;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

/// <summary> A BocListView is a named collection of column definitions. </summary>
[ParseChildren(true, "ColumnDefinitionCollection")]
public class BocListView: BusinessObjectControlItem
{
  private string? _itemID;
  private string? _title;
  /// <summary> 
  ///   The <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocListView"/>. 
  /// </summary>
  private BocColumnDefinitionCollection _columnDefinitions;

  /// <summary> Initialize a new instance. </summary>
  public BocListView (
      IBusinessObjectBoundWebControl? ownerControl,
      string title,
      BocColumnDefinition[]? columnDefinitions)
  {
    _title = title;
    _columnDefinitions = new BocColumnDefinitionCollection(
      ownerControl);

    if (columnDefinitions != null)
      _columnDefinitions.AddRange(columnDefinitions);
  }

  /// <summary> Initialize a new instance. </summary>
  public BocListView (string title, BocColumnDefinition[] columnDefinitions)
    : this(null, title, columnDefinitions)
  {
  }

  /// <summary> Initialize a new instance. </summary>
  public BocListView ()
    : this(null, string.Empty, null)
  {
  }

  protected override void OnOwnerControlChanged ()
  {
    base.OnOwnerControlChanged();
    _columnDefinitions.OwnerControl = OwnerControl;
  }

  public override string ToString ()
  {
    string? displayName = ItemID;
    if (string.IsNullOrEmpty(displayName))
      displayName = Title;
    if (string.IsNullOrEmpty(displayName))
      return DisplayedTypeName;
    else
      return string.Format("{0}: {1}", displayName, DisplayedTypeName);
  }

  /// <summary> Gets or sets the programmatic name of the <see cref="BocListView"/>. </summary>
  /// <value> A <see cref="string"/> providing an identifier for this <see cref="BocListView"/>. </value>
  [PersistenceMode(PersistenceMode.Attribute)]
  [Description("The ID of this view.")]
  [Category("Misc")]
  [DefaultValue("")]
  [NotifyParentProperty(true)]
  [Browsable(true)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
  public override string? ItemID
  {
    get { return _itemID; }
    set { _itemID = value; }
  }

  /// <summary> Gets or sets the displayed name of the <see cref="BocListView"/>. </summary>
  /// <value> A <see cref="string"/> representing this <see cref="BocListView"/> on the rendered page. </value>
  [PersistenceMode(PersistenceMode.Attribute)]
  [Category("Appearance")]
  [DefaultValue("")]
  [NotifyParentProperty(true)]
  [AllowNull]
  public string Title
  {
    get { return _title ?? string.Empty; }
    set { _title = value; }
  }

  /// <summary> 
  ///   Gets the <see cref="BocColumnDefinition"/> objects stored in the <see cref="BocListView"/>.  
  /// </summary>
  /// <value>
  ///   An array of <see cref="BocColumnDefinition"/> objects that comprise this <see cref="BocListView"/>.
  /// </value>
  [PersistenceMode(PersistenceMode.InnerDefaultProperty)]
  [Category("Data")]
  [DefaultValue((string?)null)]
  [NotifyParentProperty(true)]
  public BocColumnDefinitionCollection ColumnDefinitions
  {
    get { return _columnDefinitions; }
  }

  /// <summary> Gets the human readable name of this type. </summary>
  protected virtual string DisplayedTypeName
  {
    get { return "ListView"; }
  }
}


}
