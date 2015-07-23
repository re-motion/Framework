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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Filters enumeration values based on a list of ids.
  /// </summary>
  public class DisabledIdentifiersEnumerationFilter : IEnumerationValueFilter
  {
    private readonly HashSet<string> _disabledIDs;

    public DisabledIdentifiersEnumerationFilter (string[] disabledIDs)
    {
      ArgumentUtility.CheckNotNull ("disabledIDs", disabledIDs);
      _disabledIDs = new HashSet<string> (disabledIDs);
    }

    /// <summary>
    /// Determines whether the specified value is enabled. A value is enabled if it is not in the list of <see cref="DisabledIDs"/>.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="businessObject">The business object hosting the property for which the check is performed.</param>
    /// <param name="property">The property for which the check is performed.</param>
    /// <returns>
    /// 	<see langword="true" /> if the specified value is enabled; otherwise, <see langword="false" />.
    /// </returns>
    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("property", property);

      return !_disabledIDs.Contains (value.Identifier);
    }

    public ICollection<string> DisabledIDs
    {
      get { return _disabledIDs; }
    }
  }
}