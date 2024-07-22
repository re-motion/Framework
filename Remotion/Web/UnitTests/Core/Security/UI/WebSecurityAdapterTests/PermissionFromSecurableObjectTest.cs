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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Web.Security.UI;
using Remotion.Web.UI;
using Remotion.Web.UnitTests.Core.Security.Domain;

namespace Remotion.Web.UnitTests.Core.Security.UI.WebSecurityAdapterTests
{
  [TestFixture]
  public class PermissionFromSecurableObjectTest
  {
    private IWebSecurityAdapter _securityAdapter;
    private WebPermissionProviderTestHelper _testHelper;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      _securityAdapter = new WebSecurityAdapter();

      _testHelper = new WebPermissionProviderTestHelper();

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle(() => _testHelper.SecurityProvider);
      serviceLocator.RegisterSingle(() => _testHelper.PrincipalProvider);
      serviceLocator.RegisterSingle(() => _testHelper.FunctionalSecurityStrategy);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void HasAccessGranted_WithoutHandler ()
    {
      bool hasAccess = _securityAdapter.HasAccess(_testHelper.CreateSecurableObject(), null);

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccessGranted_WithinSecurityFreeSection ()
    {
      bool hasAccess;
      using (SecurityFreeSection.Activate())
      {
        hasAccess = _securityAdapter.HasAccess(_testHelper.CreateSecurableObject(), new EventHandler(TestEventHandler));
      }

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccessGranted ()
    {
      _testHelper.ExpectHasAccess(new Enum[] { GeneralAccessTypes.Read }, true);

      bool hasAccess = _securityAdapter.HasAccess(_testHelper.CreateSecurableObject(), new EventHandler(TestEventHandler));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccessDenied ()
    {
      _testHelper.ExpectHasAccess(new Enum[] { GeneralAccessTypes.Read }, false);

      bool hasAccess = _securityAdapter.HasAccess(_testHelper.CreateSecurableObject(), new EventHandler(TestEventHandler));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.False);
    }

    [Test]
    public void HasAccessGranted_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject(new Enum[] { GeneralAccessTypes.Read }, true);

      bool hasAccess = _securityAdapter.HasAccess(null, new EventHandler(TestEventHandler));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.True);
    }

    [Test]
    public void HasAccessDenied_WithSecurableObjectSetToNull ()
    {
      _testHelper.ExpectHasStatelessAccessForSecurableObject(new Enum[] { GeneralAccessTypes.Read }, false);

      bool hasAccess = _securityAdapter.HasAccess(null, new EventHandler(TestEventHandler));

      _testHelper.VerifyAll();
      Assert.That(hasAccess, Is.False);
    }

    [DemandTargetMethodPermission(SecurableObject.Method.Show)]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}
