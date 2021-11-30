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
using Remotion.Security;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebMenuItemTests
{
  [TestFixture]
  public class SecurityTestWithEnabled : BaseTest
  {
    private Mock<IWebSecurityAdapter> _mockWebSecurityAdapter;
    private Mock<ISecurableObject> _mockSecurableObject;
    private Mock<Command> _mockCommand;

    [SetUp]
    public void Setup ()
    {
      _mockWebSecurityAdapter = new Mock<IWebSecurityAdapter>(MockBehavior.Strict);
      _mockSecurableObject = new Mock<ISecurableObject>(MockBehavior.Strict);
      _mockCommand = new Mock<Command>(MockBehavior.Strict, CommandType.None, _mockWebSecurityAdapter.Object, (IWxeSecurityAdapter)null);
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = false;
      _mockCommand.Setup(_ => _.HasAccess(_mockSecurableObject.Object)).Verifiable();

      bool isEnabled = menuItem.EvaluateEnabled();

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      _mockCommand.Verify(_ => _.HasAccess(_mockSecurableObject.Object), Times.Never());
      Assert.That(isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToInvisible ()
    {
      WebMenuItem menuItem = CreateWebMenuItem();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.IsDisabled = true;
      _mockCommand.Setup(_ => _.HasAccess(_mockSecurableObject.Object)).Verifiable();

      bool isEnabled = menuItem.EvaluateEnabled();

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      _mockCommand.Verify(_ => _.HasAccess(_mockSecurableObject.Object), Times.Never());
      Assert.That(isEnabled, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand();
      menuItem.IsDisabled = false;

      bool isEnabled = menuItem.EvaluateEnabled();
      Assert.That(isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand();
      menuItem.IsDisabled = true;

      bool isEnabled = menuItem.EvaluateEnabled();
      Assert.That(isEnabled, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      WebMenuItem menuItem = CreateWebMenuItem();
      menuItem.IsDisabled = false;
      _mockCommand.Setup(_ => _.HasAccess(_mockSecurableObject.Object)).Returns(true).Verifiable();

      bool isEnabled = menuItem.EvaluateEnabled();

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      _mockCommand.Verify();
      Assert.That(isEnabled, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      WebMenuItem menuItem = CreateWebMenuItem();
      menuItem.IsDisabled = false;
      _mockCommand.Setup(_ => _.HasAccess(_mockSecurableObject.Object)).Returns(false).Verifiable();

      bool isEnabled = menuItem.EvaluateEnabled();

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      _mockCommand.Verify();
      Assert.That(isEnabled, Is.False);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebMenuItem menuItem = CreateWebMenuItem();
      menuItem.IsDisabled = true;

      bool isEnabled = menuItem.EvaluateEnabled();

      _mockWebSecurityAdapter.Verify();
      _mockSecurableObject.Verify();
      _mockCommand.Verify();
      Assert.That(isEnabled, Is.False);
    }

    private WebMenuItem CreateWebMenuItem ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand();
      _mockCommand.Protected().SetupSet<IControl>("OwnerControlImplementation", null);
      menuItem.Command = _mockCommand.Object;
      _mockWebSecurityAdapter.Reset();
      _mockSecurableObject.Reset();
      _mockCommand.Reset();

      return menuItem;
    }

    private WebMenuItem CreateWebMenuItemWithoutCommand ()
    {
      WebMenuItem menuItem = new WebMenuItem();
      menuItem.Command.Type = CommandType.None;
      menuItem.Command = null;
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.SecurableObject = _mockSecurableObject.Object;

      return menuItem;
    }
  }
}
