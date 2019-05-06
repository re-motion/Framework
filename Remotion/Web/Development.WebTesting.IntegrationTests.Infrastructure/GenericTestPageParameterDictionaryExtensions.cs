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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Extension methods for <see cref="Dictionary{String,GenericTestPageParameter}"/>
  /// simplifying the addition of new <see cref="GenericTestPageParameter"/>.
  /// </summary>
  public static class GenericTestPageParameterDictionaryExtensions
  {
    /// <summary>
    /// Adds an existing <see cref="GenericTestPageParameter"/> to the dictionary.
    /// </summary>
    public static void Add ([NotNull] this Dictionary<string, GenericTestPageParameter> dictionary, [NotNull] GenericTestPageParameter parameter)
    {
      ArgumentUtility.CheckNotNull ("dictionary", dictionary);
      ArgumentUtility.CheckNotNull ("parameter", parameter);

      if (!dictionary.ContainsKey (parameter.Name))
      {
        dictionary.Add (parameter.Name, parameter);
      }
    }

    /// <summary>
    /// Creates a new <see cref="GenericTestPageParameter"/> and adds it to the dictionary.
    /// </summary>
    public static void Add (
        [NotNull] this Dictionary<string, GenericTestPageParameter> dictionary,
        [NotNull] string name,
        [NotNull] params string[] arguments)
    {
      ArgumentUtility.CheckNotNull ("dictionary", dictionary);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNullOrItemsNull ("arguments", arguments);

      var parameter = new GenericTestPageParameter (name, arguments);
      dictionary.Add (parameter);
    }
  }
}