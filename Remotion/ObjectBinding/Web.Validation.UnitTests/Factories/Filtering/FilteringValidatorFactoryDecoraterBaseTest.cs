using Remotion.Globalization;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  public class FilteringValidatorFactoryDecoraterBaseTest
  {
    public void SetResourceManagerMock (IControlWithResourceManager targetMock)
    {
      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager> ();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy)).Return (true);
      targetMock.Expect (c => c.GetResourceManager ()).Return (resourceManagerMock);
    }
  }
}