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
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.ServiceLocation
{
  [TestFixture]
  public class ParticipantRegistrationTest
  {
    [Test]
    public void DefaultServiceLocator_ReturnsMixinParticipant ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      var participants = serviceLocator.GetAllInstances<IParticipant>().ToArray();
      Assert.That (participants.Select (p => p.GetType()), Is.EqualTo (new[] { typeof (MixinParticipant) }));
    }

    [Test]
    public void DefaultServiceConfigurationDiscoveryService_ReturnsMixinParticpant ()
    {
      var discoveryService = DefaultServiceConfigurationDiscoveryService.Create();
      var participantService = discoveryService.GetDefaultConfiguration(typeof (IParticipant));

      Assert.That (participantService, Is.Not.Null);
      Assert.That (participantService.ImplementationInfos.Select (i => i.ImplementationType), Is.EqualTo (new[] { typeof (MixinParticipant) }));
    }
  }
}