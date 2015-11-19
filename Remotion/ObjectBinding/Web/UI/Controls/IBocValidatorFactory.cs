using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using JetBrains.Annotations;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public interface IBocValidatorFactory<in T> where T : IBusinessObjectBoundEditableWebControl
  {
    IEnumerable<BaseValidator> CreateValidators ([NotNull] T control, bool isReadOnly);
  }
}