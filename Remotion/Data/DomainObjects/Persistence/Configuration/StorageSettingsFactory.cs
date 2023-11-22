﻿using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public static class StorageSettingsFactory
  {
    public static IStorageSettingsFactory CreateForSqlServer (string connectionString)
    {
      return new RdbmsStorageSettingsFactory(connectionString, typeof(SqlStorageObjectFactory));
    }

    public static IStorageSettingsFactory CreateForSqlServer (string connectionString, Type storageSettingsFactoryType)
    {
      return new RdbmsStorageSettingsFactory(connectionString, storageSettingsFactoryType);
    }
  }
}
