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
  public class BocBooleanValueControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (DisabledTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (ReadOnlyTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    public void GenericTests (GenericSelectorTestSetupAction<BocBooleanValueSelector, BocBooleanValueControlObject> testAction)
    {
      testAction (Helper, e => e.BooleanValues(), "booleanValue");
    }

    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (DomainPropertyControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    [RemotionTestCaseSource (typeof (DisplayNameControlSelectorTestCaseFactory<BocBooleanValueSelector, BocBooleanValueControlObject>))]
    public void TestControlSelectors (GenericSelectorTestSetupAction<BocBooleanValueSelector, BocBooleanValueControlObject> testAction)
    {
      testAction (Helper, e => e.BooleanValues(), "booleanValue");
    }

    [Test]
    public void TestIsDisabled_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.BooleanValues().GetByLocalID ("DeceasedField_Disabled");

      Assert.That (control.IsDisabled(), Is.True);
      Assert.That (() => control.SetTo (false), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestIsReadOnly_SetMethodsThrow ()
    {
      var home = Start();

      var control = home.BooleanValues().GetByLocalID ("DeceasedField_ReadOnly");

      Assert.That (control.IsReadOnly(), Is.True);
      Assert.That (() => control.SetTo (false), Throws.InstanceOf<MissingHtmlException>());
    }

    [Test]
    public void TestGetState ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_ReadOnly");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_Disabled");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_NoAutoPostBack");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));

      bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_TriState");
      Assert.That (bocBooleanValue.GetState(), Is.EqualTo (false));
    }

    [Test]
    public void TestIsTriState ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_Normal");
      Assert.That (bocBooleanValue.IsTriState(), Is.EqualTo (false));

      bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_TriState");
      Assert.That (bocBooleanValue.IsTriState(), Is.EqualTo (true));
    }

    [Test]
    public void TestSetTo_WithRequiredValue ()
    {
      var home = Start();

      var normalBocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_Normal");
      var noAutoPostBackBocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_NoAutoPostBack");

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

    [Test]
    public void TestSetTo_WithNullableValue ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_TriState");

      bocBooleanValue.SetTo (null);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.Empty);

      bocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.EqualTo ("False"));

      bocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("TriStateCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    [Test]
    public void TestSetTo_True_WithRequiredValueAndUnitialized ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_NormalAndUnitialized");

      bocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("True"));

      bocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("False"));

      bocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("True"));
    }

    [Test]
    public void TestSetTo_False_WithRequiredValueAndUnitialized ()
    {
      var home = Start();

      var bocBooleanValue = home.BooleanValues().GetByLocalID ("DeceasedField_NormalAndUnitialized");

      bocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("False"));

      bocBooleanValue.SetTo (true);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("True"));

      bocBooleanValue.SetTo (false);
      Assert.That (home.Scope.FindIdEndingWith ("NormalAndUnitializedCurrentValueLabel").Text, Is.EqualTo ("False"));
    }

    private WxePageObject Start ()
    {
      return Start ("BocBooleanValue");
    }
  }
}