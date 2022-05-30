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
using System.IO;
using System.Linq;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class InMemoryReflectionBusinessObjectStorageProvider : IReflectionBusinessObjectStorageProvider
  {
    private readonly AutoInitDictionary<Type, Dictionary<Guid, MemoryStream>> _reflectionBusinessObjectData =
        new AutoInitDictionary<Type, Dictionary<Guid, MemoryStream>>(() => new Dictionary<Guid, MemoryStream>());

    public InMemoryReflectionBusinessObjectStorageProvider ()
    {
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> GetObjectIDsForType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _reflectionBusinessObjectData
          .SelectMany(e => e.Value.Keys)
          .ToArray();
    }

    /// <inheritdoc />
    public Stream GetReadObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var oldMemoryStream = _reflectionBusinessObjectData[type].GetValueOrDefault(id);
      if (oldMemoryStream == null)
        return null;

      // Create a new MemoryStream because the old one might have been disposed
      var newMemoryStream = new MemoryStream(oldMemoryStream.ToArray(), false);
      _reflectionBusinessObjectData[type][id] = newMemoryStream;

      oldMemoryStream.Dispose();

      return newMemoryStream;
    }

    /// <inheritdoc />
    public Stream GetWriteObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      _reflectionBusinessObjectData[type].GetValueOrDefault(id)?.Dispose();

      var newMemoryStream = new MemoryStream();
      _reflectionBusinessObjectData[type][id] = newMemoryStream;

      return newMemoryStream;
    }
  }
}
