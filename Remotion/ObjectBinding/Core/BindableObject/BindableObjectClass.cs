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
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Impelements <see cref="IBusinessObjectClass"/> for the <b>Bindable Object</b> implementation of <see cref="IBusinessObject"/>.
  /// </summary>
  public class BindableObjectClass : IBusinessObjectClass
  {
    private readonly Type _targetType;
    private readonly Type _concreteType;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly PropertyCollection _properties;
    private readonly BusinessObjectProviderAttribute _businessObjectProviderAttribute;
    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    protected BindableObjectClass (
        Type concreteType,
        BindableObjectProvider businessObjectProvider,
        IEnumerable<PropertyBase> properties)
        : this (concreteType, businessObjectProvider, SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(), properties)
    {
    }

    public BindableObjectClass (
        Type concreteType,
        BindableObjectProvider businessObjectProvider,
        BindableObjectGlobalizationService bindableObjectGlobalizationService,
        IEnumerable<PropertyBase> properties)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      Assertion.IsFalse (concreteType.IsValueType, "mixed types cannot be value types");
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);
      ArgumentUtility.CheckNotNull ("properties", properties);

      _targetType = MixinTypeUtility.GetUnderlyingTargetType (concreteType);
      _concreteType = concreteType;
      _businessObjectProvider = businessObjectProvider;
      _businessObjectProviderAttribute = AttributeUtility.GetCustomAttribute<BusinessObjectProviderAttribute> (concreteType, true);
      _properties = new PropertyCollection (properties);
      _bindableObjectGlobalizationService = bindableObjectGlobalizationService;

      foreach (PropertyBase property in _properties.ToArray())
        property.SetReflectedClass (this);
    }

    /// <summary> Gets the type name as presented to the user. </summary>
    /// <returns> The human readable identifier of this type. </returns>
    /// <remarks> The result of this method may depend on the current culture. </remarks>
    public string GetDisplayName ()
    {
      var type = TypeAdapter.Create (_targetType);
      return _bindableObjectGlobalizationService.GetTypeDisplayName (type, type);
    }

    /// <summary> Returns the <see cref="IBusinessObjectProperty"/> for the passed <paramref name="propertyIdentifier"/>. </summary>
    /// <param name="propertyIdentifier"> 
    ///   A <see cref="String"/> uniquely identifying an <see cref="IBusinessObjectProperty"/> in this <see cref="BindableObjectClass"/>.
    /// </param>
    /// <returns> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> 
    ///   or <see langword="null" /> if the <see cref="IBusinessObjectProperty"/> does not exist on this <see cref="BindableObjectClass"/>. 
    /// </returns>
    public IBusinessObjectProperty GetPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      if (!Properties.Contains (propertyIdentifier))
        return null;

      return Properties[propertyIdentifier];
    }

    /// <summary>
    ///   Returns a flag that indicates whether the <paramref name="propertyIdentifier"/> is valid.
    /// </summary>
    /// <param name="propertyIdentifier">The name of the property.</param>
    /// <returns>
    ///   <see langword="true" /> if a property with the <paramref name="propertyIdentifier"/> exists, otherwise <see langword="false" />.
    /// </returns>
    [Obsolete (
        "Use GetPropertyDefinition (string) instead. If the call returns null, the property does not exist on the BindableObjectClass. (1.13.177.0)")]
    public bool HasPropertyDefinition (string propertyIdentifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyIdentifier", propertyIdentifier);

      return Properties.Contains (propertyIdentifier);
    }

    /// <summary> 
    ///   Returns the <see cref="IBusinessObjectProperty"/> instances defined for this business object class.
    /// </summary>
    /// <returns> An array of <see cref="IBusinessObjectProperty"/> instances.</returns>
    public IBusinessObjectProperty[] GetPropertyDefinitions ()
    {
      return Properties.ToArray();
    }

    /// <summary> Gets the <see cref="IBusinessObjectProvider"/> for this business object class. </summary>
    /// <value> An instance of the <see cref="IBusinessObjectProvider"/> type.</value>
    public IBusinessObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary>
    ///   Gets a flag that specifies whether a referenced object of this business object class needs to be 
    ///   written back to its container if some of its values have changed.
    /// </summary>
    /// <value> <see langword="true"/> if the <see cref="IBusinessObject"/> must be reassigned to its container. </value>
    /// <example>
    ///   The following pseudo code shows how this value affects the binding behaviour.
    ///   <code><![CDATA[
    ///   Address address = person.Address;
    ///   address.City = "Vienna";
    ///   // the RequiresWriteBack property of the 'Address' business object class specifies 
    ///   // whether the following statement is required:
    ///   person.Address = address;
    ///   ]]></code>
    /// </example>
    public bool RequiresWriteBack
    {
      get { return false; }
    }

    /// <summary> Gets the identifier (i.e. the type name) for this business object class. </summary>
    /// <value> 
    ///   A string that uniquely identifies the business object class within the business object model. 
    /// </value>
    public string Identifier
    {
      get { return TypeUtility.GetPartialAssemblyQualifiedName (_targetType); }
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public BusinessObjectProviderAttribute BusinessObjectProviderAttribute
    {
      get { return _businessObjectProviderAttribute; }
    }

    protected BindableObjectGlobalizationService BindableObjectGlobalizationService
    {
      get { return _bindableObjectGlobalizationService; }
    }

    /// <summary>
    /// Gets the <see cref="PropertyCollection"/> containing the properties for this <see cref="BindableObjectClass"/>.
    /// </summary>
    protected virtual PropertyCollection Properties
    {
      get { return _properties; }
    }
  }
}