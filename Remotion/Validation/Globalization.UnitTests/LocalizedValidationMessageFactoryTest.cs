using System.Text.RegularExpressions;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;
using Rhino.Mocks;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class LocalizedValidationMessageFactoryTest
  {
    private LocalizedValidationMessageFactory _factory;
    private IPropertyInformation _propertyStub;
    private ValidationMessage _validationMessageStub;

    [SetUp]
    public void SetUp ()
    {
      _factory = new LocalizedValidationMessageFactory (SafeServiceLocator.Current.GetInstance<IGlobalizationService>());
      _propertyStub = MockRepository.GenerateStub<IPropertyInformation>();
      _validationMessageStub = MockRepository.GenerateStub<ValidationMessage>();
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new EqualValidator (new object(), _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      //TODO RM-5906: for all tests Assert.That (((ResourceManagerBasedValidationMessage) validationMessage).ResourceIdentifier, Is.EqualTo (...));

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter the value \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExactLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new ExactLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter exactly {0} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithExclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new ExclusiveRangeValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value between \"{0}\" and \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new GreaterThanOrEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithGreaterThanValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new GreaterThanValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value greater than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithInclusiveRangeValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new InclusiveRangeValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value between \"{0}\" and \"{1}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new LengthValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanOrEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new LessThanOrEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than or equal to \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLessThanValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new LessThanValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value less than \"{0}\"."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMaximumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new MaximumLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithMinimumLengthValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new MinimumLengthValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter at least {0} and no more than {1} characters."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEmptyValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new NotEmptyValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotEqualValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new NotEqualValidator (5, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a valid value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithNotNullValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new NotNullValidator (_validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithPredicateValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new PredicateValidator ((validatedInstance, validatedValue, context) => true, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      Assert.That (validationMessage.ToString(), Is.EqualTo ("Enter a value that meets the specified condition."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithRegularExpressionValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new RegularExpressionValidator (new Regex (".*"), _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

      Assert.That (validationMessage, Is.Not.Null);
      Assert.That (validationMessage, Is.InstanceOf<ResourceManagerBasedValidationMessage>());

      var localizedValidationMessageString = validationMessage.ToString();
      Assert.That (localizedValidationMessageString, Is.EqualTo ("Enter a value in the correct format."));
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithScalePrecisionValidator_ReturnsLocalizedValidationMessage ()
    {
      var validator = new ScalePrecisionValidator (5, 10, _validationMessageStub);
      var validationMessage = _factory.CreateValidationMessageForPropertyValidator (validator, _propertyStub);

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