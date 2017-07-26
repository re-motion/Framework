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
  /// Contains test for <see cref="ILocalIDControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class LocalIDControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, LocalIDTestParameters>
      where TControlSelector : ILocalIDControlSelector<TControl>
      where TControl : ControlObject
  {
    public LocalIDControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "LocalID";
    }

    [TestMethod]
    public void Test_ByItemID_Get_Returns_NotNull ()
    {
      var control = Selector.GetByLocalID (Parameters.VisibleControlLocalID);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Test_ByItemID_Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.GetByLocalID (Parameters.HiddenControlLocalID),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void Test_ByItemID_GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByLocalIDOrNull (Parameters.VisibleControlLocalID);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void Test_ByItemID_GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByLocalIDOrNull (Parameters.HiddenControlLocalID);

      Assert.That (control, Is.Null);
    }

    [TestMethod]
    public void Test_ByItemID_Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByLocalID (Parameters.VisibleControlLocalID);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    public void Test_ByItemID_Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByLocalID (Parameters.HiddenControlLocalID);

      Assert.That (controlVisible, Is.False);
    }
  }
}