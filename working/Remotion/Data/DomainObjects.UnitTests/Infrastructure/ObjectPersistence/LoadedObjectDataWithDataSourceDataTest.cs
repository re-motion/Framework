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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class LoadedObjectDataWithDataSourceDataTest : StandardMappingTest
  {
    private ILoadedObjectData _loadedObjectData;
    private DataContainer _dataSourceData;

    public override void SetUp ()
    {
      base.SetUp ();

      _loadedObjectData = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order1);
      _dataSourceData = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
    }

    [Test]
    public void Initialization ()
    {
      var dataWithDataSource = new LoadedObjectDataWithDataSourceData (_loadedObjectData, _dataSourceData);

      Assert.That (dataWithDataSource.LoadedObjectData, Is.SameAs (_loadedObjectData));
      Assert.That (dataWithDataSource.DataSourceData, Is.SameAs (_dataSourceData));
      Assert.That (dataWithDataSource.IsNull, Is.False);
    }

    [Test]
    public void Initialization_Null ()
    {
      var dataWithDataSource = new LoadedObjectDataWithDataSourceData (new NullLoadedObjectData(), null);

      Assert.That (dataWithDataSource.LoadedObjectData.IsNull, Is.True);
      Assert.That (dataWithDataSource.DataSourceData, Is.Null);
      Assert.That (dataWithDataSource.IsNull, Is.True);
    }

    [Test]
    public void Initialization_IDsDontMatch ()
    {
      var loadedObjectDataStub = LoadedObjectDataObjectMother.CreateLoadedObjectDataStub (DomainObjectIDs.Order3);
      Assert.That (
          () => new LoadedObjectDataWithDataSourceData (loadedObjectDataStub, _dataSourceData),
          Throws.ArgumentException.With.Message.EqualTo (
              "The ID of the dataSourceData parameter does not match the loadedObjectData.\r\nParameter name: dataSourceData"));
    }

    [Test]
    public void Initialization_Null_DataContainerNotNull ()
    {
      Assert.That (
          () => new LoadedObjectDataWithDataSourceData (new NullLoadedObjectData(), _dataSourceData),
          Throws.ArgumentException.With.Message.EqualTo (
              "The dataSourceData parameter must be null when loadedObjectData.IsNull is true.\r\nParameter name: dataSourceData"));
    }

    [Test]
    public void Initialization_Null_LoadedObjectDataNotNull ()
    {
      Assert.That (
          () => new LoadedObjectDataWithDataSourceData (_loadedObjectData, null),
          Throws.ArgumentException.With.Message.EqualTo (
              "The dataSourceData parameter must not be null when loadedObjectData.IsNull is false.\r\nParameter name: dataSourceData"));
    }
  }
}