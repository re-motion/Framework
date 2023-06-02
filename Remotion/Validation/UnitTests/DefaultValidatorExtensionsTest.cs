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
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotNull();

      Assert.That(createdValidator, Is.InstanceOf<NotNullValidator>());
      var notEmptyValidator = (NotNullValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForStringArray_ReturnsNotEmptyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string[]>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
      var notEmptyValidator = (NotEmptyCollectionValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForByteArray_ReturnsNotEmptyBinaryValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, byte[]>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyBinaryValidator>());
      var notEmptyValidator = (NotEmptyBinaryValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForCollection_ReturnsNotEmptyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, ICollection>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
      var notEmptyValidator = (NotEmptyCollectionValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForGenericCollection_ReturnsNotEmptyGenericCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, ICollection<object>>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
      var notEmptyValidator = (NotEmptyCollectionValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForReadOnlyCollectionCollection_ReturnsNotEmptyReadOnlyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, IReadOnlyCollection<object>>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
      var notEmptyValidator = (NotEmptyCollectionValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmpty_ForList_ReturnsNotEmptyReadOnlyCollectionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, List<object>>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.NotEmpty();

      Assert.That(createdValidator, Is.InstanceOf<NotEmptyCollectionValidator>());
      var notEmptyValidator = (NotEmptyCollectionValidator)createdValidator;
      Assert.That(notEmptyValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmptyOrWhitespace_ForString_ReturnsNotEmptyOrWhitespaceValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderMock = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderMock
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderMock.Object.NotEmptyOrWhitespace();

      addingPropertyValidationRuleBuilderMock.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyOrWhitespaceValidator>());
      var notEmptyOrWhitespaceValidator = (NotEmptyOrWhitespaceValidator)createdValidator;
      Assert.That(notEmptyOrWhitespaceValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEmptyOrWhitespace_ForStringArray_ReturnsNotEmptyOrWhitespaceValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderMock = new Mock<IAddingPropertyValidationRuleBuilder<object, string[]>>();
      addingPropertyValidationRuleBuilderMock
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderMock.Object.NotEmptyOrWhitespace();

      addingPropertyValidationRuleBuilderMock.Verify();
      Assert.That(createdValidator, Is.InstanceOf<NotEmptyOrWhitespaceValidator>());
      var notEmptyOrWhitespaceValidator = (NotEmptyOrWhitespaceValidator)createdValidator;
      Assert.That(notEmptyOrWhitespaceValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void Length_ForString_ReturnsLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.Length(17, 42);

      Assert.That(createdValidator, Is.InstanceOf<LengthValidator>());
      var lengthValidator = (LengthValidator)createdValidator;
      Assert.That(lengthValidator.Min, Is.EqualTo(17));
      Assert.That(lengthValidator.Max, Is.EqualTo(42));
      Assert.That(lengthValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void MinLength_ForString_ReturnsMinimumLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.MinLength(17);

      Assert.That(createdValidator, Is.InstanceOf<MinimumLengthValidator>());
      var minimumLengthValidator = (MinimumLengthValidator)createdValidator;
      Assert.That(minimumLengthValidator.Min, Is.EqualTo(17));
      Assert.That(minimumLengthValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void MaxLength_ForString_ReturnsMaximumLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.MaxLength(42);

      Assert.That(createdValidator, Is.InstanceOf<MaximumLengthValidator>());
      var maximumLengthValidator = (MaximumLengthValidator)createdValidator;
      Assert.That(maximumLengthValidator.Max, Is.EqualTo(42));
      Assert.That(maximumLengthValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void ExactLength_ForString_ReturnsExactLengthValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.ExactLength(23);

      Assert.That(createdValidator, Is.InstanceOf<ExactLengthValidator>());
      var exactLengthValidator = (ExactLengthValidator)createdValidator;
      Assert.That(exactLengthValidator.Length, Is.EqualTo(23));
      Assert.That(exactLengthValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void Equal_ForObject_ReturnsEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var compareValue = new object();
      addingPropertyValidationRuleBuilderStub.Object.Equal(compareValue);

      Assert.That(createdValidator, Is.InstanceOf<EqualValidator>());
      var equalValidator = (EqualValidator)createdValidator;
      Assert.That(equalValidator.ComparisonValue, Is.EqualTo(compareValue));
      Assert.That(equalValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void NotEqual_ForObject_ReturnsNotEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var compareValue = new object();
      addingPropertyValidationRuleBuilderStub.Object.NotEqual(compareValue);

      Assert.That(createdValidator, Is.InstanceOf<NotEqualValidator>());
      var notEqualValidator = (NotEqualValidator)createdValidator;
      Assert.That(notEqualValidator.ComparisonValue, Is.EqualTo(compareValue));
      Assert.That(notEqualValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void LessThan_ForInt_ReturnsLessThanValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.LessThan(23);

      Assert.That(createdValidator, Is.InstanceOf<LessThanValidator>());
      var lessThanValidator = (LessThanValidator)createdValidator;
      Assert.That(lessThanValidator.ComparisonValue, Is.EqualTo(23));
      Assert.That(lessThanValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void LessThanOrEqual_ForInt_ReturnsLessThanOrEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.LessThanOrEqual(23);

      Assert.That(createdValidator, Is.InstanceOf<LessThanOrEqualValidator>());
      var lessThanValidator = (LessThanOrEqualValidator)createdValidator;
      Assert.That(lessThanValidator.ComparisonValue, Is.EqualTo(23));
      Assert.That(lessThanValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void GreaterThan_ForInt_ReturnsGreaterThanValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.GreaterThan(23);

      Assert.That(createdValidator, Is.InstanceOf<GreaterThanValidator>());
      var greaterThanValidator = (GreaterThanValidator)createdValidator;
      Assert.That(greaterThanValidator.ComparisonValue, Is.EqualTo(23));
      Assert.That(greaterThanValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void GreaterThanOrEqual_ForInt_ReturnsGreaterThanOrEqualValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.GreaterThanOrEqual(23);

      Assert.That(createdValidator, Is.InstanceOf<GreaterThanOrEqualValidator>());
      var greaterThanOrEqualValidator = (GreaterThanOrEqualValidator)createdValidator;
      Assert.That(greaterThanOrEqualValidator.ComparisonValue, Is.EqualTo(23));
      Assert.That(greaterThanOrEqualValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void ExclusiveBetween_ForInt_ReturnsExclusiveRangeValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.ExclusiveBetween(17, 42);

      Assert.That(createdValidator, Is.InstanceOf<ExclusiveRangeValidator>());
      var exclusiveRangeValidator = (ExclusiveRangeValidator)createdValidator;
      Assert.That(exclusiveRangeValidator.From, Is.EqualTo(17));
      Assert.That(exclusiveRangeValidator.To, Is.EqualTo(42));
      Assert.That(exclusiveRangeValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void InclusiveBetween_ForInt_ReturnsInclusiveRangeValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, int>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.InclusiveBetween(17, 42);

      Assert.That(createdValidator, Is.InstanceOf<InclusiveRangeValidator>());
      var inclusiveRangeValidator = (InclusiveRangeValidator)createdValidator;
      Assert.That(inclusiveRangeValidator.From, Is.EqualTo(17));
      Assert.That(inclusiveRangeValidator.To, Is.EqualTo(42));
      Assert.That(inclusiveRangeValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void DecimalValidator_ForDecimal_ReturnsDecimalValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, decimal>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.DecimalValidator(8, 3);

      Assert.That(createdValidator, Is.InstanceOf<DecimalValidator>());
      var decimalValidator = (DecimalValidator)createdValidator;
      Assert.That(decimalValidator.MaxIntegerPlaces, Is.EqualTo(8));
      Assert.That(decimalValidator.MaxDecimalPlaces, Is.EqualTo(3));
      Assert.That(decimalValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void Matches_ForString_ReturnsRegularExpressionValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, string>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      addingPropertyValidationRuleBuilderStub.Object.Matches(".*");

      Assert.That(createdValidator, Is.InstanceOf<RegularExpressionValidator>());
      var regularExpressionValidator = (RegularExpressionValidator)createdValidator;
      Assert.That(regularExpressionValidator.Regex.ToString(), Is.EqualTo(".*"));
      Assert.That(regularExpressionValidator.ValidationMessage, Is.SameAs(validationMessage));
    }

    [Test]
    public void Must_ForObject_ReturnsPredicateValidator ()
    {
      var validationMessage = new InvariantValidationMessage("Fake message");
      var initParameters = new PropertyValidationRuleInitializationParameters(validationMessage);
      IPropertyValidator createdValidator = null;
      var addingPropertyValidationRuleBuilderStub = new Mock<IAddingPropertyValidationRuleBuilder<object, object>>();
      addingPropertyValidationRuleBuilderStub
          .Setup(_ => _.SetValidator(It.IsAny<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>()))
          .Callback<Func<PropertyValidationRuleInitializationParameters, IPropertyValidator>>(func => createdValidator = func.Invoke(initParameters));

      var predicateMock = new Mock<PredicateValidator.Predicate>();
      predicateMock
          .Setup(_ => _.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<PropertyValidatorContext>()))
          .Returns(true)
          .Verifiable();

      addingPropertyValidationRuleBuilderStub.Object.Must(predicateMock.Object);

      Assert.That(createdValidator, Is.InstanceOf<PredicateValidator>());
      var predicateValidator = (PredicateValidator)createdValidator;

      var instanceToValidate = new object();
      var propertyStub = new Mock<IPropertyInformation>();
      predicateValidator.Validate(new PropertyValidatorContext(new ValidationContext(instanceToValidate), instanceToValidate, propertyStub.Object, null));

      predicateMock.Verify(p => p.Invoke(It.IsAny<object>(), It.IsAny<object>(), It.IsAny<PropertyValidatorContext>()), Times.Once());
      Assert.That(predicateValidator.ValidationMessage, Is.SameAs(validationMessage));
    }
  }
}
