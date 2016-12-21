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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Combines a set of <see cref="IEnumerationValueFilter"/> objects, returning <see langword="false" /> if any filter returns <see langword="false" />.
  /// </summary>
  public class CompositeEnumerationValueFilter : IEnumerationValueFilter
  {
    private readonly IEnumerationValueFilter[] _filters;

    public CompositeEnumerationValueFilter (IEnumerationValueFilter[] filters)
    {
      ArgumentUtility.CheckNotNull ("filters", filters);
      _filters = filters;
    }

    public ReadOnlyCollection<IEnumerationValueFilter> Filters
    {
      get { return new ReadOnlyCollection<IEnumerationValueFilter> (_filters); }
    }

    public bool IsEnabled (IEnumerationValueInfo value, IBusinessObject businessObject, IBusinessObjectEnumerationProperty property)
    {
      ArgumentUtility.CheckNotNull ("value", value);
      ArgumentUtility.CheckNotNull ("property", property);

      return !_filters.Any (f => !f.IsEnabled (value, businessObject, property));
    }
  }
}