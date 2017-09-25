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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.MenuTabTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private NavigationCommand _mockNavigationCommand;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository();
      _mockWebSecurityAdapter = _mocks.StrictMock<IWebSecurityAdapter>();
      _mockNavigationCommand = _mocks.StrictMock<NavigationCommand> (CommandType.Href, _mockWebSecurityAdapter, (IWxeSecurityAdapter) null);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      mainMenuTab.IsDisabled = true;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
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
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (true);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.That (isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = false;
      Expect.Call (_mockNavigationCommand.HasAccess (null)).Return (false);
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.That (isEnabled, Is.False);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTab ();
      mainMenuTab.IsDisabled = true;
      _mocks.ReplayAll ();

      bool isEnabled = mainMenuTab.EvaluateEnabled ();

      _mocks.VerifyAll ();
      Assert.That (isEnabled, Is.False);
    }

    private MainMenuTab CreateMainMenuTab ()
    {
      MainMenuTab mainMenuTab = CreateMainMenuTabWithCommandSetNull ();
      mainMenuTab.Command = _mockNavigationCommand;
      _mocks.BackToRecordAll();

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
