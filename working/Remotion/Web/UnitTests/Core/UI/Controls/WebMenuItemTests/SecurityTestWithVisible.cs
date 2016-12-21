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
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebMenuItemTests
{
  [TestFixture]
  public class SecurityTestWithVisible : BaseTest
  {
    private MockRepository _mocks;
    private IWebSecurityAdapter _mockWebSecurityAdapter;
    private ISecurableObject _mockSecurableObject;
    private Command _mockCommand;
    private ServiceLocatorScope _serviceLocatorStub;

    [SetUp]
    public void Setup ()
    {
      _mocks = new MockRepository ();
      _mockWebSecurityAdapter = _mocks.StrictMock<IWebSecurityAdapter> ();
      _mockSecurableObject = _mocks.StrictMock<ISecurableObject> ();
      _mockCommand = _mocks.StrictMock<Command> ();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterMultiple<IWebSecurityAdapter> (() => _mockWebSecurityAdapter);
      serviceLocator.RegisterMultiple<IWxeSecurityAdapter>();
      _serviceLocatorStub = new ServiceLocatorScope (serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorStub.Dispose();
    }

    [Test]
    public void EvaluateTrue_FromTrueAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.That (isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithMissingPermissionBehaviorSetToDisabled ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Disabled;
      menuItem.IsVisible = false;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Repeat.Never ();
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.That (isVisible, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsVisible = true;

      bool isVisible = menuItem.EvaluateVisible ();
      Assert.That (isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromFalseAndWithCommandSetNull ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.IsVisible = false;

      bool isVisible = menuItem.EvaluateVisible ();
      Assert.That (isVisible, Is.False);
    }


    [Test]
    public void EvaluateTrue_FromTrueAndWithAccessGranted ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (true);
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.That (isVisible, Is.True);
    }

    [Test]
    public void EvaluateFalse_FromTrueAndWithAccessDenied ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = true;
      Expect.Call (_mockCommand.HasAccess (_mockSecurableObject)).Return (false);
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.That (isVisible, Is.False);
    }


    [Test]
    public void EvaluateFalse_FromFalse ()
    {
      WebMenuItem menuItem = CreateWebMenuItem ();
      menuItem.IsVisible = false;
      _mocks.ReplayAll ();

      bool isVisible = menuItem.EvaluateVisible ();

      _mocks.VerifyAll ();
      Assert.That (isVisible, Is.False);
    }

    private WebMenuItem CreateWebMenuItem ()
    {
      WebMenuItem menuItem = CreateWebMenuItemWithoutCommand ();
      menuItem.Command = _mockCommand;
      _mocks.BackToRecordAll();

      return menuItem;
    }

    private WebMenuItem CreateWebMenuItemWithoutCommand ()
    {
      WebMenuItem menuItem = new WebMenuItem ();
      menuItem.Command.Type = CommandType.None;
      menuItem.Command = null;
      menuItem.MissingPermissionBehavior = MissingPermissionBehavior.Invisible;
      menuItem.SecurableObject = _mockSecurableObject;
      
      return menuItem;
    }
  }
}
