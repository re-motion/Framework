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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  //TODO: doc
  public class EnumerationProperty : PropertyBase, IBusinessObjectEnumerationProperty
  {
    private readonly Enum _undefinedValue;
    private readonly IEnumerationValueFilter _enumerationValueFilter;

    /// <exception cref="InvalidOperationException">
    /// The enum type has an UndefinedEnumValueAttribute with a value that does not match the enum's type.
    /// <para>- or -</para>
    /// <para>The property is nullable and the property's type defines an UndefinedEnumValueAttribute</para>
    /// </exception>
    public EnumerationProperty (Parameters parameters)
        : base (parameters)
    {
      if (AttributeUtility.IsDefined<FlagsAttribute> (parameters.UnderlyingType, false))
      {
        throw new InvalidOperationException (
            string.Format (
                "The property '{0}' defined on type '{1}' is a flags-enum, which is not supported.",
                Identifier,
                PropertyInfo.DeclaringType));
      }
      _undefinedValue = GetUndefinedValue();
      
      var filterProvider = new EnumValueFilterProvider<DisableEnumValuesAttribute> (
          PropertyInfo, 
          t => AttributeUtility.GetCustomAttributes<DisableEnumValuesAttribute> (t, true));
      _enumerationValueFilter = filterProvider.GetEnumerationValueFilter ();
    }

    /// <summary> Returns a list of all the enumeration's values. </summary>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
    /// </returns>
    public IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject)
    {
      var valueInfos = new List<IEnumerationValueInfo>();
      foreach (Enum value in Enum.GetValues (UnderlyingType))
      {
        IEnumerationValueInfo enumerationValueInfo = GetValueInfoByValue (value, businessObject);
        if (enumerationValueInfo != null)
          valueInfos.Add (enumerationValueInfo);
      }
      return valueInfos.ToArray();
    }

    /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enabled enum values. </param>
    /// <returns> A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return Array.FindAll (GetAllValues (businessObject), current => current.IsEnabled);
    }

    /// <overloads> Returns a specific enumeration value. </overloads>
    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> 
    /// The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/> or <see langword="null"/> if the 
    /// <paramref name="value"/> represents <see langword="null"/>. 
    /// </returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      if (value == null)
        return null;

      var enumValue = (Enum) value;
      if (IsUndefinedValue (enumValue))
        return null;

      return new EnumerationValueInfo (value, GetIdenfier (enumValue), GetDisplayName (enumValue), IsEnabled (enumValue, businessObject));
    }

    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="identifier">The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for.</param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> 
    /// The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/> or <see langword="null"/> if the 
    /// <paramref name="identifier"/> represents <see langword="null"/>. 
    /// </returns>
    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      if (string.IsNullOrEmpty (identifier))
        return null;

      return GetValueInfoByValue (StringUtility.Parse (UnderlyingType, identifier, null), businessObject);
    }

    public override object ConvertFromNativePropertyType (object nativeValue)
    {
      if (nativeValue != null)
      {
        if (IsUndefinedValue ((Enum) nativeValue))
          return null;
      }

      return base.ConvertFromNativePropertyType (nativeValue);
    }

    public override object ConvertToNativePropertyType (object publicValue)
    {
      if (publicValue == null && _undefinedValue != null)
        return base.ConvertToNativePropertyType (_undefinedValue);

      return base.ConvertToNativePropertyType (publicValue);
    }

    private string GetIdenfier (Enum value)
    {
      return value.ToString();
    }

    private string GetDisplayName (Enum value)
    {
      return BindableObjectGlobalizationService.GetEnumerationValueDisplayName (value);
    }

    private bool IsEnabled (Enum value, IBusinessObject businessObject)
    {
      if (!Enum.IsDefined (UnderlyingType, value))
        return false;

      return _enumerationValueFilter.IsEnabled (new EnumerationValueInfo (value, GetIdenfier (value), null, true), businessObject, this);
    }

    private bool IsUndefinedValue (Enum value)
    {
      return value.Equals (_undefinedValue);
    }

    private Enum GetUndefinedValue ()
    {
      var undefinedEnumValueAttribute = AttributeUtility.GetCustomAttribute<UndefinedEnumValueAttribute> (UnderlyingType, false);

      if (undefinedEnumValueAttribute == null)
        return null;

      if (!UnderlyingType.IsInstanceOfType (undefinedEnumValueAttribute.GetValue()))
      {
        throw new InvalidOperationException (
            string.Format (
                "The enum type '{0}' defines a '{1}' with an enum value that belongs to a different enum type.",
                UnderlyingType,
                typeof (UndefinedEnumValueAttribute)));
      }

      if (IsNullable)
      {
        throw new InvalidOperationException (
            string.Format (
                "The property '{0}' defined on type '{1}' must not be nullable since the property's type already defines a '{2}'.",
                Identifier,
                PropertyInfo.DeclaringType,
                typeof (UndefinedEnumValueAttribute)));
      }

      return undefinedEnumValueAttribute.GetValue();
    }
  }
}
