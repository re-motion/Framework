using Moq;
using NUnit.Framework;

namespace Remotion.Validation.UnitTests
{
  [TestFixture]
  public class CachingValidatorProviderTest
  {
    [Test]
    public void GetValidator_Once_ReturnsBuiltInstance ()
    {
      var validatorStub = new Mock<IValidator>();
      var validatorBuilderStub = new Mock<IValidatorBuilder>();
      validatorBuilderStub.Setup(_ => _.BuildValidator(typeof (DomainType))).Returns(validatorStub.Object);

      var validatorProvider = new CachingValidatorProvider(validatorBuilderStub.Object);

      Assert.That(validatorProvider.GetValidator(typeof (DomainType)), Is.SameAs(validatorStub.Object));
    }

    [Test]
    public void GetValidator_Twice_ReturnsCachedInstance ()
    {
      var validatorBuilderStub = new Mock<IValidatorBuilder>();
      validatorBuilderStub
          .Setup(_ => _.BuildValidator(typeof (DomainType)))
          .Returns(new Mock<IValidator>().Object);

      var validatorProvider = new CachingValidatorProvider(validatorBuilderStub.Object);

      var instance1 = validatorProvider.GetValidator(typeof (DomainType));
      var instance2 = validatorProvider.GetValidator(typeof (DomainType));
      Assert.That(instance1, Is.SameAs(instance2));
    }

    private class DomainType
    {
    }
  }
}
