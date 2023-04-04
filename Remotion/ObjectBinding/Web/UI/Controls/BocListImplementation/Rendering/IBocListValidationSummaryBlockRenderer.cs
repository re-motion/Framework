using System;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  public interface IBocListValidationSummaryBlockRenderer
  {
    void Render (BocListRenderingContext renderingContext, BocListValidationFailureRepository validationFailureRepository);
  }
}
