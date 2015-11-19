using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  internal static class BaseValidatorReadOnlyCollectionExtension
  {
    /// <summary>
    /// Gets the validator of the given type of T or null if no validator was found.
    /// </summary>
    /// <typeparam name="T">Type of the validator.</typeparam>
    internal static T GetValidator<T> (this IReadOnlyCollection<BaseValidator> value) where T : BaseValidator
    {
      if (value == null || !value.Any ())
        return null;

      return value.OfType<T> ().FirstOrDefault ();
    } 
  }
}