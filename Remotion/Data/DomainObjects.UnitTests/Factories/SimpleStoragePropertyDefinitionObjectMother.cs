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
  public static class SimpleStoragePropertyDefinitionObjectMother
  {
    public static readonly SimpleStoragePropertyDefinition IDProperty = 
        new SimpleStoragePropertyDefinition (typeof (object), ColumnDefinitionObjectMother.IDColumn);

    public static readonly SimpleStoragePropertyDefinition ClassIDProperty =
        new SimpleStoragePropertyDefinition (typeof (string), ColumnDefinitionObjectMother.ClassIDColumn);

    public static readonly SimpleStoragePropertyDefinition TimestampProperty =
        new SimpleStoragePropertyDefinition (typeof (object), ColumnDefinitionObjectMother.TimestampColumn);

    public static SimpleStoragePropertyDefinition CreateStorageProperty (
        string columnName = null, IStorageTypeInformation storageTypeInformation = null, bool isPartOfPrimaryKey = false)
    {
      return new SimpleStoragePropertyDefinition (
          typeof (object), ColumnDefinitionObjectMother.CreateColumn (columnName, storageTypeInformation, isPartOfPrimaryKey));
    }

    public static SimpleStoragePropertyDefinition CreateStringStorageProperty (string columnName)
    {
      return new SimpleStoragePropertyDefinition (typeof (object), ColumnDefinitionObjectMother.CreateStringColumn (columnName));
    }

    public static SimpleStoragePropertyDefinition CreateGuidStorageProperty (string columnName)
    {
      return new SimpleStoragePropertyDefinition (typeof (object), ColumnDefinitionObjectMother.CreateGuidColumn (columnName));
    }
  }
}