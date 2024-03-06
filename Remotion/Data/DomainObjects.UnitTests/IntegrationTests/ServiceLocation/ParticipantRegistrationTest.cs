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
using System.ComponentModel.Design;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests.ServiceLocation
{
  [TestFixture]
  public class ParticipantRegistrationTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void DefaultServiceLocator_ReturnsDomainObjectParticipant ()
    {
      var participants = _serviceLocator.GetAllInstances<IParticipant>().ToArray();
      Assert.That(participants.Select(p => p.GetType()), Has.Member(typeof(DomainObjectParticipant)));
    }

    [Test]
    public void DefaultServiceLocator_OrdersDomainObjectParticipantAfterMixinParticipant ()
    {
      var participants = _serviceLocator.GetAllInstances<IParticipant>().ToArray();
      Assert.That(participants.Select(p => p.GetType()), Is.EqualTo(new[] { typeof(MixinParticipant), typeof(DomainObjectParticipant) }));
    }

    [Test]
    public void DefaultServiceConfigurationDiscoveryService_ReturnsDomainObjectParticipant ()
    {
      var typeDiscoveryService = _serviceLocator.GetInstance<ITypeDiscoveryService>();
      var defaultServiceConfigurationDiscoveryService = new DefaultServiceConfigurationDiscoveryService(typeDiscoveryService);
      var services = defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration();
      var participantService = services.SingleOrDefault(s => s.ServiceType == typeof(IParticipant));

      Assert.That(participantService, Is.Not.Null);
      Assert.That(participantService.ImplementationInfos.Select(i => i.ImplementationType), Has.Member(typeof(DomainObjectParticipant)));
    }

    [Test]
    public void DefaultServiceConfigurationDiscoveryService_ReturnsMixinParticipant ()
    {
      var typeDiscoveryService = _serviceLocator.GetInstance<ITypeDiscoveryService>();
      var defaultServiceConfigurationDiscoveryService = new DefaultServiceConfigurationDiscoveryService(typeDiscoveryService);
      var services = defaultServiceConfigurationDiscoveryService.GetDefaultConfiguration();
      var participantService = services.SingleOrDefault(s => s.ServiceType == typeof(IParticipant));

      Assert.That(participantService, Is.Not.Null);
      Assert.That(
          participantService.ImplementationInfos.Select(i => i.ImplementationType),
          Is.EqualTo(new[] { typeof(MixinParticipant), typeof(DomainObjectParticipant) }));
    }
  }
}
