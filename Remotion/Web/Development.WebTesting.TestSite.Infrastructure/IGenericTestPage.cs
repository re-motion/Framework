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
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;

namespace Remotion.Web.Development.WebTesting.TestSite.Infrastructure
{
  /// <summary>
  /// Represents a test page in the generic test page.
  /// </summary>
  public interface IGenericTestPage<in TOptions>
  {
    /// <summary>
    /// Adds the required parameters to the specified <paramref name="parameterCollection"/>.
    /// </summary>
    void AddParameters ([NotNull] Dictionary<string, GenericTestPageParameter> parameterCollection, [NotNull] TOptions options);

    /// <summary>
    /// Creates a new control from the specified <paramref name="options"/>.
    /// </summary>
    [NotNull]
    Control CreateControl ([NotNull] TOptions options);
  }
}
