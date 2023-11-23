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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Linq.SqlBackend.MappingResolution;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class CustomDataTypeStorageObjectFactory : SqlStorageObjectFactory
  {
    public override IStorageTypeInformationProvider CreateStorageTypeInformationProvider (RdbmsProviderDefinition rdmsStorageProviderDefinition)
    {
      return new CustomDataTypeStorageTypeInformationProviderDecorator(
          base.CreateStorageTypeInformationProvider(rdmsStorageProviderDefinition));
    }

    protected override IDataStoragePropertyDefinitionFactory CreateDataStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider,
        IStorageSettings storageSettings)
    {
      return new CustomDataTypeStorageProeprtyDefinitionFactoryDecorator(
          base.CreateDataStoragePropertyDefinitionFactory(
              storageProviderDefinition,
              storageTypeInformationProvider,
              storageNameProvider,
              storageSettings),
          storageNameProvider);
    }

    protected override IMappingResolver CreateMappingResolver (
        RdbmsProviderDefinition storageProviderDefinition,
        IRdbmsPersistenceModelProvider persistenceModelProvider)
    {
      return new SimpleDataTypeMappingResolverDecorator(
          base.CreateMappingResolver(storageProviderDefinition, persistenceModelProvider),
          persistenceModelProvider);
    }
  }
}
