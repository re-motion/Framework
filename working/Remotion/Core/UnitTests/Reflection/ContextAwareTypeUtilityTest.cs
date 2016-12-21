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
using NUnit.Framework;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Configuration.TypeResolution;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Reflection.TypeResolution;
using Remotion.UnitTests.Configuration.TypeDiscovery;
using Remotion.UnitTests.Design;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ContextAwareTypeUtilityTest
  {
    private ITypeDiscoveryService _oldTypeDiscoveryService;
    private TypeDiscoveryConfiguration _oldTypeDiscoveryConfiguration;
    private ITypeResolutionService _oldTypeResolutionService;
    private TypeResolutionConfiguration _oldTypeResolutionConfiguration;

    [SetUp]
    public void SetUp ()
    {
      _oldTypeDiscoveryService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      _oldTypeDiscoveryConfiguration = TypeDiscoveryConfiguration.Current;
      _oldTypeResolutionService = ContextAwareTypeUtility.GetTypeResolutionService();
      _oldTypeResolutionConfiguration = TypeResolutionConfiguration.Current;

      PrivateInvoke.SetNonPublicStaticField (
          typeof (ContextAwareTypeUtility),
          "s_defaultTypeDiscoveryService",
          new Lazy<ITypeDiscoveryService> (() => TypeDiscoveryConfiguration.Current.CreateTypeDiscoveryService()));
      PrivateInvoke.SetNonPublicStaticField (
          typeof (ContextAwareTypeUtility),
          "s_defaultTypeResolutionService",
          new Lazy<ITypeResolutionService> (() => TypeResolutionConfiguration.Current.CreateTypeResolutionService()));
      DesignerUtility.ClearDesignMode();
      TypeDiscoveryConfiguration.SetCurrent (new TypeDiscoveryConfiguration());
      TypeResolutionConfiguration.SetCurrent (new TypeResolutionConfiguration (new DefaultTypeResolutionService()));
    }

    [TearDown]
    public void TearDown ()
    {
      PrivateInvoke.SetNonPublicStaticField (
          typeof (ContextAwareTypeUtility),
          "s_defaultTypeDiscoveryService",
          new Lazy<ITypeDiscoveryService> (() => _oldTypeDiscoveryService));
      PrivateInvoke.SetNonPublicStaticField (
          typeof (ContextAwareTypeUtility),
          "s_defaultTypeResolutionService",
          new Lazy<ITypeResolutionService> (() => _oldTypeResolutionService));
      DesignerUtility.ClearDesignMode();

      TypeDiscoveryConfiguration.SetCurrent (_oldTypeDiscoveryConfiguration);
      TypeResolutionConfiguration.SetCurrent (_oldTypeResolutionConfiguration);
    }

    [Test]
    public void GetTypeDiscoveryService_ComesFromConfiguration ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof (FakeTypeDiscoveryService);

      var defaultService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      Assert.That (defaultService, Is.InstanceOf (typeof (FakeTypeDiscoveryService)));
    }

    [Test]
    public void GetTypeDiscoveryService_Cached ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof (FakeTypeDiscoveryService);

      var defaultService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      var defaultService2 = ContextAwareTypeUtility.GetTypeDiscoveryService();

      Assert.That (defaultService, Is.SameAs (defaultService2));
    }

    [Test]
    public void GetTypeDiscoveryService_DesignModeContext ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      var designerHostMock = MockRepository.GenerateStub<IDesignerHost>();
      designerHostMock.Expect (_ => _.GetService (typeof (ITypeDiscoveryService))).Return (typeDiscoveryServiceStub);

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeUtility.GetTypeDiscoveryService(), Is.SameAs (typeDiscoveryServiceStub));

      designerHostMock.VerifyAllExpectations();
    }

    [Test]
    public void GetTypeResolutionService_ComesFromConfiguration ()
    {
      var typeResolutionServiceStub = MockRepository.GenerateStub<ITypeResolutionService>();
      TypeResolutionConfiguration.SetCurrent (new TypeResolutionConfiguration (typeResolutionServiceStub));

      var defaultService = ContextAwareTypeUtility.GetTypeResolutionService();
      Assert.That (defaultService, Is.SameAs (typeResolutionServiceStub));
    }

    [Test]
    public void GetTypeResolutionService_Cached ()
    {
      TypeResolutionConfiguration.SetCurrent (new TypeResolutionConfiguration (new DefaultTypeResolutionService()));
      var defaultService = ContextAwareTypeUtility.GetTypeResolutionService();

      TypeResolutionConfiguration.SetCurrent (new TypeResolutionConfiguration (new DefaultTypeResolutionService()));
      var defaultService2 = ContextAwareTypeUtility.GetTypeResolutionService();

      Assert.That (defaultService, Is.SameAs (defaultService2));
    }

    [Test]
    public void GetTypeResolutionService_DesignModeContext ()
    {
      var typeResolutionServiceStub = MockRepository.GenerateStub<ITypeResolutionService>();
      var designerHostMock = MockRepository.GenerateStub<IDesignerHost>();
      designerHostMock.Expect (_ => _.GetService (typeof (ITypeResolutionService))).Return (typeResolutionServiceStub);

      DesignerUtility.SetDesignMode (new StubDesignModeHelper (designerHostMock));
      Assert.That (ContextAwareTypeUtility.GetTypeResolutionService(), Is.SameAs (typeResolutionServiceStub));

      designerHostMock.VerifyAllExpectations();
    }
  }
}
