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
  /// Contains test for <see cref="ISingleControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class SingleControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, SingleTestParameters>
      where TControlSelector : ISingleControlSelector<TControl>
      where TControl : ControlObject
  {
    public SingleControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "Single";
    }

    [TestMethod (PageType = GenericPageTypes.NoAmbiguous)]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.Single();

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod (PageType = GenericPageTypes.Hidden)]
    [Category ("LongRunning")]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.Single(),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod (PageType = GenericPageTypes.AmbiguousSection)]
    public void Get_Throws_AmbiguousException ()
    {
      Assert.That (
          () => Selector.Single(),
          Throws.InstanceOf<AmbiguousException>());
    }

    [TestMethod (PageType = GenericPageTypes.NoAmbiguous)]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.SingleOrNull();

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod (PageType = GenericPageTypes.Hidden)]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.SingleOrNull();

      Assert.That (control, Is.Null);
    }

    [TestMethod (PageType = GenericPageTypes.AmbiguousSection)]
    public void GetOrNull_Throws_AmbiguousException ()
    {
      Assert.That (
          () => Selector.SingleOrNull(),
          Throws.InstanceOf<AmbiguousException>());
    }
  }
}