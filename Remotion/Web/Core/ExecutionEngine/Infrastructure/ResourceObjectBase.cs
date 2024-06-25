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
using Remotion.Collections;
using Remotion.Utilities;
using Remotion.Web.Resources;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  public abstract class ResourceObjectBase
  {
    private readonly string _resourceRoot;

    protected ResourceObjectBase ()
    {
      _resourceRoot = "~/";
    }

    protected ResourceObjectBase (IResourcePathBuilder resourcePathBuilder, Assembly assembly)
    {
      ArgumentUtility.CheckNotNull("resourcePathBuilder", resourcePathBuilder);
      ArgumentUtility.CheckNotNull("assembly", assembly);

      _resourceRoot = resourcePathBuilder.BuildAbsolutePath(assembly);
    }

    public string ResourceRoot
    {
      get { return _resourceRoot; }
    }

    public abstract string GetResourcePath (NameObjectCollection variables);
  }
}
