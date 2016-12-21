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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  public static class EmptyViewDefinitionObjectMother
  {
    public static EmptyViewDefinition Create (StorageProviderDefinition storageProviderDefinition)
    {
      return Create (storageProviderDefinition, new EntityNameDefinition (null, "EmptyView"));
    }

    public static EmptyViewDefinition Create (StorageProviderDefinition storageProviderDefinition, EntityNameDefinition viewName)
    {
      return Create (
          storageProviderDefinition,
          viewName,
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0]);
    }

    public static EmptyViewDefinition Create (
        StorageProviderDefinition storageProviderDefinition,
        EntityNameDefinition viewName,
        ObjectIDStoragePropertyDefinition objectIDProperty,
        IRdbmsStoragePropertyDefinition timestampProperty,
        IEnumerable<IRdbmsStoragePropertyDefinition> dataProperties)
    {
      return new EmptyViewDefinition (
          storageProviderDefinition,
          viewName,
          objectIDProperty,
          timestampProperty,
          dataProperties,
          new EntityNameDefinition[0]);
    }

    public static EmptyViewDefinition CreateWithSynonyms (
        StorageProviderDefinition storageProviderDefinition, IEnumerable<EntityNameDefinition> synonyms)
    {
      return new EmptyViewDefinition (
          storageProviderDefinition,
          new EntityNameDefinition (null, "EmptyView"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty,
          SimpleStoragePropertyDefinitionObjectMother.TimestampProperty,
          new IRdbmsStoragePropertyDefinition[0],
          synonyms);
    }
  }
}