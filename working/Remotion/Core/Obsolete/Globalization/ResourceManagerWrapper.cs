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
using System.Collections.Specialized;
using System.Globalization;
using System.Resources;

// ReSharper disable once CheckNamespace

namespace Remotion.Globalization.Implementation
{
  [Obsolete ("Dummy declaration for DependDB. Moved to Remotion.Globalization.dll", true)]
  internal abstract class ResourceManagerWrapper
  {
    public static ResourceManagerSet CreateWrapperSet (IEnumerable<ResourceManager> resourceManagers)
    {
      throw new NotImplementedException();
    }

    public ResourceManagerWrapper (ResourceManager resourceManager)
    {
      throw new NotImplementedException();
    }

    public abstract ResourceManager ResourceManager { get; }


    public abstract NameValueCollection GetAllStrings (string prefix);

    public abstract bool TryGetString (string id, out string value);

    public static CultureInfo[] GetCultureHierarchy (CultureInfo mostSpecialized)
    {
      throw new NotImplementedException();
    }
  }
}