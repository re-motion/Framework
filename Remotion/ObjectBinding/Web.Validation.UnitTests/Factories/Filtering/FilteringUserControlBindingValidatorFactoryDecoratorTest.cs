﻿using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories.Decorators;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringUserControlBindingValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundUserControlBindingValidatorFactory (
              new IUserControlBindingValidatorFactory[] {new UserControlBindingValidatorFactory(), new FluentValidationUserControlBindingValidatorFactory() });
      var factory = new FilteringUserControlBindingValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<UserControlBinding>();

      var validators = factory.CreateValidators (control, false).ToArray();
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (UserControlBindingValidationFailureDisptacher), typeof(UserControlBindingValidator) }));
      Assert.That (validators, Has.All.Property ("EnableViewState").False);
    }
  }
}