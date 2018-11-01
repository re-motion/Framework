using System;
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

    public IBusinessObjectProperty GetPropertyStub (bool isRequired, bool isValueType)
    {
      return GetProperyStub<IBusinessObjectProperty> (isRequired, isValueType);
    }

    public IBusinessObjectReferenceProperty GetReferencePropertyStub (bool isRequired, bool isValueType)
    {
      return GetProperyStub<IBusinessObjectReferenceProperty> (isRequired, isValueType);
    }

    private T GetProperyStub<T> (bool isRequired, bool isValueType) where T : IBusinessObjectProperty
    {
      var propertyMock = MockRepository.GenerateStub (typeof (T));
      var typeMock = MockRepository.GenerateStub<Type> ();
      typeMock.Stub (t => t.IsValueType).Return (isValueType);
      propertyMock.Stub (m => ((T)m).PropertyType).Return (typeMock);
      propertyMock.Stub (m => ((T) m).IsRequired).Return (isRequired);
      return (T)propertyMock;
    }
  }
}