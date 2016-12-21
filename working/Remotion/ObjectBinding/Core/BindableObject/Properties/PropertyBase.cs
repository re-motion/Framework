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
using System.Threading;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public abstract class PropertyBase : IBusinessObjectProperty
  {
    public sealed class Parameters
    {
      [NotNull]
      public readonly BindableObjectProvider BusinessObjectProvider;

      [NotNull]
      public readonly IPropertyInformation PropertyInfo;

      [NotNull]
      public readonly Type UnderlyingType;

      [NotNull]
      public readonly Lazy<Type> ConcreteType;

      [CanBeNull]
      public readonly IListInfo ListInfo;

      public readonly bool IsRequired;

      public readonly bool IsReadOnly;

      [NotNull]
      public readonly IDefaultValueStrategy DefaultValueStrategy;

      [NotNull]
      public readonly IBindablePropertyReadAccessStrategy BindablePropertyReadAccessStrategy;

      [NotNull]
      public readonly IBindablePropertyWriteAccessStrategy BindablePropertyWriteAccessStrategy;

      [NotNull]
      public readonly BindableObjectGlobalizationService BindableObjectGlobalizationService;

      public Parameters (
          [NotNull] BindableObjectProvider businessObjectProvider,
          [NotNull] IPropertyInformation propertyInfo,
          [NotNull] Type underlyingType,
          [NotNull] Lazy<Type> concreteType,
          [CanBeNull] IListInfo listInfo,
          bool isRequired,
          bool isReadOnly,
          [NotNull] IDefaultValueStrategy defaultValueStrategy,
          [NotNull] IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy,
          [NotNull] IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy,
          [NotNull] BindableObjectGlobalizationService bindableObjectGlobalizationService)
      {
        ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
        ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
        ArgumentUtility.CheckNotNull ("underlyingType", underlyingType);
        ArgumentUtility.CheckNotNull ("concreteType", concreteType);
        ArgumentUtility.CheckNotNull ("defaultValueStrategy", defaultValueStrategy);
        ArgumentUtility.CheckNotNull ("bindablePropertyReadAccessStrategy", bindablePropertyReadAccessStrategy);
        ArgumentUtility.CheckNotNull ("bindablePropertyWriteAccessStrategy", bindablePropertyWriteAccessStrategy);
        ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);

        BusinessObjectProvider = businessObjectProvider;
        PropertyInfo = propertyInfo;
        UnderlyingType = underlyingType;
        ConcreteType = new Lazy<Type> (
            () =>
            {
              var actualConcreteType = concreteType.Value;
              if (!underlyingType.IsAssignableFrom (actualConcreteType))
              {
                throw new InvalidOperationException (
                    string.Format (
                        "The concrete type must be assignable to the underlying type '{0}'.\r\nConcrete type: {1}",
                        underlyingType.FullName,
                        actualConcreteType.FullName));
              }
              return actualConcreteType;
            },
            LazyThreadSafetyMode.ExecutionAndPublication);
        ListInfo = listInfo;
        IsRequired = isRequired;
        IsReadOnly = isReadOnly;
        DefaultValueStrategy = defaultValueStrategy;
        BindablePropertyReadAccessStrategy = bindablePropertyReadAccessStrategy;
        BindablePropertyWriteAccessStrategy = bindablePropertyWriteAccessStrategy;
        BindableObjectGlobalizationService = bindableObjectGlobalizationService;
      }
    }

    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly IPropertyInformation _propertyInfo;
    private readonly IListInfo _listInfo;
    private readonly bool _isRequired;
    private readonly Type _underlyingType;
    private readonly bool _isReadOnly;
    private readonly bool _isNullable;
    private BindableObjectClass _reflectedClass;
    private readonly IDefaultValueStrategy _defaultValueStrategy;
    private readonly Func<object, object> _valueGetter;
    private readonly Action<object, object> _valueSetter;
    private readonly IBindablePropertyReadAccessStrategy _bindablePropertyReadAccessStrategy;
    private readonly IBindablePropertyWriteAccessStrategy _bindablePropertyWriteAccessStrategy;
    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    protected PropertyBase (Parameters parameters)
    {
      ArgumentUtility.CheckNotNull ("parameters", parameters);

      if (parameters.PropertyInfo.GetIndexParameters().Length > 0)
        throw new InvalidOperationException ("Indexed properties are not supported.");

      _businessObjectProvider = parameters.BusinessObjectProvider;
      _propertyInfo = parameters.PropertyInfo;
      _underlyingType = parameters.UnderlyingType;
      _listInfo = parameters.ListInfo;
      _isRequired = parameters.IsRequired;
      _isReadOnly = parameters.IsReadOnly;
      _defaultValueStrategy = parameters.DefaultValueStrategy;
      _bindablePropertyReadAccessStrategy = parameters.BindablePropertyReadAccessStrategy;
      _bindablePropertyWriteAccessStrategy = parameters.BindablePropertyWriteAccessStrategy;
      _bindableObjectGlobalizationService = parameters.BindableObjectGlobalizationService;
      _isNullable = GetNullability();
      _valueGetter = Maybe.ForValue (_propertyInfo.GetGetMethod (true)).Select (mi => mi.GetFastInvoker<Func<object, object>>()).ValueOrDefault();
      _valueSetter = Maybe.ForValue (_propertyInfo.GetSetMethod (true)).Select (mi => mi.GetFastInvoker<Action<object, object>>()).ValueOrDefault();
    }

    /// <summary> Gets a flag indicating whether this property contains multiple values. </summary>
    /// <value> <see langword="true"/> if this property contains multiple values. </value>
    /// <remarks> Multiple values are provided via any type implementing <see cref="IList"/>. </remarks>
    public bool IsList
    {
      get { return _listInfo != null; }
    }

    /// <summary>Gets the <see cref="IListInfo"/> for this <see cref="IBusinessObjectProperty"/>.</summary>
    /// <value>An onject implementing <see cref="IListInfo"/>.</value>
    /// <exception cref="InvalidOperationException">Thrown if the property is not a list property.</exception>
    public IListInfo ListInfo
    {
      get
      {
        if (_listInfo == null)
          throw new InvalidOperationException (string.Format ("Cannot access ListInfo for non-list properties.\r\nProperty: {0}", Identifier));
        return _listInfo;
      }
    }

    /// <summary> Gets the type of the property. </summary>
    /// <remarks> 
    ///   <para>
    ///     This is the type of elements returned by the <see cref="IBusinessObject.GetProperty(IBusinessObjectProperty)"/> method
    ///     and set via the <see cref="IBusinessObject.SetProperty(IBusinessObjectProperty,object)"/> method.
    ///   </para><para>
    ///     If <see cref="IsList"/> is <see langword="true"/>, the property type must implement the <see cref="IList"/> 
    ///     interface, and the items contained in this list must have a type of <see cref="ListInfo"/>.<see cref="IListInfo.ItemType"/>.
    ///   </para>
    /// </remarks>
    public Type PropertyType
    {
      get { return _propertyInfo.PropertyType; }
    }

    /// <summary> Gets an identifier that uniquely defines this property within its class. </summary>
    /// <value> A <see cref="String"/> by which this property can be found within its <see cref="IBusinessObjectClass"/>. </value>
    public string Identifier
    {
      get { return ShortenName (_propertyInfo.Name); }
    }

    // Truncates the name to the part to the right of the last '.', if any.
    private string ShortenName (string name)
    {
      return name.Substring (name.LastIndexOf ('.') + 1);
    }

    /// <summary> Gets the property name as presented to the user. </summary>
    /// <value> The human readable identifier of this property. </value>
    /// <remarks> The value of this property may depend on the current culture. </remarks>
    public string DisplayName
    {
      get
      {
        if (_reflectedClass == null)
        {
          throw new InvalidOperationException (
              string.Format ("The reflected class for the property '{0}.{1}' is not set.", _propertyInfo.DeclaringType.Name, _propertyInfo.Name));
        }

        return _bindableObjectGlobalizationService.GetPropertyDisplayName (_propertyInfo, TypeAdapter.Create(_reflectedClass.TargetType));
      }
    }

    /// <summary> Gets a flag indicating whether this property is required. </summary>
    /// <value> <see langword="true"/> if this property is required. </value>
    /// <remarks> Setting required properties to <see langword="null"/> may result in an error. </remarks>
    public bool IsRequired
    {
      get { return _isRequired; }
    }

    /// <summary> Indicates whether this property can be accessed by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can access this property. </returns>
    /// <remarks> The result may depend on the class, the user's authorization and/or the instance value. </remarks>
    public bool IsAccessible (IBusinessObject obj)
    {
      // obj can be null

      return _bindablePropertyReadAccessStrategy.CanRead (obj, this);
    }

    public object GetValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      if (_valueGetter == null)
        throw new InvalidOperationException ("Property has no getter.");

      try
      {
        return _valueGetter (obj);
      }
      catch (Exception ex)
      {
        BusinessObjectPropertyAccessException propertyAccessException;
        if (_bindablePropertyReadAccessStrategy.IsPropertyAccessException (obj, this, ex, out propertyAccessException))
          throw propertyAccessException;
        throw;
      }
    }

    public void SetValue (IBusinessObject obj, object value)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      if (_valueSetter == null)
        throw new InvalidOperationException ("Property has no setter.");

      try
      {
        _valueSetter (obj, value);
      }
      catch (Exception ex)
      {
        BusinessObjectPropertyAccessException propertyAccessException;
        if (_bindablePropertyWriteAccessStrategy.IsPropertyAccessException (obj, this, ex, out propertyAccessException))
          throw propertyAccessException;
        throw;
      }
    }

    public bool IsDefaultValue (IBusinessObject obj)
    {
      ArgumentUtility.CheckNotNull ("obj", obj);

      return _defaultValueStrategy.IsDefaultValue (obj, this);
    }

    /// <summary> Indicates whether this property can be modified by the user. </summary>
    /// <param name="obj"> The object to evaluate this property for, or <see langword="null"/>. </param>
    /// <returns> <see langword="true"/> if the user can set this property. </returns>
    /// <remarks> The result may depend on the user's authorization and/or the object. </remarks>
    public bool IsReadOnly (IBusinessObject obj)
    {
      // obj can be null

      if (_isReadOnly)
        return true;

      return !_bindablePropertyWriteAccessStrategy.CanWrite (obj, this);
    }

    /// <summary> Gets the <see cref="BindableObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="BindableObjectProvider"/> type. </value>
    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this property. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type. </value>
    IBusinessObjectProvider IBusinessObjectProperty.BusinessObjectProvider
    {
      get { return BusinessObjectProvider; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectClass"/> that was used to retrieve this property.</summary>
    /// <value>An instance of the <see cref="IBusinessObjectClass"/> type.</value>
    IBusinessObjectClass IBusinessObjectProperty.ReflectedClass
    {
      get { return ReflectedClass; }
    }

    /// <summary>Gets the <see cref="IBusinessObjectClass"/> that was used to retrieve this property.</summary>
    /// <value>An instance of the <see cref="BindableObjectClass"/> type.</value>
    public BindableObjectClass ReflectedClass
    {
      get
      {
        if (_reflectedClass == null)
        {
          throw new InvalidOperationException (
              string.Format (
                  "Accessing the ReflectedClass of a property is invalid until the property has been associated with a class.\r\nProperty '{0}'",
                  Identifier));
        }

        return _reflectedClass;
      }
    }

    public IPropertyInformation PropertyInfo
    {
      get { return _propertyInfo; }
    }

    public virtual object ConvertFromNativePropertyType (object nativeValue)
    {
      return nativeValue;
    }

    public virtual object ConvertToNativePropertyType (object publicValue)
    {
      return publicValue;
    }

    public void SetReflectedClass (BindableObjectClass reflectedClass)
    {
      ArgumentUtility.CheckNotNull ("reflectedClass", reflectedClass);
      if (BusinessObjectProvider != reflectedClass.BusinessObjectProvider)
      {
        throw new ArgumentException (
            string.Format (
                "The BusinessObjectProvider of property '{0}' does not match the BusinessObjectProvider of class '{1}'.",
                Identifier,
                reflectedClass.Identifier),
            "reflectedClass");
      }

      if (_reflectedClass != null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The ReflectedClass of a property cannot be changed after it was assigned.\r\nClass '{0}'\r\nProperty '{1}'",
                _reflectedClass.Identifier,
                Identifier));
      }

      _reflectedClass = reflectedClass;
    }

    protected Type UnderlyingType
    {
      get { return _underlyingType; }
    }

    protected bool IsNullable
    {
      get { return _isNullable; }
    }

    protected BindableObjectGlobalizationService BindableObjectGlobalizationService
    {
      get { return _bindableObjectGlobalizationService; }
    }

    private bool GetNullability ()
    {
      return Nullable.GetUnderlyingType (IsList ? ListInfo.ItemType : PropertyType) != null;
    }
  }
}