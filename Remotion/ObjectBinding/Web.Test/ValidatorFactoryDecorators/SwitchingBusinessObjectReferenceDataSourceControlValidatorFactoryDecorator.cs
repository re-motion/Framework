using System;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingBusinessObjectReferenceDataSourceControlValidatorFactoryDecorator
      : SwitchingValidatorFactoryDecorator<BusinessObjectReferenceDataSourceControl>, IBusinessObjectReferenceDataSourceControlValidatorFactory
  {
    public SwitchingBusinessObjectReferenceDataSourceControlValidatorFactoryDecorator (
        SwitchingValidatorFactoryState switchingValidatorFactoryState,
        IBocValidatorFactory<BusinessObjectReferenceDataSourceControl> filteringFactory,
        IBocValidatorFactory<BusinessObjectReferenceDataSourceControl> nonFilteringFactory)
        : base (switchingValidatorFactoryState, filteringFactory, nonFilteringFactory)
    {
    }
  }
}