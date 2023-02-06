using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleBuilders;
using Remotion.Validation.Rules;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests
{
  [TestFixture]
  public class DefaultValidatorExtensionsTest
  {
    [Test]
    public void NotNull_ForAny_ReturnsNotNullValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotNull();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotNullValidator>());
    }

    [Test]
    public void NotEmpty_ForStringArray_ReturnsNotEmptyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string[]>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
    }

    [Test]
    public void NotEmpty_ForByteArray_ReturnsNotEmptyBinaryValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, byte[]>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyBinaryValidator>());
    }

    [Test]
    public void NotEmpty_ForString_ReturnsNotEmptyStringValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyStringValidator>());
    }

    [Test]
    public void NotEmpty_ForCollection_ReturnsNotEmptyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, ICollection>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
    }

    [Test]
    public void NotEmpty_ForCollection_ReturnsNotEmptyGenericCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, ICollection<object>>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
    }

    [Test]
    public void NotEmpty_ForCollection_ReturnsNotEmptyReadOnlyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, IReadOnlyCollection<object>>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.NotEmpty();

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
    }

    [Test]
    public void Length_ForString_ReturnsLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.Length(17, 42);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LengthValidator>());
      var lengthValidator = (LengthValidator)createdValidator;
      Assert.That(lengthValidator.Min, Is.EqualTo(17));
      Assert.That(lengthValidator.Max, Is.EqualTo(42));
    }

    [Test]
    public void MinLength_ForString_ReturnsLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.MinLength(17);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LengthValidator>());
      var lengthValidator = (LengthValidator)createdValidator;
      Assert.That(lengthValidator.Min, Is.EqualTo(17));
      Assert.That(lengthValidator.Max, Is.Null);
    }

    [Test]
    public void MaxLength_ForString_ReturnsLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.MaxLength(42);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LengthValidator>());
      var lengthValidator = (LengthValidator)createdValidator;
      Assert.That(lengthValidator.Min, Is.EqualTo(0));
      Assert.That(lengthValidator.Max, Is.EqualTo(42));
    }

    [Test]
    public void ExactLength_ForString_ReturnsLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.ExactLength(23);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LengthValidator>());
      var lengthValidator = (LengthValidator)createdValidator;
      Assert.That(lengthValidator.Min, Is.EqualTo(23));
      Assert.That(lengthValidator.Max, Is.EqualTo(23));
    }

    [Test]
    public void Equal_ForObject_ReturnsEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var compareValue = new object();
      addingPropertyValidationRuleBuilder.Object.Equal(compareValue);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<EqualValidator>());
      var equalValidator = (EqualValidator)createdValidator;
      Assert.That(equalValidator.ComparisonValue, Is.EqualTo(compareValue));
    }

    [Test]
    public void NotEqual_ForObject_ReturnsNotEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var compareValue = new object();
      addingPropertyValidationRuleBuilder.Object.NotEqual(compareValue);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEqualValidator>());
      var notEqualValidator = (NotEqualValidator)createdValidator;
      Assert.That(notEqualValidator.ComparisonValue, Is.EqualTo(compareValue));
    }

    [Test]
    public void LessThan_ForInt_ReturnsLessThanValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.LessThan(23);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LessThanValidator>());
      var lessThanValidator = (LessThanValidator)createdValidator;
      Assert.That(lessThanValidator.ComparisonValue, Is.EqualTo(23));
    }

    [Test]
    public void LessThanOrEqual_ForInt_ReturnsLessThanOrEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.LessThanOrEqual(23);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<LessThanOrEqualValidator>());
      var lessThanValidator = (LessThanOrEqualValidator)createdValidator;
      Assert.That(lessThanValidator.ComparisonValue, Is.EqualTo(23));
    }

    [Test]
    public void GreaterThan_ForInt_ReturnsGreaterThanValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.GreaterThan(23);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<GreaterThanValidator>());
      var greaterThanValidator = (GreaterThanValidator)createdValidator;
      Assert.That(greaterThanValidator.ComparisonValue, Is.EqualTo(23));
    }

    [Test]
    public void GreaterThanOrEqual_ForInt_ReturnsGreaterThanOrEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.GreaterThanOrEqual(23);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<GreaterThanOrEqualValidator>());
      var greaterThanOrEqualValidator = (GreaterThanOrEqualValidator)createdValidator;
      Assert.That(greaterThanOrEqualValidator.ComparisonValue, Is.EqualTo(23));
    }

    [Test]
    public void ExclusiveBetween_ForInt_ReturnsExclusiveRangeValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.ExclusiveBetween(17, 42);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<ExclusiveRangeValidator>());
      var exclusiveRangeValidator = (ExclusiveRangeValidator)createdValidator;
      Assert.That(exclusiveRangeValidator.From, Is.EqualTo(17));
      Assert.That(exclusiveRangeValidator.To, Is.EqualTo(42));
    }

    [Test]
    public void InclusiveBetween_ForInt_ReturnsInclusiveRangeValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.InclusiveBetween(17, 42);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<InclusiveRangeValidator>());
      var inclusiveRangeValidator = (InclusiveRangeValidator)createdValidator;
      Assert.That(inclusiveRangeValidator.From, Is.EqualTo(17));
      Assert.That(inclusiveRangeValidator.To, Is.EqualTo(42));
    }

    [Test]
    public void DecimalValidator_ForDecimal_ReturnsDecimalValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, decimal>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.DecimalValidator(8, 3);

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<DecimalValidator>());
      var decimalValidator = (DecimalValidator)createdValidator;
      Assert.That(decimalValidator.MaxIntegerPlaces, Is.EqualTo(8));
      Assert.That(decimalValidator.MaxDecimalPlaces, Is.EqualTo(3));
    }

    [Test]
    public void Matches_ForString_ReturnsRegularExpressionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilder.Object.Matches(".*");

      addingPropertyValidationRuleBuilder.Verify();
      Assert.That(createdValidator, Is.InstanceOf<RegularExpressionValidator>());
      var regularExpressionValidator = (RegularExpressionValidator)createdValidator;
      Assert.That(regularExpressionValidator.Regex.ToString(), Is.EqualTo(".*"));
    }

    [Test]
    public void Must_ForObject_ReturnsPredicateValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilder = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilder.Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var predicate = new Mock<PredicateValidator.Predicate>();
      predicate.Setup(_ => _.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<PropertyValidatorContext>()))
          .Returns(true);

      addingPropertyValidationRuleBuilder.Object.Must(predicate.Object);
      addingPropertyValidationRuleBuilder.Verify();

      Assert.That(createdValidator, Is.InstanceOf<PredicateValidator>());
      var predicateValidator = (PredicateValidator)createdValidator;

      var instanceToValidate = new object();
      var property = new Mock<IPropertyInformation>();
      predicateValidator.Validate(new PropertyValidatorContext(new ValidationContext(instanceToValidate), instanceToValidate, property.Object, null));
      predicate.Verify();
    }
  }
}
