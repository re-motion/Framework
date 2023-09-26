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
using System.Linq;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition for displaying a string made up from different property paths. </summary>
  /// <remarks> Note that values in these columnDefinitions can usually not be modified directly. </remarks>
  public class BocCompoundColumnDefinition : BocValueColumnDefinition
  {
    /// <summary>
    ///   A format string describing how the values accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
    /// </summary>
    private string _formatString = string.Empty;

    /// <summary>
    ///   The collection of <see cref="PropertyPathBinding"/> objects used by
    ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
    /// </summary>
    private readonly PropertyPathBindingCollection _propertyPathBindings;

    public BocCompoundColumnDefinition ()
    {
      _propertyPathBindings = new PropertyPathBindingCollection(null);
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    public override string GetStringValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull("obj", obj);

      var formatters = _propertyPathBindings.Cast<PropertyPathBinding>()
                                            .Select(b => new BusinessObjectPropertyPath.Formatter(obj, b.GetPropertyPath()))
                                            .ToArray();

      return string.Format(_formatString, formatters);
    }

    /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
    protected override void OnOwnerControlChanged ()
    {
      _propertyPathBindings.OwnerControl = OwnerControl;
      base.OnOwnerControlChanged();
    }

    /// <summary>
    ///   Gets or sets the format string describing how the values accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> objects are merged by <see cref="GetStringValue"/>.
    /// </summary>
    /// <value> 
    ///   A <see cref="string"/> containing a format item for each 
    ///   <see cref="BusinessObjectPropertyPath"/> to be displayed. The indices must match the 
    ///   order of the <see cref="BusinessObjectPropertyPath"/> objects to be formatted.
    /// </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Format")]
    [Description("A format string describing how the values accessed through the Property Path are merged.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public string FormatString
    {
      get { return _formatString; }
      set { _formatString = value ?? string.Empty; }
    }

    /// <summary>
    ///   Gets the collection of <see cref="PropertyPathBinding"/> objects used by
    ///   <see cref="GetStringValue"/> to access the values of an <see cref="IBusinessObject"/>.
    /// </summary>
    /// <value> A collection of <see cref="PropertyPathBinding"/> objects. </value>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [Category("Data")]
    [Description("The Property Paths used to access the values of Business Object.")]
    [NotifyParentProperty(true)]
    public PropertyPathBindingCollection PropertyPathBindings
    {
      get { return _propertyPathBindings; }
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return serviceLocator.GetInstance<IBocCompoundColumnRenderer>();
    }

    /// <summary>Gets or sets the text displayed in the column title. Must not be empty.</summary>
    /// <value>A <see cref="WebString"/> representing the title of this column.</value>
    [Description("The assigned value of the column title, must not be empty.")]
    [DefaultValue(typeof(WebString), "")]
    [NotifyParentProperty(true)]
    public override WebString ColumnTitle
    {
      get { return base.ColumnTitle; }
      set
      {
        ArgumentUtility.CheckNotNullOrEmpty("ColumnTitle.Value", value.GetValue());
        base.ColumnTitle = value;
      }
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "CompoundColumnDefinition"; }
    }

    protected override IComparer<BocListRow> CreateCellValueComparer ()
    {
      return new CompoundComparer<BocListRow>(
          _propertyPathBindings
              .Cast<PropertyPathBinding>()
              .Select(b => b.GetPropertyPath().CreateComparer()));
    }
  }
}
