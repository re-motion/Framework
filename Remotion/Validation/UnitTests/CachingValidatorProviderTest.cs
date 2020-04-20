using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.Validation.UnitTests
{
  [TestFixture]
  public class CachingValidatorProviderTest
  {
    [Test]
    public void GetValidator_Once_ReturnsBuiltInstance ()
    {
      var validatorStub = MockRepository.GenerateStub<IValidator>();
      var validatorBuilderStub = MockRepository.GenerateStub<IValidatorBuilder>();
      validatorBuilderStub.Stub (_ => _.BuildValidator (typeof (DomainType))).Return (validatorStub);

      var validatorProvider = new CachingValidatorProvider (validatorBuilderStub);

      Assert.That (validatorProvider.GetValidator (typeof (DomainType)), Is.SameAs (validatorStub));
    }

    [Test]
    public void GetValidator_Twice_ReturnsCachedInstance ()
    {
      var validatorBuilderStub = MockRepository.GenerateStub<IValidatorBuilder>();
      validatorBuilderStub
          .Stub (_ => _.BuildValidator (typeof (DomainType)))
          .WhenCalled (mi => mi.ReturnValue = MockRepository.GenerateStub<IValidator>());

      var validatorProvider = new CachingValidatorProvider (validatorBuilderStub);

      var instance1 = validatorProvider.GetValidator (typeof (DomainType));
      var instance2 = validatorProvider.GetValidator (typeof (DomainType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    private class DomainType
    {
    }
  }
}
