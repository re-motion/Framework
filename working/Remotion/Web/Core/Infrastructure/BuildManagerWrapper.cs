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
using System.Collections;
using System.Web;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// Implements the <see cref="IBuildManager"/> interface and delegates calls to <see cref="IISBuildManager"/> or <see cref="NonHostedBuildManager"/>,
  /// depending on whether a hosted environment is detected.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IBuildManager), Lifetime = LifetimeKind.Singleton)]
  public class BuildManagerWrapper : IBuildManager
  {
    private readonly IBuildManager _innerBuildManager;

    public BuildManagerWrapper ()
    {
      if (IsUsingWebServer())
        _innerBuildManager = new IISBuildManager();
      else
        _innerBuildManager = new NonHostedBuildManager();
    }

    public Type GetType (string typeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      return _innerBuildManager.GetType (typeName, throwOnError, ignoreCase);
    }

    public Type GetCompiledType (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("virtualPath", virtualPath);
      return _innerBuildManager.GetCompiledType (virtualPath);
    }

    public IList CodeAssemblies
    {
      get { return _innerBuildManager.CodeAssemblies; }
    }

    private bool IsUsingWebServer ()
    {
      if (HttpRuntime.IISVersion != null)
        return true;
      if (HttpRuntime.AppDomainId != null)
        return true;
      return false;
    }
  }
}