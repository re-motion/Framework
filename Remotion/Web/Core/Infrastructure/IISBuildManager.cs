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
using System.Web.Compilation;
using Remotion.Utilities;

namespace Remotion.Web.Infrastructure
{
  /// <summary>
  /// Implementation of <see cref="IBuildManager"/> intended to be used when the code is running as an application in <b>IIS</b> 
  /// or the Visual Studio development web server (Cassini). Calls are delegated to <see cref="BuildManager"/>.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  public class IISBuildManager : IBuildManager
  {
    public IISBuildManager ()
    {
    }

    public Type GetType (string typeName, bool throwOnError, bool ignoreCase)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);
      return BuildManager.GetType (typeName, throwOnError: throwOnError, ignoreCase: ignoreCase);
    }

    public Type GetCompiledType (string virtualPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("virtualPath", virtualPath);
      return BuildManager.GetCompiledType (virtualPath);
    }

    public IList CodeAssemblies
    {
      get { return BuildManager.CodeAssemblies; }
    }
  }
}