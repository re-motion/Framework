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
    private static class Conditions
    {
      public const string RootFunction = "RootFunction";
      public const string CurrentFunction = "CurrentFunction";
      public const string CurrentPage = "CurrentPage";
      public const string ClientSide = "ClientSide";
    }

    private static class Buttons
    {
      public const string SetPageDirtyButton = "SetPageDirtyButton";
      public const string SetCurrentFunctionDirtyButton = "SetCurrentFunctionDirtyButton";
      public const string DisableDirtyStateOnCurrentPageButton = "DisableDirtyStateOnCurrentPageButton";
      public const string ExecuteSubFunctionButton = "ExecuteSubFunctionButton";
      public const string ExecuteNextStepButton = "ExecuteNextStepButton";
      public const string ExecuteSubFunctionWithDisabledDirtyStateButton = "ExecuteSubFunctionWithDisabledDirtyStateButton";
      public const string CancelExecutingFunctionButton = "CancelExecutingFunctionButton";
    }

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
      home.WebButtons().GetByLocalID(Buttons.SetPageDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(), Is.True);
    }

    [Test]
    public void SetPageDirty_CurrentPageConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetPageDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage, "OtherDirtyState" }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { "OtherDirtyState" }), Is.False);
    }

    [Test]
    public void SetPageDirtyFromClient_ClientSideConditionEvaluatesAsDirty ()
    {
      Start();
      ExecuteSetPageDirtyOnClientSide();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.ClientSide, Conditions.CurrentPage }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void SetCurrentFunctionDirty_PageEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(), Is.True);
    }

    [Test]
    public void SetCurrentFunctionDirty_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction, Conditions.CurrentPage }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void SetCurrentFunctionDirty_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction, Conditions.CurrentPage }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void SetSubFunctionDirty_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void SetSubFunctionDirty_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);
    }

    [Test]
    public void SetRootFunctionDirty_WhenCurrentFunctionIsSubFunction_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void SetRootFunctionDirty_WhenCurrentFunctionIsSubFunction_CurrentFunctionConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void SetSubFunctionDirty_AndReturnToRootFunction_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);

      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);
    }

    [Test]
    public void SetSubFunctionDirty_AndReturnToRootFunction_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);

      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void SetSubFunctionDirty_AndCancelSubFunction_RootFunctionConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);

      home.WebButtons().GetByLocalID(Buttons.CancelExecutingFunctionButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.False);
    }

    [Test]
    public void DisablePageDirtyState_PageIsDirtyFromServer_CurrentPageConditionEvaluatesAsNotDirty ()
    {
      var someCondition = "OtherDirtyState";

      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetPageDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage, someCondition }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { someCondition }), Is.False);

      home.WebButtons().GetByLocalID(Buttons.DisableDirtyStateOnCurrentPageButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage, someCondition }), Is.False);
      Assert.That(ExecuteSmartPageIsDirty(new[] { someCondition }), Is.False);
    }

    [Test]
    public void DisablePageDirtyState_PageIsDirtyFromClient_ClientSideConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.DisableDirtyStateOnCurrentPageButton).Click();

      ExecuteSetPageDirtyOnClientSide();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.ClientSide, Conditions.CurrentPage}), Is.False);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void DisablePageDirtyState_PageIsDirtyFromCurrentFunction_CurrentFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.DisableDirtyStateOnCurrentPageButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();


      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction, Conditions.CurrentPage}), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubFunction_CurrentFunctionConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromServer_CurrentPageConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentPage }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromRootFunction_RootFunctionConditionEvaluatesAsDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction, Conditions.CurrentFunction }), Is.True);
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubFunction_ReturnsFromSubFunction_CurrentFunctionConditionEvaluatesAsNotDirty ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();
      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_CurrentFunctionConditionEvaluatesAsDirtyForSubSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.True);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_RootFunctionConditionEvaluatesAsDirtyForSubSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromRootFunction_CurrentFunctionConditionEvaluatesAsNotDirtyForSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromRootFunction_RootFunctionConditionEvaluatesAsDirtyForSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromRootFunction_CurrentFunctionConditionEvaluatesAsNotDirtyForSubSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromRootFunction_RootFunctionConditionEvaluatesAsDirtyForSubSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.True);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_ReturnsFromSubFunction_CurrentFunctionConditionEvaluatesAsNotDirtyForSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_ReturnsFromSubFunction_RootFunctionConditionEvaluatesAsNotDirtyForSubFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_ReturnsFromSubFunction_CurrentFunctionConditionEvaluatesAsNotDirtyForRootFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.CurrentFunction }), Is.False);
    }

    [Test]
    public void DisableWxeFunctionDirtyState_PageIsDirtyFromSubSubFunction_ReturnsFromSubFunction_RootFunctionConditionEvaluatesAsNotDirtyForRootFunction ()
    {
      var home = Start();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionWithDisabledDirtyStateButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteSubFunctionButton).Click();

      home.WebButtons().GetByLocalID(Buttons.SetCurrentFunctionDirtyButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();
      home.WebButtons().GetByLocalID(Buttons.ExecuteNextStepButton).Click();

      Assert.That(ExecuteSmartPageIsDirty(new[] { Conditions.RootFunction }), Is.False);
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

    private void ExecuteSetPageDirtyOnClientSide ()
    {
      JavaScriptExecutor.ExecuteVoidStatement((IJavaScriptExecutor)Driver.Native, "return SetPageDirtyOnClientSide();");
    }

    private WxePageObject Start ()
    {
      return Start<WxePageObject>("DirtyStateTest.wxe");
    }
  }
}
