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
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.ObjectBinding.Sample
{
  public class SessionStateReflectionBusinessObjectStorageProvider : IReflectionBusinessObjectStorageProvider
  {
    private static readonly string s_sessionKeyForStorageProvider = typeof(SessionStateReflectionBusinessObjectStorageProvider).GetFullNameChecked();

    private readonly IHttpContextProvider _httpContextProvider;
    private readonly IReflectionBusinessObjectStorageProviderFactory _reflectionBusinessObjectStorageProviderFactory;

    public SessionStateReflectionBusinessObjectStorageProvider (
        IHttpContextProvider httpContextProvider,
        IReflectionBusinessObjectStorageProviderFactory reflectionBusinessObjectStorageProviderFactory)
    {
      ArgumentUtility.CheckNotNull("httpContextProvider", httpContextProvider);
      ArgumentUtility.CheckNotNull("reflectionBusinessObjectStorageProviderFactory", reflectionBusinessObjectStorageProviderFactory);

      _httpContextProvider = httpContextProvider;
      _reflectionBusinessObjectStorageProviderFactory = reflectionBusinessObjectStorageProviderFactory;
    }

    /// <inheritdoc />
    public IReadOnlyCollection<Guid> GetObjectIDsForType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var reflectionBusinessObjectStorageProvider = GetReflectionBusinessObjectStorageProviderForCurrentSession();
      return reflectionBusinessObjectStorageProvider.GetObjectIDsForType(type);
    }

    /// <inheritdoc />
    public Stream GetReadObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var reflectionBusinessObjectStorageProvider = GetReflectionBusinessObjectStorageProviderForCurrentSession();
      return reflectionBusinessObjectStorageProvider.GetReadObjectStream(type, id);
    }

    /// <inheritdoc />
    public Stream GetWriteObjectStream (Type type, Guid id)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var reflectionBusinessObjectStorageProvider = GetReflectionBusinessObjectStorageProviderForCurrentSession();
      return reflectionBusinessObjectStorageProvider.GetWriteObjectStream(type, id);
    }

    private IReflectionBusinessObjectStorageProvider GetReflectionBusinessObjectStorageProviderForCurrentSession ()
    {
      var httpContext = _httpContextProvider.GetCurrentHttpContext();
      var httpSession = httpContext.Session;
      if (httpSession == null)
        return _reflectionBusinessObjectStorageProviderFactory.CreateBusinessObjectStorageProvider();

      var reflectionBusinessObjectStorageProvider = (IReflectionBusinessObjectStorageProvider) httpSession[s_sessionKeyForStorageProvider];
      if (reflectionBusinessObjectStorageProvider == null)
      {
        reflectionBusinessObjectStorageProvider = _reflectionBusinessObjectStorageProviderFactory.CreateBusinessObjectStorageProvider();
        httpSession[s_sessionKeyForStorageProvider] = reflectionBusinessObjectStorageProvider;
      }

      return reflectionBusinessObjectStorageProvider;
    }
  }
}
