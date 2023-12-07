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
using Remotion.Development.UnitTesting;
using Remotion.Reflection;
using Remotion.Reflection.TypeResolution;
using Remotion.ServiceLocation;
using Remotion.UnitTests.Configuration.TypeDiscovery;
using Remotion.Utilities;
#if !NETFRAMEWORK
using Remotion.Development.UnitTesting.IsolatedCodeRunner;
#endif

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class ContextAwareTypeUtilityTest
  {
    private ITypeDiscoveryService _oldTypeDiscoveryService;
    private TypeDiscoveryConfiguration _oldTypeDiscoveryConfiguration;
    private ITypeResolutionService _oldTypeResolutionService;

    [SetUp]
    public void SetUp ()
    {
      _oldTypeDiscoveryService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      _oldTypeDiscoveryConfiguration = TypeDiscoveryConfiguration.Current;

      var fields = PrivateInvoke.GetNonPublicStaticField(typeof(ContextAwareTypeUtility), "s_fields");
      Assertion.IsNotNull(fields);
      PrivateInvoke.SetPublicField(
          fields,
          "DefaultTypeDiscoveryService",
          new Lazy<ITypeDiscoveryService>(() => TypeDiscoveryConfiguration.Current.CreateTypeDiscoveryService()));
      TypeDiscoveryConfiguration.SetCurrent(new TypeDiscoveryConfiguration());
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

      TypeDiscoveryConfiguration.SetCurrent(_oldTypeDiscoveryConfiguration);
    }

    [Test]
    [Ignore("RM-5931")]
    public void GetTypeDiscoveryService_LoadsFromSafeServiceLocator ()
    {
      var defaultService = ContextAwareTypeUtility.GetTypeDiscoveryService();
      Assert.That(defaultService, Is.SameAs(SafeServiceLocator.Current.GetInstance<ITypeDiscoveryService>()));
    }

    [Test]
    [Obsolete]
    public void GetTypeResolutionService_LoadsFromSafeServiceLocator ()
    {
      var defaultService = ContextAwareTypeUtility.GetTypeResolutionService();
      Assert.That(defaultService, Is.SameAs(SafeServiceLocator.Current.GetInstance<ITypeResolutionService>()));
    }
  }
}
