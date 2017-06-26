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
  /// Contains test for <see cref="IDomainPropertyControlSelector{TControlObject}"/> that are executed by using <see cref="TestCaseSourceAttribute"/>.
  /// </summary>
  public class DomainPropertyControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, DomainPropertyTestParameters>
      where TControlSelector : IDomainPropertyControlSelector<TControl>
      where TControl : ControlObject
  {
    public DomainPropertyControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string GetTestPrefix ()
    {
      return "DomainProperty";
    }

    [TestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.GetByDomainProperty (Parameters.VisibleDomainProperty);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameters.HiddenDomainProperty)),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void Get_WithClass_Returns_NotNull ()
    {
      var control = Selector.GetByDomainProperty (Parameters.VisibleDomainProperty, Parameters.CorrectDomainClass);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Get_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameters.VisibleDomainProperty, Parameters.IncorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameters.HiddenDomainProperty, Parameters.IncorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameters.HiddenDomainProperty, Parameters.CorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [TestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameters.VisibleDomainProperty);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameters.HiddenDomainProperty);

      Assert.That (control, Is.Null);
    }

    [TestMethod]
    public void GetOrNull_WithClass_Returns_NotNull ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameters.VisibleDomainProperty, Parameters.CorrectDomainClass);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameters.FoundControlID));
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void GetOrNull_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameters.VisibleDomainProperty, Parameters.IncorrectDomainClass),
          Is.Null);
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameters.HiddenDomainProperty, Parameters.IncorrectDomainClass),
          Is.Null);
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameters.HiddenDomainProperty, Parameters.CorrectDomainClass),
          Is.Null);
    }

    [TestMethod]
    public void Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameters.VisibleDomainProperty);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    public void Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameters.HiddenDomainProperty);

      Assert.That (controlVisible, Is.False);
    }

    [TestMethod]
    public void Exists_WithClass_Returns_NotNull ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameters.VisibleDomainProperty, Parameters.CorrectDomainClass);

      Assert.That (controlVisible, Is.True);
    }

    [TestMethod]
    [Category ("LongRunning")]
    public void Exists_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameters.VisibleDomainProperty, Parameters.IncorrectDomainClass),
          Is.False);
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameters.HiddenDomainProperty, Parameters.IncorrectDomainClass),
          Is.False);
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameters.HiddenDomainProperty, Parameters.CorrectDomainClass),
          Is.False);
    }
  }
}