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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class ColumnDefinitionObjectMother
  {
    public static readonly ColumnDefinition IDColumn =
        new ColumnDefinition ("ID", StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation (false), true);

    public static readonly ColumnDefinition ClassIDColumn =
        new ColumnDefinition ("ClassID", StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation (true), false);

    public static readonly ColumnDefinition TimestampColumn =
        new ColumnDefinition ("Timestamp", StorageTypeInformationObjectMother.CreateDateTimeStorageTypeInformation (true), false);

    public static ColumnDefinition CreateColumn (string columnName = null, IStorageTypeInformation storageTypeInformation = null, bool isPartOfPrimaryKey = false)
    {
      return new ColumnDefinition (
          columnName ?? GetUniqueColumnName(),
          storageTypeInformation ?? StorageTypeInformationObjectMother.CreateStorageTypeInformation(),
          isPartOfPrimaryKey);
    }

    public static ColumnDefinition CreateColumn (IStorageTypeInformation storageTypeInformation)
    {
      return new ColumnDefinition (GetUniqueColumnName(), storageTypeInformation, false);
    }

    public static ColumnDefinition CreateStringColumn (string columnName)
    {
      return CreateColumn (columnName, StorageTypeInformationObjectMother.CreateVarchar100StorageTypeInformation());
    }

    public static ColumnDefinition CreateGuidColumn (string columnName)
    {
      return CreateColumn (columnName, StorageTypeInformationObjectMother.CreateUniqueIdentifierStorageTypeInformation());
    }

    private static string GetUniqueColumnName ()
    {
      return Guid.NewGuid().ToString();
    }
  }
}