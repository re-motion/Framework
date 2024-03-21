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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Compares two <see cref="DomainObject"/> instances based on a <see cref="SortedPropertySpecification"/>. The property values are retrieved
  /// without raising any events. If the <see cref="DomainObject"/> instances are not loaded, their data is lazily loaded.
  /// </summary>
  public class SortedPropertyComparer : IComparer<DomainObject>
  {
    public static IComparer<DomainObject> CreateCompoundComparer (
        IEnumerable<SortedPropertySpecification> sortedPropertySpecifications,
        IDataContainerMapReadOnlyView dataManager,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("sortedPropertySpecifications", sortedPropertySpecifications);
      ArgumentUtility.CheckNotNull("dataManager", dataManager);

      var comparers = sortedPropertySpecifications.Select(sp => (IComparer<DomainObject>)new SortedPropertyComparer(sp, dataManager, valueAccess));
      return new CompoundComparer<DomainObject>(comparers);
    }

    public SortedPropertySpecification SortedPropertySpecification { get; }
    public IDataContainerMapReadOnlyView DataContainerMap { get; }
    public ValueAccess ValueAccess { get; }

    public SortedPropertyComparer (
        SortedPropertySpecification sortedPropertySpecification,
        IDataContainerMapReadOnlyView dataContainerMap,
        ValueAccess valueAccess)
    {
      ArgumentUtility.CheckNotNull("sortedPropertySpecification", sortedPropertySpecification);
      ArgumentUtility.CheckNotNull("dataContainerMap", dataContainerMap);

      SortedPropertySpecification = sortedPropertySpecification;
      DataContainerMap = dataContainerMap;
      ValueAccess = valueAccess;
    }

    public int Compare (DomainObject? x, DomainObject? y)
    {
      if (x == null && y == null)
        return 0;
      if (x == null)
        return SortedPropertySpecification.Order == SortOrder.Ascending ? 1 : -1;
      if (y == null)
        return SortedPropertySpecification.Order == SortOrder.Ascending ? -1 : 1;

      var valueX = GetComparedKey(x);
      var valueY = GetComparedKey(y);

      if (SortedPropertySpecification.Order == SortOrder.Ascending)
        return Comparer.Default.Compare(valueX, valueY);
      else
        return -Comparer.Default.Compare(valueX, valueY);
    }

    private object? GetComparedKey (DomainObject domainObject)
    {
      var dataContainer = DataContainerMap[domainObject.ID];
      if (dataContainer == null)
        return null;

      if (!SortedPropertySpecification.PropertyDefinition.TypeDefinition.IsAssignableFrom(dataContainer.ClassDefinition))
        return null;

      return dataContainer.GetValueWithoutEvents(SortedPropertySpecification.PropertyDefinition, ValueAccess);
    }
  }
}
