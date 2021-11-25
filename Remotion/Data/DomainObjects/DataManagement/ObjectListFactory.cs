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
using System.Collections.Concurrent;
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Creates instances that implement the <see cref="IObjectList{TDomainObject}"/> interface via reflection
  /// for use with the data management classes (mostly <see cref="VirtualCollectionEndPoint"/>).
  /// </summary>
  /// <seealso cref="DomainObjectCollectionFactory"/>
  public static class ObjectListFactory
  {
    private static readonly ConcurrentDictionary<Type, ConstructorInfo> s_virtualObjectListConstructorInfos =
        new ConcurrentDictionary<Type, ConstructorInfo>();

    public static IObjectList<IDomainObject> Create (IVirtualCollectionData virtualCollectionData)
    {
      ArgumentUtility.CheckNotNull("virtualCollectionData", virtualCollectionData);

      var ctor = GetConstructorInfoForVirtualObjectListFromCache(virtualCollectionData.RequiredItemType);

      return (IObjectList<IDomainObject>)ctor.Invoke(new object[] { virtualCollectionData });
    }

    private static ConstructorInfo GetConstructorInfoForVirtualObjectListFromCache (Type domainObjectType)
    {
      return s_virtualObjectListConstructorInfos.GetOrAdd(
          domainObjectType,
          type =>
          {
            var collectionType = typeof(VirtualObjectList<>).MakeGenericType(type);

            var ctor = collectionType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(IVirtualCollectionData) },
                null);

            Assertion.IsNotNull(ctor, "Constructor for type '{0}' not found.", collectionType);
            return ctor;
          });
    }
  }
}
