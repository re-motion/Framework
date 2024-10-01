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
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Development.UnitTesting;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.ObjectBinding.UnitTests
{
  [SetUpFixture]
  public class SetUpFixture
  {
    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      try
      {
        BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);

        var storageProviderDefinition = new RdbmsProviderDefinition(
            StubStorageProvider.StorageProviderID,
            new StubStorageFactory(),
            "NonExistingRdbms",
            "NonExistingReadOnlyRdbms");

        var storageSettings = new StorageSettings(storageProviderDefinition, new[] { storageProviderDefinition });

        var defaultServiceLocator = DefaultServiceLocator.Create();
        defaultServiceLocator.RegisterSingle<IStorageSettings>(() => storageSettings);

        ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);
        Dev.Null = MappingConfiguration.Current;
      }
      catch (Exception e)
      {
        Console.WriteLine("SetUpFixture failed: " + e);
        Console.WriteLine();
        throw;
      }
    }
  }
}
