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
using System;
using System.Collections.Generic;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  /// <summary>
  /// A comparer to sort <see cref="BocValidationErrorIndicatorColumnDefinition"/> columns.
  /// </summary>
  /// <remarks>
  /// The comparer only checks whether there are validation failures, not how many there are.
  /// </remarks>
  public class BocListRowWithValidationFailureComparer : IComparer<BocListRow>
  {
    public IBocListValidationFailureRepository ValidationFailureRepository { get; }

    public BocListRowWithValidationFailureComparer (IBocListValidationFailureRepository validationFailureRepository)
    {
      ArgumentUtility.CheckNotNull("validationFailureRepository", validationFailureRepository);

      ValidationFailureRepository = validationFailureRepository;
    }

    public int Compare (BocListRow? first, BocListRow? second)
    {
      return (first, second) switch
      {
        (null, null) => 0,
        (null, _) => 1,
        (_, null) => -1,
        _ => CompareRowValidationFailures(first, second)
      };
    }

    private int CompareRowValidationFailures (BocListRow first, BocListRow second)
    {
      var firstBusinessObject = first.BusinessObject;
      var secondBusinessObject = second.BusinessObject;

      var firstHasValidationFailures = ValidationFailureRepository.HasValidationFailuresForDataRow(firstBusinessObject);
      var secondHasValidationFailures = ValidationFailureRepository.HasValidationFailuresForDataRow(secondBusinessObject);

      return (firstHasValidationFailures, secondHasValidationFailures) switch
      {
        (true, false) => -1,
        (false, true) => 1,
        _ => 0
      };
    }
  }
}
