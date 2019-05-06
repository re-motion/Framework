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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Extension methods for <see cref="GenericTestPageParameter"/>
  /// </summary>
  public static class GenericTestPageParameterExtensions
  {
    /// <summary>
    /// Creates a new <see cref="GenericTestPageParameter"/> with <paramref name="additionalArguments"/> appended to the previous ones.
    /// </summary>
    /// <returns></returns>
    public static GenericTestPageParameter AppendArguments (this GenericTestPageParameter parameter, params string[] additionalArguments)
    {
      ArgumentUtility.CheckNotNull ("parameter", parameter);
      ArgumentUtility.CheckNotNullOrItemsNull ("additionalArguments", additionalArguments);

      var newArguments = parameter.Arguments.Concat (additionalArguments).ToArray();
      return new GenericTestPageParameter (parameter.Name, newArguments);
    }
  }
}