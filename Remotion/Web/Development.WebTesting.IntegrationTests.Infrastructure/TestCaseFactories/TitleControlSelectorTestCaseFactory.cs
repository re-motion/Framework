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
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories
{
  /// <summary>
  /// Contains tests for <see cref="ITitleControlSelector{TControlObject}"/> that are executed via <see cref="TestCaseSourceAttribute"/>
  /// </summary>
  public class TitleControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, TitleGenericTestPageParameter>
      where TControlSelector : ITitleControlSelector<TControl>
      where TControl : ControlObject
  {
    public TitleControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string TestPrefix
    {
      get { return "TitleControlSelector"; }
    }

    [GenericPageTestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.GetByTitle(Parameter.VisibleControlTitle);

      Assert.That(control, Is.Not.Null);
      Assert.That(control.Scope.Id, Is.EqualTo(Parameter.FoundControlID));
    }

    [GenericPageTestMethod(SearchTimeout = SearchTimeout.UseShortTimeout)]
    public void Get_Throws_WebTestException ()
    {
      Assert.That(
          () => Selector.GetByTitle(Parameter.HiddenControlTitle),
          Throws.InstanceOf<WebTestException>());
    }

    [GenericPageTestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByTitleOrNull(Parameter.VisibleControlTitle);

      Assert.That(control, Is.Not.Null);
      Assert.That(control.Scope.Id, Is.EqualTo(Parameter.FoundControlID));
    }

    [GenericPageTestMethod]
    public void GetOrNull_Returns_NotNull_After_IFrameSwitch ()
    {
      SwitchToIFrame();

      var control = Selector.GetByTitleOrNull(Parameter.VisibleControlTitle);

      Assert.That(control, Is.Not.Null);
      Assert.That(control.Scope.Id, Is.EqualTo(Parameter.FoundControlID));
    }

    [GenericPageTestMethod]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByTitleOrNull(Parameter.HiddenControlTitle);

      Assert.That(control, Is.Null);
    }

    [GenericPageTestMethod]
    public void Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByTitle(Parameter.VisibleControlTitle);

      Assert.That(controlVisible, Is.True);
    }

    [GenericPageTestMethod]
    public void Exists_Returns_True_After_IFrameSwitch ()
    {
      SwitchToIFrame();

      var controlVisible = Selector.ExistsByTitle(Parameter.VisibleControlTitle);

      Assert.That(controlVisible, Is.True);
    }

    [GenericPageTestMethod]
    public void Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByTitle(Parameter.HiddenControlTitle);

      Assert.That(controlVisible, Is.False);
    }
  }
}
