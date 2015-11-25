using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocReferenceValueValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<IBocReferenceValue>, IBocReferenceValueValidatorFactory
  {
    public SwitchingBocReferenceValueValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocReferenceValue> filteringFactory,
        IBocValidatorFactory<IBocReferenceValue> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}