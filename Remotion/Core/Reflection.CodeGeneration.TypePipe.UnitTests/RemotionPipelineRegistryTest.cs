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
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery;
using Remotion.TypePipe;
using Remotion.TypePipe.TypeAssembly;
using Rhino.Mocks;

namespace Remotion.Reflection.CodeGeneration.TypePipe.UnitTests
{
  [TestFixture]
  public class RemotionPipelineRegistryTest
  {
    private IParticipant[] _participants;

    private IPipeline _defaultPipeline;

    [SetUp]
    public void SetUp ()
    {
      var action = (Action<object, IProxyTypeAssemblyContext>) ((id, ctx) => ctx.ProxyType.AddField ("field", 0, typeof (int)));
      var participantStub = MockRepository.GenerateStub<IParticipant>();
      // Modify proxy type to avoid no-modification optimization.
      participantStub.Stub (_ => _.Participate (Arg<object>.Is.Anything, Arg<IProxyTypeAssemblyContext>.Is.Anything)).Do (action);
      _participants = new[] { participantStub };

      var registry = new RemotionPipelineRegistry (_participants);
      _defaultPipeline = registry.DefaultPipeline;
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_defaultPipeline.ParticipantConfigurationID, Is.EqualTo ("remotion-default-pipeline"));
      Assert.That (_defaultPipeline.Participants, Is.EqualTo (_participants));
    }

    [Test]
    public void Initialization_IntegrationTest_AddsNonApplicationAssemblyAttribute_OnModuleCreation ()
    {
      // Creates new in-memory assembly.
      var type = _defaultPipeline.ReflectionService.GetAssembledType (typeof (RequestedType));

      Assert.That (type, Is.Not.SameAs (typeof (RequestedType)));
      Assert.That (type.Assembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false), Is.True);
    }

    public class RequestedType {}
  }
}