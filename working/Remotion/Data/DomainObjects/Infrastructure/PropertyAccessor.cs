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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an encapsulation of a <see cref="DomainObject">DomainObject's</see> property for simple access as well as static methods
  /// supporting working with properties.
  /// </summary>
  public struct PropertyAccessor
  {
    private readonly IDomainObject _domainObject;
    private readonly PropertyAccessorData _propertyData;
    private readonly ClientTransaction _clientTransaction;

    /// <summary>
    /// Initializes the <see cref="PropertyAccessor"/> object.
    /// </summary>
    /// <param name="domainObject">The domain object whose property is to be encapsulated.</param>
    /// <param name="propertyData">a <see cref="PropertyAccessorData"/> object describing the property to be accessed.</param>
    /// <param name="clientTransaction">The transaction to be used for accessing the property.</param>
    /// <exception cref="ArgumentNullException">One of the parameters passed to the constructor is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">The domain object does not have a property with the given identifier.</exception>
    public PropertyAccessor (IDomainObject domainObject, PropertyAccessorData propertyData, ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);
      ArgumentUtility.CheckNotNull ("propertyData", propertyData);
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      DomainObjectCheckUtility.CheckIfRightTransaction (domainObject, clientTransaction);

      _domainObject = domainObject;
      _clientTransaction = clientTransaction;
      _propertyData = propertyData;
    }

    /// <summary>
    /// Gets the domain object of this property.
    /// </summary>
    /// <value>The domain object this <see cref="PropertyAccessor"/> is associated with.</value>
    public IDomainObject DomainObject
    {
      get { return _domainObject; }
    }

    /// <summary>
    /// Gets the <see cref="PropertyAccessorData"/> object describing the property to be accessed.
    /// </summary>
    /// <value>The property data to be accessed.</value>
    public PropertyAccessorData PropertyData
    {
      get { return _propertyData; }
    }

    /// <summary>
    /// Gets the client transaction used to access this property.
    /// </summary>
    /// <value>The client transaction.</value>
    public ClientTransaction ClientTransaction
    {
      get { return _clientTransaction; }
    }

    /// <summary>
    /// Indicates whether the property's value has been changed in its current transaction.
    /// </summary>
    /// <value>True if the property's value has changed; false otherwise.</value>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public bool HasChanged
    {
      get
      {
        CheckTransactionalStatus (ClientTransaction);
        return PropertyData.GetStrategy().HasChanged (this, ClientTransaction);
      }
    }

    /// <summary>
    /// Indicates whether  the property's value (for simple and related object properties) or one of its elements (for related object collection
    /// properties) has been assigned since instantiation, loading, commit or rollback, regardless of whether the current value differs from the
    /// original value.
    /// </summary>
    /// <remarks>This property differs from <see cref="HasChanged"/> in that for <see cref="HasChanged"/> to be true, the property's value (or its
    /// elements) actually must have changed in an assignment operation. <see cref="HasBeenTouched"/> is true also if a property gets assigned the
    /// same value it originally had. This can be useful to determine whether the property has been written once since the last load, commit, or
    /// rollback operation.
    /// </remarks>
    public bool HasBeenTouched
    {
      get
      {
        CheckTransactionalStatus (ClientTransaction);
        return PropertyData.GetStrategy().HasBeenTouched (this, ClientTransaction);
      }
    }

    /// <summary>
    /// Gets a value indicating whether the property's value is <see langword="null"/>.
    /// </summary>
    /// <value>True if this instance is null; otherwise, false.</value>
    /// <remarks>This can be used to efficiently check whether a related object property has a value without actually loading the related
    /// object.</remarks>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public bool IsNull
    {
      get
      {
        CheckTransactionalStatus (ClientTransaction);
        return PropertyData.GetStrategy().IsNull (this, ClientTransaction);
      }
    }

    /// <summary>
    /// Gets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type.
    /// </typeparam>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public T GetValue<T> ()
    {
      CheckType(typeof (T));

      object value = GetValueWithoutTypeCheck ();

      Assertion.DebugAssert (
          value != null || NullableTypeUtility.IsNullableType (PropertyData.PropertyType),
          "Property '{0}' is a value type but the DataContainer returned null.",
          PropertyData.PropertyIdentifier);

      try
      {
        return (T) value;
      }
      catch (InvalidCastException ex)
      {
        Assertion.IsNotNull (value, "Otherwise, the cast would have succeeded (ref type) or thrown a NullReferenceException (value type).");
        var message = string.Format (
            "The property '{0}' was expected to hold an object of type '{1}', but it returned an object of type '{2}'.",
            PropertyData.PropertyIdentifier,
            PropertyData.PropertyType,
            value.GetType());
        throw new InvalidTypeException (message, ex);
      }
    }

    /// <summary>
    /// Gets the property's value without performing a type check.
    /// </summary>
    /// <returns>The value of the encapsulated property. For simple value properties,
    /// this is the property value. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</returns>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public object GetValueWithoutTypeCheck ()
    {
      CheckTransactionalStatus (ClientTransaction);
      return PropertyData.GetStrategy ().GetValueWithoutTypeCheck (this, ClientTransaction);
    }


    /// <summary>
    /// Gets the ID of the related object for related object properties.
    /// </summary>
    /// <returns>The ID of the related object stored in the encapsulated property.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public ObjectID GetRelatedObjectID ()
    {
      CheckTransactionalStatus (ClientTransaction);

      if (PropertyData.Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (PropertyData.RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetValueWithoutTypeCheck (this, ClientTransaction);
    }

    /// <summary>
    /// Gets the property's value from that moment when the property's domain object was enlisted in the current <see cref="DomainObjects.ClientTransaction"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="InvalidTypeException">
    /// The type requested via <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
    /// </exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public T GetOriginalValue<T> ()
    {
      CheckType (typeof (T));
      return (T) GetOriginalValueWithoutTypeCheck ();
    }

    /// <summary>
    /// Gets the property's original value without performing a type check.
    /// </summary>
    /// <returns>The original value of the encapsulated property in the current transaction.</returns>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public object GetOriginalValueWithoutTypeCheck ()
    {
      CheckTransactionalStatus (ClientTransaction);
      return PropertyData.GetStrategy().GetOriginalValueWithoutTypeCheck (this, ClientTransaction);
    }

    /// <summary>
    /// Gets the original ID of the related object for related object properties.
    /// </summary>
    /// <returns>The ID of the related object originally stored in the encapsulated property.</returns>
    /// <exception cref="InvalidOperationException">The property type is not <see cref="PropertyKind.RelatedObject"/> or the property is a virtual
    /// relation end point (i.e. the other side of the relation holds the foreign key).</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public ObjectID GetOriginalRelatedObjectID ()
    {
      CheckTransactionalStatus (ClientTransaction);

      if (PropertyData.Kind != PropertyKind.RelatedObject)
        throw new InvalidOperationException ("This operation can only be used on related object properties.");

      if (PropertyData.RelationEndPointDefinition.IsVirtual)
        throw new InvalidOperationException ("ObjectIDs only exist on the real side of a relation, not on the virtual side.");

      return (ObjectID) ValuePropertyAccessorStrategy.Instance.GetOriginalValueWithoutTypeCheck (this, ClientTransaction);
    }

    /// <summary>
    /// Sets the property's value.
    /// </summary>
    /// <typeparam name="T">
    /// The property value type. This must be the same as the type returned by <see cref="PropertyAccessorData.PropertyType"/>: For simple value properties,
    /// this is the simple property type. For related objects, this is the related object's type. For related object collections,
    /// this is <see cref="ObjectList{T}"/>, where "T" is the related objects' type. The type parameter can usually be inferred and needn't be
    /// specified in such cases.
    /// </typeparam>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The type <typeparamref name="T"/> is not the same as the property's type indicated by <see cref="PropertyAccessorData.PropertyType"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public void SetValue<T> (T value)
    {
      CheckType (typeof (T));
      SetValueWithoutTypeCheck (value);
    }

    /// <summary>
    /// Sets the property's value without performing an exact type check on the given value. The value must still be asssignable to
    /// <see cref="PropertyAccessorData.PropertyType"/>, though.
    /// </summary>
    /// <param name="value">The value to be set. For simple value properties,
    /// this is the value to be set. For related objects, this is the related object. For related object collections,
    /// this is an <see cref="ObjectList{T}"/>, where "T" is the related objects' type.</param>
    /// <exception cref="InvalidTypeException">
    /// The given <paramref name="value"/> is not assignable to the property because of its type.
    /// </exception>
    /// <exception cref="InvalidOperationException">The property is a related object collection; such properties cannot be set.</exception>
    /// <exception cref="ClientTransactionsDifferException">The <see cref="DomainObject"/> cannot be used in the current <see cref="DomainObjects.ClientTransaction"/>.</exception>
    /// <exception cref="ObjectInvalidException">The object is invalid in the associated <see cref="ClientTransaction"/>.</exception>
    public void SetValueWithoutTypeCheck (object value)
    {
      CheckTransactionalStatus (ClientTransaction);
      PropertyData.GetStrategy().SetValueWithoutTypeCheck (this, ClientTransaction, value);
    }

    /// <summary>
    /// Returns the full property name of the property represented by this <see cref="PropertyAccessor"/>.
    /// </summary>
    /// <returns>
    /// The same value as in <see cref="PropertyAccessorData.PropertyIdentifier"/>.
    /// </returns>
    public override string ToString ()
    {
      return PropertyData.PropertyIdentifier;
    }

    [AssertionMethod]
    private void CheckTransactionalStatus (ClientTransaction clientTransaction)
    {
      Assertion.DebugAssert (ReferenceEquals (ClientTransaction, clientTransaction));
      DomainObjectCheckUtility.EnsureNotInvalid (_domainObject, clientTransaction);
    }

    private void CheckType (Type typeToCheck)
    {
      ArgumentUtility.CheckNotNull ("typeToCheck", typeToCheck);
      if (PropertyData.PropertyType != typeToCheck)
        throw new InvalidTypeException (PropertyData.PropertyIdentifier, typeToCheck, PropertyData.PropertyType);
    }
  }
}
