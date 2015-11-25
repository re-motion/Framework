using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocListValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocListValidatorFactory (
              new IBocListValidatorFactory[] { new BocListValidatorFactory(), new BocListValidatorValidatorFactory() });
      var factory = new FilteringBocListValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocList>();
      var editModeControllerMock = MockRepository.GenerateMock<IEditModeController>();
      editModeControllerMock.Expect (c => c.IsListEditModeActive).Return (true);
      editModeControllerMock.Expect (c => c.IsRowEditModeActive).Return (true);
      editModeControllerMock.Expect (c => c.EnableEditModeValidator).Return (true);
      control.Expect (c => c.EditModeController).Return (editModeControllerMock);
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocListValidator), typeof (EditModeValidator) }));
    }
  }
}