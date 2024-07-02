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
using Moq.Protected;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithVisible : BaseTest
  {
    private Mock<IWebSecurityAdapter> _mockWebSecurityAdapter;
    private Mock<NavigationCommand> _mockNavigationCommand;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void Setup ()
    {
      _mockWebSecurityAdapter = new Mock<IWebSecurityAdapter>(MockBehavior.Strict);
      _mockNavigationCommand = new Mock<NavigationCommand>(MockBehavior.Strict);

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
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      mainMenuTab.IsVisible = true;
      _mockNavigationCommand.Setup(_ => _.HasAccess(null)).Verifiable();

      bool isVisible = mainMenuTab.EvaluateVisible();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify(_ => _.HasAccess(null), Times.Never());
      Assert.That(isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      mainMenuTab.IsVisible = false;
      _mockNavigationCommand.Setup(_ => _.HasAccess(null)).Verifiable();

      bool isVisible = mainMenuTab.EvaluateVisible();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify(_ => _.HasAccess(null), Times.Never());
      Assert.That(isVisible, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand();
      mainMenuTab.IsVisible = true;

      bool isVisible = mainMenuTab.EvaluateVisible();
      Assert.That(isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand();
      mainMenuTab.IsVisible = false;

      bool isVisible = mainMenuTab.EvaluateVisible();
      Assert.That(isVisible, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab();
      mainMenuTab.IsVisible = true;
      _mockNavigationCommand.Setup(_ => _.HasAccess(null)).Returns(true).Verifiable();

      bool isVisible = mainMenuTab.EvaluateVisible();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That(isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab();
      mainMenuTab.IsVisible = true;
      _mockNavigationCommand.Setup(_ => _.HasAccess(null)).Returns(false).Verifiable();

      bool isVisible = mainMenuTab.EvaluateVisible();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That(isVisible, Is.False);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab();
      mainMenuTab.IsVisible = false;

      bool isVisible = mainMenuTab.EvaluateVisible();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That(isVisible, Is.False);
    }

    private MainMenuTab CreateMainMenuTab ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithoutCommand();
      _mockNavigationCommand.Protected().SetupSet<IControl>("OwnerControlImplementation", It.IsAny<IControl>());
      mainMenuTab.Command = _mockNavigationCommand.Object;
      _mockNavigationCommand.Reset();
      _mockWebSecurityAdapter.Reset();

      return mainMenuTab;
    }

    private MainMenuTab CreateMainMenuTabWithoutCommand ()
    {
      MainMenuTab mainMenuTab = new MainMenuTab();
      mainMenuTab.Command.Type = CommandType.None;
      mainMenuTab.Command = null;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;

      return mainMenuTab;
    }
  }
}
