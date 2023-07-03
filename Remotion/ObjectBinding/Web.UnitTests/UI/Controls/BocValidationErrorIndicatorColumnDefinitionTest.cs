using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls
{
  [TestFixture]
  public class BocValidationErrorIndicatorColumnDefinitionTest
  {
    [Test]
    public void GetComparer_ReturnsInstanceOfBocListRowWithValidationFailureComparer ()
    {
      var column = new BocValidationErrorIndicatorColumnDefinition();

      var failureRepository = new Mock<IBocListValidationFailureRepository>();
      var bocListStub = new Mock<IBocList>();
      bocListStub.Setup(_ => _.ValidationFailureRepository).Returns(failureRepository.Object);
      column.OwnerControl = bocListStub.Object;

      var comparer = column.CreateCellValueComparer();
      Assert.That(comparer, Is.InstanceOf<BocListRowWithValidationFailureComparer>());
      Assert.That(((BocListRowWithValidationFailureComparer)comparer).ValidationFailureRepository, Is.SameAs(failureRepository.Object));
    }
  }
}
