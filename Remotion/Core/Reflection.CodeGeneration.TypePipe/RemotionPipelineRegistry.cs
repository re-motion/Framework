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
using Remotion.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.TypePipe
{
  /// <summary>
  /// Creates and registers a <see cref="IPipelineRegistry.DefaultPipeline"/> containing the specified participants.
  /// Uses the <see cref="RemotionPipelineFactory"/> which creates pipeline instances that immediately apply the
  /// <see cref="NonApplicationAssemblyAttribute"/> to the in-memory assembly in order to retain original re-mix behavior.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor(typeof(IPipelineRegistry), Lifetime = LifetimeKind.Singleton)]
  public class RemotionPipelineRegistry : DefaultPipelineRegistry
  {
    private static IPipeline CreateDefaultPipeline (IEnumerable<IParticipant> defaultPipelineParticipants)
    {
      var remotionPipelineFactory = new RemotionPipelineFactory();
      // We don't resolve the settings from IoC here as this the "remotion-default-pipeline" and customization is not needed at this point
      var settings = new PipelineSettingsProvider().GetSettings();
      return remotionPipelineFactory.Create("remotion-default-pipeline", settings, defaultPipelineParticipants);
    }

    public RemotionPipelineRegistry (IEnumerable<IParticipant> defaultPipelineParticipants)
        : base(CreateDefaultPipeline(ArgumentUtility.CheckNotNull("defaultPipelineParticipants", defaultPipelineParticipants)))
    {
    }
  }
}
