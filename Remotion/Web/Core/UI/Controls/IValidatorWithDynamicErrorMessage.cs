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
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// <see cref="IValidatorWithDynamicErrorMessage" /> defines an API for validators which need to have their <see cref="IValidator.ErrorMessage"/> updated at a late point.
  /// </summary>
  public interface IValidatorWithDynamicErrorMessage : IValidator
  {
    /// <summary>
    /// Performs validation on the already validated associated input control again and updates the <see cref="IValidator.ErrorMessage" /> property.
    /// </summary>
    void RefreshErrorMessage ();
  }
}
