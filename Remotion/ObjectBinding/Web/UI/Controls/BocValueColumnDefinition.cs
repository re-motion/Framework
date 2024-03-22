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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition for displaying data and an optional command. </summary>
  public abstract class BocValueColumnDefinition : BocCommandEnabledColumnDefinition, IBocSortableColumnDefinition, IBocColumnDefinitionWithRowHeaderSupport, IBocColumnDefinitionWithValidationSupport, IBocColumnDefinitionWithRowIconSupport
  {
    private bool _enforceWidth;
    private bool _isSortable = true;
    private bool _isRowHeader;

    protected BocValueColumnDefinition ()
    {
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    [NotNull]
    public abstract string GetStringValue (IBusinessObject obj);

    /// <summary>
    /// Creates an implementation of <see cref="IComparer{T}"/> that can be used to comparere two <see cref="BocListRow"/> instances based on the this
    /// column definition.
    /// </summary>
    /// <returns>An implementation of <see cref="IComparer{T}"/>, typed to <see cref="BocListRow"/>. Does not return <see langword="null" />.</returns>
    [NotNull]
    protected abstract IComparer<BocListRow> CreateCellValueComparer ();

    protected abstract IValidationFailureMatcher GetValidationFailureMatcher ();

    /// <summary> 
    ///   Gets or sets a flag that determines whether to hide overflowing contents in the data rows instead of 
    ///   breaking into a new line. 
    /// </summary>
    /// <value> <see langword="true"/> to enforce the width. </value>
    /// <remarks> 
    ///     <see cref="BocColumnDefinition.Width"/> must not be of type <see cref="UnitType.Percentage"/>, 
    ///     if the width is to be enforced.
    /// </remarks>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Layout")]
    [Description("Hides overflowing contents in the data rows instead of breaking into a new line.")]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    public bool EnforceWidth
    {
      get { return _enforceWidth; }
      set { _enforceWidth = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to enable sorting for this columns. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag determining whether to enable sorting for this columns.")]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    public bool IsSortable
    {
      get { return _isSortable; }
      set { _isSortable = value; }
    }

    /// <summary> Gets or sets a flag that determines whether to use the value of this column as a row header. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag determining whether to use the value if this column as a row header.")]
    [DefaultValue(true)]
    [NotifyParentProperty(true)]
    public bool IsRowHeader
    {
      get { return _isRowHeader; }
      set { _isRowHeader = value; }
    }

    /// <summary> Gets or sets a value that that determines when the row icon be displayed in this column. </summary>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behaviour")]
    [Description("An enum determining row icon behaviour for this column.")]
    [DefaultValue(Controls.RowIconMode.Automatic)]
    [NotifyParentProperty(true)]
    public RowIconMode RowIconMode { get; set; } = RowIconMode.Automatic;

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "ValueColumnDefinition"; }
    }

    IComparer<BocListRow> IBocSortableColumnDefinition.CreateCellValueComparer ()
    {
      return CreateCellValueComparer();
    }

    IValidationFailureMatcher IBocColumnDefinitionWithValidationSupport.GetValidationFailureMatcher ()
    {
      return GetValidationFailureMatcher();
    }
  }
}
