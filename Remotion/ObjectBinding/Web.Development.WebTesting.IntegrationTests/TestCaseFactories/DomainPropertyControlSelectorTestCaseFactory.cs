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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories
{
  /// <summary>
  /// Contains tests for <see cref="IDomainPropertyControlSelector{TControlObject}"/> that are executed via <see cref="RemotionTestCaseSourceAttribute"/>
  /// </summary>
  public class DomainPropertyControlSelectorTestCaseFactory<TControlSelector, TControl>
      : ControlSelectorTestCaseFactoryBase<TControlSelector, TControl, DomainPropertyGenericTestPageParameter>
      where TControlSelector : IDomainPropertyControlSelector<TControl>
      where TControl : ControlObject
  {
    public DomainPropertyControlSelectorTestCaseFactory ()
    {
    }

    /// <inheritdoc />
    protected override string TestPrefix
    {
      get { return "DomainPropertyControlSelector"; }
    }

    [GenericPageTestMethod]
    public void Get_Returns_NotNull ()
    {
      var control = Selector.GetByDomainProperty (Parameter.VisibleDomainProperty);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameter.FoundControlID));
    }

    [Category ("LongRunning")]
    [GenericPageTestMethod]
    public void Get_Throws_MissingHtmlException ()
    {
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameter.HiddenDomainProperty)),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [GenericPageTestMethod]
    public void Get_WithClass_Returns_NotNull ()
    {
      var control = Selector.GetByDomainProperty (Parameter.VisibleDomainProperty, Parameter.CorrectDomainClass);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameter.FoundControlID));
    }

    [Category ("LongRunning")]
    [GenericPageTestMethod]
    public void Get_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameter.VisibleDomainProperty, Parameter.IncorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameter.HiddenDomainProperty, Parameter.IncorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
      Assert.That (
          (TestDelegate) (() => Selector.GetByDomainProperty (Parameter.HiddenDomainProperty, Parameter.CorrectDomainClass)),
          Throws.InstanceOf<MissingHtmlException>());
    }

    [GenericPageTestMethod]
    public void GetOrNull_Returns_NotNull ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameter.VisibleDomainProperty);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameter.FoundControlID));
    }

    [GenericPageTestMethod]
    public void GetOrNull_Returns_Null ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameter.HiddenDomainProperty);

      Assert.That (control, Is.Null);
    }

    [GenericPageTestMethod]
    public void GetOrNull_WithClass_Returns_NotNull ()
    {
      var control = Selector.GetByDomainPropertyOrNull (Parameter.VisibleDomainProperty, Parameter.CorrectDomainClass);

      Assert.That (control, Is.Not.Null);
      Assert.That (control.Scope.Id, Is.EqualTo (Parameter.FoundControlID));
    }

    [Category ("LongRunning")]
    [GenericPageTestMethod]
    public void GetOrNull_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameter.VisibleDomainProperty, Parameter.IncorrectDomainClass),
          Is.Null);
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameter.HiddenDomainProperty, Parameter.IncorrectDomainClass),
          Is.Null);
      Assert.That (
          () => Selector.GetByDomainPropertyOrNull (Parameter.HiddenDomainProperty, Parameter.CorrectDomainClass),
          Is.Null);
    }

    [GenericPageTestMethod]
    public void Exists_Returns_True ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameter.VisibleDomainProperty);

      Assert.That (controlVisible, Is.True);
    }

    [GenericPageTestMethod]
    public void Exists_Returns_False ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameter.HiddenDomainProperty);

      Assert.That (controlVisible, Is.False);
    }

    [GenericPageTestMethod]
    public void Exists_WithClass_Returns_NotNull ()
    {
      var controlVisible = Selector.ExistsByDomainProperty (Parameter.VisibleDomainProperty, Parameter.CorrectDomainClass);

      Assert.That (controlVisible, Is.True);
    }

    [Category ("LongRunning")]
    [GenericPageTestMethod]
    public void Exists_WithClass_Throws_MissingHtmlException ()
    {
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameter.VisibleDomainProperty, Parameter.IncorrectDomainClass),
          Is.False);
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameter.HiddenDomainProperty, Parameter.IncorrectDomainClass),
          Is.False);
      Assert.That (
          () => Selector.ExistsByDomainProperty (Parameter.HiddenDomainProperty, Parameter.CorrectDomainClass),
          Is.False);
    }
  }
}