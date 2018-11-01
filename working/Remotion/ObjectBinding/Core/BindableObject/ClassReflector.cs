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
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="ClassReflector"/> is used to create an instance of type <see cref="BindableObjectClass"/> for a <see cref="Type"/>.
  /// </summary>
  public class ClassReflector : IClassReflector
  {
    private readonly Type _targetType;
    private readonly Type _concreteType;
    private readonly BindableObjectProvider _businessObjectProvider;
    private readonly IMetadataFactory _metadataFactory;
    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    public ClassReflector (
        Type targetType,
        BindableObjectProvider businessObjectProvider,
        IMetadataFactory metadataFactory,
        BindableObjectGlobalizationService bindableObjectGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("metadataFactory", metadataFactory);
      ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);

      _targetType = targetType;
      _concreteType = BindableObjectProvider.GetConcreteTypeForBindableObjectImplementation (_targetType);
      _businessObjectProvider = businessObjectProvider;
      _metadataFactory = metadataFactory;
      _bindableObjectGlobalizationService = bindableObjectGlobalizationService;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    /// <summary>
    /// Builds the <see cref="BindableObjectClass"/> for the provided metadata (<see cref="TargetType"/>) and the properties calculated with
    /// <see cref="GetProperties"/>.
    /// </summary>
    public virtual BindableObjectClass GetMetadata ()
    {
      if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (_concreteType))
        return new BindableObjectClassWithIdentity (_concreteType, _businessObjectProvider, _bindableObjectGlobalizationService, GetProperties ());

      if (typeof (IBusinessObject).IsAssignableFrom (_concreteType))
        return new BindableObjectClass (_concreteType, _businessObjectProvider, _bindableObjectGlobalizationService, GetProperties());

      throw new NotSupportedException (
          string.Format ("Type '{0}' does not implement the required IBusinessObject interface.", _concreteType.FullName));
    }

    protected IEnumerable<PropertyBase> GetProperties ()
    {
      IPropertyFinder propertyFinder = _metadataFactory.CreatePropertyFinder (_concreteType);

      Dictionary<string, PropertyBase> propertiesByName = new Dictionary<string, PropertyBase>();
      foreach (IPropertyInformation propertyInfo in propertyFinder.GetPropertyInfos())
      {
        PropertyReflector propertyReflector = _metadataFactory.CreatePropertyReflector (_concreteType, propertyInfo, _businessObjectProvider);
        PropertyBase property = propertyReflector.GetMetadata();
        if (propertiesByName.ContainsKey (property.Identifier))
        {
          string message = string.Format (
              "Type '{0}' has two properties called '{1}', this is currently not supported.",
              TargetType.FullName,
              property.Identifier);
          throw new NotSupportedException (message);
        }
        else
          propertiesByName.Add (property.Identifier, property);
      }

      return propertiesByName.Values;
    }
  }
}
