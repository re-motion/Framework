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
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// Reports a notice in the form of "look at the rows" if inline errors have been rendered in the <see cref="BocList"/>.
  /// </summary>
  [ImplementationFor(typeof(IBocListValidationFailureHandler), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class CheckRowsBocListValidationFailureHandler : IBocListValidationFailureHandler
  {
    public const int Position = -1_000_000_000;

    public CheckRowsBocListValidationFailureHandler ()
    {
    }

    /// <inheritdoc />
    public void HandleValidationFailures (ValidationFailureHandlingContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var bocList = context.BocList;

      // To determine if failures were rendered inline in the rows, we check if any row/cell failures have been handled at the
      // time of the context creation. Checking them against the now value would return false results as other handlers might have ran already.
      var rowOrCellFailuresWereRendered = context.InitialUnhandledRowAndCellFailureCount < context.InitialRowAndCellFailureCount;
      if (rowOrCellFailuresWereRendered)
      {
        context.ReportErrorMessage(bocList.GetResourceManager().GetString(BocList.ResourceIdentifier.ValidationFailuresFoundInListErrorMessage));
      }
    }
  }
}
