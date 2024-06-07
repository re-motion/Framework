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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Mixins;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  [Extends(typeof(BindableDomainObjectMetadataFactory))]
  public class OrganizationalStructureBindableDomainObjectMetadataFactoryMixin
      : Mixin<BindableDomainObjectMetadataFactory, OrganizationalStructureBindableDomainObjectMetadataFactoryMixin.INext>
  {
    public interface INext
    {
      PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider);
    }

    private class RedirectingDefaultValueStrategy : IDefaultValueStrategy
    {
      private readonly IPropertyInformation _delegatedPropertyInfo;
      private readonly PropertyBase _targetProperty;
      private readonly BindableDomainObjectDefaultValueStrategy _bindableDomainObjectDefaultValueStrategy;

      public RedirectingDefaultValueStrategy (IPropertyInformation delegatedPropertyInfo, PropertyBase targetProperty)
      {
        ArgumentUtility.CheckNotNull("delegatedPropertyInfo", delegatedPropertyInfo);
        ArgumentUtility.CheckNotNull("targetProperty", targetProperty);

        _delegatedPropertyInfo = delegatedPropertyInfo;
        _targetProperty = targetProperty;
        _bindableDomainObjectDefaultValueStrategy = new BindableDomainObjectDefaultValueStrategy();
      }

      public bool IsDefaultValue (IBusinessObject obj, PropertyBase property)
      {
        ArgumentUtility.CheckNotNull("obj", obj);
        ArgumentUtility.CheckNotNull("property", property);

        if (_delegatedPropertyInfo.Equals(property.PropertyInfo.GetOriginalDeclaration()))
          return _bindableDomainObjectDefaultValueStrategy.IsDefaultValue(obj, _targetProperty);

        return _bindableDomainObjectDefaultValueStrategy.IsDefaultValue(obj, property);
      }
    }

    [OverrideTarget]
    public virtual PropertyReflector CreatePropertyReflector (
        Type concreteType,
        IPropertyInformation propertyInfo,
        BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull("concreteType", concreteType);
      ArgumentUtility.CheckNotNull("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull("businessObjectProvider", businessObjectProvider);

      if (concreteType == typeof(Position)
          && propertyInfo.Name == "Delegable"
          && TypeAdapter.Create(typeof(Position)).Equals(propertyInfo.GetOriginalDeclaringType()))
      {
        var delegatedPropertyInfo = propertyInfo.GetOriginalDeclaration();
        var targetPropertyInfo = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty(() => ((Position)null!).Delegation));
        var targetProperty = Next.CreatePropertyReflector(concreteType, targetPropertyInfo, businessObjectProvider).GetMetadata();

        return BindableDomainObjectPropertyReflector.Create(
            propertyInfo,
            businessObjectProvider,
            new Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader.DomainModelConstraintProvider(),
            new RedirectingDefaultValueStrategy(delegatedPropertyInfo, targetProperty));
      }

      return Next.CreatePropertyReflector(concreteType, propertyInfo, businessObjectProvider);
    }
  }
}
