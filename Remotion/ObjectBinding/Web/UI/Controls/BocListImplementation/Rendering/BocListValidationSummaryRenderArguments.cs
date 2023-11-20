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
using Remotion.ObjectBinding.Web.UI.Controls.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// <see cref="BocListValidationSummaryRenderArguments"/> is a parameter object
  /// for <see cref="BocListValidationSummaryRenderer"/>.<see cref="BocListValidationSummaryRenderer.Render"/>.
  /// </summary>
  public readonly struct BocListValidationSummaryRenderArguments
  {
    /// <summary>
    /// Gets the <see cref="IBocListColumnIndexProvider"/> that is used to create a column description if a column has no title text.
    /// </summary>
    public IBocListColumnIndexProvider ColumnIndexProvider { get; }

    /// <summary>
    /// Gets the <see cref="BocListValidationFailureWithLocationInformation"/>s that should be displayed in the validation summary.
    /// </summary>
    public IEnumerable<BocListValidationFailureWithLocationInformation> ValidationFailures { get; }

    /// <summary>
    /// Gets the row index of the row that is currently being rendered.
    /// </summary>
    public int RowIndex { get; }

    public BocListValidationSummaryRenderArguments (
        IBocListColumnIndexProvider columnIndexProvider,
        IEnumerable<BocListValidationFailureWithLocationInformation> validationFailures,
        int rowIndex)
    {
      ArgumentUtility.CheckNotNull(nameof(columnIndexProvider), columnIndexProvider);
      ArgumentUtility.CheckNotNull(nameof(validationFailures), validationFailures);

      ColumnIndexProvider = columnIndexProvider;
      ValidationFailures = validationFailures;
      RowIndex = rowIndex;
    }
  }
}
