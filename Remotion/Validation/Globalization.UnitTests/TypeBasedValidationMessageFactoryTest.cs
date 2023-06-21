using Moq;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.Validation.Globalization.UnitTests.TestHelpers;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Globalization.UnitTests
{
  [TestFixture]
  public class TypeBasedValidationMessageFactoryTest
  {
    private TypeBasedValidationMessageFactory _factory;
    private Mock<IPropertyInformation> _propertyStub;
    private Mock<ITypeInformation> _objectStub;
    private Mock<IMemberInformationGlobalizationService> _serviceStub;

    [SetUp]
    public void SetUp ()
    {
      _serviceStub = new Mock<IMemberInformationGlobalizationService>();
      _factory = new TypeBasedValidationMessageFactory(_serviceStub.Object);

      _propertyStub = new Mock<IPropertyInformation>();
      _objectStub = new Mock<ITypeInformation>();
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithTypeNotHavingLocalizations_ReturnsNull ()
    {
      var validator = new StubPropertyValidator();
      var result = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void CreateValidationMessageForPropertyValidator_WithLocalizedType_ReturnsLocalizedValidationMessage ()
    {
      var typeInformation = TypeAdapter.Create(typeof(StubPropertyValidator));

      var validator = new StubPropertyValidator();

      var actualMessage = "LocalizedValidationMessage";
      _serviceStub
          .Setup(_ => _.TryGetTypeDisplayName(typeInformation, typeInformation, out actualMessage))
          .Returns(true);

      var result = _factory.CreateValidationMessageForPropertyValidator(validator, _propertyStub.Object);

      Assert.That(result, Is.InstanceOf<DelegateBasedValidationMessage>());
      Assert.That(result.ToString(), Is.EqualTo("LocalizedValidationMessage"));
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithTypeNotHavingLocalizations_ReturnsNull ()
    {
      var validator = new StubObjectValidator();
      var result = _factory.CreateValidationMessageForObjectValidator(validator, _objectStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void CreateValidationMessageForObjectValidator_WithLocalizedType_ReturnsLocalizedValidationMessage ()
    {
      var typeInformation = TypeAdapter.Create(typeof(StubObjectValidator));

      var validator = new StubObjectValidator();

      var actualMessage = "LocalizedValidationMessage";
      _serviceStub
          .Setup(_ => _.TryGetTypeDisplayName(typeInformation, typeInformation, out actualMessage))
          .Returns(true);

      var result = _factory.CreateValidationMessageForObjectValidator(validator, _objectStub.Object);

      Assert.That(result, Is.InstanceOf<DelegateBasedValidationMessage>());
      Assert.That(result.ToString(), Is.EqualTo("LocalizedValidationMessage"));
    }
  }
}
