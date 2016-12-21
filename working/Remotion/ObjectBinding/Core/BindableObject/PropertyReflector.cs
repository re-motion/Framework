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
using System.Collections.ObjectModel;
using Remotion.ExtensibleEnums;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.Utilities;
using TypeExtensions = Remotion.Reflection.TypeExtensions;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Use the <see cref="PropertyReflector"/> to create <see cref="IBusinessObjectProperty"/> implementations for the bindable object implementation
  /// of the business object interfaces.
  /// </summary>
  public class PropertyReflector
  {
    public static PropertyReflector Create (IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      return ObjectFactory.Create<PropertyReflector> (true,ParamList.Create (propertyInfo,businessObjectProvider));
    }

    private readonly IPropertyInformation _propertyInfo;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly IDefaultValueStrategy _defaultValueStrategy;
    private readonly IBindablePropertyReadAccessStrategy _bindablePropertyReadAccessStrategy;
    private readonly IBindablePropertyWriteAccessStrategy _bindablePropertyWriteAccessStrategy;
    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    protected PropertyReflector (
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider)
        : this (
            propertyInfo,
            businessObjectProvider,
            new BindableObjectDefaultValueStrategy(),
            SafeServiceLocator.Current.GetInstance<IBindablePropertyReadAccessStrategy>(),
            SafeServiceLocator.Current.GetInstance<IBindablePropertyWriteAccessStrategy>(),
            SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>())
    {
    }

    public PropertyReflector (
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDefaultValueStrategy defaultValueStrategy,
        IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy,
        IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy,
        BindableObjectGlobalizationService bindableObjectGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("defaultValueStrategy", defaultValueStrategy);
      ArgumentUtility.CheckNotNull ("bindablePropertyReadAccessStrategy", bindablePropertyReadAccessStrategy);
      ArgumentUtility.CheckNotNull ("bindablePropertyWriteAccessStrategy", bindablePropertyWriteAccessStrategy);
      ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);

      _propertyInfo = propertyInfo;
      _businessObjectProvider = businessObjectProvider;
      _bindablePropertyReadAccessStrategy = bindablePropertyReadAccessStrategy;
      _bindablePropertyWriteAccessStrategy = bindablePropertyWriteAccessStrategy;
      _bindableObjectGlobalizationService = bindableObjectGlobalizationService;
      _defaultValueStrategy = defaultValueStrategy;
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    public PropertyBase GetMetadata ()
    {
      Type underlyingType = GetUnderlyingType();
      PropertyBase.Parameters parameters = CreateParameters (underlyingType);

      if (underlyingType == typeof (Boolean))
        return new BooleanProperty (parameters);
      else if (underlyingType == typeof (Byte))
        return new ByteProperty (parameters);
      else if (underlyingType == typeof (DateTime) && GetDateTimeType() == DateTimeType.Date)
        return new DateProperty (parameters);
      else if (underlyingType == typeof (DateTime))
        return new DateTimeProperty (parameters);
      else if (underlyingType == typeof (Decimal))
        return new DecimalProperty (parameters);
      else if (underlyingType == typeof (Double))
        return new DoubleProperty (parameters);
      else if (underlyingType.IsEnum && !AttributeUtility.IsDefined<FlagsAttribute> (underlyingType, false))
        return new EnumerationProperty (parameters);
      else if (ExtensibleEnumUtility.IsExtensibleEnumType (underlyingType))
        return new ExtensibleEnumerationProperty (parameters);
      else if (underlyingType == typeof (Guid))
        return new GuidProperty (parameters);
      else if (underlyingType == typeof (Int16))
        return new Int16Property (parameters);
      else if (underlyingType == typeof (Int32))
        return new Int32Property (parameters);
      else if (underlyingType == typeof (Int64))
        return new Int64Property (parameters);
      else if (underlyingType == typeof (Single))
        return new SingleProperty (parameters);
      else if (underlyingType == typeof (String))
        return new StringProperty (parameters, GetMaxLength());
        // IBusinessObject check should be after all regular checks to prevent unnecessary checks for Single and String in the code generation.
      else if (IsReferenceProperty (parameters))
        return new ReferenceProperty (parameters);
      else
        return GetMetadata (parameters);
    }

    private bool IsReferenceProperty (PropertyBase.Parameters parameters)
    {
      return typeof (IBusinessObject).IsAssignableFrom (parameters.UnderlyingType)
             || typeof (IBusinessObject).IsAssignableFrom (parameters.ConcreteType.Value);
    }

    protected virtual PropertyBase GetMetadata (PropertyBase.Parameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      return new NotSupportedProperty (parameters);
    }

    private Lazy<Type> GetConcreteType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return new Lazy<Type> (() => BindableObjectProvider.GetConcreteTypeForBindableObjectImplementation (type));
    }

    protected virtual Type GetUnderlyingType ()
    {
      return Nullable.GetUnderlyingType (GetItemType()) ?? GetItemType();
    }

    protected virtual Type GetItemType ()
    {
      if (_propertyInfo.PropertyType.IsArray)
        return _propertyInfo.PropertyType.GetElementType();

      if (TypeExtensions.CanAscribeTo (_propertyInfo.PropertyType, typeof (IList<>)))
        return TypeExtensions.GetAscribedGenericArguments (_propertyInfo.PropertyType, typeof (IList<>))[0];

      if (typeof (IList).IsAssignableFrom (_propertyInfo.PropertyType))
        return GetItemTypeFromAttribute();

      return _propertyInfo.PropertyType;
    }

    protected virtual IListInfo GetListInfo ()
    {
      if (IsListProperty())
        return new ListInfo (_propertyInfo.PropertyType, GetItemType());

      return null;
    }

    protected virtual bool GetIsRequired ()
    {
      if (_propertyInfo.PropertyType.IsEnum && AttributeUtility.IsDefined<UndefinedEnumValueAttribute> (_propertyInfo.PropertyType, false))
        return false;
      if (_propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType (_propertyInfo.PropertyType) == null)
        return true;
      return false;
    }

    protected virtual bool GetIsReadOnly ()
    {
      ObjectBindingAttribute attribute = _propertyInfo.GetCustomAttribute<ObjectBindingAttribute> (true);
      if (attribute != null && attribute.ReadOnly)
        return true;

      if (TypeExtensions.CanAscribeTo (_propertyInfo.PropertyType, typeof (ReadOnlyCollection<>)))
        return true;

      if (_propertyInfo.CanBeSetFromOutside)
        return false;

      if (IsListProperty() && !_propertyInfo.PropertyType.IsArray)
        return false;

      return true;
    }

    protected virtual int? GetMaxLength ()
    {
      return null;
    }

    protected virtual DateTimeType GetDateTimeType ()
    {
      return _propertyInfo.IsDefined<DatePropertyAttribute> (true) ? DateTimeType.Date : DateTimeType.DateTime;
    }

    protected virtual bool IsListProperty ()
    {
      return typeof (IList).IsAssignableFrom (_propertyInfo.PropertyType);
    }

    protected IDefaultValueStrategy GetDefaultValueStrategy ()
    {
      return _defaultValueStrategy;
    }

    private PropertyBase.Parameters CreateParameters (Type underlyingType)
    {
      return new PropertyBase.Parameters (
          _businessObjectProvider,
          _propertyInfo,
          underlyingType,
          GetConcreteType (underlyingType),
          GetListInfo(),
          GetIsRequired(),
          GetIsReadOnly(),
          GetDefaultValueStrategy(),
          _bindablePropertyReadAccessStrategy,
          _bindablePropertyWriteAccessStrategy,
          _bindableObjectGlobalizationService);
    }

    private Type GetItemTypeFromAttribute ()
    {
      ItemTypeAttribute itemTypeAttribute = _propertyInfo.GetCustomAttribute<ItemTypeAttribute> (true);
      if (itemTypeAttribute == null)
        throw new Exception ("ItemTypeAttribute is required for properties of type IList.");

      return itemTypeAttribute.ItemType;
    }
  }
}