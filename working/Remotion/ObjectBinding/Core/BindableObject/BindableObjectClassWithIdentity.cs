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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  //TODO: doc
  public class BindableObjectClassWithIdentity : BindableObjectClass, IBusinessObjectClassWithIdentity
  {
    private readonly Type _getObjectServiceType;

    protected BindableObjectClassWithIdentity (
        Type concreteType,
        BindableObjectProvider businessObjectProvider,
        IEnumerable<PropertyBase> properties)
        : this (concreteType, businessObjectProvider, SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>(), properties)
    {
    }

    public BindableObjectClassWithIdentity (
        Type concreteType,
        BindableObjectProvider businessObjectProvider,
        BindableObjectGlobalizationService bindableObjectGlobalizationService,
        IEnumerable<PropertyBase> properties)
        : base (concreteType, businessObjectProvider, bindableObjectGlobalizationService, properties)
    {
      _getObjectServiceType = GetGetObjectServiceType();
    }

    public IBusinessObjectWithIdentity GetObject (string uniqueIdentifier)
    {
      IGetObjectService service = GetGetObjectService();
      return service.GetObject (this, uniqueIdentifier);
    }

    private IGetObjectService GetGetObjectService ()
    {
      var service = (IGetObjectService) BusinessObjectProvider.GetService (_getObjectServiceType);
      if (service == null)
      {
        throw new InvalidOperationException (
            string.Format (
                "The '{0}' required for loading objectes of type '{1}' is not registered with the '{2}' associated with this type.",
                _getObjectServiceType.FullName,
                TargetType.FullName,
                typeof (BusinessObjectProvider).FullName));
      }
      return service;
    }

    private Type GetGetObjectServiceType ()
    {
      var attribute = AttributeUtility.GetCustomAttribute<IBusinessObjectServiceTypeAttribute<IGetObjectService>> (ConcreteType, true);
      if (attribute == null)
        return typeof (IGetObjectService);
      return attribute.Type;
    }
  }
}
