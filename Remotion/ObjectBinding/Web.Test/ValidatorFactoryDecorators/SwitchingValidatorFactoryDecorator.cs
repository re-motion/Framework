using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;

namespace OBWTest.ValidatorFactoryDecorators
{
  public class SwitchingValidatorFactoryDecorator<T> : IBocValidatorFactory<T>
      where T : IBusinessObjectBoundEditableWebControl
  {
    private readonly SwitchingValidatorFactoryState _switchingValidatorFactoryState;
    private readonly IBocValidatorFactory<T> _filteringFactory;
    private readonly IBocValidatorFactory<T> _nonFilteringFactory;

    public SwitchingValidatorFactoryDecorator (SwitchingValidatorFactoryState switchingValidatorFactoryState, IBocValidatorFactory<T> filteringFactory, IBocValidatorFactory<T> nonFilteringFactory)
    {
      ArgumentUtility.CheckNotNull ("filteringFactory", filteringFactory);
      ArgumentUtility.CheckNotNull ("nonFilteringFactory", nonFilteringFactory);
      ArgumentUtility.CheckNotNull ("switchingValidatorFactoryState", switchingValidatorFactoryState);
      
      _switchingValidatorFactoryState = switchingValidatorFactoryState;
      _filteringFactory = filteringFactory;
      _nonFilteringFactory = nonFilteringFactory;
    }

    public IEnumerable<BaseValidator> CreateValidators (T control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return _switchingValidatorFactoryState.UseFilteringFactory
          ? _filteringFactory.CreateValidators (control, isReadOnly)
          : _nonFilteringFactory.CreateValidators (control, isReadOnly);
    }
  }
}