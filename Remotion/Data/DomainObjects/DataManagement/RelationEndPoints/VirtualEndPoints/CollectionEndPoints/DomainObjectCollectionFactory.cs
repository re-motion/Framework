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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints
{
  /// <summary>
  /// Creates <see cref="DomainObjectCollection"/> instances via reflection for use with the data management classes (mostly 
  /// <see cref="DomainObjectCollectionEndPoint"/>).
  /// </summary>
  /// <seealso cref="ObjectListFactory"/>
  public class DomainObjectCollectionFactory
  {
    private static readonly ConcurrentDictionary<Type, (bool CanAscribe, Type? ItemType)> s_genericEnumerableTypeCache = new();

    public static readonly DomainObjectCollectionFactory Instance = new DomainObjectCollectionFactory();

    private DomainObjectCollectionFactory ()
    {
    }

    /// <summary>
    /// Creates a collection of the given <paramref name="collectionType"/> via reflection, passing in the given 
    /// <see cref="IDomainObjectCollectionData"/> object as the data storage strategy.
    /// The collection must provide a constructor that takes a single parameter of type <see cref="IDomainObjectCollectionData"/>.
    /// </summary>
    /// <param name="collectionType">The type of the collection to create.</param>
    /// <param name="dataStrategy">The data strategy to use for the new collection.</param>
    /// <returns>An instance of the given <paramref name="collectionType"/>.</returns>
    /// <exception cref="MissingMethodException">The <paramref name="collectionType"/> does not provide a constructor taking
    /// a single parameter of type <see cref="IDomainObjectCollectionData"/>.</exception>
    public DomainObjectCollection CreateCollection (Type collectionType, IDomainObjectCollectionData dataStrategy)
    {
      ArgumentUtility.CheckNotNull("collectionType", collectionType);
      ArgumentUtility.CheckNotNull("dataStrategy", dataStrategy);

      var ctor = collectionType.GetConstructor(
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          new[] { typeof(IDomainObjectCollectionData) },
          null);

      if (ctor == null)
        throw CreateMissingConstructorException(collectionType);

      return (DomainObjectCollection)ctor.Invoke(new[] { dataStrategy });
    }

    /// <summary>
    /// Creates a stand-alone collection of the given <paramref name="collectionType"/> via reflection. The collection is initialized to have
    /// the given <paramref name="requiredItemType"/> and initial <paramref name="content"/>.
    /// The collection must provide a constructor that takes a single parameter of type <see cref="IDomainObjectCollectionData"/>.
    /// </summary>
    /// <param name="collectionType">The type of the collection to create.</param>
    /// <param name="content">The initial content of the collection. This must match <paramref name="requiredItemType"/> and it cannot contain
    /// duplicates or <see langword="null" /> values.</param>
    /// <param name="requiredItemType">The required item type of the collection.</param>
    /// <returns>A stand-alone instance of <paramref name="collectionType"/>.</returns>
    public DomainObjectCollection CreateCollection (Type collectionType, IEnumerable<IDomainObject> content, Type? requiredItemType)
    {
      ArgumentUtility.CheckNotNull("collectionType", collectionType);
      ArgumentUtility.CheckNotNull("content", content);

      var eventRaiser = new IndirectDomainObjectCollectionEventRaiser();

      var dataStore = new DomainObjectCollectionData();
      dataStore.AddRangeAndCheckItems(content, requiredItemType);

      var dataStrategy = DomainObjectCollection.CreateDataStrategyForStandAloneCollection(dataStore, requiredItemType, eventRaiser);
      var collection = CreateCollection(collectionType, dataStrategy);

      eventRaiser.EventRaiser = collection;

      return collection;
    }

    /// <summary>
    /// Creates a stand-alone collection of the given <paramref name="collectionType"/> via reflection, inferring the collection's required item
    /// type from the <paramref name="collectionType"/>. The collection is initialized to have the given initial <paramref name="content"/>.
    /// The collection must provide a constructor that takes a single parameter of type <see cref="IDomainObjectCollectionData"/>.
    /// </summary>
    /// <param name="collectionType">The type of the collection to create.</param>
    /// <param name="content">The initial content of the collection. This must not contain duplicates or <see langword="null" /> values.</param>
    /// <returns>A stand-alone instance of <paramref name="collectionType"/>.</returns>
    /// <remarks>
    /// The required item type of the collection is inferred from its interfaces: if an implementation of <see cref="IEnumerable{T}"/> is found
    /// in the collection's list of interfaces, that implementation's type parameter is used as the required item type. If none is found,
    /// no required item type is set.
    /// </remarks>
    public DomainObjectCollection CreateCollection (Type collectionType, IEnumerable<IDomainObject> content)
    {
      ArgumentUtility.CheckNotNull("collectionType", collectionType);
      ArgumentUtility.CheckNotNull("content", content);

      var requiredItemType = GetRequiredItemType(collectionType);
      return CreateCollection(collectionType, content, requiredItemType);
    }

    /// <summary>
    /// Creates a stand-alone read-only collection of the given <paramref name="collectionType"/> via reflection. Read-onlyness is enforced by a
    /// <see cref="ReadOnlyDomainObjectCollectionDataDecorator"/>. The collection is initialized to have the given initial <paramref name="content"/>.
    /// The collection must provide a constructor that takes a single parameter of type <see cref="IDomainObjectCollectionData"/>.
    /// </summary>
    /// <param name="collectionType">The type of the collection to create.</param>
    /// <param name="content">The initial content of the collection. This must not contain duplicates or <see langword="null" /> values.</param>
    /// <returns>A stand-alone read-only instance of <paramref name="collectionType"/>.</returns>
    /// <remarks>
    /// The <see cref="DomainObjectCollection"/> returned is read-only, its content cannot be changed.
    /// </remarks>
    public DomainObjectCollection CreateReadOnlyCollection (Type collectionType, IEnumerable<IDomainObject> content)
    {
      ArgumentUtility.CheckNotNull("collectionType", collectionType);
      ArgumentUtility.CheckNotNull("content", content);

      var dataStrategy = new ReadOnlyDomainObjectCollectionDataDecorator(new DomainObjectCollectionData(content));
      return CreateCollection(collectionType, dataStrategy);
    }

    private Type? GetRequiredItemType (Type collectionType)
    {
      return s_genericEnumerableTypeCache.GetOrAdd(
               collectionType,
               static type =>
               {
                 var canAscribeTo = type.CanAscribeTo(typeof(IEnumerable<>));
                 return ValueTuple.Create(
                     canAscribeTo,
                     canAscribeTo
                         ? type.GetAscribedGenericArguments(typeof(IEnumerable<>))[0]
                         : null);
               })
             .ItemType;
    }

    private MissingMethodException CreateMissingConstructorException (Type collectionType)
    {
      var message = string.Format(
          "Cannot create an instance of '{0}' because that type does not provide a constructor taking an IDomainObjectCollectionData object." + Environment.NewLine
          + "Example: " + Environment.NewLine
          + "public class {1} : ObjectList<...>" + Environment.NewLine
          + "{{" + Environment.NewLine
          + "  public {1} (IDomainObjectCollectionData dataStrategy)" + Environment.NewLine
          + "    : base (dataStrategy)" + Environment.NewLine
          + "  {{" + Environment.NewLine
          + "  }}" + Environment.NewLine
          + "}}",
          collectionType,
          collectionType.Name);
      return new MissingMethodException(message);
    }

  }
}
