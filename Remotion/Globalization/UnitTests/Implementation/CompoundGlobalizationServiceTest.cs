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
using Moq;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundGlobalizationServiceTest
  {
    private CompoundGlobalizationService _service;
    private Mock<IGlobalizationService> _innerService1;
    private Mock<IGlobalizationService> _innerService2;
    private Mock<IGlobalizationService> _innerService3;
    private Mock<ITypeInformation> _typeInformationStub;

    [SetUp]
    public void SetUp ()
    {
      _innerService1 = new Mock<IGlobalizationService>(MockBehavior.Strict);
      _innerService2 = new Mock<IGlobalizationService>(MockBehavior.Strict);
      _innerService3 = new Mock<IGlobalizationService>(MockBehavior.Strict);

      _typeInformationStub = new Mock<ITypeInformation>();

      _service = new CompoundGlobalizationService(new[] { _innerService1.Object, _innerService2.Object, _innerService3.Object });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_service.GlobalizationServices, Is.EqualTo(new[] { _innerService3.Object, _innerService2.Object, _innerService1.Object }));
    }

    [Test]
    public void GetResourceManager ()
    {
      var resourceManagerStub1 = new Mock<IResourceManager>();
      var resourceManagerStub2 = new Mock<IResourceManager>();
      var resourceManagerStub3 = new Mock<IResourceManager>();

      var sequence = new MockSequence();
      _innerService3.InSequence(sequence).Setup(mock => mock.GetResourceManager(_typeInformationStub.Object)).Returns(resourceManagerStub1.Object).Verifiable();
      _innerService2.InSequence(sequence).Setup(mock => mock.GetResourceManager(_typeInformationStub.Object)).Returns(resourceManagerStub2.Object).Verifiable();
      _innerService1.InSequence(sequence).Setup(mock => mock.GetResourceManager(_typeInformationStub.Object)).Returns(resourceManagerStub3.Object).Verifiable();

      var result = _service.GetResourceManager(_typeInformationStub.Object);

      Assert.That(result, Is.TypeOf(typeof(ResourceManagerSet)));
      Assert.That(((ResourceManagerSet)result).ResourceManagers, Is.EqualTo(new[] { resourceManagerStub1.Object, resourceManagerStub2.Object, resourceManagerStub3.Object }));

      _innerService1.Verify();
      _innerService2.Verify();
      _innerService3.Verify();
    }
  }
}
