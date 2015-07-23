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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  public static class StoragePropertyDefinitionTestHelper
  {
    public static ColumnDefinition GetIDColumnDefinition (ObjectIDStoragePropertyDefinition objectIDStoragePropertyDefinition)
    {
      return ((SimpleStoragePropertyDefinition) objectIDStoragePropertyDefinition.ValueProperty).ColumnDefinition;
    }

    public static ColumnDefinition GetClassIDColumnDefinition (ObjectIDStoragePropertyDefinition objectIDStoragePropertyDefinition)
    {
      return ((SimpleStoragePropertyDefinition) objectIDStoragePropertyDefinition.ClassIDProperty).ColumnDefinition;
    }

    public static ColumnDefinition GetSingleColumn (IRdbmsStoragePropertyDefinition rdbmsStoragePropertyDefinition)
    {
      var columns = rdbmsStoragePropertyDefinition.GetColumns().ToArray();
      Assert.That (columns, Has.Length.EqualTo (1));
      return columns.Single();
    }

    public static IEnumerable<ColumnDefinition> GetColumns (IEnumerable<IRdbmsStoragePropertyDefinition> rdbmsStoragePropertyDefinitions)
    {
      return rdbmsStoragePropertyDefinitions.SelectMany (p => p.GetColumns());
    }
  }
}