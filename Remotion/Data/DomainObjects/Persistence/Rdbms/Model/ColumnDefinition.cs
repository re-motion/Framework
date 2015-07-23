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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// Represents a column in a relational database.
  /// </summary>
  public class ColumnDefinition
  {
    private readonly string _name;
    private readonly bool _isPartOfPrimaryKey;
    private readonly IStorageTypeInformation _storageTypeInfo;

    public ColumnDefinition (string name, IStorageTypeInformation storageTypeInfo, bool isPartOfPrimaryKey)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("storageTypeInfo", storageTypeInfo);
      
      _name = name;
      _storageTypeInfo = storageTypeInfo;
      _isPartOfPrimaryKey = isPartOfPrimaryKey;
    }

    public string Name
    {
      get { return _name; }
    }

    public IStorageTypeInformation StorageTypeInfo
    {
      get { return _storageTypeInfo; }
    }

    public bool IsPartOfPrimaryKey
    {
      get { return _isPartOfPrimaryKey; }
    }

    public override string ToString ()
    {
      return Name;
    }
  }
}