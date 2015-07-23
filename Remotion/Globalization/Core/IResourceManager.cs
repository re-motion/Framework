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
using System.Collections.Specialized;
using JetBrains.Annotations;

namespace Remotion.Globalization
{
  /// <summary>
  ///   An interface for defining a string resource manager.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public interface IResourceManager : INullObject
  {
    /// <summary>Searches for all string resources inside the resource manager whose name is prefixed with a matching tag.</summary>
    /// <param name="prefix"> The prefix all returned string resources must have. Use <see langword="null"/> to return all strings.</param>
    /// <returns>A collection of string pairs, the key being the resource's ID, the value being the string.</returns>
    [NotNull]
    NameValueCollection GetAllStrings ([CanBeNull]string prefix);

    /// <summary>Tries to get the value of the specified string resource.</summary>
    /// <param name="id">The ID of the resource to get. Must not be <see langword="null" />.</param>
    /// <param name="value">The value of the resource lookup result.</param>
    /// <returns><see langword="true"/> if the <see cref="IResourceManager"/> contains the specified resource.</returns>
    bool TryGetString ([NotNull]string id, out string value);

    /// <summary>Returns the name of the resource manager.</summary>
    string Name { get; }
  }
}
