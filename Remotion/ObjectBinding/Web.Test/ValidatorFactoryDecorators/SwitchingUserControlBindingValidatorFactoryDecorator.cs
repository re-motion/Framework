using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingUserControlBindingValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<UserControlBinding>, IUserControlBindingValidatorFactory
  {
    public SwitchingUserControlBindingValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<UserControlBinding> filteringFactory,
        IBocValidatorFactory<UserControlBinding> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}