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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping.SortExpressions
{
  /// <summary>
  /// Defines how to sort a number of items in a <see cref="DomainObjectCollection"/>.
  /// </summary>
  public class SortExpressionDefinition
  {
    private readonly ReadOnlyCollection<SortedPropertySpecification> _sortedProperties;

    public SortExpressionDefinition (IEnumerable<SortedPropertySpecification> sortedProperties)
    {
      ArgumentUtility.CheckNotNull ("sortedProperties", sortedProperties);
      _sortedProperties = sortedProperties.ToList().AsReadOnly();

      if (_sortedProperties.Count == 0)
        throw new ArgumentException ("A SortExpressionDefinition must contain at least one sorted property.", "sortedProperties");
    }

    public ReadOnlyCollection<SortedPropertySpecification> SortedProperties
    {
      get { return _sortedProperties; }
    }

    public override string ToString ()
    {
      return string.Join (", ", SortedProperties);
    }
  }
}