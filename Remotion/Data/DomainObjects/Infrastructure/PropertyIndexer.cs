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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexer to access a specific property of a domain object. Instances of this value type are returned by
  /// <see cref="DomainObjects.DomainObject.Properties"/>.
  /// </summary>
  public struct PropertyIndexer
  {
    private readonly IDomainObject _domainObject;

    /// <summary>
    /// Initializes a new <see cref="PropertyIndexer"/> instance.
    /// </summary>
    /// <param name="domainObject">The domain object whose properties should be accessed with this <see cref="PropertyIndexer"/>.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="domainObject"/> parameter is <see langword="null"/>.</exception>
    public PropertyIndexer (IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      _domainObject = domainObject;
    }

    /// <summary>
    /// Gets the <see cref="IDomainObject"/> associated with this <see cref="PropertyIndexer"/>.
    /// </summary>
    /// <value>The domain object associated with this <see cref="PropertyIndexer"/>.</value>
    public IDomainObject DomainObject
    {
      get { return _domainObject; }
    }

    /// <summary>
    /// Selects the property of the domain object with the given name.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="propertyName"/> parameter does not denote a valid mapping property of the domain object.
    /// </exception>
    public PropertyAccessor this[[NotNull] string propertyName]
    {
      get
      {
        // Overloaded member checks the arguments
        ArgumentUtility.DebugCheckNotNullOrEmpty("propertyName", propertyName);

        return this[propertyName, ClientTransaction];
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given short name and declaring type.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to be accessed.</param>
    /// <param name="domainObjectType">The type declaring the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this indexer are null.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="shortPropertyName"/> parameter does not denote a valid mapping property declared on the <paramref name="domainObjectType"/>.
    /// </exception>
    public PropertyAccessor this[[NotNull] Type domainObjectType, [NotNull] string shortPropertyName]
    {
      get
      {
        // Overloaded member checks the arguments
        ArgumentUtility.DebugCheckNotNull("domainObjectType", domainObjectType);
        ArgumentUtility.DebugCheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

        return this[domainObjectType, shortPropertyName, ClientTransaction];
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given name.
    /// </summary>
    /// <param name="propertyName">The name of the property to be accessed.</param>
    /// <param name="transaction">The transaction to use for accessing the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="propertyName"/> parameter does not denote a valid mapping property of the domain object.
    /// </exception>
    public PropertyAccessor this[[NotNull] string propertyName, [NotNull] ClientTransaction transaction]
    {
      get
      {
        // PropertyAccessorDataCache checks the argument
        ArgumentUtility.DebugCheckNotNullOrEmpty("propertyName", propertyName);
        // GetPropertyAccessor checks the argument
        ArgumentUtility.DebugCheckNotNull("transaction", transaction);

        var data = PropertyAccessorDataCache.GetMandatoryPropertyAccessorData(propertyName);
        return GetPropertyAccessor(transaction, data);
      }
    }

    /// <summary>
    /// Selects the property of the domain object with the given short name and declaring type.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to be accessed.</param>
    /// <param name="domainObjectType">The type declaring the property.</param>
    /// <param name="transaction">The transaction to use for accessing the property.</param>
    /// <returns>A <see cref="PropertyAccessor"/> instance encapsulating the requested property.</returns>
    /// <exception cref="ArgumentNullException">One or more of the parameters passed to this indexer are null.</exception>
    /// <exception cref="ArgumentException">
    /// The <paramref name="shortPropertyName"/> parameter does not denote a valid mapping property declared on the <paramref name="domainObjectType"/>.
    /// </exception>
    public PropertyAccessor this[[NotNull] Type domainObjectType, [NotNull] string shortPropertyName, [NotNull] ClientTransaction transaction]
    {
      get
      {
        // PropertyAccessorDataCache checks the argument
        ArgumentUtility.DebugCheckNotNull("domainObjectType", domainObjectType);
        // PropertyAccessorDataCache checks the argument
        ArgumentUtility.DebugCheckNotNullOrEmpty("shortPropertyName", shortPropertyName);
        // GetPropertyAccessor checks the argument
        ArgumentUtility.DebugCheckNotNull("transaction", transaction);

        var data = PropertyAccessorDataCache.GetMandatoryPropertyAccessorData(domainObjectType, shortPropertyName);
        return GetPropertyAccessor(transaction, data);
      }
    }

    /// <summary>
    /// Gets the number of properties defined by the domain object. This corresponds to the number of <see cref="PropertyAccessor"/> objects
    /// indexable by this structure and enumerated by <see cref="AsEnumerable()"/>.
    /// </summary>
    /// <returns>The number of properties defined by the domain object.</returns>
    public int GetPropertyCount ()
    {
      var endPointDefinitions = ClassDefinition.GetRelationEndPointDefinitions();
      return ClassDefinition.GetPropertyDefinitions().Count + endPointDefinitions.Count(endPointDefinition => endPointDefinition.IsVirtual);
    }

    /// <summary>
    /// Returns an implementation of <see cref="IEnumerable{T}"/> that enumerates over all the properties indexed by this <see cref="PropertyIndexer"/>
    /// in the <see cref="DomainObject"/>'s <see cref="DomainObjects.DomainObject.DefaultTransactionContext"/>.
    /// </summary>
    /// <returns>A sequence containing <see cref="PropertyAccessor"/> objects for each property of this <see cref="PropertyIndexer"/>'s 
    /// <see cref="DomainObject"/>.</returns>
    public IEnumerable<PropertyAccessor> AsEnumerable ()
    {
      return AsEnumerable(ClientTransaction);
    }

    /// <summary>
    /// Returns an implementation of <see cref="IEnumerable{T}"/> that enumerates over all the properties indexed by this <see cref="PropertyIndexer"/>
    /// in the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="transaction">The transaction to be used to enumerate the properties.</param>
    /// <returns>A sequence containing <see cref="PropertyAccessor"/> objects for each property of this <see cref="PropertyIndexer"/>'s 
    /// <see cref="DomainObject"/>.</returns>
    [NotNull]
    public IEnumerable<PropertyAccessor> AsEnumerable ([NotNull] ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("transaction", transaction);
      DomainObjectCheckUtility.CheckIfRightTransaction(_domainObject, transaction);

      foreach (PropertyDefinition propertyDefinition in ClassDefinition.GetPropertyDefinitions())
        yield return this[propertyDefinition.PropertyName, transaction];

      foreach (IRelationEndPointDefinition endPointDefinition in ClassDefinition.GetRelationEndPointDefinitions())
      {
        if (endPointDefinition.IsVirtual)
        {
          Assertion.DebugAssert(!endPointDefinition.IsAnonymous, "!Definition.IsAnonymous");
          yield return this[endPointDefinition.PropertyName, transaction];
        }
      }
    }

    /// <summary>
    /// Determines whether the domain object contains a property with the specified identifier.
    /// </summary>
    /// <param name="propertyIdentifier">The long property identifier to check for.</param>
    /// <returns>
    /// True if the domain object contains a property named as specified by <paramref name="propertyIdentifier"/>; otherwise, false.
    /// </returns>
    public bool Contains ([NotNull] string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("propertyIdentifier", propertyIdentifier);

      return PropertyAccessorDataCache.GetPropertyAccessorData(propertyIdentifier) != null;
    }


    /// <summary>
    /// Determines whether the domain object contains a property with the specified short name and declaring type.
    /// </summary>
    /// <param name="domainObjectType">The type declaring the property with the given <paramref name="shortPropertyName"/>.</param>
    /// <param name="shortPropertyName">The short property name to check for.</param>
    /// <returns>
    /// True if the domain object contains a property named as specified by <paramref name="shortPropertyName"/> declared on
    /// <paramref name="domainObjectType"/>; otherwise, false.
    /// </returns>
    public bool Contains ([NotNull] Type domainObjectType, [NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull("domainObjectType", domainObjectType);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      return PropertyAccessorDataCache.GetPropertyAccessorData(domainObjectType, shortPropertyName) != null;
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at a given declaring type upwards the inheritance hierarchy.
    /// </summary>
    /// <param name="typeToStartSearch">The type to start searching from.</param>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy, starting from <paramref name="typeToStartSearch"/>.</returns>
    /// <exception cref="ArgumentNullException">One or more of the arguments passed to this method are <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    public PropertyAccessor Find ([NotNull] Type typeToStartSearch, [NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull("typeToStartSearch", typeToStartSearch);
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);

      var propertyAccessorData = PropertyAccessorDataCache.FindPropertyAccessorData(typeToStartSearch, shortPropertyName);

      if (propertyAccessorData == null)
      {
        var message = string.Format(
            "The domain object type '{0}' does not have or inherit a mapping property with the short name '{1}'.",
            typeToStartSearch.GetFullNameSafe(),
            shortPropertyName);
        throw new ArgumentException(message, "shortPropertyName");
      }

      return GetPropertyAccessor(ClientTransaction, propertyAccessorData);
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at the type of the given <see cref="DomainObject"/> argument.
    /// </summary>
    /// <typeparam name="TDomainObject">The type to start searching from.</typeparam>
    /// <param name="thisDomainObject">The domain object parameter used for inference of type <typeparamref name="TDomainObject"/>.</param>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy, starting from <typeparamref name="TDomainObject"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="shortPropertyName"/>parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    /// <remarks>
    /// This method exists as a convenience overload of <see cref="Find(Type, string)"/>. Instead of needing to specify a lengthy <c>typeof(...)</c>
    /// expression, this method can usually infer the type to search from the <c>this</c> parameter passed as the first argument.
    /// </remarks>
    [NotNull]
    public PropertyAccessor Find<TDomainObject> ([CanBeNull] TDomainObject thisDomainObject, [NotNull] string shortPropertyName)
        where TDomainObject : DomainObject
    {
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);
      return Find(typeof(TDomainObject), shortPropertyName);
    }

    /// <summary>
    /// Finds a property with the specified short name, starting its search at the type of the <see cref="DomainObject"/> whose properties
    /// are represented by this indexer.
    /// </summary>
    /// <param name="shortPropertyName">The short name of the property to find.</param>
    /// <returns>A <see cref="PropertyAccessor"/> encapsulating the first property with the given <paramref name="shortPropertyName"/>
    /// found when traversing upwards through the inheritance hierarchy</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="shortPropertyName"/>parameter is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">No matching property could be found.</exception>
    /// <remarks>
    /// This method exists as a convenience overload of <see cref="Find(Type, string)"/>. Instead of needing to specify a starting type for the search, 
    /// this method assumes that it should start at the actual type of the current <see cref="DomainObject"/>.
    /// </remarks>
    [NotNull]
    public PropertyAccessor Find ([NotNull] string shortPropertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty("shortPropertyName", shortPropertyName);
      return Find(ClassDefinition.Type, shortPropertyName);
    }

    /// <summary>
    /// Gets all related objects of the associated <see cref="DomainObject"/>.
    /// </summary>
    /// <returns>An enumeration of all <see cref="DomainObject"/> directly referenced by the associated <see cref="DomainObject"/> in the form of
    /// <see cref="PropertyKind.RelatedObject"/> and <see cref="PropertyKind.RelatedObjectCollection"/> properties.</returns>
    [NotNull]
    public IEnumerable<DomainObject> GetAllRelatedObjects ()
    {
      foreach (var property in AsEnumerable())
      {
        switch (property.PropertyData.Kind)
        {
          case PropertyKind.RelatedObject:
            var value = (DomainObject?)property.GetValueWithoutTypeCheck();
            if (value != null)
              yield return value;
            break;
          case PropertyKind.RelatedObjectCollection:
            var values = (IEnumerable?)property.GetValueWithoutTypeCheck();
            Assertion.IsNotNull(values, "Collection property '{0}' does not have a value.", property.PropertyData.PropertyIdentifier);
            foreach (DomainObject relatedObject in values)
              yield return relatedObject;
            break;
        }
      }
    }

    private ClassDefinition ClassDefinition
    {
      get
      {
        var objectID = _domainObject.ID;
        Assertion.DebugIsNotNull(objectID, "DomainObject.ID must not be null.");
        return objectID.ClassDefinition;
      }
    }

    private PropertyAccessorDataCache PropertyAccessorDataCache
    {
      get { return ClassDefinition.PropertyAccessorDataCache; }
    }

    private ClientTransaction ClientTransaction
    {
      get { return _domainObject.GetDefaultTransactionContext().ClientTransaction; }
    }

    private PropertyAccessor GetPropertyAccessor ([NotNull] ClientTransaction transaction, [NotNull] PropertyAccessorData data)
    {
      // PropertyAccesor-ctor checks the argument
      return new PropertyAccessor(_domainObject, data, transaction);
    }
  }
}
