using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBocListValidatorFactoryDecorator : SwitchingValidatorFactoryDecorator<IBocList>, IBocListValidatorFactory
  {
    public SwitchingBocListValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<IBocList> filteringFactory,
        IBocValidatorFactory<IBocList> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}