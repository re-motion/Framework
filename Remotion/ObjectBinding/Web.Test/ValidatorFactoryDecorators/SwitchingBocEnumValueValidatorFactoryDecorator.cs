using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocEnumValueValidatorFactoryDecorator : SwitchingValidatorFactoryDecorator<IBocEnumValue>, IBocEnumValueValidatorFactory
  {
    public SwitchingBocEnumValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocEnumValue> filteringFactory,
        IBocValidatorFactory<IBocEnumValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}