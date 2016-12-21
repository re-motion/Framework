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

namespace Remotion.Context
{
  /// <summary>
  /// Common interface for classes implementing a storage mechanism for <see cref="SafeContext"/>.
  /// </summary>
  // By default, use HttpContextStorageProvider. Only if not available, use CallContextStorageProvider.
  public interface ISafeContextStorageProvider
  {
    /// <summary>
    /// Retrieves a data item from the context storage.
    /// </summary>
    /// <param name="key">The key identifying the data item.</param>
    /// <returns>The data item identified by the given key, or <see langword="null"/> if no such item exists in the storage.</returns>
    object GetData (string key);

    /// <summary>
    /// Sets a data item in the context storage, overwriting a previous value identified by the same key.
    /// </summary>
    /// <param name="key">The key identifying the data item.</param>
    /// <param name="value">The value to be stored in the context storage.</param>
    void SetData (string key, object value);

    /// <summary>
    /// Frees the resources used by a specific data item in the context storage.
    /// </summary>
    /// <param name="key">The key identifying the data item to be freed.</param>
    void FreeData (string key);
  }
}
