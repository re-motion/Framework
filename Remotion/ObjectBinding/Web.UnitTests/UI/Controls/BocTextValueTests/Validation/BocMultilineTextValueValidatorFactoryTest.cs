using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocTextValueTests.Validation
{
  [TestFixture]
  public class BocMultilineTextValueValidatorFactoryTest
  {
    private IBocMultilineTextValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocMultilineTextValueValidatorFactory();
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (ControlCharactersCharactersValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (ControlCharactersCharactersValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired);
      var validators = _validatorFactory.CreateValidators (control, isReadonly);

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (LengthValidator), typeof (ControlCharactersCharactersValidator)  }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (LengthValidator), typeof (ControlCharactersCharactersValidator)  }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_WithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired);
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators (control, isReadonly);
      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    private IBocMultilineTextValue GetControl (bool isRequired)
    {
      var controlMock = MockRepository.GenerateMock<IBocMultilineTextValue>();
      controlMock.Expect (c => c.IsRequired).Return (isRequired);
      controlMock.Expect (c => c.TextBoxStyle).Return (new TextBoxStyle());

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }
  }
}