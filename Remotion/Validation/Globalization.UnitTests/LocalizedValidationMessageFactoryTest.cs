using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class LocalizedValidationMessageFactoryTest
  {
    private LocalizedValidationMessageFactory _factory;
    private IPropertyInformation _propertyStub;

    [SetUp]
    public void SetUp ()
    {
      _factory = new LocalizedValidationMessageFactory (SafeServiceLocator.Current.GetInstance<IGlobalizationService>());
      _propertyStub = MockRepository.GenerateStub<IPropertyInformation>();
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationMessageForPropertyValidator_WithTypeNotIPropertyValidator_ThrowsArgumentException ()
    {
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (EqualValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      //TODO RM-5906: for all tests Assert.That (((ResourceManagerBasedValidationMessage) validationMessage).ResourceIdentifier, Is.EqualTo (...));

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter the value \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (ExactLengthValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter exactly {0} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (ExclusiveRangeValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value between \"{0}\" and \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (GreaterThanOrEqualValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (GreaterThanValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithInclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (InclusiveRangeValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value between \"{0}\" and \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (LengthValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (LessThanOrEqualValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (LessThanValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (MaximumLengthValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (MinimumLengthValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (NotEmptyValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (NotEqualValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a valid value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (NotNullValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithPredicateValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (PredicateValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value that meets the specified condition."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithRegularExpressionValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (RegularExpressionValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      var localizedValidationMessageString = validationMessage.ToString();
      Assert.That (localizedValidationMessageString, Is.EqualTo ("Enter a value in the correct format."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithScalePrecisionValidator_ReturnsLocalizedValidationMessage ()
    {
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (typeof (ScalePrecisionValidator), _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.StringStarting ("Enter a decimal number"));
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationMessageForObjectValidator_NoLocalizationsAvailableForObjectValidators_ReturnsNull ()
    {
    }

    [Test]
    [Ignore ("TODO RM-5906")]
    public void CreateValidationMessageForObjectValidator_WithLocalizedType_ReturnsLocalizedValidationMessage ()
    {
    }
  }
}