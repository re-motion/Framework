using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ServiceLocation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.Validation
{
  [TestFixture]
  public class IBocListValidationFailureHandlerTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
    }

    [Test]
    public void GetInstance_Once ()
    {
      var service = _serviceLocator.GetInstance<IBocListValidationFailureHandler>();

      Assert.That(service, Is.Not.Null);
      Assert.That(service, Is.TypeOf(typeof(CompoundBocListValidationFailureHandler)));
      Assert.That(
          ((CompoundBocListValidationFailureHandler)service).ValidationFailureHandlers.Select(s => s.GetType()),
          Is.EqualTo(
              new[]
              {
                  typeof(CheckRowsBocListValidationFailureHandler),
                  typeof(BocListRowAndCellValidationFailureHandler),
                  typeof(BocListListValidationFailureHandler)
              }));
    }

    [Test]
    public void GetInstance_Twice_ReturnsSameInstance ()
    {
      var service1 = _serviceLocator.GetInstance<IBocListValidationFailureHandler>();
      var service2 = _serviceLocator.GetInstance<IBocListValidationFailureHandler>();

      Assert.That(service1, Is.SameAs(service2));
    }
  }
}
