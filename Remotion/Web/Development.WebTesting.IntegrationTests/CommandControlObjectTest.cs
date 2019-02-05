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
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class CommandControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<CommandSelector, CommandControlObject>))]
    public void GenericTests (GenericSelectorTestAction<CommandSelector, CommandControlObject> testAction)
    {
      testAction (Helper, e => e.Commands(), "command");
    }

    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<CommandSelector, CommandControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<CommandSelector, CommandControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<CommandSelector, CommandControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<CommandSelector, CommandControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<CommandSelector, CommandControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<CommandSelector, CommandControlObject> testAction)
    {
      testAction (Helper, e => e.Commands(), "command");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.Commands().GetByLocalID ("TestCommand3");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.Click(), Throws.Exception.Message.EqualTo (AssertionExceptionUtility.CreateControlDisabledException ("Command").Message));
    }

    [Test]
    public void TestClick ()
    {
      var home = Start();

      var command1 = home.Commands().GetByLocalID ("Command1");
      home = command1.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.EqualTo ("Command1ItemID"));

      var command2 = home.Commands().GetByLocalID ("Command2");
      home = command2.Click().Expect<WxePageObject>();
      Assert.That (home.Scope.FindId ("TestOutputLabel").Text, Is.Empty);
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject> ("CommandTest.wxe");
    }
  }
}