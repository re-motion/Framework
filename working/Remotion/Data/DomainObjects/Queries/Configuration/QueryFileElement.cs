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
using System.Configuration;
using System.IO;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  public class QueryFileElement : ConfigurationElement, INamedConfigurationElement
  {
    public static string GetRootedPath (string path)
    {
      ArgumentUtility.CheckNotNull ("path", path);
      if (Path.IsPathRooted (path))
        return Path.GetFullPath (path);
      else
        return Path.GetFullPath (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, path));
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _queryFileFileNameProperty;

    public QueryFileElement ()
    {
      _queryFileFileNameProperty = new ConfigurationProperty (
          "filename",
          typeof (string),
          null,
          ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

      _properties.Add (_queryFileFileNameProperty);
    }

    public QueryFileElement (string fileName) : this()
    {
      ArgumentUtility.CheckNotNull ("fileName", fileName);

      FileName = fileName;
    }

    public string FileName
    {
      get { return (string) this[_queryFileFileNameProperty]; }
      protected set { this[_queryFileFileNameProperty] = value; }
    }

    public string RootedFileName
    {
      get { return GetRootedPath (FileName); }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    string INamedConfigurationElement.Name
    {
      get { return FileName; }
    }
  }
}
