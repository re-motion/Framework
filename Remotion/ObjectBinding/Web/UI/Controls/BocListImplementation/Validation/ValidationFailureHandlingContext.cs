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
using System.Collections.Generic;
using System.Text;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// Represents the context required for handling validation failures. Contains the <see cref="IBocList"/> control to handle validation for
  /// and functionality to receive reported error messages and stores them for later use.
  /// </summary>
  public class ValidationFailureHandlingContext
  {
    /// <summary>
    /// The <see cref="IBocList"/> to be handled in this context.
    /// </summary>
    public IBocList BocList { get; }

    /// <summary>
    /// The <see cref="IBocListValidationFailureRepository"/> to be used in this context.
    /// </summary>
    public IBocListValidationFailureRepository ValidationFailureRepository => BocList.ValidationFailureRepository;

    /// <summary>
    /// Returns the number of list failures currently contained in the repository at the time of the context creation.
    /// </summary>
    public int InitialListFailureCount { get; }

    /// <summary>
    /// Returns the number of unhandled list failures currently contained in the repository at the time of the context creation.
    /// </summary>
    public int InitialUnhandledListFailureCount { get; }

    /// <summary>
    /// Returns the number of row and cell failures currently contained in the repository at the time of the context creation.
    /// </summary>
    public int InitialRowAndCellFailureCount { get; }

    /// <summary>
    /// Returns the number of unhandled row and cell failures currently contained in the repository at the time of the context creation.
    /// </summary>
    public int InitialUnhandledRowAndCellFailureCount { get; }

    private readonly List<string> _errorMessages = new();

    public ValidationFailureHandlingContext (IBocList bocList)
    {
      ArgumentUtility.CheckNotNull("bocList", bocList);

      BocList = bocList;

      var validationFailureRepository = BocList.ValidationFailureRepository;
      InitialListFailureCount = validationFailureRepository.GetListFailureCount();
      InitialUnhandledListFailureCount = validationFailureRepository.GetUnhandledListFailureCount();
      InitialRowAndCellFailureCount = validationFailureRepository.GetRowAndCellFailureCount();
      InitialUnhandledRowAndCellFailureCount = validationFailureRepository.GetUnhandledRowAndCellFailureCount();
    }

    /// <summary>
    /// Appends all reported error messages to the <see cref="StringBuilder"/>
    /// </summary>
    public void AppendErrorMessages (StringBuilder stringBuilder)
    {
      ArgumentUtility.CheckNotNull("stringBuilder", stringBuilder);

      if (stringBuilder.Length > 0 && !stringBuilder.ToString().EndsWith("\n"))
        stringBuilder.AppendLine();

      _errorMessages.ForEach(e => stringBuilder.AppendLine(e));
    }

    /// <summary>
    /// Reports an error message to this context
    /// </summary>
    /// <param name="errorMessage">Must not be <see langword="null"/> or empty.</param>
    public void ReportErrorMessage (string errorMessage)
    {
      ArgumentUtility.CheckNotNullOrEmpty("errorMessage", errorMessage);

      _errorMessages.Add(errorMessage);
    }
  }
}
