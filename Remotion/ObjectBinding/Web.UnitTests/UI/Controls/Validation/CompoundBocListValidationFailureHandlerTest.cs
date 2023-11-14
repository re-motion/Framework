using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class CompoundBocListValidationFailureHandlerTest
  {
    [Test]
    public void HandleValidationFailures_CallsContainedHandlers ()
    {
      var handler1Mock = new Mock<IBocListValidationFailureHandler>();
      var handler2Mock = new Mock<IBocListValidationFailureHandler>();

      var bocListMock = new BocListMock();

      var validationFailureHandler = new CompoundBocListValidationFailureHandler(new[] { handler1Mock.Object, handler2Mock.Object });
      var context = new ValidationFailureHandlingContext(bocListMock);

      var sequence = new VerifiableSequence();
      handler1Mock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HandleValidationFailures(context))
          .Verifiable();
      handler2Mock
          .InVerifiableSequence(sequence)
          .Setup(_ => _.HandleValidationFailures(context))
          .Verifiable();

      validationFailureHandler.HandleValidationFailures(context);

      handler1Mock.Verify();
      handler2Mock.Verify();
      sequence.Verify();
    }
  }
}
