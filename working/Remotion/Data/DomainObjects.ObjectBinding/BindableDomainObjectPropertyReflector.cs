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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using ParamList = Remotion.TypePipe.ParamList;
using PropertyReflector = Remotion.ObjectBinding.BindableObject.PropertyReflector;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// Use the <see cref="BindableDomainObjectPropertyReflector"/> to create <see cref="IBusinessObjectProperty"/> implementations for the 
  /// bindable domain object extension of the business object interfaces.
  /// </summary>
  public class BindableDomainObjectPropertyReflector : PropertyReflector
  {
    public static BindableDomainObjectPropertyReflector Create (
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IDefaultValueStrategy defaultValueStrategy)
    {
      return ObjectFactory.Create<BindableDomainObjectPropertyReflector> (
          true,
          ParamList.Create (propertyInfo, businessObjectProvider, domainModelConstraintProvider, defaultValueStrategy));
    }

    private readonly IDomainModelConstraintProvider _domainModelConstraintProvider;

    protected BindableDomainObjectPropertyReflector (
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IDefaultValueStrategy defaultValueStrategy)
        : this (propertyInfo,
            businessObjectProvider,
            domainModelConstraintProvider,
            defaultValueStrategy,
            SafeServiceLocator.Current.GetInstance<IBindablePropertyReadAccessStrategy>(),
            SafeServiceLocator.Current.GetInstance<IBindablePropertyWriteAccessStrategy>(),
            SafeServiceLocator.Current.GetInstance<BindableObjectGlobalizationService>())
    {
    }

    public BindableDomainObjectPropertyReflector (
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider,
        IDomainModelConstraintProvider domainModelConstraintProvider,
        IDefaultValueStrategy defaultValueStrategy,
        IBindablePropertyReadAccessStrategy bindablePropertyReadAccessStrategy,
        IBindablePropertyWriteAccessStrategy bindablePropertyWriteAccessStrategy,
        BindableObjectGlobalizationService bindableObjectGlobalizationService)
        : base (
            propertyInfo,
            businessObjectProvider,
            defaultValueStrategy,
            bindablePropertyReadAccessStrategy,
            bindablePropertyWriteAccessStrategy,
            bindableObjectGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("domainModelConstraintProvider", domainModelConstraintProvider);

      _domainModelConstraintProvider = domainModelConstraintProvider;
    }

    protected override bool GetIsRequired ()
    {
      if (base.GetIsRequired())
        return true;
      return !_domainModelConstraintProvider.IsNullable (PropertyInfo);
    }

    protected override int? GetMaxLength ()
    {
      return base.GetMaxLength() ?? _domainModelConstraintProvider.GetMaxLength (PropertyInfo);
    }
  }
}