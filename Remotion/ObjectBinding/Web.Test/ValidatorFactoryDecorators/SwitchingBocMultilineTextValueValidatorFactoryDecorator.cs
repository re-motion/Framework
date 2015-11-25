using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocMultilineTextValueValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<IBocMultilineTextValue>, IBocMultilineTextValueValidatorFactory
  {
    public SwitchingBocMultilineTextValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocMultilineTextValue> filteringFactory,
        IBocValidatorFactory<IBocMultilineTextValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}