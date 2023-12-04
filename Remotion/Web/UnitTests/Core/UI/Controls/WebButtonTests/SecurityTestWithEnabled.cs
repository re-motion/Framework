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
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private Mock<IWebSecurityAdapter> _mockWebSecurityAdapter;
    private Mock<ISecurableObject> _mockSecurableObject;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void Setup ()
    {
      _mockWebSecurityAdapter = new Mock<IWebSecurityAdapter>(MockBehavior.Strict);
      _mockSecurableObject = new Mock<ISecurableObject>(MockBehavior.Strict);

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterMultiple<IWebSecurityAdapter>(() => _mockWebSecurityAdapter.Object);
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = true;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebButton button = CreateButtonWithClickEventHandler();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      button.Enabled = false;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.False);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutWebSeucrityProvider ()
    {
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterMultiple<IWebSecurityAdapter>();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();
      using (new ServiceLocatorScope(serviceLocator))
      {
        WebButton button = CreateButtonWithClickEventHandler();
        button.Enabled = true;

        bool enabled = button.Enabled;

        _mockWebSecurityAdapter.Verify();
        _mockSecurableObject.Verify();
        Assert.That(enabled, Is.True);
      }
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutWebSeucrityProvider ()
    {
      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterMultiple<IWebSecurityAdapter>();
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();
      using (new ServiceLocatorScope(serviceLocator))
      {
        WebButton button = CreateButtonWithClickEventHandler();
        button.Enabled = false;

        bool enabled = button.Enabled;

        _mockWebSecurityAdapter.Verify();
        _mockSecurableObject.Verify();
        Assert.That(enabled, Is.False);
      }
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler();
      button.Enabled = true;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithoutClickEventHandler ()
    {
      WebButton button = CreateButtonWithoutClickEventHandler();
      button.Enabled = false;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.False);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndAccessGranted ()
    {
      _mockWebSecurityAdapter.Setup(_ => _.HasAccess(_mockSecurableObject.Object, new EventHandler(TestHandler))).Returns(true).Verifiable();
      WebButton button = CreateButtonWithClickEventHandler();
      button.Enabled = true;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndAccessDenied ()
    {
      _mockWebSecurityAdapter.Setup(_ => _.HasAccess(_mockSecurableObject.Object, new EventHandler(TestHandler))).Returns(false).Verifiable();
      WebButton button = CreateButtonWithClickEventHandler();
      button.Enabled = true;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.False);
    }

    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebButton button = CreateButtonWithClickEventHandler();
      button.Enabled = false;

      bool enabled = button.Enabled;

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      Assert.That(enabled, Is.False);
    }

    private void TestHandler (object sender, EventArgs e)
    {
    }

    private WebButton CreateButtonWithClickEventHandler ()
    {
      WebButton button = new WebButton();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject.Object;
      button.Click += TestHandler;

      return button;
    }

    private WebButton CreateButtonWithoutClickEventHandler ()
    {
      WebButton button = new WebButton();
      button.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      button.SecurableObject = _mockSecurableObject.Object;

      return button;
    }
  }
}
