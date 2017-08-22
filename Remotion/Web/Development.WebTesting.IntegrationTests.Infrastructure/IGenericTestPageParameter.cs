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
using JetBrains.Annotations;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Represents parameters that are passed from the generic test page to the test.
  /// </summary>
  public interface IGenericTestPageParameter
  {
    /// <summary>
    /// The amount of test parameters that are expected from the generic test page.
    /// </summary>
    /// <remarks>
    /// Setting <see cref="Count"/> to <c>0</c> will skip test parameter parsing.
    /// </remarks>
    int Count { get; }

    /// <summary>
    /// The id which will be used to identify the correct test parameters.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    /// Applies the specified <paramref name="data"/> to the properties.
    /// </summary>
    /// <remarks>
    /// The amount of items in <paramref name="data"/> are equal to <see cref="Count"/>.
    /// Only called if <see cref="Count"/> != <c>0</c>.
    /// </remarks>
    void Apply ([NotNull] GenericTestPageParameter data);
  }
}