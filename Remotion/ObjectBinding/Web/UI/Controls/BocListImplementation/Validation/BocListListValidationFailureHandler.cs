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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// Handles all validation failures for the <see cref="IBusinessObjectProperty"/> of the <see cref="IBocList"/>.
  /// </summary>
  [ImplementationFor(typeof(IBocListValidationFailureHandler), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocListListValidationFailureHandler : IBocListValidationFailureHandler
  {
    public const int Position = 1_000_000_000;

    public BocListListValidationFailureHandler ()
    {
    }

    /// <summary>
    ///   Handles all unhandled validation failures on the <see cref="IBusinessObjectProperty"/>
    ///   of the <see cref="IBocList"/>.
    ///   Reports all found failures as separate messages to the <see cref="ValidationFailureHandlingContext"/>.
    /// </summary>
    public void HandleValidationFailures (ValidationFailureHandlingContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);

      var validationFailures = context.ValidationFailureRepository.GetUnhandledValidationFailuresForBocList(true);

      foreach (var validationFailure in validationFailures)
      {
        context.ReportErrorMessage(validationFailure.Failure.ErrorMessage);
      }
    }
  }
}
