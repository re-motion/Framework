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
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.ServiceLocation;
using Remotion.Utilities;

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

      _sqlStorageObjectFactory = new SqlStorageObjectFactory(
          StorageSettings,
          ServiceLocator.Current.GetInstance<ITypeConversionProvider>(),
          ServiceLocator.Current.GetInstance<IDataContainerValidator>());
      _definition = new RdbmsProviderDefinition("StorageProviderID", _sqlStorageObjectFactory, "ConnectionString");
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
