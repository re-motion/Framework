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
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.FluentControlSelection;
using Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.TestParameters;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories
{
  /// <summary>
  /// Contains test for <see cref="IIndexControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class IndexControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, IndexTestParameters>
      where TControlSelector : IIndexControlSelector<TControl>
      where TControl : ControlObject
  {
    public IndexControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "Index";
    }

    [TestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.GetByIndex (Parameters.VisibleControlIndex);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.GetByIndex (Parameters.HiddenControlIndex),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByIndexOrNull (Parameters.VisibleControlIndex);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull_After_IFrameSwitch ()
    {
      SwitchToIFrame();

      var control = Selector.GetByIndexOrNull (Parameters.VisibleControlIndex);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByIndexOrNull (Parameters.HiddenControlIndex);

      Assert.That (control, Is.Null);
    }

    [TestMethod]
    public void Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByIndex (Parameters.VisibleControlIndex);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    public void Exists_Returns_True_After_IFrameSwitch ()
    {
      SwitchToIFrame();

      var controlVisible = Selector.ExistsByIndex (Parameters.VisibleControlIndex);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    public void Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByIndex (Parameters.HiddenControlIndex);

      Assert.That (controlVisible, Is.False);
    }
  }
}