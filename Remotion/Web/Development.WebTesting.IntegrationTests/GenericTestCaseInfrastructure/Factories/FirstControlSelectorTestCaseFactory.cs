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
  /// Contains test for <see cref="IFirstControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class FirstControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, FirstTestParameters>
      where TControlSelector : IFirstControlSelector<TControl>
      where TControl : ControlObject
  {
    public FirstControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "First";
    }

    [TestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.First();

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod (PageType = GenericPageTypes.Hidden)]
    [Category ("LongRunning")]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.First(),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.FirstOrNull();
      
      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull_After_IFrameSwitch ()
    {
      SwitchToIFrame();

      var control = Selector.FirstOrNull();

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod (PageType = GenericPageTypes.Hidden)]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.FirstOrNull();

      Assert.That (control, Is.Null);
    }
  }
}