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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using ClassReflector = Remotion.ObjectBinding.BindableObject.ClassReflector;
using ParamList = Remotion.TypePipe.ParamList;
using PropertyReflector = Remotion.ObjectBinding.BindableObject.PropertyReflector;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectMetadataFactory"/> implements the <see cref="IMetadataFactory"/> interface for domain objects.
  /// </summary>
  public class BindableDomainObjectMetadataFactory : IMetadataFactory
  {
    public static BindableDomainObjectMetadataFactory Create ()
    {
      return ObjectFactory.Create<BindableDomainObjectMetadataFactory> (true, ParamList.Empty);
    }

    private readonly BindableObjectGlobalizationService _bindableObjectGlobalizationService;

    protected BindableDomainObjectMetadataFactory ()
      : this (SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>())
    {
    }

    public BindableDomainObjectMetadataFactory (BindableObjectGlobalizationService bindableObjectGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("bindableObjectGlobalizationService", bindableObjectGlobalizationService);

      _bindableObjectGlobalizationService = bindableObjectGlobalizationService;
    }

    public virtual IClassReflector CreateClassReflector (Type targetType, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new ClassReflector (targetType, businessObjectProvider, this, _bindableObjectGlobalizationService);
    }

    public virtual IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      return new BindableDomainObjectPropertyFinder (concreteType);
    }

    public virtual PropertyReflector CreatePropertyReflector (
        Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return BindableDomainObjectPropertyReflector.Create (
          propertyInfo,
          businessObjectProvider,
          new DomainModelConstraintProvider(),
          new BindableDomainObjectDefaultValueStrategy());
    }
  }
}