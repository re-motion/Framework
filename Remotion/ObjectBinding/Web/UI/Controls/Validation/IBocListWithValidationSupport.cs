using System.Collections.Generic;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public interface IBocListWithValidationSupport
  {
    IEnumerable<IBocColumnDefinitionWithValidationSupport> GetColumnsWithValidationSupport ();
    BocListValidationFailureRepository ValidationFailureRepository { get; }
  }
}
