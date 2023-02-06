using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Reflection;
using Remotion.Validation.Attributes.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.UnitTests.TestDomain;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Attributes.Validation
{
  [TestFixture]
  public class NotEmptyOrWhitespaceValidationAttributeTest
  {
    private NotEmptyOrWhitespaceValidationAttribute _attribute;
    private Mock<IValidationMessageFactory> _validationMessageFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _attribute = new NotEmptyOrWhitespaceValidationAttribute();
      _validationMessageFactoryStub = new Mock<IValidationMessageFactory>();
    }

    [Test]
    public void GetPropertyValidator ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      var validationMessageStub = new Mock<ValidationMessage>();
      _validationMessageFactoryStub
          .Setup(_ => _.CreateValidationMessageForPropertyValidator(It.IsAny<NotEmptyOrWhitespaceValidator>(), propertyInformation))
          .Returns(validationMessageStub.Object);

      var result = _attribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(result[0], Is.TypeOf(typeof(NotEmptyOrWhitespaceValidator)));
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage, Is.Not.Null);

      validationMessageStub.Setup(_ => _.ToString()).Returns("Stub Message");
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("Stub Message"));
    }

    [Test]
    public void GetPropertyValidator_CustomErrorMessage ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      _attribute.ErrorMessage = "CustomMessage";

      var result = _attribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object).ToArray();

      Assert.That(result.Length, Is.EqualTo(1));
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage, Is.InstanceOf<InvariantValidationMessage>());
      Assert.That(((NotEmptyOrWhitespaceValidator)result[0]).ValidationMessage.ToString(), Is.EqualTo("CustomMessage"));
    }

    [Test]
    [Ignore("TODO RM-5906")]
    public void GetPropertyValidator_WithValidationMessageFactoryReturnsNull_ThrowsInvalidOperationException ()
    {
      var propertyInformation = PropertyInfoAdapter.Create(typeof(Customer).GetProperty("LastName"));
      Assert.Throws<InvalidOperationException>(() => _attribute.GetPropertyValidators(propertyInformation, _validationMessageFactoryStub.Object));
    }
  }
}
