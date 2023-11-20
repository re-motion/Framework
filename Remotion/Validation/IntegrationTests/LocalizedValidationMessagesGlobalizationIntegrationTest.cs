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
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Globalization;
using Remotion.Validation.Implementation;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.Validators;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public class LocalizedValidationMessagesGlobalizationIntegrationTest : IntegrationTestBase
  {
    private static string [] cultures = { "en" , "de", "fr", "it" };

    [Test]
    public void LocalizedValidatorErrorMessages ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "Test";

      var validator = ValidationBuilder.BuildValidator<Person>();

      var result = validator.Validate(person);

      Assert.That(result.IsValid, Is.False);
      Assert.That(
          result.Errors
              .Where(e => e.ValidatedProperties.Count != 0)
              .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
          Is.EquivalentTo(new[] { "FirstName: Enter a value.", "LastName: Enter a valid value." }));
    }

    [Test]
    public void CultureIsSetBeforeValidatorIsCreated_UsesNewCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      using (new CultureScope("de-AT"))
      {
        var validator = ValidationBuilder.BuildValidator<Person>();

        var result = validator.Validate(person);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors
                .Where(e => e.ValidatedProperties.Count != 0)
                .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
            Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
      }
    }

    [Test]
    public void CultureIsSetAfterValidatorIsCreated_UsesNewCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(CultureInfo.InvariantCulture));

      var validator = ValidationBuilder.BuildValidator<Person>();

      using (new CultureScope("de-AT"))
      {
        var result = validator.Validate(person);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors
                .Where(e => e.ValidatedProperties.Count != 0)
                .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
            Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
      }
    }

    [Test]
    public void CultureIsSetAfterValidationResultIsCreated_UsesPreviousCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      var validator = ValidationBuilder.BuildValidator<Person>();

      using (new CultureScope("de-AT"))
      {
        var result = validator.Validate(person);

        // ValidationError is immutable.

        using (new CultureScope(""))
        {
          Assert.That(result.IsValid, Is.False);
          Assert.That(
              result.Errors
                  .Where(e => e.ValidatedProperties.Count != 0)
                  .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
              Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
        }
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void LengthValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new LengthValidator(40, 50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new LengthValidator(
          min: 10,
          max: 20,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          "test");

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Match(@" 10 .* 20 "));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void ExactLengthValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new ExactLengthValidator(40, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new ExactLengthValidator(
          10,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          "test");

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 10 "));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void MinimumLengthValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new MinimumLengthValidator(40, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new MinimumLengthValidator(
          10,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          "test");

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 10 "));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void MaximumLengthValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new MaximumLengthValidator(50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new MaximumLengthValidator(
          2,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          "test");

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 2 "));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void GreaterThanValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new GreaterThanValidator(50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new GreaterThanValidator(
          2,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          1);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 2"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void GreaterThanOrEqualValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new GreaterThanOrEqualValidator(50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new GreaterThanOrEqualValidator(
          3,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          2);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 3"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void LessThanValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new LessThanValidator(50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new LessThanValidator(
          2,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          3);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 2"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void LessThanOrEqualValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(new LessThanOrEqualValidator(50, new InvariantValidationMessage("fake")), propertyStub);
      var validator = new LessThanOrEqualValidator(
          1,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          2);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Contain(" 1"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void DecimalValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(
          new DecimalValidator(10, 15, false, new InvariantValidationMessage("fake")),
          propertyStub);
      var validator = new DecimalValidator(
          maxIntegerPlaces: 5,
          maxDecimalPlaces: 3,
          false,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          new decimal(278345.11676));

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Match(@" 5 .* 3 "));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void EqualValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(
          new EqualValidator(10, new InvariantValidationMessage("fake")),
          propertyStub);
      var validator = new EqualValidator(
          5,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          3);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Match("\"5\"|«5»"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void ExclusiveRangeValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(
          new ExclusiveRangeValidator(5, 7, new InvariantValidationMessage("fake")),
          propertyStub);
      var validator = new ExclusiveRangeValidator(
          2,
          4,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          4);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Match(@" 2 .* 4"));
      }
    }

    [Test]
    [TestCaseSource(nameof(cultures))]
    public void InclusiveRangeValidator_CanBuildFormattedValidationMessage (string culture)
    {
      var factory = new LocalizedValidationMessageFactory(SafeServiceLocator.Current.GetInstance<IGlobalizationService>());

      var propertyStub = new NullPropertyInformation();
      var validationMessage = factory.CreateValidationMessageForPropertyValidator(
          new InclusiveRangeValidator(5, 7, new InvariantValidationMessage("fake")),
          propertyStub);
      var validator = new InclusiveRangeValidator(
          2,
          4,
          validationMessage);

      var instanceToValidate = new object();
      var context = new PropertyValidatorContext(
          new ValidationContext(instanceToValidate),
          instanceToValidate,
          propertyStub,
          1);

      using (new CultureScope(culture))
      {
        var validationFailures = validator.Validate(context).ToArray();

        Assert.That(validationFailures.Length, Is.EqualTo(1));
        Assert.That(validationFailures[0].LocalizedValidationMessage, Does.Match(@" 2 .* 4"));
      }
    }
  }
}
