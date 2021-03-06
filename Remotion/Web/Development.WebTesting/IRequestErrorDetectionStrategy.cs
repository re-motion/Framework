﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Web.Development.WebTesting.RequestErrorDetectionStrategies;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// A request error detection strategy determines how to check for a request error.
  /// </summary>
  /// <seealso cref="AspNetRequestErrorDetectionStrategy" />
  /// <seealso cref="NullRequestErrorDetectionStrategy" />
  public interface IRequestErrorDetectionStrategy
  {
    /// <summary>
    /// Check if the given <paramref name="scope"/> contains a request error.
    /// </summary>
    /// <exception cref="WebTestException">
    /// Thrown if a request error is found.
    /// </exception>
    void CheckPageForError ([NotNull] ElementScope scope);
  }
}
