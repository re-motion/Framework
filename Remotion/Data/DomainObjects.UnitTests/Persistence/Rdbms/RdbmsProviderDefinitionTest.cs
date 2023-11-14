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
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class RdbmsProviderDefinitionTest : StandardMappingTest
  {
    private StorageProviderDefinition _definition;
    private SqlStorageObjectFactory _sqlStorageObjectFactory;

    public override void SetUp ()
    {
      base.SetUp();

      _sqlStorageObjectFactory = new SqlStorageObjectFactory();
      _definition = new RdbmsProviderDefinition("StorageProviderID", _sqlStorageObjectFactory, "ConnectionString");

      FakeConfigurationWrapper configurationWrapper = new FakeConfigurationWrapper();
      configurationWrapper.SetUpConnectionString("SqlProvider", "ConnectionString", null);
      ConfigurationWrapper.SetCurrent(configurationWrapper);
    }

    [Test]
    public void Initialize_FromArguments ()
    {
      RdbmsProviderDefinition provider = new RdbmsProviderDefinition("Provider", _sqlStorageObjectFactory, "ConnectionString");

      Assert.That(provider.Name, Is.EqualTo("Provider"));
      Assert.That(provider.Factory, Is.TypeOf(typeof(SqlStorageObjectFactory)));
      Assert.That(provider.ConnectionString, Is.EqualTo("ConnectionString"));
    }

    [Test]
    public void Initialize_FromConfig ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add("description", "The Description");
      config.Add("factoryType", "Remotion.Data.DomainObjects::Persistence.Rdbms.SqlServer.Sql2014.SqlStorageObjectFactory");
      config.Add("connectionString", "SqlProvider");

      RdbmsProviderDefinition provider = new RdbmsProviderDefinition("Provider", config);

      Assert.That(provider.Name, Is.EqualTo("Provider"));
      Assert.That(provider.Description, Is.EqualTo("The Description"));
      Assert.That(provider.Factory, Is.TypeOf(typeof(SqlStorageObjectFactory)));
      Assert.That(provider.ConnectionString, Is.EqualTo("ConnectionString"));
      Assert.That(config, Is.Empty);
    }

    [Test]
    public void Initialize_FromConfig_InvalidFactoryType ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add("description", "The Description");
      config.Add("factoryType", typeof(InvalidRdbmsStorageObjectFactory).AssemblyQualifiedName);
      config.Add("connectionString", "SqlProvider");
      Assert.That(
          () => new RdbmsProviderDefinition("Provider", config),
          Throws.InstanceOf<ConfigurationErrorsException>()
              .With.Message.EqualTo(
                  "The factory type for the storage provider defined by 'Provider' must implement the 'IRdbmsStorageObjectFactory' interface. "
                  + "'InvalidRdbmsStorageObjectFactory' does not implement that interface."));
    }

    [Test]
    public void Initialize_FromConfig_WithMissingFactoryType ()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add("description", "The Description");
      config.Add("connectionString", "SqlProvider");
      Assert.That(
          () => new RdbmsProviderDefinition("Provider", config),
          Throws.InstanceOf<ConfigurationErrorsException>()
              .With.Message.EqualTo("The attribute 'factoryType' is missing in the configuration of the 'Provider' provider."));
    }

    [Test]
    public void IsIdentityTypeSupportedFalse ()
    {
      Assert.That(_definition.IsIdentityTypeSupported(typeof(int)), Is.False);
    }

    [Test]
    public void IsIdentityTypeSupportedTrue ()
    {
      Assert.That(_definition.IsIdentityTypeSupported(typeof(Guid)), Is.True);
    }

    [Test]
    public void IsIdentityTypeSupportedNull ()
    {
      Assert.That(
          () => _definition.IsIdentityTypeSupported(null),
          Throws.InstanceOf<ArgumentNullException>());
    }

    [Test]
    public void CheckValidIdentityType ()
    {
      _definition.CheckIdentityType(typeof(Guid));
    }

    [Test]
    public void CheckInvalidIdentityType ()
    {
      Assert.That(
          () => _definition.CheckIdentityType(typeof(string)),
          Throws.InstanceOf<IdentityTypeNotSupportedException>()
              .With.Message.EqualTo(
                  "The storage provider defined by 'RdbmsProviderDefinition' does not support identity values of type 'System.String'."));
    }

    [Test]
    public void CheckDetailsOfInvalidIdentityType ()
    {
      try
      {
        _definition.CheckIdentityType(typeof(string));
        Assert.Fail("Test expects an IdentityTypeNotSupportedException.");
      }
      catch (IdentityTypeNotSupportedException ex)
      {
        Assert.That(ex.InvalidIdentityType, Is.EqualTo(typeof(string)));
      }
    }
  }
}
