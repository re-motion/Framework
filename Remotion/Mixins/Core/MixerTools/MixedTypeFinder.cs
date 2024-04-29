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
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.Mixins.Context;
using Remotion.Utilities;

namespace Remotion.Mixins.MixerTools
{
  // Change to be an ITypeDiscoveryService decorator
  public class MixedTypeFinder : IMixedTypeFinder
  {
    private static readonly ILogger s_logger = LazyLoggerFactory.CreateLogger<MixedTypeFinder>();

    private readonly ITypeDiscoveryService _typeDiscoveryService;

    public MixedTypeFinder (ITypeDiscoveryService typeDiscoveryService)
    {
      ArgumentUtility.CheckNotNull("typeDiscoveryService", typeDiscoveryService);

      _typeDiscoveryService = typeDiscoveryService;
    }

    public ITypeDiscoveryService TypeDiscoveryService
    {
      get { return _typeDiscoveryService; }
    }

    public IEnumerable<Type> FindMixedTypes (MixinConfiguration configuration)
    {
      ArgumentUtility.CheckNotNull("configuration", configuration);

      var types = _typeDiscoveryService.GetTypes(null, false);
      s_logger.LogInformation(
          "Retrieving class contexts for {0} configured mixin targets and {1} loaded types.",
          configuration.ClassContexts.Count,
          types.Count);

      return from t in types.Cast<Type>()
             where !t.IsDefined(typeof(IgnoreForMixinConfigurationAttribute), false)
             let context = configuration.GetContext(t)
             where context != null && !MixinTypeUtility.IsGeneratedConcreteMixedType(t) && ShouldProcessContext(context)
             select t;
    }

    private bool ShouldProcessContext (ClassContext context)
    {
      if (context.Type.IsGenericTypeDefinition)
      {
        s_logger.LogDebug("Type {0} is a generic type definition and is thus ignored.", context.Type);
        return false;
      }

      if (context.Type.IsInterface)
      {
        s_logger.LogDebug("Type {0} is an interface and is thus ignored.", context.Type);
        return false;
      }

      return true;
    }
  }
}
