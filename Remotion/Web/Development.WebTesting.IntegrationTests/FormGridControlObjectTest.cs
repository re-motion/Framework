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
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlObjects.Selectors;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class FormGridControlObjectTest : IntegrationTest
  {
    [Test]
    [RemotionTestCaseSource (typeof (HtmlIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [RemotionTestCaseSource (typeof (IndexControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [RemotionTestCaseSource (typeof (LocalIDControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [RemotionTestCaseSource (typeof (TitleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [RemotionTestCaseSource (typeof (FirstControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    [RemotionTestCaseSource (typeof (SingleControlSelectorTestCaseFactory<FormGridSelector, FormGridControlObject>))]
    public void TestControlSelectors (GenericSelectorTestAction<FormGridSelector, FormGridControlObject> testAction)
    {
      testAction (Helper, e => e.FormGrids(), "formGrid");
    }

    // Exists as unused member for future FormGrid tests.
    // ReSharper disable once UnusedMember.Local
    private WebFormsTestPageObject Start ()
    {
      return Start<WebFormsTestPageObject> ("FormGridTest.aspx");
    }
  }
}