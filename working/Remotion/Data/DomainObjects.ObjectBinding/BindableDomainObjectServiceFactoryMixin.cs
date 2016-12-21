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
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ObjectBinding
{
  /// <summary>
  /// The <see cref="BindableDomainObjectServiceFactoryMixin"/> is an extension of the <see cref="BindableObjectServiceFactory"/> used by
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
  ///     <term><see cref="IGetObjectService"/></term>
  ///     <description><see cref="BindableDomainObjectGetObjectService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="ISearchAvailableObjectsService"/></term>
  ///     <description><see cref="BindableDomainObjectCompoundSearchService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  [Extends (typeof (BindableObjectServiceFactory))]
  public class BindableDomainObjectServiceFactoryMixin
      : Mixin<BindableObjectServiceFactory, IBusinessObjectServiceFactory>, IBusinessObjectServiceFactory
  {
    public BindableDomainObjectServiceFactoryMixin ()
    {
    }

    [OverrideTarget]
    public virtual IBusinessObjectService CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("serviceType", serviceType, typeof (IBusinessObjectService));

      if (provider.ProviderAttribute is BindableDomainObjectProviderAttribute)
      {
        if (serviceType == typeof (IGetObjectService))
          return new BindableDomainObjectGetObjectService();

        if (serviceType == typeof (ISearchAvailableObjectsService))
          return new BindableDomainObjectCompoundSearchService();
      }

      return Next.CreateService (provider, serviceType);
    }
  }
}
