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
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Represents a property of a domain object that is persisted by the framework.
  /// </summary>
  public sealed class PropertyValue
  {
    public static bool IsTypeSupported (Type propertyType)
    {
      ArgumentUtility.CheckNotNull("propertyType", propertyType);

      return propertyType.IsValueType
          || ReflectionUtility.IsStringPropertyValueType(propertyType)
          || ReflectionUtility.IsBinaryPropertyValueType(propertyType)
          || ReflectionUtility.IsTypePropertyValueType(propertyType)
          || ReflectionUtility.IsObjectIDPropertyValueType(propertyType)
          || ReflectionUtility.IsExtensibleEnumPropertyValueType(propertyType)
          || ReflectionUtility.IsStructuralEquatablePropertyValueType(propertyType);
    }

    public static bool AreValuesDifferent (object? value1, object? value2)
    {
      if (ReferenceEquals(value1, value2))
        return false;

      if (value1 is byte[])
        return true;

      if (StructuralComparisons.StructuralEqualityComparer.Equals(value1, value2))
        return false;

      return true;
    }

    private readonly PropertyDefinition _definition;

    private object? _value;
    private object? _originalValue;
    private bool _hasBeenTouched;

    // construction and disposing

    /// <summary>
    /// Initializes a new <b>PropertyValue</b> with a given <see cref="PropertyDefinition"/> and an initial <see cref="Value"/>.
    /// </summary>
    /// <param name="definition">The <see cref="PropertyDefinition"/> to use for initializing the <b>PropertyValue</b>. Must not be <see langword="null"/>.</param>
    /// <param name="value">The initial <see cref="Value"/> for the <b>PropertyValue</b>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="definition"/> is <see langword="null"/>.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.InvalidTypeException">
    /// <paramref name="value"/> does not match the required type specified in <paramref name="definition"/>.
    /// </exception>
    public PropertyValue (PropertyDefinition definition, object? value)
    {
      ArgumentUtility.CheckNotNull("definition", definition);

      if (!IsTypeSupported(definition.PropertyType))
      {
        var message = string.Format("The property '{0}' (declared on type '{1}') is invalid because its values cannot be copied. Only value types, "
            + "strings, the Type type, byte arrays, types implementing IStructuralEquatable, and ObjectIDs are currently supported, but the property's type is '{2}'.",
            definition.PropertyName, definition.TypeDefinition.Type.GetFullNameSafe(), definition.PropertyType.GetFullNameSafe());
        throw new NotSupportedException(message);
      }

      CheckValue(value, definition);

      _definition = definition;
      _value = value;
      _originalValue = value;
      _hasBeenTouched = false;
    }

    // methods and properties

    /// <summary>
    /// Gets or sets the value of the <see cref="PropertyValue"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="PropertyValue"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    /// <exception cref="Remotion.Data.DomainObjects.InvalidTypeException">
    /// <paramref name="value"/> does not match the required type specified in <see cref="PropertyDefinition"/>.
    /// </exception>
    public object? Value
    {
      get
      {
        return _value;
      }
      set
      {
        CheckValue(value, _definition);
        _value = value;
        Touch();
      }
    }

    public void Touch ()
    {
      _hasBeenTouched = true;
    }

    /// <summary>
    /// Gets the original <see cref="Value"/> of the <see cref="PropertyValue"/> at the point of instantiation, loading, commit or rollback.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="PropertyValue"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public object? OriginalValue
    {
      get
      {
        return _originalValue;
      }
    }

    /// <summary>
    /// Indicates if the <see cref="Value"/> of the <see cref="PropertyValue"/> has changed since instantiation, loading, commit or rollback.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="PropertyValue"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public bool HasChanged
    {
      get
      {
        return AreValuesDifferent(_value, _originalValue);
      }
    }

    /// <summary>
    /// Indicates if the <see cref="Value"/> of the <see cref="PropertyValue"/> has been assigned since instantiation, loading, commit or rollback,
    /// regardless of whether the current value differs from the <see cref="OriginalValue"/>.
    /// </summary>
    /// <exception cref="ObjectInvalidException">The <see cref="DomainObject"/> is invalid and its <see cref="PropertyValue"/> has been discarded. 
    /// See <see cref="ObjectInvalidException"/> for further information.</exception>
    public bool HasBeenTouched
    {
      get
      {
        return _hasBeenTouched;
      }
    }

    public bool IsRelationProperty
    {
      get { return _definition.IsObjectID; }
    }

    public void SetDataFromSubTransaction (PropertyValue source)
    {
      ArgumentUtility.CheckNotNull("source", source);

      if (source._definition != _definition)
      {
        var message = string.Format(
            "Cannot set this property's value from '{0}'; the properties do not have the same property definition.",
            source._definition);
        throw new ArgumentException(message, "source");
      }

      _value = source._value;

      if (source.HasBeenTouched || HasChanged)
        Touch();
    }

    public void CommitState ()
    {
      if (HasChanged)
        _originalValue = _value;

      _hasBeenTouched = false;
    }

    public void RollbackState ()
    {
      if (HasChanged)
        _value = _originalValue;

      _hasBeenTouched = false;
    }

    private void CheckValue (object? value, PropertyDefinition definition)
    {
      if (value != null)
      {
        if (!definition.PropertyType.IsInstanceOfType(value))
          throw new InvalidTypeException(definition.PropertyName, definition.PropertyType, value.GetType());

        if (value is Enum)
          CheckEnumValue(value, definition);
      }
      else
      {
        if (!definition.IsNullablePropertyType)
        {
          throw new InvalidOperationException(string.Format("Property '{0}' does not allow null values.", definition.PropertyName));
        }
      }
    }

    private void CheckEnumValue (object value, PropertyDefinition definition)
    {
      if (value != null)
      {
        Type underlyingType = definition.IsNullable ? NullableTypeUtility.GetBasicType(definition.PropertyType) : definition.PropertyType;
        if (!EnumUtility.IsValidEnumValue(underlyingType, value))
        {
          string message = string.Format(
              "Value '{0}' for property '{1}' is not defined by enum type '{2}'.",
              value,
              definition.PropertyName,
              underlyingType);

          throw new InvalidEnumValueException(message, definition.PropertyName, underlyingType, value);
        }
      }
    }

    #region Serialization
    internal void DeserializeFromFlatStructure (FlattenedDeserializationInfo info)
    {
      _hasBeenTouched = info.GetBoolValue();
      _value = info.GetNullableValue<object>();
      if (_hasBeenTouched)
        _originalValue = info.GetNullableValue<object>();
      else
        _originalValue = _value;
    }

    internal void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      info.AddBoolValue(_hasBeenTouched);
      info.AddValue(_value);
      if (_hasBeenTouched)
        info.AddValue(_originalValue);
    }
    #endregion
  }
}
