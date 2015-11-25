using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocBooleanValueValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<IBocBooleanValue>, IBocBooleanValueValidatorFactory
  {
    public SwitchingBocBooleanValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocBooleanValue> filteringFactory,
        IBocValidatorFactory<IBocBooleanValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}