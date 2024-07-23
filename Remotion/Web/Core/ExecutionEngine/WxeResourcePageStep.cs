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
using System.Reflection;
using Remotion.ServiceLocation;
using Remotion.Web.ExecutionEngine.Infrastructure;
using Remotion.Web.Resources;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   Calls pages that are stored in the resource directory.
  /// </summary>
  /// <remarks>
  ///   The resource directory is <c>&lt;ApplicationRoot&gt;/res/&lt;AssemblyName&gt;/</c>.
  /// </remarks>
  public class WxeResourcePageStep : WxePageStep
  {
    /// <summary>
    ///   Calls the page using the calling assemby's resource directory.
    /// </summary>
#if !NETFRAMEWORK
    [Obsolete("Use a constructor with type/assembly argument instead.", error: true, DiagnosticId = WebDiagnosticIDs.RMWEB0002_ObsoleteWxeResourcePageStepConstructor)]
#endif
    public WxeResourcePageStep (string pageName)
        : this(Assembly.GetCallingAssembly(), pageName)
    {
    }

    /// <summary>
    ///   Calls the page using the calling assemby's resource directory.
    /// </summary>
#if !NETFRAMEWORK
    [Obsolete("Use a constructor with type/assembly argument instead.", error: true, DiagnosticId = WebDiagnosticIDs.RMWEB0002_ObsoleteWxeResourcePageStepConstructor)]
#endif
    public WxeResourcePageStep (WxeVariableReference page)
        : this(Assembly.GetCallingAssembly(), page)
    {
    }

    /// <summary>
    ///   Calls the page using the resource directory of the assembly's type.
    /// </summary>
    public WxeResourcePageStep (Type resourceType, string pageName)
        : this(resourceType.Assembly, pageName)
    {
    }

    /// <summary>
    ///   Calls the page using the resource directory of the assembly's type.
    /// </summary>
    public WxeResourcePageStep (Type resourceType, WxeVariableReference page)
        : this(resourceType.Assembly, page)
    {
    }

    /// <summary>
    ///   Calls the page using the assemby's resource directory.
    /// </summary>
    public WxeResourcePageStep (Assembly resourceAssembly, string pageName)
        : base(new ResourceObject(ResourcePathBuilder, resourceAssembly, pageName))
    {
    }

    /// <summary>
    ///   Calls the page using the assemby's resource directory.
    /// </summary>
    public WxeResourcePageStep (Assembly resourceAssembly, WxeVariableReference page)
        : base(new ResourceObjectWithVarRef(ResourcePathBuilder, resourceAssembly, page))
    {
    }

    private static IResourcePathBuilder ResourcePathBuilder
    {
      get { return SafeServiceLocator.Current.GetInstance<IResourcePathBuilder>(); }
    }
  }
}
