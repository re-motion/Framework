using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringBocReferenceValueValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundBocReferenceValueValidatorFactory (
              new IBocReferenceValueValidatorFactory[] { new BocReferenceValueValidatorFactory(), new BocValidatorFactory() });
      var factory = new FilteringBocReferenceValueValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<IBocReferenceValue>();
      control.Expect (c => c.IsRequired).Return (true);
      SetResourceManagerMock (control);

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (BocValidator) }));
    }
  }
}