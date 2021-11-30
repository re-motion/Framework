using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using JetBrains.Annotations;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// Defines a factory for validators used to validate a specific <see cref="IBusinessObjectBoundEditableWebControl"/>.
  /// </summary>
  public interface IBocValidatorFactory<in T> where T : IBusinessObjectBoundEditableWebControl
  {
    /// <summary>
    /// Creates a sequence of <see cref="BaseValidator"/> objects for the <paramref name="control"/> instance.
    /// </summary>
    /// <param name="control">The <see cref="IBusinessObjectBoundEditableWebControl"/> to create the validators for. Must not be <see langword="null" />.</param>
    /// <param name="isReadOnly"><see langword="true" /> if the <paramref name="control"/> instance is used to display a read-only value.</param>
    IEnumerable<BaseValidator> CreateValidators ([NotNull] T control, bool isReadOnly);
  }
}
