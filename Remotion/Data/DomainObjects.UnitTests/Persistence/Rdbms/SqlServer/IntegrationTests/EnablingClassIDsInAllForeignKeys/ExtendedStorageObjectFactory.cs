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
using Remotion.Data.DomainObjects.Validation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.EnablingClassIDsInAllForeignKeys
{
  public class ExtendedStorageObjectFactory : SqlStorageObjectFactory
  {
    public ExtendedStorageObjectFactory (IStorageSettings storageSettings, ITypeConversionProvider typeConversionProvider, IDataContainerValidator dataContainerValidator)
        : base(storageSettings, typeConversionProvider, dataContainerValidator)
    {
    }
    protected override IRelationStoragePropertyDefinitionFactory CreateRelationStoragePropertyDefinitionFactory (
        RdbmsProviderDefinition storageProviderDefinition,
        IStorageTypeInformationProvider storageTypeInformationProvider,
        IStorageNameProvider storageNameProvider)
    {
      return new RelationStoragePropertyDefinitionFactory(
          storageProviderDefinition,
          true,
          storageNameProvider,
          storageTypeInformationProvider,
          StorageSettings);
    }
  }
}
