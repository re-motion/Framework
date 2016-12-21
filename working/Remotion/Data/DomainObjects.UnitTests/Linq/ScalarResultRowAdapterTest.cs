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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Linq
{
  [TestFixture]
  public class ScalarResultRowAdapterTest
  {
    private ScalarResultRowAdapter _queryResultRowAdapter;
    private IStorageTypeInformationProvider _storageTypeInformationProviderStub;
    private IStorageTypeInformation _storageTypeInformationStub;
    private string _scalarValue;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationProviderStub = MockRepository.GenerateStub<IStorageTypeInformationProvider>();
      _storageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();

      _scalarValue = "5";
      _queryResultRowAdapter = new ScalarResultRowAdapter (_scalarValue, _storageTypeInformationProviderStub);
    }

    [Test]
    public void GetValue ()
    {
      _storageTypeInformationProviderStub.Stub (stub => stub.GetStorageType (typeof(int))).Return (_storageTypeInformationStub);
      _storageTypeInformationStub.Stub (stub => stub.ConvertFromStorageType (_scalarValue)).Return (1);

      var result = _queryResultRowAdapter.GetValue<int> (new ColumnID ("column1", 0));

      Assert.That (result, Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (IndexOutOfRangeException), ExpectedMessage = "Only one scalar value is available, column ID 'col: test (4)' is invalid.")]
    public void GetValue_InvalidColumnID ()
    {
      _queryResultRowAdapter.GetValue<int> (new ColumnID ("test", 4));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Scalar queries cannot return entities.")]
    public void GetEntity ()
    {
      _queryResultRowAdapter.GetEntity<int> ();
    }
  }
}