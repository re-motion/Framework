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
using Remotion.TypePipe;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// The <see cref="BindableObjectServiceFactory"/> is the default implementation of the <see cref="IBusinessObjectServiceFactory"/>
  /// and provides service instances common for all bindable object implementations.
  /// </summary>
  /// <remarks>
  /// The following <see cref="IBusinessObjectService"/> interfaces are supported.
  /// <list type="bullet">
  ///   <listheader>
  ///     <term>Service Interface</term>
  ///     <description>Service creates instance of type</description>
  ///   </listheader>
  ///   <item>
  ///     <term><see cref="BindableObjectGlobalizationService"/></term>
  ///     <description><see cref="BindableObjectGlobalizationService"/></description>
  ///   </item>
  ///   <item>
  ///     <term><see cref="IBusinessObjectStringFormatterService"/></term>
  ///     <description><see cref="BusinessObjectStringFormatterService"/></description>
  ///   </item>
  /// </list>
  /// </remarks>
  public class BindableObjectServiceFactory : IBusinessObjectServiceFactory
  {
    public static BindableObjectServiceFactory Create ()
    {
      return ObjectFactory.Create<BindableObjectServiceFactory>(true, ParamList.Empty);
    }

    protected BindableObjectServiceFactory ()
    {
    }

    public virtual IBusinessObjectService? CreateService (IBusinessObjectProviderWithIdentity provider, Type serviceType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("serviceType", serviceType, typeof(IBusinessObjectService));

      if (serviceType == typeof(IBusinessObjectStringFormatterService))
        return new BusinessObjectStringFormatterService();

      return null;
    }
  }
}
