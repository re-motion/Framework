using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocTextValueValidatorFactoryDecorator : SwitchingValidatorFactoryDecorator<IBocTextValue>, IBocTextValueValidatorFactory
  {
    public SwitchingBocTextValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocTextValue> filteringFactory,
        IBocValidatorFactory<IBocTextValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}