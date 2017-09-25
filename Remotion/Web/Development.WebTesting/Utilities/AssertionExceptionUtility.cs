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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.Utilities
{
  /// <summary>
  /// Provides method for common exceptions
  /// </summary>
  public static class AssertionExceptionUtility
  {
    [NotNull]
    public static Exception CreateControlDisabledException ()
    {
      return new MissingHtmlException ("The control is currently in a disabled state. Therefore, the operation is not possible.");
    }

    [NotNull]
    public static Exception CreateControlDisabledException ([NotNull] string controlName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("controlName", controlName);

      return new MissingHtmlException (
          string.Format ("The '{0}' is currently in a disabled state. Therefore, the operation is not possible.", controlName));
    }

  }
}