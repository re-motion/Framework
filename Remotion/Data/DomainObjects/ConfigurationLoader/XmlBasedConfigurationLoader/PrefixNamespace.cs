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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class PrefixNamespace
  {
    // types

    // static members and constants

    public static readonly PrefixNamespace QueryConfigurationNamespace = new PrefixNamespace(
        "q", "http://www.re-motion.org/Data/DomainObjects/Queries/2.0");

    // member fields

    private string _prefix;
    private string _uri;

    // construction and disposing

    public PrefixNamespace (string prefix, string uri)
    {
      ArgumentUtility.CheckNotNullOrEmpty("prefix", prefix);
      ArgumentUtility.CheckNotNullOrEmpty("uri", uri);

      _prefix = prefix;
      _uri = uri;
    }

    // methods and properties

    public string Prefix
    {
      get { return _prefix; }
    }

    public string Uri
    {
      get { return _uri; }
    }
  }
}
