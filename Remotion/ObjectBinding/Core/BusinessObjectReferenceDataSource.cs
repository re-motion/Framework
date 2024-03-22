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
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  ///   This data source provides access to the objects returned by other data sources using the specified property.
  /// </summary>
  /// <remarks>
  ///   This class acts as both source (<see cref="IBusinessObjectDataSource"/>) and consumer 
  ///   (<see cref="IBusinessObjectBoundControl"/>) of business objects. 
  ///   <note>
  ///     <see cref="IBusinessObjectDataSource.BusinessObject"/> and <see cref="IBusinessObjectBoundControl.Value"/>
  ///     are always identical.
  ///   </note>
  /// </remarks>
  /// <seealso cref="IBusinessObjectReferenceDataSource"/>
  /// <seealso cref="IBusinessObjectBoundEditableControl"/>
  public class BusinessObjectReferenceDataSource : BusinessObjectReferenceDataSourceBase, IBusinessObjectBoundEditableControl
  {
    private IBusinessObjectDataSource? _dataSource;
    private string? _propertyIdentifier;
    private IBusinessObjectReferenceProperty? _property;
    private bool _propertyDirty = true;
    private DataSourceMode _mode = DataSourceMode.Edit;

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
    ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
    ///  </value>
    [Category("Data")]
    public IBusinessObjectDataSource? DataSource
    {
      get { return _dataSource; }
      set
      {
        if (value == this)
          throw new ArgumentException("Assigning a reference data source as its own data source is not allowed.", "value");
        if (_dataSource != null)
          _dataSource.Unregister(this);
        _dataSource = value;
        if (value != null)
          value.Register(this);
        _propertyDirty = true;
      }
    }

    /// <summary>
    ///   Gets the <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> 
    ///   to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   The <see cref="IBusinessObjectDataSource"/> providing the <see cref="IBusinessObject"/> to which this
    ///   <see cref="IBusinessObjectReferenceDataSource"/> connects.
    ///  </value>
    /// <remarks> Identical to <see cref="DataSource"/>. </remarks>
    public override sealed IBusinessObjectDataSource? ReferencedDataSource
    {
      get { return _dataSource; }
    }

    public override DataSourceMode Mode
    {
      get { return _mode; }
      set { _mode = value; }
    }

    /// <summary>
    ///   Gets or sets the string representation of the <see cref="ReferenceProperty"/>.
    /// </summary>
    /// <value> 
    ///   A string that can be used to query the <see cref="IBusinessObjectClass.GetPropertyDefinition"/> method for
    ///   the <see cref="IBusinessObjectReferenceProperty"/> returned by <see cref="ReferenceProperty"/>. 
    /// </value>
    [Category("Data")]
    public string? PropertyIdentifier
    {
      get { return _propertyIdentifier; }
      set
      {
        _propertyIdentifier = value;
        _propertyDirty = true;
      }
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
    ///   <see cref="IBusinessObject"/> to which this <see cref="IBusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
    ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
    /// </value>
    /// <remarks> Identical to <see cref="ReferenceProperty"/>. </remarks>
    IBusinessObjectProperty? IBusinessObjectBoundControl.Property
    {
      get { return ReferenceProperty; }
      set { _property = (IBusinessObjectReferenceProperty?)value; }
    }

    /// <summary>
    ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used to access the 
    ///   <see cref="IBusinessObject"/> to which this <see cref="BusinessObjectReferenceDataSource"/> connects.
    /// </summary>
    /// <value> 
    ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the 
    ///   <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>.
    /// </value>
    [Browsable(false)]
    public override sealed IBusinessObjectReferenceProperty? ReferenceProperty
    {
      get
      {
        if (_propertyDirty)
        {
          if (_dataSource != null && _dataSource.BusinessObjectClass != null && _propertyIdentifier != null)
            _property = (IBusinessObjectReferenceProperty?)_dataSource.BusinessObjectClass.GetPropertyDefinition(_propertyIdentifier);
          else
            _property = null;
          _propertyDirty = false;
        }
        return _property;
      }
    }

    protected override string GetDataSourceIdentifier ()
    {
      return string.Format("{0}", GetType().Name);
    }

    /// <summary> Gets or sets the value provided by the <see cref="BusinessObjectReferenceDataSource"/>. </summary>
    /// <value> The <see cref="IBusinessObject"/> accessed using <see cref="IBusinessObjectBoundControl.Property"/>. </value>
    object? IBusinessObjectBoundControl.Value
    {
      get { return BusinessObject; }
      set { BusinessObject = ArgumentUtility.CheckType<IBusinessObject>("value", value); }
    }

    bool IBusinessObjectBoundControl.HasValue
    {
      get { return HasValue(); }
    }

    /// <summary>
    ///   Gets a flag specifying whether the <see cref="BusinessObjectReferenceDataSource"/> has a valid configuration.
    /// </summary>
    /// <value> 
    ///   <see langword="false"/> if the <see cref="ReferenceProperty"/> is not accessible for the 
    ///   <see cref="DataSource"/>'s <see cref="IBusinessObjectDataSource.BusinessObjectClass"/>
    ///   and <see cref="IBusinessObjectDataSource.BusinessObject"/>.
    /// </value>
    [Browsable(false)]
    bool IBusinessObjectBoundControl.HasValidBinding
    {
      get
      {
        if (_dataSource == null || _property == null)
          return true;

        return _property.IsAccessible(_dataSource.BusinessObject);
      }
    }

    /// <summary> Returns always <see langword="false"/>. </summary>
    bool IBusinessObjectBoundEditableControl.IsReadOnly
    {
      get { return false; }
    }
  }
}
