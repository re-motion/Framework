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
using System.ComponentModel;
using System.Globalization;
using Remotion.Collections;
using Remotion.ServiceLocation;

namespace Remotion.Utilities
{
  /// <summary> 
  ///   Provides functionality to get the <see cref="TypeConverter"/> for a <see cref="Type"/> and to convert a value
  ///   from a source <see cref="Type"/> into a destination <see cref="Type"/>.
  /// </summary>
  [ImplementationFor (typeof (ITypeConversionProvider), Lifetime = LifetimeKind.Singleton)]
  public class TypeConversionProvider : ITypeConversionProvider
  {
    private readonly IDataStore<Type, TypeConverter> _typeConverters = DataStoreFactory.CreateWithLocking<Type, TypeConverter>();

    /// <summary> Creates a new instace of the <see cref="TypeConversionProvider"/> type. </summary>
    /// <returns> An instance of the <see cref="TypeConversionProvider"/> type. </returns>
    [Obsolete ("Use SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>() instead if the global instance suffices, otherwise create a new instance via the constructor. (Version 1.15.8.0)", true)]
    public static TypeConversionProvider Create ()
    {
      throw new NotSupportedException (
          "Use SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>() instead if the global instance suffices, otherwise create a new instance via the constructor.");
    }

    /// <summary> Gets the current <see cref="TypeConversionProvider"/>. </summary>
    /// <value> An instance of the <see cref="TypeConversionProvider"/> type. </value>
    [Obsolete ("Use SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>() instead. (Version 1.15.8.0)")]
    public static ITypeConversionProvider Current
    {
      get { return SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(); }
    }

    /// <summary> Sets the current <see cref="TypeConversionProvider"/>. </summary>
    /// <param name="provider"> A <see cref="TypeConversionProvider"/>. Must not be <see langword="null"/>. </param>
    [Obsolete ("Configure the current ITypeConversionProvider via the application's IoC container instead. (Version 1.15.8.0)", true)]
    public static void SetCurrent (TypeConversionProvider provider)
    {
      throw new NotSupportedException ("Configure the current TypeConversionProvider via the application's IoC container instead.");
    }

    private readonly ITypeConverterFactory _typeConverterFactory;
    private readonly Dictionary<Type, TypeConverter> _additionalTypeConverters = new Dictionary<Type, TypeConverter>();
    private readonly BidirectionalStringConverter _stringConverter = new BidirectionalStringConverter();

    public TypeConversionProvider (ITypeConverterFactory typeConverterFactory)
    {
      ArgumentUtility.CheckNotNull ("typeConverterFactory", typeConverterFactory);

      _typeConverterFactory = typeConverterFactory;
    }

    public virtual TypeConverterResult GetTypeConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverterResult additionalTypeConverterResult = GetAdditionalTypeConverter (sourceType, destinationType);
      if (!additionalTypeConverterResult.Equals (TypeConverterResult.Empty))
        return additionalTypeConverterResult;

      TypeConverterResult typeConverterResult = GetTypeConverterFromFactory (sourceType, destinationType);
      if (!typeConverterResult.Equals (TypeConverterResult.Empty))
        return typeConverterResult;

      TypeConverterResult stringConverterResult = GetStringConverter (sourceType, destinationType);
      if (!stringConverterResult.Equals (TypeConverterResult.Empty))
        return stringConverterResult;

      return TypeConverterResult.Empty;
    }

    public virtual TypeConverter GetTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter converter = GetAdditionalTypeConverter (type);
      if (converter != null)
        return converter;

      converter = GetTypeConverterFromFactory (type);
      if (converter != null)
        return converter;

      if (type == typeof (string))
        return _stringConverter;

