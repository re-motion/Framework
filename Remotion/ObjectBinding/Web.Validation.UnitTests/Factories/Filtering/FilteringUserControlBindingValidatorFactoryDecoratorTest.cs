using System;
using System.Linq;
using NUnit.Framework;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Factories;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Validation.UnitTests.Factories.Filtering
{
  [TestFixture]
  public class FilteringUserControlBindingValidatorFactoryDecoratorTest : FilteringValidatorFactoryDecoraterBaseTest
  {
    [Test]
    public void CreateValidators ()
    {
      var compoundFactory =
          new CompoundUserControlBindingValidatorFactory (
              new IUserControlBindingValidatorFactory[] { new UserControlBindingValidatorValidatorFactory() });
      var factory = new FilteringUserControlBindingValidatorFactoryDecorator (compoundFactory);

      var control = MockRepository.GenerateMock<UserControlBinding>();

      var validators = factory.CreateValidators (control, false);
      Assert.That (
          validators.Select (v => v.GetType()),
          Is.EquivalentTo (new[] { typeof (UserControlBindingValidator) }));
    }
  }
}