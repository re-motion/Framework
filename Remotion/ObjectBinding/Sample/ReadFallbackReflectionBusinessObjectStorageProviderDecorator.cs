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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Sample
{
  public class ReadFallbackReflectionBusinessObjectStorageProviderDecorator : IReflectionBusinessObjectStorageProvider
  {
    private readonly IReflectionBusinessObjectStorageProvider _readFallbackBusinessObjectStorageProvider;
    private readonly IReflectionBusinessObjectStorageProvider _innerBusinessObjectStorageProvider;

    public ReadFallbackReflectionBusinessObjectStorageProviderDecorator (
        IReflectionBusinessObjectStorageProvider innerBusinessObjectStorageProvider,
        IReflectionBusinessObjectStorageProvider readFallbackBusinessObjectStorageProvider)
    {
      ArgumentUtility.CheckNotNull("innerBusinessObjectStorageProvider", innerBusinessObjectStorageProvider);
      ArgumentUtility.CheckNotNull("readFallbackBusinessObjectStorageProvider", readFallbackBusinessObjectStorageProvider);

      _innerBusinessObjectStorageProvider = innerBusinessObjectStorageProvider;
      _readFallbackBusinessObjectStorageProvider = readFallbackBusinessObjectStorageProvider;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> GetObjectIDsForType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _readFallbackBusinessObjectStorageProvider.GetObjectIDsForType(type)
          .Concat(_innerBusinessObjectStorageProvider.GetObjectIDsForType(type))
          .Distinct()
          .ToArray();
    }

    /// <inheritdoc />
    public Stream GetReadObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _innerBusinessObjectStorageProvider.GetReadObjectStream(type, id)
             ?? _readFallbackBusinessObjectStorageProvider.GetReadObjectStream(type, id);
    }

    /// <inheritdoc />
    public Stream GetWriteObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      return _innerBusinessObjectStorageProvider.GetWriteObjectStream(type, id);
    }
  }
}
