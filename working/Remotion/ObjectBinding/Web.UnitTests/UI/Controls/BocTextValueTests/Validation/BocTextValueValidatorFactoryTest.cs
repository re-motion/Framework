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
  public class BocTextValueValidatorFactoryTest
  {
    private IBocTextValueValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocTextValueValidatorFactory();
    }

    [Test]
    [TestCase (false, false, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Required/Not ReadOnly")]
    public void CreateValidators_UndefinedProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, BocTextValueType.Undefined);
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new Type[0], Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_StringProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, BocTextValueType.String);
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (LengthValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (LengthValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_StringPropertyWithMaxLength (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, BocTextValueType.String);
      control.TextBoxStyle.MaxLength = 10;
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (DateTimeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (DateTimeValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_DateTimeProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, BocTextValueType.DateTime);
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (CompareValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (CompareValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_DateProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, BocTextValueType.Date);
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (RequiredFieldValidator), typeof (NumericValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Not Required/ReadOnly")]
    [TestCase (false, false, new[] { typeof (NumericValidator) }, Description = "Not Required/Not ReadOnly")]
    public void CreateValidators_NumericProperty (bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      CheckNumericValueValidators (BocTextValueType.Byte, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Int16, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Int32, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Int64, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Decimal, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Double, isRequired, isReadonly, expectedValidatorTypes);
      CheckNumericValueValidators (BocTextValueType.Single, isRequired, isReadonly, expectedValidatorTypes);
    }

    private void CheckNumericValueValidators (BocTextValueType type, bool isRequired, bool isReadonly, Type[] expectedValidatorTypes)
    {
      var control = GetControl (isRequired, type);
      var validators = _validatorFactory.CreateValidators (control, isReadonly).ToList();

      Assert.That (validators.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
    }

    private IBocTextValue GetControl (bool isRequired, BocTextValueType valueType)
    {
      var controlMock = MockRepository.GenerateMock<IBocTextValue>();
      controlMock.Expect (c => c.ActualValueType).Return (valueType);
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