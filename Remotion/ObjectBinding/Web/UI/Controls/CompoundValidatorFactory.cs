using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public class CompoundValidatorFactory<T> : IBocValidatorFactory<T> where T : IBusinessObjectBoundEditableWebControl
  {
    private readonly IReadOnlyCollection<IBocValidatorFactory<T>> _innerFactories;

    public IReadOnlyCollection<IBocValidatorFactory<T>> VlidatorFactories
    {
      get { return _innerFactories; }
    }

    public CompoundValidatorFactory (IEnumerable<IBocValidatorFactory<T>> innerFactories)
    {
      ArgumentUtility.CheckNotNull ("innerFactories", innerFactories);

      _innerFactories = innerFactories.ToList().AsReadOnly();
    }

    public IEnumerable<BaseValidator> CreateValidators (T control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      return _innerFactories.SelectMany (i => i.CreateValidators (control, isReadOnly));
    }
  }
}