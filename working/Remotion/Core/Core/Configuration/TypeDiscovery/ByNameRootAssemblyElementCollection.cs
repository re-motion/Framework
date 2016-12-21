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
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.Configuration.TypeDiscovery
{
  /// <summary>
  /// Configures a set of <see cref="ByNameRootAssemblyElement"/> objects.
  /// </summary>
  public class ByNameRootAssemblyElementCollection : ConfigurationElementCollection, IEnumerable<ByNameRootAssemblyElement>
  {
    public override ConfigurationElementCollectionType CollectionType
    {
      get { return ConfigurationElementCollectionType.BasicMap; }
    }

    protected override ConfigurationElement CreateNewElement ()
    {
      throw new NotSupportedException ("Elements of this collection can only be created from an element name.");
    }

    protected override ConfigurationElement CreateNewElement (string elementName)
    {
      switch (elementName)
      {
        case "include":
          return new ByNameRootAssemblyElement ();
        default:
          throw new NotSupportedException ("Only elements called 'include' are supported.");
      }
    }

    protected override bool IsElementName (string elementName)
    {
      return elementName == "include";
    }

    protected override object GetElementKey (ConfigurationElement element)
    {
      return ((ByNameRootAssemblyElement) element).Name;
    }

    public new IEnumerator<ByNameRootAssemblyElement> GetEnumerator ()
    {
      foreach (var item in (IEnumerable) this)
      {
        yield return (ByNameRootAssemblyElement) item;
      }
    }

    public void Add (ByNameRootAssemblyElement element)
    {
      ArgumentUtility.CheckNotNull ("element", element);
      BaseAdd (element);
    }

    public void RemoveAt (int index)
    {
      BaseRemoveAt (index);
    }

    public void Clear ()
    {
      BaseClear ();
    }

    public NamedRootAssemblyFinder CreateRootAssemblyFinder (IAssemblyLoader assemblyLoader)
    {
      ArgumentUtility.CheckNotNull ("assemblyLoader", assemblyLoader);
      return new NamedRootAssemblyFinder (this.Select (element => element.CreateSpecification()), assemblyLoader);
    }
  }
}
