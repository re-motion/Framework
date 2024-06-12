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
using System.Web;
using Remotion.Collections;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web.Resources;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public class ResourceObjectWithVarRef : ResourceObjectBase
  {
    private readonly WxeVariableReference _pathReference;

    public ResourceObjectWithVarRef (WxeVariableReference pathReference)
    {
      ArgumentUtility.CheckNotNull("pathReference", pathReference);
      _pathReference = pathReference;
    }

    public ResourceObjectWithVarRef (IResourcePathBuilder resourcePathBuilder, Assembly assembly, WxeVariableReference pathReference)
        : base(resourcePathBuilder , assembly)
    {
      ArgumentUtility.CheckNotNull("pathReference", pathReference);
      _pathReference = pathReference;
    }

    public override string GetResourcePath (NameObjectCollection variables)
    {
      ArgumentUtility.CheckNotNull("variables", variables);

      object? pageObject =  variables[_pathReference.Name];
      if (pageObject == null)
        throw new InvalidOperationException(string.Format("The variable '{0}' could not be found in the list of variables.", _pathReference.Name));

      string? page = pageObject as string;
      if (page == null)
      {
        throw new InvalidCastException(
            string.Format(
                "The variable '{0}' was of type '{1}'. Expected type is '{2}'.",
                _pathReference.Name,
                pageObject.GetType().GetFullNameSafe(),
                typeof(string).GetFullNameSafe()));
      }

      return VirtualPathUtility.Combine(
          VirtualPathUtility.AppendTrailingSlash(ResourceRoot),
          page);
    }

    public WxeVariableReference PathReference
    {
      get { return _pathReference; }
    }
  }
}
