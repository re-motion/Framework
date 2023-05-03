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

namespace Remotion.ObjectBinding.Validation
{
  /// <summary>
  /// Defines the required API to determine validation errors relevant to a <see cref="Remotion.ObjectBinding.IBusinessObject"/>.
  /// </summary>
  public interface IValidationFailureMatcher
  {
    /// <summary>
    /// Gets all <see cref="Remotion.ObjectBinding.Validation.BusinessObjectValidationFailure"/>s belonging to a given <see cref="Remotion.ObjectBinding.IBusinessObject"/>
    /// based on some <see cref="Remotion.ObjectBinding.Validation.IBusinessObjectValidationResult"/>.
    /// </summary>
    /// <param name="businessObject">The <see cref="Remotion.ObjectBinding.BusinessObject"/> for which to return the failures. Must not be <see langword="null"/>.</param>
    /// <param name="validationResult">Contains the actual failures. Must not be <see langword="null"/>.</param>
    IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (IBusinessObject businessObject, IBusinessObjectValidationResult validationResult);
  }
}
