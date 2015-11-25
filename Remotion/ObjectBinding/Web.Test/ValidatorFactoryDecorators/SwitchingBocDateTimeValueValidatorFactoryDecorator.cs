using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocDateTimeValueValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<IBocDateTimeValue>, IBocDateTimeValueValidatorFactory
  {
    public SwitchingBocDateTimeValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocDateTimeValue> filteringFactory,
        IBocValidatorFactory<IBocDateTimeValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}