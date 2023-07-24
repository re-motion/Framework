// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System.Collections.Generic;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  /// <summary>
  /// Read-only wrapper around an <see cref="IBusinessObjectValidationResult"/>, which does not allow changing the <c>markedAsHandled</c> state of a validation failure.
  /// </summary>
  public class ReadOnlyBusinessObjectValidationResultDecorator : IBusinessObjectValidationResult
  {
    private readonly IBusinessObjectValidationResult _innerValidationResult;

    public ReadOnlyBusinessObjectValidationResultDecorator (IBusinessObjectValidationResult innerValidationResult)
    {
      ArgumentUtility.CheckNotNull(nameof(innerValidationResult), innerValidationResult);

      _innerValidationResult = innerValidationResult;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetValidationFailures (IBusinessObject businessObject, IBusinessObjectProperty businessObjectProperty, bool markAsHandled)
    {
      ArgumentUtility.CheckNotNull(nameof(businessObject), businessObject);

      return _innerValidationResult.GetValidationFailures(businessObject, businessObjectProperty, markAsHandled: false);
    }

    /// <inheritdoc />
    public IReadOnlyCollection<BusinessObjectValidationFailure> GetUnhandledValidationFailures (IBusinessObject businessObject, bool includePartiallyHandledFailures = false, bool markAsHandled = false)
    {
      ArgumentUtility.CheckNotNull(nameof(businessObject), businessObject);

      return _innerValidationResult.GetUnhandledValidationFailures(businessObject, includePartiallyHandledFailures, markAsHandled: false);
    }
  }
}
