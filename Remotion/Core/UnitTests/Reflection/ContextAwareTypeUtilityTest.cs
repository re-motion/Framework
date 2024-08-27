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
using System.IO;
using Moq;
using NUnit.Framework;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Configuration.TypeResolution;
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Reflection.TypeResolution;
using Remotion.UnitTests.Configuration.TypeDiscovery;
using Remotion.Utilities;
using Remotion.Development.UnitTesting.IsolatedCodeRunner;

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

      var fields = PrivateInvoke.GetNonPublicStaticField(typeof(ContextAwareTypeUtility), "s_fields");
      Assertion.IsNotNull(fields);
      PrivateInvoke.SetPublicField(
          fields,
          "DefaultTypeDiscoveryService",
          new Lazy<ITypeDiscoveryService>(() => TypeDiscoveryConfiguration.Current.CreateTypeDiscoveryService()));
      PrivateInvoke.SetPublicField(
          fields,
          "DefaultTypeResolutionService",
          new Lazy<ITypeResolutionService>(() => TypeResolutionConfiguration.Current.CreateTypeResolutionService()));
      TypeDiscoveryConfiguration.SetCurrent(new TypeDiscoveryConfiguration());
      TypeResolutionConfiguration.SetCurrent(new TypeResolutionConfiguration(new DefaultTypeResolutionService()));
    }

    [TearDown]
    public void TearDown ()
    {
      var fields = PrivateInvoke.GetNonPublicStaticField(typeof(ContextAwareTypeUtility), "s_fields");
      Assertion.IsNotNull(fields);
      PrivateInvoke.SetPublicField(
          fields,
          "DefaultTypeDiscoveryService",
          new Lazy<ITypeDiscoveryService>(() => _oldTypeDiscoveryService));
      PrivateInvoke.SetPublicField(
          fields,
          "DefaultTypeResolutionService",
          new Lazy<ITypeResolutionService>(() => _oldTypeResolutionService));

      TypeDiscoveryConfiguration.SetCurrent(_oldTypeDiscoveryConfiguration);
      TypeResolutionConfiguration.SetCurrent(_oldTypeResolutionConfiguration);
    }

    [Test]
    public void GetTypeDiscoveryService_ComesFromConfiguration ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof(FakeTypeDiscoveryService);

      var defaultService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      Assert.That(defaultService, Is.InstanceOf(typeof(FakeTypeDiscoveryService)));
    }

    [Test]
    public void GetTypeDiscoveryService_Cached ()
    {
      TypeDiscoveryConfiguration.Current.Mode = TypeDiscoveryMode.CustomTypeDiscoveryService;
      TypeDiscoveryConfiguration.Current.CustomTypeDiscoveryService.Type = typeof(FakeTypeDiscoveryService);

      var defaultService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      var defaultService2 = ContextAwareTypeUtility.GetTypeDiscoveryService();

      Assert.That(defaultService, Is.SameAs(defaultService2));
    }

    [Test]
    public void GetTypeResolutionService_ComesFromConfiguration ()
    {
      var typeResolutionServiceStub = new Mock<ITypeResolutionService>();
      TypeResolutionConfiguration.SetCurrent(new TypeResolutionConfiguration(typeResolutionServiceStub.Object));

      var defaultService = ContextAwareTypeUtility.GetTypeResolutionService();
      Assert.That(defaultService, Is.SameAs(typeResolutionServiceStub.Object));
    }

    [Test]
    public void GetTypeResolutionService_Cached ()
    {
      TypeResolutionConfiguration.SetCurrent(new TypeResolutionConfiguration(new DefaultTypeResolutionService()));
      var defaultService = ContextAwareTypeUtility.GetTypeResolutionService();

      TypeResolutionConfiguration.SetCurrent(new TypeResolutionConfiguration(new DefaultTypeResolutionService()));
      var defaultService2 = ContextAwareTypeUtility.GetTypeResolutionService();

      Assert.That(defaultService, Is.SameAs(defaultService2));
    }

    [Test]
    public void GetTypeDiscoveryService_WithCustomImplementationFromConfigFile_DoesNotThrowOnInitialization ()
    {
      var relativePath = @"Reflection\TestDomain\ContextAwareTypeUtilityTest\app.config";
      var fullPath = Path.Combine(TestContext.CurrentContext.TestDirectory, relativePath);
      Assert.That(File.Exists(fullPath));

      // We run this in a new process to ensure that the config can be loaded
      // without any initialization problems causing recursive initialization (see RM-8064)
      var isolatedCodeRunner = new IsolatedCodeRunner(TestAction);
      isolatedCodeRunner.ConfigFile = fullPath;
      Assert.That(() => isolatedCodeRunner.Run(), Throws.Nothing);

      static void TestAction (string[] args)
      {
        var typeDiscoveryService = ContextAwareTypeUtility.GetTypeDiscoveryService();
        Assert.That(typeDiscoveryService, Is.TypeOf<FakeTypeDiscoveryService>());
      }
    }
  }
}
