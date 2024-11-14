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
using Remotion.Reflection.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Implementation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.UnitTests
{
  /// <summary>
  /// Adapts the default <see cref="RemotionPipelineRegistry"/> to enable <see cref="PipelineSettings"/>.<see cref="PipelineSettings.EnableSerializationWithoutAssemblySaving"/>.
  /// </summary>
  [ImplementationFor(typeof(IPipelineRegistry), RegistrationType = RegistrationType.Decorator)]
  public class RemotionTestsPipelineRegistry : DefaultPipelineRegistry
  {
    private static IPipeline CreateDefaultPipeline (IEnumerable<IParticipant> defaultPipelineParticipants)
    {
      var remotionPipelineFactory = new RemotionPipelineFactory();
      var settings = PipelineSettings.From(new PipelineSettingsProvider().GetSettings())
          .Build();
      return remotionPipelineFactory.Create("remotion-test-pipeline", settings, defaultPipelineParticipants);
    }

    public RemotionTestsPipelineRegistry (IPipelineRegistry _, IEnumerable<IParticipant> defaultPipelineParticipants)
        : base(CreateDefaultPipeline(ArgumentUtility.CheckNotNull("defaultPipelineParticipants", defaultPipelineParticipants)))
    {
      // Throw away the decorated pipeline registry instance since we want to override it entirely
    }
  }
}
