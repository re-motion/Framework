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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class ScalarResultRowAdapterTest
  {
    private ScalarResultRowAdapter _queryResultRowAdapter;
    private Mock<IStorageTypeInformationProvider> _storageTypeInformationProviderStub;
    private Mock<IStorageTypeInformation> _storageTypeInformationStub;
    private string _scalarValue;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProviderStub = new Mock<IStorageTypeInformationProvider>();
      _storageTypeInformationStub = new Mock<IStorageTypeInformation>();

      _scalarValue = "5";
      _queryResultRowAdapter = new ScalarResultRowAdapter(_scalarValue, _storageTypeInformationProviderStub.Object);
    }

    [Test]
    public void GetValue ()
    {
      _storageTypeInformationProviderStub.Setup(stub => stub.GetStorageType(typeof(int))).Returns(_storageTypeInformationStub.Object);
      _storageTypeInformationStub.Setup(stub => stub.ConvertFromStorageType(_scalarValue)).Returns(1);

      var result = _queryResultRowAdapter.GetValue<int>(new ColumnID("column1", 0));

      Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void GetValue_InvalidColumnID ()
    {
      Assert.That(
          () => _queryResultRowAdapter.GetValue<int>(new ColumnID("test", 4)),
          Throws.InstanceOf<IndexOutOfRangeException>()
              .With.Message.EqualTo(
                  "Only one scalar value is available, column ID 'col: test (4)' is invalid."));
    }

    [Test]
    public void GetEntity ()
    {
      Assert.That(
          () => _queryResultRowAdapter.GetEntity<int>(),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo("Scalar queries cannot return entities."));
    }
  }
}
