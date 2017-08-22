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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocCheckBoxControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocCheckBoxSelector, BocCheckBoxControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<BocCheckBoxSelector, BocCheckBoxControlObject> testAction)
    {
      testAction (Helper, e => e.CheckBoxes(), "checkBox");
    }

    [Test]
    public void TestIsReadOnly ()
    {
      var home = Start();

      var bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_Normal");
      Assert.That (bocCheckBox.IsReadOnly(), Is.False);

      bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocCheckBox.IsReadOnly(), Is.True);
    }

    [Test]
    public void TestGetState ()
    {
      var home = Start();

      var bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_Normal");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_Disabled");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));

      bocCheckBox = home.CheckBoxes().GetByLocalID ("DeceasedField_NoAutoPostBack");
      Assert.That (bocCheckBox.GetState(), Is.EqualTo (false));
    }

    [Test]
    public void TestSetTo ()
    {
      var home = Start();

      var normalBocBooleanValue = home.CheckBoxes().GetByLocalID ("DeceasedField_Normal");
      var noAutoPostBackBocBooleanValue = home.CheckBoxes().GetByLocalID ("DeceasedField_NoAutoPostBack");

      normalBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("True"));

      noAutoPostBackBocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (true, Opt.ContinueImmediately()); // same value, does not trigger post back
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("False"));

      normalBocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalCurrentValueLabel").Text, Is.EqualTo ("False"));
      Assert.That (home.Scope.FindIdEndingWith ("NoAutoPostBackCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocCheckBox");
    }
  }
}