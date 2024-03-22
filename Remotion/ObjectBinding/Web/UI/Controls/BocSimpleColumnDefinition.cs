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
using Remotion.Mixins;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Sorting;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary> A column definition for displaying a single property path. </summary>
  /// <remarks>
  ///   Note that using the methods of <see cref="BusinessObjectPropertyPath"/>, 
  ///   the original value of this property can be retreived or changed.
  /// </remarks>
  public class BocSimpleColumnDefinition : BocValueColumnDefinition, IBusinessObjectClassSource
  {
    private string _formatString = string.Empty;
    private readonly PropertyPathBinding _propertyPathBinding;
    private string _editModeControlType = string.Empty;
    private bool _isReadOnly;
    private bool _enableIcon;

    public BocSimpleColumnDefinition ()
    {
      _formatString = string.Empty;
      _propertyPathBinding = new PropertyPathBinding();
    }

    /// <summary> Passes the new OwnerControl to the <see cref="PropertyPathBindingCollection"/>. </summary>
    protected override void OnOwnerControlChanged ()
    {
      _propertyPathBinding.OwnerControl = OwnerControl;
      base.OnOwnerControlChanged();
    }

    /// <summary> Creates a string representation of the data displayed in this column. </summary>
    /// <param name="obj"> The <see cref="IBusinessObject"/> to be displayed in this column. </param>
    /// <returns> A <see cref="string"/> representing the contents of <paramref name="obj"/>. </returns>
    public override string GetStringValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull("obj", obj);

      var propertyPath = _propertyPathBinding.GetPropertyPath();

      var result = propertyPath.GetResult(
          obj,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.GetResultForFirstListEntry);

      return result.GetString(_formatString);
    }

    /// <summary>
    ///   Gets or sets the format string describing how the value accessed through the 
    ///   <see cref="BusinessObjectPropertyPath"/> object is formatted.
    /// </summary>
    /// <value> 
    ///   A <see cref="string"/> representing a valid format string. 
    /// </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Format")]
    [Description("A format string describing how the value accessed through the Property Path is formatted.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public string FormatString
    {
      get { return _formatString; }
      set { _formatString = value ?? string.Empty; }
    }

    public IBusinessObjectPropertyPath GetPropertyPath ()
    {
      return _propertyPathBinding.GetPropertyPath();
    }

    public void SetPropertyPath (IBusinessObjectPropertyPath propertyPath)
    {
      _propertyPathBinding.SetPropertyPath(propertyPath);
    }

    [DefaultValue(false)]
    [Category("Data")]
    public bool IsDynamic
    {
      get { return _propertyPathBinding.IsDynamic; }
      set { _propertyPathBinding.IsDynamic = value; }
    }

    /// <summary>
    ///   Gets or sets the string representation of the <see cref="GetPropertyPath"/>. 
    ///   Must not be <see langword="null"/> or emtpy.
    /// </summary>
    /// <value> A <see cref="string"/> representing the <see cref="GetPropertyPath"/>. </value>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Data")]
    [Description("The string representation of the Property Path. Must not be emtpy.")]
    //  No default value
    [NotifyParentProperty(true)]
    public string? PropertyPathIdentifier
    {
      get { return _propertyPathBinding.PropertyPathIdentifier; }
      set { _propertyPathBinding.PropertyPathIdentifier = value; }
    }

    /// <summary> 
    ///   Returns an instance the class specified by the <see cref="EditModeControlType"/> property, which will then be used for editing this 
    ///   column during edit mode.
    /// </summary>
    public IBusinessObjectBoundEditableWebControl? CreateEditModeControl ()
    {
      if (string.IsNullOrEmpty(_editModeControlType))
        return null;

      Type type = WebTypeUtility.GetType(_editModeControlType, true)!;
      return (IBusinessObjectBoundEditableWebControl)ObjectFactory.Create(type, ParamList.Empty);
    }

    /// <summary>
    ///   Gets or sets the type of the <see cref="IBusinessObjectBoundEditableWebControl"/> to be instantiated 
    ///   for editing the value of this column during edit mode.
    /// </summary>
    /// <remarks>
    ///    Optionally uses the abbreviated type name as defined in <see cref="TypeUtility.ParseAbbreviatedTypeName"/>. 
    /// </remarks>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("The IBusinessObjectBoundEditableWebControl to be used for editing the value of this column during edit mode.")]
    [DefaultValue("")]
    [NotifyParentProperty(true)]
    public string EditModeControlType
    {
      get { return _editModeControlType; }
      set { _editModeControlType = value ?? string.Empty; }
    }

    /// <summary>
    ///   Gets or sets a flag that determines whether the displayed value can be edited if the row is in edit mode.
    /// </summary>
    /// <remarks> It is only possible to explicitly disable the editing of the value. </remarks>
    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Behavior")]
    [Description("A flag that determines whether the displayed value can be edited if the row is in edit mode.")]
    [DefaultValue(false)]
    [NotifyParentProperty(true)]
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    [PersistenceMode(PersistenceMode.Attribute)]
    [Category("Appearance")]
    [Description("Flag that determines whether to show the icon in front of the value. Only allowed for reference properties")]
    [DefaultValue(true)]
    public bool EnableIcon
    {
      get { return _enableIcon; }
      set { _enableIcon = value; }
    }

    protected override IBocColumnRenderer GetRendererInternal (IServiceLocator serviceLocator)
    {
      ArgumentUtility.CheckNotNull("serviceLocator", serviceLocator);

      return serviceLocator.GetInstance<IBocSimpleColumnRenderer>();
    }

    /// <summary> Gets the displayed value of the column title. </summary>
    /// <remarks> 
    ///   If <see cref="BocColumnDefinition.ColumnTitle"/> is empty or <see langowrd="null"/>, 
    ///   the <c>DisplayName</c> of the <see cref="IBusinessObjectProperty"/> is returned.
    /// </remarks>
    /// <value> A <see cref="string"/> representing this column's title row contents. </value>
    public override WebString ColumnTitleDisplayValue
    {
      get
      {
        if (!ColumnTitle.IsEmpty)
          return ColumnTitle;

        IBusinessObjectPropertyPath propertyPath;
        try
        {
          propertyPath = _propertyPathBinding.GetPropertyPath();
        }
        catch (ParseException)
        {
          // gracefully recover in column header
          return WebString.Empty;
        }

        var lastProperty = propertyPath.Properties.LastOrDefault();
        if (lastProperty == null)
          return WebString.Empty;

        return WebString.CreateFromText(lastProperty.DisplayName);
      }
    }

    /// <summary> The human readable name of this type. </summary>
    protected override string DisplayedTypeName
    {
      get { return "SimpleColumnDefinition"; }
    }

    protected override IComparer<BocListRow> CreateCellValueComparer ()
    {
      return GetPropertyPath().CreateComparer();
    }

    protected override IValidationFailureMatcher GetValidationFailureMatcher ()
    {
      return new BusinessObjectPropertyPathValidationFailureMatcher(GetPropertyPath());
    }

    IBusinessObjectClass? IBusinessObjectClassSource.BusinessObjectClass
    {
      get { return _propertyPathBinding.BusinessObjectClass; }
    }
  }
}
