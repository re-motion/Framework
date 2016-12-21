// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
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
  [CLSCompliant (false)]
  [Extends (typeof (BindableObjectServiceFactory), AdditionalDependencies = new[] { typeof (BindableDomainObjectServiceFactoryMixin) })]
  public class SecurityManagerObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public SecurityManagerObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (serviceType == typeof (SubstitutionPropertiesSearchService))
        return new SubstitutionPropertiesSearchService();

      if (serviceType == typeof (RolePropertiesSearchService))
        return new RolePropertiesSearchService();

      if (serviceType == typeof (TenantPropertyTypeSearchService))
        return new TenantPropertyTypeSearchService();

      if (serviceType == typeof (GroupPropertyTypeSearchService))
        return new GroupPropertyTypeSearchService();

      if (serviceType == typeof (UserPropertyTypeSearchService))
        return new UserPropertyTypeSearchService();

      if (serviceType == typeof (PositionPropertyTypeSearchService))
        return new PositionPropertyTypeSearchService();

      if (serviceType == typeof (GroupTypePropertyTypeSearchService))
        return new GroupTypePropertyTypeSearchService();

      if (serviceType == typeof (AbstractRoleDefinitionPropertyTypeSearchService))
        return new AbstractRoleDefinitionPropertyTypeSearchService();

      return Next.CreateService (provider, serviceType);
    }
  }
}