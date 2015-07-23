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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Allows to disable specific values of an extensible enum type in the context of business object properties. This attribute can be applied
  /// either to the class defining the extensible enum values via extension methods or to a business object property with an extensible enum type.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class DisableExtensibleEnumValuesAttribute : Attribute, IDisableEnumValuesAttribute
  {
    private readonly IEnumerationValueFilter _filter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a custom filter type.
    /// </summary>
    /// <param name="filterType">The type of the filter to use, must implement <see cref="IEnumerationValueFilter"/>.</param>
    public DisableExtensibleEnumValuesAttribute (Type filterType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("filterType", filterType, typeof (IEnumerationValueFilter));
      _filter = (IEnumerationValueFilter) Activator.CreateInstance (filterType);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="ids">The ids to disable.</param>
    public DisableExtensibleEnumValuesAttribute (params string[] ids)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("ids", ids);

      _filter = new DisabledIdentifiersEnumerationFilter (ids);
    }

    // The following constructors are added due to CLS compliance (CS3016).

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="disabledEnumValueID1">The disabled enum values.</param>
    public DisableExtensibleEnumValuesAttribute (string disabledEnumValueID1) 
        : this (new[] { 
            ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID1) })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="disabledEnumValueID1">A disabled enum value.</param>
    /// <param name="disabledEnumValueID2">A disabled enum value.</param>
    public DisableExtensibleEnumValuesAttribute (
        string disabledEnumValueID1,
        string disabledEnumValueID2)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID1),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID2),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="disabledEnumValueID1">A disabled enum value.</param>
    /// <param name="disabledEnumValueID2">A disabled enum value.</param>
    /// <param name="disabledEnumValueID3">A disabled enum value.</param>
    public DisableExtensibleEnumValuesAttribute (
        string disabledEnumValueID1,
        string disabledEnumValueID2,
        string disabledEnumValueID3)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID1),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID2),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID3),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="disabledEnumValueID1">A disabled enum value.</param>
    /// <param name="disabledEnumValueID2">A disabled enum value.</param>
    /// <param name="disabledEnumValueID3">A disabled enum value.</param>
    /// <param name="disabledEnumValueID4">A disabled enum value.</param>
    public DisableExtensibleEnumValuesAttribute (
        string disabledEnumValueID1,
        string disabledEnumValueID2,
        string disabledEnumValueID3,
        string disabledEnumValueID4)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID1),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID2),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID3),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID4),
      })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DisableExtensibleEnumValuesAttribute"/> class with a number of enum ids to disable.
    /// </summary>
    /// <param name="disabledEnumValueID1">A disabled enum value.</param>
    /// <param name="disabledEnumValueID2">A disabled enum value.</param>
    /// <param name="disabledEnumValueID3">A disabled enum value.</param>
    /// <param name="disabledEnumValueID4">A disabled enum value.</param>
    /// <param name="disabledEnumValueID5">A disabled enum value.</param>
    public DisableExtensibleEnumValuesAttribute (
        string disabledEnumValueID1, 
        string disabledEnumValueID2, 
        string disabledEnumValueID3, 
        string disabledEnumValueID4, 
        string disabledEnumValueID5)
      : this (new[] { 
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID1),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID2),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID3),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID4),
          ArgumentUtility.CheckNotNull ("disabledEnumValueID1", disabledEnumValueID5),
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