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
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation
{
  /// <summary>
  /// The <see cref="IBocListValidationFailureHandler"/> interface provides functionality to handle validation failures.
  /// </summary>
  /// <remarks>
  ///   New inheritors of the <see cref="IBocListValidationFailureHandler"/> interface should have a position between -10e9 and 10e9.
  /// </remarks>
  /// <seealso cref="BocListListValidationFailureHandler"/>
  /// <seealso cref="BocListRowAndCellValidationFailureHandler"/>
  public interface IBocListValidationFailureHandler
  {
    /// <summary>
    /// Handles validation failures.
    /// </summary>
    /// <remarks>
    /// <para>
    ///   Implementations of this method should handle some part of the validation failures belonging to the <see cref="IBocList"/> found in its
    ///   <see cref="IBocListValidationFailureRepository"/>.
    /// </para>
    /// <para>
    ///   Use <paramref name="context"/> to access the <see cref="IBocList"/>
    ///   and <see cref="IBocListValidationFailureRepository" /> and for reporting the relevant validation failures.
    /// </para>
    /// <para>
    ///   All remaining unhandled failures will be handled by the <see cref="BocListRowAndCellValidationFailureHandler"/> (provided the service has not been removed).
    /// </para>
    /// <para>
    ///   For the <see cref="IBocListValidationFailureRepository"/> to contain any business object property validation failures,
    ///   your implementation has to be placed before the <see cref="BocListListValidationFailureHandler"/> in the set of <see cref="IBocListValidationFailureHandler"/>
    ///   implementations.
    /// </para>
    /// </remarks>
    void HandleValidationFailures (ValidationFailureHandlingContext context);
  }
}
