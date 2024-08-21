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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectIDStringSerialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Uniquely identifies a <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// <see cref="ObjectID"/> supports values of type <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  [TypeConverter(typeof(ObjectIDConverter))]
  public sealed class ObjectID : IComparable
  {
    // types

    // static members and constants

    /// <summary>
    /// Tests whether two specified <see cref="ObjectID"/> objects are equivalent.
    /// </summary>
    /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the equality operator.</param>
    /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the equality operator.</param>
    /// <returns></returns>
    public static bool operator == (ObjectID? id1, ObjectID? id2)
    {
      return Equals(id1, id2);
    }

    /// <summary>
    /// Tests whether two specified <see cref="ObjectID"/> objects are different.
    /// </summary>
    /// <param name="id1">The <see cref="ObjectID"/> object that is to the left of the inequality operator.</param>
    /// <param name="id2">The <see cref="ObjectID"/> object that is to the right of the inequality operator.</param>
    /// <returns></returns>
    public static bool operator != (ObjectID? id1, ObjectID? id2)
    {
      return !Equals(id1, id2);
    }

    /// <summary>
    /// Determines whether the specified <see cref="ObjectID"/> instances are considered equal.
    /// </summary>
    /// <param name="id1">The first <see cref="ObjectID"/> to compare.</param>
    /// <param name="id2">The second <see cref="ObjectID"/> to compare.</param>
    /// <returns><see langword="true"/> if the both <see cref="ObjectID"/>s are equal; otherwise, <see langword="false"/>.</returns>
    public static bool Equals (ObjectID? id1, ObjectID? id2)
    {
      if (ReferenceEquals(id1, id2))
        return true;
      if (ReferenceEquals(id1, null))
        return false;

      return id1.Equals(id2);
    }

    /// <summary>
    /// Converts the string representation of the ID to an <see cref="ObjectID"/> instance. If the operation fails, an exception is thrown.
    /// </summary>
    /// <param name="objectIDString">A string containing the object ID to convert.</param>
    /// <returns>
    ///   An <see cref="ObjectID"/> instance equivalent to the object ID contained in <paramref name="objectIDString"/>. Must not be <see langword="null"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="objectIDString"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="objectIDString"/> is an empty string.</exception>
    /// <exception cref="System.FormatException"><paramref name="objectIDString"/> does not contain the string representation of an object ID.</exception>
    /// <remarks>
    /// If the probability that parsing fails is high, consider using <see cref="TryParse"/> instead, as it is more performant in the error case.
    /// </remarks>
    public static ObjectID Parse (string objectIDString)
    {
      ArgumentUtility.CheckNotNull("objectIDString", objectIDString);
      return ObjectIDStringSerializer.Instance.Parse(objectIDString);
    }

    /// <summary>
    /// Converts the string representation of the ID to an <see cref="ObjectID"/> instance. A return value indicates whether the operation succeeded.
    /// </summary>
    /// <param name="objectIDString">A string containing the object ID to convert.</param>
    /// <param name="result">If the conversion completes successfully, this parameter is set to an <see cref="ObjectID"/> instance equivalent to the 
    /// object ID contained in <paramref name="objectIDString"/>. Otherwise, it is set to <see langword="null" />.</param>
    /// <returns>
    /// <see langword="true" /> if the conversion completed successfully, <see langword="false" /> otherwise.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="objectIDString"/> is <see langword="null"/>.</exception>
    /// <exception cref="System.ArgumentException"><paramref name="objectIDString"/> is an empty string.</exception>
    /// <remarks>
    /// If you expect <paramref name="objectIDString"/> to always hold a valid <see cref="ObjectID"/> string, use <see cref="Parse"/> instead. Use
    /// this method only if an invalid string constitutes a supported use case.
    /// </remarks>
    public static bool TryParse (string objectIDString, [MaybeNullWhen(false)] out ObjectID result)
    {
      ArgumentUtility.CheckNotNull("objectIDString", objectIDString);
      return ObjectIDStringSerializer.Instance.TryParse(objectIDString, out result);
    }

    private readonly object _value;
    private readonly ClassDefinition _classDefinition;
    private int _cachedHashCode;
    private string? _cachedSerializedValue;

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified class ID and ID value.
    /// </summary>
    /// <param name="classID">The ID of the class of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classID"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="classID"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.<br /> -or- <br />
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters.
    ///   Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.IReadOnlyStorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classID"/> could not be found in the mapping configuration.
    public ObjectID (string classID, object value)
      : this(MappingConfiguration.Current.GetClassDefinition(ArgumentUtility.CheckNotNullOrEmpty("classID", classID)), value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified class type and ID value.
    /// </summary>
    /// <param name="classType">The <see cref="System.Type"/> of the class of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classType"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.<br /> -or- <br />
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters.
    ///   Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.IReadOnlyStorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classType"/> could not be found in the mapping configuration.
    public ObjectID (Type classType, object value)
      : this(MappingConfiguration.Current.GetTypeDefinition(ArgumentUtility.CheckNotNull("classType", classType)), value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <b>ObjectID</b> class with the specified <see cref="Mapping.ClassDefinition"/> and ID value.
    /// </summary>
    /// <param name="classDefinition">The <see cref="Mapping.ClassDefinition"/> of the object. Must not be <see langword="null"/>.</param>
    /// <param name="value">The ID value used to identify the object in the storage provider. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="classDefinition"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="System.ArgumentException">
    ///   <paramref name="value"/> is an empty <see cref="System.String"/>.<br /> -or- <br />
    ///   <paramref name="value"/> is an empty <see cref="System.Guid"/>.<br /> -or- <br />
    ///   <paramref name="value"/> has an unsupported type or is a string and contains invalid characters.
    ///   Supported types are <see cref="System.Guid"/>, <see cref="System.Int32"/> and <see cref="System.String"/>.
    /// </exception>
    /// <exception cref="Remotion.Data.DomainObjects.Persistence.Configuration.IdentityTypeNotSupportedException">
    ///   The type of <paramref name="value"/> is not supported by the underlying <see cref="Remotion.Data.DomainObjects.Persistence.IReadOnlyStorageProvider"/>.
    /// </exception>
    /// <exception cref="Mapping.MappingException"/>The specified <paramref name="classDefinition"/> could not be found in the mapping configuration.
    public ObjectID (ClassDefinition classDefinition, object value)
    {
      ArgumentUtility.CheckNotNull("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull("value", value);

      if (classDefinition.IsAbstract)
      {
        throw CreateArgumentException(
            "classDefinition",
            "An ObjectID cannot be constructed for abstract type '{0}' of class '{1}'.",
            classDefinition.ClassType.GetAssemblyQualifiedNameSafe(),
            classDefinition.ID);
      }

      CheckValue("value", value);

      classDefinition.StorageEntityDefinition.StorageProviderDefinition.CheckIdentityType(value.GetType());

      _classDefinition = classDefinition;
      _value = value;
    }

    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> of the <see cref="Persistence.IReadOnlyStorageProvider"/> which stores the object.
    /// </summary>
    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return ClassDefinition.StorageEntityDefinition.StorageProviderDefinition; }
    }

    /// <summary>
    /// Gets the ID value used to identify the object in the storage provider.
    /// </summary>
    /// <remarks>
    /// <b>Value</b> can be of type <see cref="System.Guid"/>, <see cref="System.Int32"/> or <see cref="System.String"/>.
    /// </remarks>
    public object Value
    {
      get { return _value; }
    }

    /// <summary>
    /// The class ID of the object class.
    /// </summary>
    public string ClassID
    {
      get { return _classDefinition.ID; }
    }

    /// <summary>
    /// Gets the <see cref="Mapping.ClassDefinition"/> associated with this <see cref="ObjectID"/>.
    /// </summary>
    public ClassDefinition ClassDefinition
    {
      get { return _classDefinition; }
    }

    /// <summary>
    /// Creates a typed handle to the <see cref="DomainObject"/> identified by this <see cref="ObjectID"/>.
    /// </summary>
    /// <typeparam name="T">The handle's object type. This must either be the <see cref="Mapping.ClassDefinition.ClassType"/> of this 
    /// <see cref="ObjectID"/>, or a base type of it.</typeparam>
    /// <returns>A typed handle to the <see cref="DomainObject"/> identified by this <see cref="ObjectID"/>.</returns>
    public IDomainObjectHandle<T> GetHandle<T> () where T : IDomainObject
    {
      try
      {
        return ClassDefinition.HandleCreator(this).Cast<T>();
      }
      catch (InvalidCastException ex)
      {
        var message = string.Format("The ObjectID '{0}' cannot be represented as an 'IDomainObjectHandle<{1}>'.", this, typeof(T));
        throw new ArgumentException(message, "T", ex);
      }
    }

    /// <summary>
    /// Returns the string representation of the current <see cref="ObjectID"/>.
    /// </summary>
    /// <returns>A <see cref="String"/> that represents the current <see cref="ObjectID"/>.</returns>
    public override string ToString ()
    {
      // Use lazy initialization because of deserialization.

      // Note: The following code might result in initializing identical values on multiple threads. This is a non-issue.

      if (_cachedSerializedValue == null)
        _cachedSerializedValue = ObjectIDStringSerializer.Instance.Serialize(this);

      return _cachedSerializedValue;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode ()
    {
      // Use lazy initialization because of deserialization.

      // Note: The following code is not completely thread-safe - the hash code might be calculated twice on different threads. 
      // However, we can assume that an int assignment is atomic (and the XOR operation is fully performed before the assignment takes place), 
      // so no half-calculated values should become visible.

      // Note: We assume that a hash code value of 0 means that it wasn't initialized. In the very unlikely situation that 
      // _classID.GetHashCode () == _value.GetHashCode (), the XOR operation would yield 0 and thus the hash code would be recalculated on each call.

      // ReSharper disable NonReadonlyFieldInGetHashCode
      if (_cachedHashCode == 0)
        _cachedHashCode = _classDefinition.GetHashCode() ^ _value.GetHashCode();

      return _cachedHashCode;
      // ReSharper restore NonReadonlyFieldInGetHashCode
    }

    /// <summary>
    /// Compares this <see cref="ObjectID"/> with another <see cref="ObjectID"/>and returns an integer that indicates whether the current instance 
    /// precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <param name="obj">An <see cref="ObjectID"/> to compare with this instance.</param>
    /// <returns>
    /// Less than zero if this instance is less than <paramref name="obj"/>; zero if this instance is equal to <paramref name="obj"/>; greater than 
    /// zero if this instance is greater than <paramref name="obj"/>.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">
    /// 	<paramref name="obj"/> is not an <see cref="ObjectID"/>.
    /// </exception>
    /// <remarks>
    /// If two <see cref="ObjectID"/> instances are compared that have different <see cref="Value"/> types, their <see cref="ClassID"/>s are compared
    /// instead.
    /// </remarks>
    public int CompareTo (object? obj)
    {
      if (obj == null)
        return 1;

      var other = obj as ObjectID;
      if (other == null)
        throw new ArgumentException("The argument must be of type ObjectID.", "obj");

      var leftValue = Value;
      var rightValue = other.Value;

      if (rightValue.GetType() != leftValue.GetType())
      {
        leftValue = ClassID;
        rightValue = other.ClassID;
      }

      return ((IComparable)leftValue).CompareTo(rightValue);
    }

    /// <summary>
    /// Determines whether the specified <see cref="ObjectID"/> is equal to the current <see cref="ObjectID"/>.
    /// </summary>
    /// <param name="obj">The <see cref="ObjectID"/> to compare with the current <see cref="ObjectID"/>. </param>
    /// <returns><see langword="true"/> if the specified <see cref="ObjectID"/> is equal to the current <see cref="ObjectID"/>; otherwise, 
    /// <see langword="false"/>.</returns>
    public override bool Equals (object? obj)
    {
      if (obj == null)
        return false;
      if (GetType() != obj.GetType())
        return false;

      var other = (ObjectID)obj;
      if (!Equals(ClassID, other.ClassID))
        return false;
      if (!Equals(Value, other.Value))
        return false;

      return true;
    }

    private void CheckValue (string argumentName, object value)
    {
      Type valueType = value.GetType();

      if (valueType != typeof(Guid) && valueType != typeof(int) && valueType != typeof(string))
        throw CreateArgumentException(argumentName, "Remotion.Data.DomainObjects.ObjectID does not support values of type '{0}'.", valueType);

      if (valueType == typeof(string))
        ArgumentUtility.CheckNotEmpty(argumentName, (string)value);

      if (valueType == typeof(Guid))
        ArgumentUtility.CheckNotEmpty(argumentName, (Guid)value);
    }

    private ArgumentException CreateArgumentException (string argumentName, string message, params object[] args)
    {
      return new ArgumentException(string.Format(message, args), argumentName);
    }
  }
}
