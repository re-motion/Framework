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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Implements <see cref="IDataContainerMapReadOnlyView"/> by passing on all calls to an <see cref="InnerDataContainerMap"/>.
  /// This class is used to resolve a dependency cycle between <see cref="DataManager"/> and <see cref="ObjectLoader"/>.
  /// </summary>
  public class DelegatingDataContainerMap : IDataContainerMapReadOnlyView
  {
    public IDataContainerMapReadOnlyView? InnerDataContainerMap { get; set; }

    public DelegatingDataContainerMap ()
    {
    }

    public IEnumerator<DataContainer> GetEnumerator () => SafeInnerDataContainerMap.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator () => ((IEnumerable)SafeInnerDataContainerMap).GetEnumerator();

    public int Count => SafeInnerDataContainerMap.Count;

    public DataContainer? this [ObjectID id] => SafeInnerDataContainerMap[id];

    private IDataContainerMapReadOnlyView SafeInnerDataContainerMap
    {
      get
      {
        if (InnerDataContainerMap == null)
          throw new InvalidOperationException("InnerDataContainerMap property must be set before it can be used.");
        return InnerDataContainerMap;
      }
    }
  }
}
