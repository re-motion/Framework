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
using Remotion.SecurityManager.Domain.SearchInfrastructure.Metadata;
using Remotion.SecurityManager.Domain.SearchInfrastructure.OrganizationalStructure;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain
{
  /// <summary>
  /// The <see cref="SecurityManagerObjectServiceFactoryMixin"/> is an extension of the <see cref="BindableObjectServiceFactory"/> used by
  /// the <see cref="BindableObjectProvider"/> and provides default service instances for bindable domain object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="SubstitutionPropertiesSearchService"/></term>
  ///     <description><see cref="SubstitutionPropertiesSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  [CLSCompliant(false)]
  [Extends(typeof(BindableObjectServiceFactory), AdditionalDependencies = new[] { typeof(BindableDomainObjectServiceFactoryMixin) })]
  public class SecurityManagerObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public SecurityManagerObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService? CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("serviceType", serviceType, typeof(IBusinessObjectService));

      if (serviceType == typeof(SubstitutionPropertiesSearchService))
        return new SubstitutionPropertiesSearchService();

      if (serviceType == typeof(RolePropertiesSearchService))
        return new RolePropertiesSearchService();

      if (serviceType == typeof(TenantPropertyTypeSearchService))
        return new TenantPropertyTypeSearchService();

      if (serviceType == typeof(GroupPropertyTypeSearchService))
        return new GroupPropertyTypeSearchService();

      if (serviceType == typeof(UserPropertyTypeSearchService))
        return new UserPropertyTypeSearchService();

      if (serviceType == typeof(PositionPropertyTypeSearchService))
        return new PositionPropertyTypeSearchService();

      if (serviceType == typeof(GroupTypePropertyTypeSearchService))
        return new GroupTypePropertyTypeSearchService();

      if (serviceType == typeof(AbstractRoleDefinitionPropertyTypeSearchService))
        return new AbstractRoleDefinitionPropertyTypeSearchService();

      return Next.CreateService(provider, serviceType);
    }
  }
}
