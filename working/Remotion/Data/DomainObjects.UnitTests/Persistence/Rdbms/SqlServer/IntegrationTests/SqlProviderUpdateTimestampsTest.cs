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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests
{
  [TestFixture]
  public class SqlProviderUpdateTimestampsTest : SqlProviderBaseTest
  {
    [Test]
    public void UpdateTimestamps_ByNonExistingID ()
    {
      var objectID = new ObjectID(typeof (ClassWithAllDataTypes), new Guid ("{E067A627-BA3F-4ee5-8B61-1F46DC28DFC3}"));
      var dataContainer = DataContainer.CreateForExisting (objectID, null, pd => pd.DefaultValue);

      Assert.That (
          () => Provider.UpdateTimestamps (new[] { dataContainer }), 
          Throws.TypeOf<RdbmsProviderException>()
            .With.Message.EqualTo ("No timestamp found for object 'ClassWithAllDataTypes|e067a627-ba3f-4ee5-8b61-1f46dc28dfc3|System.Guid'."));
    }

    [Test]
    public void UpdateTimestamps_ByID ()
    {
      var dataContainerLoadedFromDB = Provider.LoadDataContainer (DomainObjectIDs.ClassWithAllDataTypes1).LocatedObject;
      var dataContainerCreatedInMemory = DataContainer.CreateForExisting (DomainObjectIDs.ClassWithAllDataTypes1, null, pd => pd.DefaultValue);
      Assert.That (dataContainerCreatedInMemory.Timestamp, Is.Null);
      
      Provider.UpdateTimestamps (new[] { dataContainerCreatedInMemory });

      Assert.That (dataContainerCreatedInMemory.Timestamp, Is.Not.Null);
      Assert.That (dataContainerCreatedInMemory.Timestamp, Is.EqualTo (dataContainerLoadedFromDB.Timestamp));
    }

    [Test]
    public void UpdateTimestamps_Multiple ()
    {
      var dataContainer1 = DataContainer.CreateForExisting (DomainObjectIDs.ClassWithAllDataTypes1, null, pd => pd.DefaultValue);
      var dataContainer2 = DataContainer.CreateForExisting (DomainObjectIDs.ClassWithAllDataTypes2, null, pd => pd.DefaultValue);
      Assert.That (dataContainer1.Timestamp, Is.Null);
      Assert.That (dataContainer2.Timestamp, Is.Null);
      
      Provider.UpdateTimestamps (new[] { dataContainer1, dataContainer2 });

      Assert.That (dataContainer1.Timestamp, Is.Not.Null);
      Assert.That (dataContainer2.Timestamp, Is.Not.Null);
    }

    [Test]
    public void UpdateTimestamps_Empty ()
    {
      Provider.UpdateTimestamps (new DataContainer[0]);
    }
  }
}
