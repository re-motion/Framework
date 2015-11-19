using System;
using System.Linq;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [TestFixture]
  public class BocDateTimeValueValidatorFactoryTest
  {
    private IBocDateTimeValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocDateTimeValueValidatorFactory();
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (BocDateTimeValueValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (BocDateTimeValueValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    private IBocDateTimeValue GetControl (bool isRequired)
    {
      var bocTextValueMock = MockRepository.GenerateMock<IBocDateTimeValue> ();
      bocTextValueMock.Expect (c => c.IsRequired).Return (isRequired);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager> ();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .IgnoreArguments ()
          .Return (true);

      bocTextValueMock.Expect (c => c.GetResourceManager ()).Return (resourceManagerMock);
      bocTextValueMock.Expect (c => c.TargetControl).Return (new Control () { ID = "ID" });

      return bocTextValueMock;
    }
  }
}