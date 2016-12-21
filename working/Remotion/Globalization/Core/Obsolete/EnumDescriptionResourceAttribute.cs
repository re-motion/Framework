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
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.Globalization
{
  /// <summary>
  /// Use this attribute to specify a resource name for an enum type. 
  /// The resource file can then contain the localized versions of the individual enum values. The identifier for each enum value is built in the following format:
  /// "&lt;namespace&gt;.&lt;type name&gt;.&lt;value&gt;"
  /// </summary>
  [Obsolete ("Use MultiLingualResourcesAttribute instead. (Version 1.15.8.0")]
  [AttributeUsage (AttributeTargets.Enum, AllowMultiple = false)]
  public class EnumDescriptionResourceAttribute: Attribute, IResourcesAttribute
  {
    private readonly string _baseName;

    public EnumDescriptionResourceAttribute (string baseName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("baseName", baseName);
      
      _baseName = baseName;
    }

    public string BaseName
    {
      get { return _baseName; }
    }

    public Assembly ResourceAssembly { get { return null; } }
  }
}