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
using System.Linq;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Allows to disable specific enum values in the context of business object properties. This attribute can be applied
  /// either to the enum or to a business object property with an enum type.
  /// </summary>
  [AttributeUsage (AttributeTargets.Enum | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class DisableEnumValuesAttribute : Attribute, IDisableEnumValuesAttribute
  {
    private readonly IEnumerationValueFilter _filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a custom filter type.
    /// </summary>
    /// <param name="filterType">The type of the filter to use, must implement <see cref="IEnumerationValueFilter"/>.</param>
    public DisableEnumValuesAttribute (Type filterType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("filterType", filterType, typeof (IEnumerationValueFilter));

      _filter = (IEnumerationValueFilter) Activator.CreateInstance (filterType);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValues">The disabled enum values.</param>
    public DisableEnumValuesAttribute (params object[] disabledEnumValues)
    {
      ArgumentUtility.CheckNotNull ("disabledEnumValues", disabledEnumValues);
      ArgumentUtility.CheckItemsType ("disabledEnumValues", disabledEnumValues, typeof (Enum));

      _filter = new ConstantEnumerationValueFilter (disabledEnumValues.Cast<Enum>().ToArray());
    }

    // The following constructors are added due to CLS compliance (CS3016).

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValue1">The disabled enum values.</param>
    public DisableEnumValuesAttribute (object disabledEnumValue1) 
        : this (new[] { 
            ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue1) })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValue1">A disabled enum value.</param>
    /// <param name="disabledEnumValue2">A disabled enum value.</param>
    public DisableEnumValuesAttribute (
        object disabledEnumValue1,
        object disabledEnumValue2)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue2),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValue1">A disabled enum value.</param>
    /// <param name="disabledEnumValue2">A disabled enum value.</param>
    /// <param name="disabledEnumValue3">A disabled enum value.</param>
    public DisableEnumValuesAttribute (
        object disabledEnumValue1,
        object disabledEnumValue2,
        object disabledEnumValue3)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue2),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue3),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValue1">A disabled enum value.</param>
    /// <param name="disabledEnumValue2">A disabled enum value.</param>
    /// <param name="disabledEnumValue3">A disabled enum value.</param>
    /// <param name="disabledEnumValue4">A disabled enum value.</param>
    public DisableEnumValuesAttribute (
        object disabledEnumValue1,
        object disabledEnumValue2,
        object disabledEnumValue3,
        object disabledEnumValue4)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue2),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue3),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue4),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableEnumValuesAttribute"/> class with a number of enum values to disable.
    /// </summary>
    /// <param name="disabledEnumValue1">A disabled enum value.</param>
    /// <param name="disabledEnumValue2">A disabled enum value.</param>
    /// <param name="disabledEnumValue3">A disabled enum value.</param>
    /// <param name="disabledEnumValue4">A disabled enum value.</param>
    /// <param name="disabledEnumValue5">A disabled enum value.</param>
    public DisableEnumValuesAttribute (
        object disabledEnumValue1, 
        object disabledEnumValue2, 
        object disabledEnumValue3, 
        object disabledEnumValue4, 
        object disabledEnumValue5)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue1),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue2),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue3),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue4),
          ArgumentUtility.CheckNotNull ("disabledEnumValue1", disabledEnumValue5),
      })
    {
    }

    /// <summary>
    /// Gets the enumeration value filter defined by this <see cref="DisableExtensibleEnumValuesAttribute"/> instance.
    /// </summary>
    /// <returns>An instance of the filter defined by this instance.</returns>
    public IEnumerationValueFilter GetEnumerationValueFilter ()
    {
      return _filter;
    }
  }
}
