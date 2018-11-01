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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Mixins;
using Remotion.Utilities;

namespace Remotion.Development.Data.UnitTesting.DomainObjects.Linq
{
  /// <summary>
  /// This mixin writes the generated Linq statements to the console. 
  /// Use the <see cref="ApplyQueryGeneratorMixinAttribute"/> to your assembly to actually apply the mixin.
  /// </summary>
  public class QueryGeneratorMixin : Mixin<object, QueryGeneratorMixin.IBaseCallRequirements>
  {
    public interface IBaseCallRequirements
    {
      IQuery CreateQuery (string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType);
    }

    [OverrideTarget]
    public IQuery CreateQuery (
        string id, StorageProviderDefinition storageProviderDefinition, string statement, CommandParameter[] commandParameters, QueryType queryType)
    {
      IQuery query = Next.CreateQuery (id, storageProviderDefinition, statement, commandParameters, queryType);
      QueryConstructed (query);
      return query;
    }

    private void QueryConstructed (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      Console.WriteLine (query.Statement);
      foreach (QueryParameter parameter in query.Parameters)
        Console.WriteLine ("{0} = {1}", parameter.Name, parameter.Value);
    }
  }
}
