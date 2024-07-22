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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Provides a base class for mixins introducing <see cref="IBusinessObject"/> to arbitrary target classes.
  /// </summary>
  /// <typeparam name="TBindableObject">The bindable object type to be used as the target class of this mixin.</typeparam>
  /// <remarks>
  /// The default mixin derived from this class is <see cref="BindableObjectMixin"/>, but a custom implementation exists for OPF's domain objects.
  /// </remarks>
  [Serializable]
  public abstract class BindableObjectMixinBase<TBindableObject> : Mixin<TBindableObject>, IBusinessObject
      where TBindableObject: class
  {
    [NonSerialized]
    private MixinConfiguration _mixinConfigurationAtInstantiationTime = null!;
    [NonSerialized]
    private BindableObjectProvider _bindableObjectProvider = null!;
    [NonSerialized]
    private DoubleCheckedLockingContainer<BindableObjectClass> _bindableObjectClass = null!;

    protected abstract Type GetTypeForBindableObjectClass ();

    /// <overloads> Gets the value accessed through the specified property. </overloads>
    /// <summary> Gets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> The <see cref="IBusinessObjectProperty"/> used to access the value. </param>
    /// <returns> The property value for the <paramref name="property"/> parameter. </returns>
    /// <exception cref="Exception">
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public object? GetProperty (IBusinessObjectProperty property)
    {
      var propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase>("property", property);

      object nativeValue = propertyBase.GetValue((IBusinessObject)Target);

      if (!propertyBase.IsList && propertyBase.IsDefaultValue(((IBusinessObject)Target)))
        return null;
      else
        return propertyBase.ConvertFromNativePropertyType(nativeValue);
    }

    /// <overloads> Sets the value accessed through the specified property. </overloads>
    /// <summary> Sets the value accessed through the specified <see cref="IBusinessObjectProperty"/>. </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="value"> The new value for the <paramref name="property"/> parameter. </param>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public void SetProperty (IBusinessObjectProperty property, object? value)
    {
      var propertyBase = ArgumentUtility.CheckNotNullAndType<PropertyBase>("property", property);

      object? nativeValue = propertyBase.ConvertToNativePropertyType(value);

      propertyBase.SetValue((IBusinessObject)Target, nativeValue);
    }

    /// <summary> 
    ///   Gets the formatted string representation of the value accessed through the specified 
    ///   <see cref="IBusinessObjectProperty"/>.
    /// </summary>
    /// <param name="property"> 
    ///   The <see cref="IBusinessObjectProperty"/> used to access the value. Must not be <see langword="null"/>.
    /// </param>
    /// <param name="format"> The format string applied by the <b>ToString</b> method. </param>
    /// <returns> The string representation of the property value for the <paramref name="property"/> parameter.  </returns>
    /// <exception cref="Exception"> 
    ///   Thrown if the <paramref name="property"/> is not part of this business object's class. 
    /// </exception>
    public string GetPropertyString (IBusinessObjectProperty property, string? format)
    {
      var stringFormatterService =
          (IBusinessObjectStringFormatterService?)BusinessObjectClass.BusinessObjectProvider.GetService(typeof(IBusinessObjectStringFormatterService));

      Assertion.IsNotNull(stringFormatterService, "An implementation of {0} must be available.", nameof(IBusinessObjectStringFormatterService));

      return stringFormatterService.GetPropertyString((IBusinessObject)Target, property, format);
    }

    /// <summary> Gets the <see cref="BindableObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="BindableObjectClass"/> instance acting as the business object's type. </value>
    public BindableObjectClass BusinessObjectClass
    {
      get { return _bindableObjectClass.Value; }
    }

    /// <summary> Gets the <see cref="IBusinessObjectClass"/> of this business object. </summary>
    /// <value> An <see cref="IBusinessObjectClass"/> instance acting as the business object's type. </value>
    IBusinessObjectClass IBusinessObject.BusinessObjectClass
    {
      get { return BusinessObjectClass; }
    }

    protected override void OnInitialized ()
    {
      base.OnInitialized();

      var typeForBindableObjectClass = GetTypeForBindableObjectClass();
      _mixinConfigurationAtInstantiationTime = MixinConfiguration.ActiveConfiguration;
      _bindableObjectProvider = BindableObjectProvider.GetProviderForBindableObjectType(typeForBindableObjectClass);
      _bindableObjectClass = new DoubleCheckedLockingContainer<BindableObjectClass>(InitializeBindableObjectClass);
    }

    private BindableObjectClass InitializeBindableObjectClass ()
    {
      // reactivate the mixin configuration to get the bindable object class originally expected
      using (_mixinConfigurationAtInstantiationTime.EnterScope())
      {
        var targetType = GetTypeForBindableObjectClass();
        return _bindableObjectProvider.GetBindableObjectClass(targetType);
      }
    }
  }
}
