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
// using NUnit.Framework;
//
using System;
using System.Linq;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// Handles all still unhandled validation failures for rows or cells.
  /// Reports a notice in the form of "errors on another page" if applicable.
  /// </summary>
  [ImplementationFor(typeof(IBocListValidationFailureHandler), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocListRowAndCellValidationFailureHandler : IBocListValidationFailureHandler
  {
    public const int Position = 500_000_000;

    public BocListRowAndCellValidationFailureHandler ()
    {
    }

    /// <summary>
    /// Handles all still unhandled validation failures by reporting a single message to the <see cref="ValidationFailureHandlingContext"/>.
    /// </summary>
    /// <remarks>
    ///   It is expected that all unhandled validation failures at this point are unhandled due to them being on a different page of the
    ///   <see cref="IBocList"/>.
    /// </remarks>
    public void HandleValidationFailures (ValidationFailureHandlingContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var hasRowsWithUnhandledValidationFailures = context.ValidationFailureRepository.GetUnhandledValidationFailuresForDataRowsAndContainingDataCells(true).Any();
      if (!hasRowsWithUnhandledValidationFailures)
        return;

      var remainingValidationFailuresText = context.BocList
          .GetResourceManager()
          .GetString(BocList.ResourceIdentifier.ValidationFailuresFoundInOtherListPagesErrorMessage);

      context.ReportErrorMessage(remainingValidationFailuresText);
    }
  }
}
