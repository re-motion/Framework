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
using System.Data;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests;

/// <summary>
/// Test-only: applies the precision and scale configured via <see cref="TestDecimalPrecisionAttribute"/> to decimal columns.
/// </summary>
public class TestSqlStorageTypeInformationProvider : SqlStorageTypeInformationProvider
{
  public TestSqlStorageTypeInformationProvider (IDateTimeDefaultStorageTypeProvider dateTimeDefaultStorageTypeProvider)
      : base(dateTimeDefaultStorageTypeProvider)
  {
  }

  protected override StorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable)
  {
    var storageType = base.GetStorageType(propertyDefinition, forceNullable);

    var attribute = propertyDefinition.PropertyInfo.GetCustomAttribute<TestDecimalPrecisionAttribute>(true);
    if (attribute == null || storageType.StorageDbType != DbType.Decimal)
      return storageType;

    return new StorageTypeInformation(
        storageType.StorageType,
        $"decimal ({attribute.Precision}, {attribute.Scale})",
        storageType.StorageDbType,
        storageType.IsStorageTypeNullable,
        storageType.StorageTypeLength,
        storageType.DotNetType,
        storageType.DotNetTypeConverter,
        attribute.Precision,
        attribute.Scale);
  }
}
