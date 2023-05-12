using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Validation.Results;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Rules
{
  [TestFixture]
  public class ObjectValidationRuleTest
  {
    [Test]
    public void Validate_ReturnsValidationFailures ()
    {
      Func<int, bool> condition = i => true;
      var context = new ValidationContext(1);

      var failure1 = new ObjectValidationFailure(
          15,
          "15 is not a number",
          "Localized validation message: 15 is not a number.");

      var failure2 = new ObjectValidationFailure(
          42,
          "42 is not a number",
          "Localized validation message: 42 is not a number.");

      var validator1Mock = new Mock<IObjectValidator>();
      validator1Mock
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(new[] { failure1 })
          .Verifiable();

      var validator2Mock = new Mock<IObjectValidator>();
      validator2Mock
          .Setup(_ => _.Validate(It.IsAny<ObjectValidatorContext>()))
          .Returns(new[] { failure2 })
          .Verifiable();

      var rule = new ObjectValidationRule<int>(condition, new[] { validator1Mock.Object, validator2Mock.Object });

      var result = rule.Validate(context);
      Assert.That(result, Is.EquivalentTo(new[] { failure1, failure2 }));
      validator2Mock.Verify();
      validator1Mock.Verify();
    }

    [Test]
    public void Validate_TryToValidateNull_ReturnsEmpty ()
    {
      Func<object, bool> condition = _ => true;

      var context = new ValidationContext(null);
      var rule = new ObjectValidationRule<object>(condition, new List<IObjectValidator>());

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithNullCondition_ReturnsEmpty ()
    {
      Func<object, bool> condition = null;

      var context = new ValidationContext(new object());
      var rule = new ObjectValidationRule<object>(condition, new List<IObjectValidator>());

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Validate_WithUnfulfilledCondition_ReturnsEmpty ()
    {
      Func<int, bool> condition = i => i > 0;

      var context = new ValidationContext(-1);
      var rule = new ObjectValidationRule<int>(condition, new List<IObjectValidator>());

      var result = rule.Validate(context);

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void IsActive_TryToValidateNull_ReturnsFalse ()
    {
      Func<object, bool> condition = _ => true;

      var context = new ValidationContext(null);
      var rule = new ObjectValidationRule<object>(condition, new List<IObjectValidator>());

      var result = rule.IsActive(context);

      Assert.That(result, Is.False);
    }

    [Test]
    public void IsActive_WithNullCondition_ReturnsTrue ()
    {
      Func<object, bool> condition = null;

      var context = new ValidationContext(new object());
      var rule = new ObjectValidationRule<object>(condition, new List<IObjectValidator>());

      var result = rule.IsActive(context);

      Assert.That(result, Is.True);
    }

    [Test]
    public void IsActive_ReturnsValueFromCondition ()
    {
      Func<object, bool> alwaysTrueCondition = _ => true;
      Func<object, bool> alwaysFalseCondition = _ => false;

      var context = new ValidationContext(new object());
      var trueRule = new ObjectValidationRule<object>(alwaysTrueCondition, new List<IObjectValidator>());
      var falseRule = new ObjectValidationRule<object>(alwaysFalseCondition, new List<IObjectValidator>());

      var resultTrueCondition = trueRule.IsActive(context);
      var resultFalseCondition = falseRule.IsActive(context);

      Assert.That(resultTrueCondition, Is.True);
      Assert.That(resultFalseCondition, Is.False);
    }
  }
}
