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
using System.Diagnostics;
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration.Loader;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public class StandardConfiguration: BaseConfiguration
  {
    private static StandardConfiguration s_instance;

    public static StandardConfiguration Instance
    {
      get
      {
        if (s_instance == null)
        {
          Debugger.Break();
          throw new InvalidOperationException("StandardConfiguration has not been Initialized by invoking Initialize()");
        }
        return s_instance;
      }
    }

    public static void EnsureInitialized ()
    {
      if (s_instance != null)
        return;

      s_instance = new StandardConfiguration();
      s_instance.DisableDatabaseAccess();
    }

    private readonly DomainObjectIDs _domainObjectIDs;
    private readonly IQueryDefinitionRepository _queries;

    private StandardConfiguration ()
    {
      _domainObjectIDs = new DomainObjectIDs(GetMappingConfiguration());
      var queryDefinitionFileLoader = new QueryDefinitionFileLoader(GetStorageSettings());
      _queries = new QueryDefinitionRepository(
          queryDefinitionFileLoader.LoadQueryDefinitions(
              Path.Combine(TestContext.CurrentContext.TestDirectory, "QueriesForStandardMapping.xml")));
    }

    public DomainObjectIDs GetDomainObjectIDs ()
    {
      return _domainObjectIDs;
    }

    public IQueryDefinitionRepository GetQueries ()
    {
      return _queries;
    }
  }
}
