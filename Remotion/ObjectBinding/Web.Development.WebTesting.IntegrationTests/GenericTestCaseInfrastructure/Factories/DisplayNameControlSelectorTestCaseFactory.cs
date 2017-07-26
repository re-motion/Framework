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
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection;
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.TestParameters;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories
{
  /// <summary>
  /// Contains test for <see cref="IDisplayNameControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class DisplayNameControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, DisplayNameTestParameters>
      where TControlSelector : IDisplayNameControlSelector<TControl>
      where TControl : ControlObject
  {
    public DisplayNameControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "DisplayName";
    }

    [TestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.GetByDisplayName (Parameters.VisibleDisplayName);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          (TestDelegate) (() => Selector.GetByDisplayName (Parameters.HiddenDisplayName)),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByDisplayNameOrNull (Parameters.VisibleDisplayName);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByDisplayNameOrNull (Parameters.HiddenDisplayName);

      Assert.That (control, Is.Null);
    }

    [TestMethod]
    public void Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByDisplayName (Parameters.VisibleDisplayName);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    public void Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByDisplayName (Parameters.HiddenDisplayName);

      Assert.That (controlVisible, Is.False);
    }
  }
}