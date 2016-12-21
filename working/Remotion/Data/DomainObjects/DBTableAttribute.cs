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
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Use the <see cref="DBTableAttribute"/> to define the distribution of the types within a persistence hierarchy into one or more database tables.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class DBTableAttribute: Attribute, IStorageSpecificIdentifierAttribute
  {
    private string _name;

    /// <summary>
    /// Initializes a new instance of the <see cref="DBTableAttribute"/> class.
    /// </summary>
    public DBTableAttribute ()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="DBTableAttribute"/> class with a custom table name.</summary>
    /// <param name="name">The name of the table. Must not be <see langword="null" /> or empty.</param>
    public DBTableAttribute (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      _name = name;
    }

    /// <summary>
    /// Gets the table name defined by this <see cref="DBTableAttribute"/>.
    /// </summary>
    public string Name
    {
      get { return _name; }
    }

    string IStorageSpecificIdentifierAttribute.Identifier
    {
      get { return Name; }
    }
  }
}
