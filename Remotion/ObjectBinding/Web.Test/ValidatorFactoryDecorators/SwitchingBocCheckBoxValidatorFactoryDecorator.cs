using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocCheckBoxValidatorFactoryDecorator : SwitchingValidatorFactoryDecorator<IBocCheckBox>, IBocCheckBoxValidatorFactory
  {
    public SwitchingBocCheckBoxValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocCheckBox> filteringFactory,
        IBocValidatorFactory<IBocCheckBox> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}