      return null;
    }

    /// <summary> 
    ///   Registers the <paramref name="converter"/> for the <paramref name="type"/>, overriding the default settings. 
    /// </summary>
    /// <param name="type"> 
    ///   The <see cref="Type"/> for which the <paramref name="converter"/> should be used. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <param name="converter"> The <see cref="TypeConverter"/> to register. Must not be <see langword="null"/>. </param>
    public void AddTypeConverter (Type type, TypeConverter converter)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("converter", converter);
      _additionalTypeConverters[type] = converter;
    }

    /// <summary>
    ///   Unregisters a special <see cref="TypeConverter"/> previously registered by using <see cref="AddTypeConverter"/>.
    /// </summary>
    /// <param name="type">
    ///   The <see cref="Type"/> whose special <see cref="TypeConverter"/> should be removed. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <remarks> If no <see cref="TypeConverter"/> has been registered, the method has no effect. </remarks>
    public void RemoveTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      _additionalTypeConverters.Remove (type);
    }

    public virtual bool CanConvert (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      if (sourceType == typeof (DBNull))
        return NullableTypeUtility.IsNullableType (destinationType);
      
      if (AreUnderlyingTypesEqual (destinationType, sourceType))
        return true;

      TypeConverterResult typeConverterResult = GetTypeConverter (sourceType, destinationType);
      return !typeConverterResult.Equals (TypeConverterResult.Empty);
    }

    public object Convert (Type sourceType, Type destinationType, object value)
    {
      return Convert (null, null, sourceType, destinationType, value);
    }

    public virtual object Convert (ITypeDescriptorContext context, CultureInfo culture, Type sourceType, Type destinationType, object value)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      bool isNullableDestinationType = NullableTypeUtility.IsNullableType (destinationType);
      if (value == DBNull.Value && isNullableDestinationType)
        return GetValueOrEmptyString (destinationType, null);

      if (value == null && !isNullableDestinationType)
        throw new NotSupportedException (string.Format ("Cannot convert value 'null' to non-nullable type '{0}'.", destinationType));
      else if (value != null && !sourceType.IsInstanceOfType (value))
        throw ArgumentUtility.CreateArgumentTypeException ("value", value.GetType(), sourceType);
      
      if (AreUnderlyingTypesEqual (sourceType, destinationType))
        return GetValueOrEmptyString (destinationType, value);

      TypeConverterResult typeConverterResult = GetTypeConverter (sourceType, destinationType);
      if (!typeConverterResult.Equals (TypeConverterResult.Empty))
      {
        switch (typeConverterResult.TypeConverterType)
        {
          case TypeConverterType.SourceTypeConverter:
            return typeConverterResult.TypeConverter.ConvertTo (context, culture, value, destinationType);
          default:
            Assertion.IsTrue (typeConverterResult.TypeConverterType == TypeConverterType.DestinationTypeConverter);
            return typeConverterResult.TypeConverter.ConvertFrom (context, culture, value);
        }
      }

      throw new NotSupportedException (string.Format ("Cannot convert value '{0}' to type '{1}'.", value, destinationType));
    }

    private object GetValueOrEmptyString (Type destinationType, object value)
    {
      if (destinationType == typeof (string) && value == null)
        return string.Empty;
      return value;
    }

    protected TypeConverterResult GetAdditionalTypeConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverter sourceTypeConverter = GetAdditionalTypeConverter (sourceType);
      if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, sourceTypeConverter);

      TypeConverter destinationTypeConverter = GetAdditionalTypeConverter (destinationType);
      if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, destinationTypeConverter);

      return TypeConverterResult.Empty;
    }

    protected TypeConverter GetAdditionalTypeConverter (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter typeConverter;
      if (_additionalTypeConverters.TryGetValue (type, out typeConverter))
        return typeConverter;

      return null;
    }

    protected TypeConverterResult GetTypeConverterFromFactory (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);

      TypeConverter sourceTypeConverter = GetTypeConverterFromFactory (sourceType);
      if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, sourceTypeConverter);

      TypeConverter destinationTypeConverter = GetTypeConverterFromFactory (destinationType);
      if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, destinationTypeConverter);

      return TypeConverterResult.Empty;
    }

    protected TypeConverter GetTypeConverterFromFactory (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      TypeConverter converter = GetTypeConverterFromCache (type);
      if (converter == null && !HasTypeInCache (type))
      {
        converter = _typeConverterFactory.CreateTypeConverterOrDefault (type);
        AddTypeConverterToCache (type, converter);
      }
      return converter;
    }

    protected TypeConverterResult GetStringConverter (Type sourceType, Type destinationType)
    {
      ArgumentUtility.CheckNotNull ("sourceType", sourceType);
      ArgumentUtility.CheckNotNull ("destinationType", destinationType);
      
      if (sourceType == typeof (string) && _stringConverter.CanConvertTo (destinationType))
        return new TypeConverterResult (TypeConverterType.SourceTypeConverter, _stringConverter);

      if (destinationType == typeof (string) && _stringConverter.CanConvertFrom (sourceType))
        return new TypeConverterResult (TypeConverterType.DestinationTypeConverter, _stringConverter);

      return TypeConverterResult.Empty;
    }

    protected void AddTypeConverterToCache (Type type, TypeConverter converter)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _typeConverters[type] = converter;
    }

    protected TypeConverter GetTypeConverterFromCache (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
     
      TypeConverter typeConverter;
      if (_typeConverters.TryGetValue (type, out typeConverter))
        return typeConverter;

      return null;
    }

    protected bool HasTypeInCache (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _typeConverters.ContainsKey (type);
    }

    private bool AreUnderlyingTypesEqual (Type destinationType, Type sourceType)
    {
      if (sourceType == destinationType)
        return true;

      if ((Nullable.GetUnderlyingType (sourceType) ?? sourceType) == (Nullable.GetUnderlyingType (destinationType) ?? destinationType))
        return true;

      return false;
    }
  }
}
