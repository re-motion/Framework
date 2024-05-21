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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderCreateNewObjectIDTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateNewObjectID ()
    {
      ClassDefinition orderClass = MappingConfiguration.Current.GetTypeDefinition(typeof(Order));
      var newObjectID = Provider.CreateNewObjectID(orderClass);

      Assert.IsNotNull(newObjectID, "ObjectID of new DataContainer.");
      Assert.That(newObjectID.ClassID, Is.EqualTo(orderClass.ID), "ClassID of ObjectID.");
      Assert.That(newObjectID.Value.GetType(), Is.EqualTo(typeof(Guid)), "Type of ID value of ObjectID.");
    }

    [Test]
    public void CreateNewObjectID_ClassDefinitionOfOtherStorageProvider ()
    {
      ClassDefinition classDefinition = MappingConfiguration.Current.GetTypeDefinition(typeof(Official));
      Assert.That(
          () => Provider.CreateNewObjectID(classDefinition),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The StorageProvider 'UnitTestStorageProviderStub' of the provided ClassDefinition does not match with this StorageProvider 'TestDomain'.",
                  "classDefinition"));
    }
  }
}
