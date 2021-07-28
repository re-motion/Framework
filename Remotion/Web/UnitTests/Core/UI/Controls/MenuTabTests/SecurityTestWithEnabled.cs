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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private Mock<IWebSecurityAdapter> _mockWebSecurityAdapter;
    private Mock<NavigationCommand> _mockNavigationCommand;

    [SetUp]
    public void Setup ()
    {
      _mockWebSecurityAdapter = new Mock<IWebSecurityAdapter> (MockBehavior.Strict);
      _mockNavigationCommand = new Mock<NavigationCommand> (MockBehavior.Strict, CommandType.Href, _mockWebSecurityAdapter.Object, (IWxeSecurityAdapter) null);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = false;
      _mockNavigationCommand.Setup (_ => _.HasAccess (null)).Verifiable();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify (_ => _.HasAccess (null), Times.Never());
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = true;
      _mockNavigationCommand.Setup (_ => _.HasAccess (null)).Verifiable();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify (_ => _.HasAccess (null), Times.Never());
      Assert.That (isEnabled, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull ();
      mainMenuTab.IsDisabled = false;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull();
      mainMenuTab.IsDisabled = true;

      bool isEnabled = mainMenuTab.EvaluateEnabled();
      Assert.That (isEnabled, Is.False);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNone ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabCommandTypeNone();
      mainMenuTab.IsDisabled = false;

      bool isEnabled = mainMenuTab.EvaluateEnabled();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNone ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabCommandTypeNone();
      mainMenuTab.IsDisabled = true;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.That (isEnabled, Is.False);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNullAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull ();
      mainMenuTab.IsDisabled = false;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNullAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull();
      mainMenuTab.IsDisabled = true;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;

      bool isEnabled = mainMenuTab.EvaluateEnabled();
      Assert.That (isEnabled, Is.False);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNoneAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabCommandTypeNone();
      mainMenuTab.IsDisabled = false;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;

      bool isEnabled = mainMenuTab.EvaluateEnabled();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNoneAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabCommandTypeNone();
      mainMenuTab.IsDisabled = true;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();
      Assert.That (isEnabled, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      _mockNavigationCommand.Setup (_ => _.HasAccess (null)).Returns (true).Verifiable();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      _mockNavigationCommand.Setup (_ => _.HasAccess (null)).Returns (false).Verifiable();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That (isEnabled, Is.False);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = true;

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mockWebSecurityAdapter.Verify();
      _mockNavigationCommand.Verify();
      Assert.That (isEnabled, Is.False);
    }

    private MainMenuTab CreateMainMenuTab ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull ();
      _mockNavigationCommand.Protected().SetupSet<IControl> ("OwnerControlImplementation", null);
      mainMenuTab.Command = _mockNavigationCommand.Object;
      _mockWebSecurityAdapter.Reset();
      _mockNavigationCommand.Reset();

      return mainMenuTab;
    }

    private MainMenuTab CreateMainMenuTabCommandTypeNone ()
    {
      MainMenuTab mainMenuTab = new MainMenuTab();
      mainMenuTab.Command.Type = CommandType.None;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;

      return mainMenuTab;
    }

    private MainMenuTab CreateMainMenuTabWithCommandSetNull ()
    {
      MainMenuTab mainMenuTab = new MainMenuTab ();
      mainMenuTab.Command = null;
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;

      return mainMenuTab;
    }
  }
}
