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
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.IntegrationTests.ExecutionEngine
{
  [TestFixture]
  public class IsDirtyTest : IntegrationTest
  {
    [Test]
    public void InitialState_PageEvaluatesAsNotDirty ()
    {
      Start();

      var dirtyState = ExecuteSmartPageIsDirty();

      Assert.That(dirtyState, Is.False);
    }

    [Test]
    public void SetPageDirty_PageEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetPageDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(), Is.True);
    }

    [Test]
    public void SetPageDirty_CurrentPageConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetPageDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentPage", "OtherDirtyState" }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { "OtherDirtyState" }), Is.False);
    }

    [Test]
    public void SetCurrentFunctionDirty_PageEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(), Is.True);
    }

    [Test]
    public void SetCurrentFunctionDirty_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction", "CurrentPage" }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentPage" }), Is.False);
    }

    [Test]
    public void SetCurrentFunctionDirty_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "RootFunction", "CurrentPage" }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentPage" }), Is.False);
    }

    [Test]
    public void SetSubFunctionDirty_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "RootFunction" }), Is.True);
    }

    [Test]
    public void SetSubFunctionDirty_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction" }), Is.True);
    }

    [Test]
    public void SetRootFunctionDirty_WhenCurrentFunctionIsSubFunction_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "RootFunction" }), Is.True);
    }

    [Test]
    public void SetRootFunctionDirty_WhenCurrentFunctionIsSubFunction_CurrentFunctionConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction" }), Is.False);
    }

    [Test]
    public void SetSubFunctionDirty_AndReturnToRootFunction_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction" }), Is.True);

      home.WebButtons().GetByLocalID("ExecuteNextStepButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction" }), Is.True);
    }

    [Test]
    public void SetSubFunctionDirty_AndReturnToRootFunction_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID("ExecuteSubFunctionButton").Click();
      home.WebButtons().GetByLocalID("SetCurrentFunctionDirtyButton").Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { "CurrentFunction" }), Is.True);

      home.WebButtons().GetByLocalID("ExecuteNextStepButton").Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { "RootFunction" }), Is.True);
    }

    private bool ExecuteSmartPageIsDirty ()
    {
      // SmartPage_IsDirty is defined on test page due to Firefox not supporting instance members

      return JavaScriptExecutor.ExecuteStatement<bool>((IJavaScriptExecutor)Driver.Native, "return SmartPage_IsDirty();");
    }

    private bool ExecuteSmartPageIsDirty (string[] dirtyStates)
    {
      // SmartPage_IsDirty is defined on test page due to Firefox not supporting instance members

      var dirtyStatesAsString = "[" + string.Join(", ", dirtyStates.Select(s => "'" + s + "'")) + "]";
      return JavaScriptExecutor.ExecuteStatement<bool>((IJavaScriptExecutor)Driver.Native, "return SmartPage_IsDirty(" + dirtyStatesAsString + ");");
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("DirtyStateTest.wxe");
    }
  }
}
