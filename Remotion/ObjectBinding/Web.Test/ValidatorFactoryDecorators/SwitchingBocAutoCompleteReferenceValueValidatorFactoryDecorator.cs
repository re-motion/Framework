using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;
using Remotion.ObjectBinding.Web.Validation.UI.Controls.Decorators;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocAutoCompleteReferenceValueValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueValidatorFactory
  {
    public SwitchingBocAutoCompleteReferenceValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocAutoCompleteReferenceValue> filteringFactory,
        IBocValidatorFactory<IBocAutoCompleteReferenceValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory,nonFilteringFactory)
    {
    }
  }
}