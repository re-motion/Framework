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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Security.Metadata
{
  public class MetadataExtractor
  {
    private IMetadataConverter _converter;
    private List<Assembly> _assemblies;

    public MetadataExtractor (IMetadataConverter converter)
    {
      ArgumentUtility.CheckNotNull ("converter", converter);

      _assemblies = new List<Assembly> ();
      _converter = converter;
    }

    public void AddAssembly (Assembly assembly)
    {
      _assemblies.Add (assembly);
    }

    public void AddAssembly (string assemblyPath)
    {
      if (!assemblyPath.EndsWith (".dll"))
        assemblyPath = assemblyPath + ".dll";

      Assembly assembly = Assembly.LoadFrom (assemblyPath);
      AddAssembly (assembly);
    }

    public void Save (string filename)
    {
      MetadataCache metadata = new MetadataCache ();
      AssemblyReflector reflector = new AssemblyReflector ();

      foreach (Assembly assembly in _assemblies)
        reflector.GetMetadata (assembly, metadata);

      _converter.ConvertAndSave (metadata, filename);
    }
  }
}
