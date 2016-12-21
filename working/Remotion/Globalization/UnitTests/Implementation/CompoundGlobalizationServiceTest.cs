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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class CompoundGlobalizationServiceTest
  {
    private MockRepository _mockRepository;
    private CompoundGlobalizationService _service;
    private IGlobalizationService _innerService1;
    private IGlobalizationService _innerService2;
    private IGlobalizationService _innerService3;
    private ITypeInformation _typeInformationStub;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();

      _innerService1 = _mockRepository.StrictMock<IGlobalizationService>();
      _innerService2 = _mockRepository.StrictMock<IGlobalizationService>();
      _innerService3 = _mockRepository.StrictMock<IGlobalizationService>();

      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();

      _service = new CompoundGlobalizationService (new[] { _innerService1, _innerService2, _innerService3 });
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_service.GlobalizationServices, Is.EqualTo (new[] { _innerService3, _innerService2, _innerService1 }));
    }

    [Test]
    public void GetResourceManager ()
    {
      var resourceManagerStub1 = MockRepository.GenerateStub<IResourceManager>();
      var resourceManagerStub2 = MockRepository.GenerateStub<IResourceManager>();
      var resourceManagerStub3 = MockRepository.GenerateStub<IResourceManager>();

      using (_mockRepository.Ordered())
      {
        _innerService3.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (resourceManagerStub1);
        _innerService2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (resourceManagerStub2);
        _innerService1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (resourceManagerStub3);
      }
      _mockRepository.ReplayAll();

      var result = _service.GetResourceManager (_typeInformationStub);

      Assert.That (result, Is.TypeOf (typeof (ResourceManagerSet)));
      Assert.That (((ResourceManagerSet) result).ResourceManagers, Is.EqualTo (new[] { resourceManagerStub1, resourceManagerStub2, resourceManagerStub3 }));

      _mockRepository.VerifyAll();
    }
  }
}