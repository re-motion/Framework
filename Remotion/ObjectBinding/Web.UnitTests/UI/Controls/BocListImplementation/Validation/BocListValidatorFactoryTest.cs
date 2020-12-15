using System;
using System.Linq;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation.Validation
{
  [TestFixture]
  public class BocListValidatorFactoryTest
  {
    private IBocListValidatorFactory _validatorFactory;

    [SetUp]
    public void SetUp ()
    {
      _validatorFactory = new BocListValidatorFactory();
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (EditModeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (false, false, new Type[0], Description = "Required/ReadOnly")]
    public void CreateValidators_ListEditMode (bool isListEditModeEnabled, bool isReadonly, Type[] expectedValidatorTypes)
    {
      // EnableEditModeValidator true
      var control1 = GetControl (isListEditModeEnabled, false, true);
      var validators1 = _validatorFactory.CreateValidators (control1, isReadonly).ToArray();

      Assert.That (validators1.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
      Assert.That (validators1, Has.All.Property ("EnableViewState").False);

      // EnableEditModeValidator false
      var control2 = GetControl (isListEditModeEnabled, false, false);
      var validators2 = _validatorFactory.CreateValidators (control2, isReadonly);

      Assert.That (validators2, Is.Empty);
    }

    [Test]
    [TestCase (true, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (true, false, new[] { typeof (EditModeValidator) }, Description = "Required/Not ReadOnly")]
    [TestCase (false, true, new Type[0], Description = "Required/ReadOnly")]
    [TestCase (false, false, new Type[0], Description = "Required/ReadOnly")]
    public void CreateValidators_RowEditMode (bool isRowEditModeActive, bool isReadonly, Type[] expectedValidatorTypes)
    {
      // EnableEditModeValidator true
      var control1 = GetControl (false, isRowEditModeActive, true);
      var validators1 = _validatorFactory.CreateValidators (control1, isReadonly).ToArray();

      Assert.That (validators1.Select (v => v.GetType()), Is.EquivalentTo (expectedValidatorTypes));
      Assert.That (validators1, Has.All.Property ("EnableViewState").False);

      // EnableEditModeValidator false
      var control2 = GetControl (false, isRowEditModeActive, false);
      var validators2 = _validatorFactory.CreateValidators (control2, isReadonly);

      Assert.That (validators2, Is.Empty);
    }

    private IBocList GetControl (bool isListEditModeEnabled, bool isRowEditModeActive, bool enableEditModeValidator)
    {
      var controlMock = MockRepository.GenerateMock<IBocList>();
      var editModeControllerMock = MockRepository.GenerateMock<IEditModeController>();
      editModeControllerMock.Expect (c => c.IsListEditModeActive).Return (isListEditModeEnabled);
      editModeControllerMock.Expect (c => c.IsRowEditModeActive).Return (isRowEditModeActive);
      editModeControllerMock.Expect (c => c.EnableEditModeValidator).Return (enableEditModeValidator);
      controlMock.Expect (c => c.EditModeController).Return (editModeControllerMock);
      controlMock.Expect (c => c.AreOptionalValidatorsEnabled).Return (true);

      var resourceManagerMock = MockRepository.GenerateMock<IResourceManager>();
      resourceManagerMock.Expect (r => r.TryGetString (Arg<string>.Is.Anything, out Arg<string>.Out ("MockValue").Dummy))
          .Return (true);

      controlMock.Expect (c => c.GetResourceManager()).Return (resourceManagerMock);
      controlMock.Expect (c => c.TargetControl).Return (new Control() { ID = "ID" });

      return controlMock;
    }
  }
}