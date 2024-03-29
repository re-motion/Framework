﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IConfigurationProvider), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Single)]
  public class ConfigurationProvider : IConfigurationProvider
  {
    public ConfigurationProvider ()
    {
    }

    public TargetClassDefinition? GetTargetClassDefinition (ClassContext? classContext)
    {
      if (classContext == null)
        return null;

      return TargetClassDefinitionFactory.CreateAndValidate(classContext);
    }

    public TargetClassDefinition? GetTargetClassDefinition (Type requestedType)
    {
      ArgumentUtility.CheckNotNull("requestedType", requestedType);

      var classContext = MixinConfiguration.ActiveConfiguration.GetContext(requestedType);
      return GetTargetClassDefinition(classContext);
    }

    public IEnumerable<Type> GetInterfacesToImplement (TargetClassDefinition targetClassDefinition, IEnumerable<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull("mixinInfos", mixinInfos);

      var implementedInterfaceFinder = new ImplementedInterfaceFinder(
          targetClassDefinition.ImplementedInterfaces,
          targetClassDefinition.ReceivedInterfaces,
          targetClassDefinition.RequiredTargetCallTypes,
          mixinInfos);

      return implementedInterfaceFinder.GetInterfacesToImplement();
    }
  }
}
