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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Linq.ExecutableQueries;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain
{
  public class StubDomainObjectQueryGenerator : IDomainObjectQueryGenerator
  {
    public IExecutableQuery<T> CreateScalarQuery<T> (string id, StorageProviderDefinition storageProviderDefinition, QueryModel queryModel)
    {
      throw new NotImplementedException();
    }

    public IExecutableQuery<IEnumerable<T>> CreateSequenceQuery<T> (
        string id,
        StorageProviderDefinition storageProviderDefinition,
        QueryModel queryModel,
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders)
    {
      return new StubSquenceQuery<T>(new QueryDefinition(id, storageProviderDefinition, "The Query", QueryType.CollectionReadWrite));
    }
  }
}
