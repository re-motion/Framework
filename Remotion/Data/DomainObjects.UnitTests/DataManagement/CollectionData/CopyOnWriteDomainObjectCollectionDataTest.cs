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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.CollectionData
{
  [TestFixture]
  public class CopyOnWriteDomainObjectCollectionDataTest : StandardMappingTest
  {
    private Order _domainObject1;
    private Order _domainObject2;
    private Order _domainObject3;

    private ObservableDomainObjectCollectionDataDecorator _copiedData;
    private CopyOnWriteDomainObjectDomainObjectCollectionData _copyOnWriteData;
    private DomainObjectCollectionData _underlyingCopiedData;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _domainObject1 = DomainObjectMother.CreateFakeObject<Order>();
      _domainObject2 = DomainObjectMother.CreateFakeObject<Order>();
      _domainObject3 = DomainObjectMother.CreateFakeObject<Order>();

      _underlyingCopiedData = new DomainObjectCollectionData(new[] { _domainObject1, _domainObject2 });
      _copiedData = new ObservableDomainObjectCollectionDataDecorator(_underlyingCopiedData);
      _copyOnWriteData = new CopyOnWriteDomainObjectDomainObjectCollectionData(_copiedData);
    }

    [Test]
    public void Initialization_SetsUpDelegationToCopiedValues_ByDefault ()
    {
      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));

      _underlyingCopiedData.Add(_domainObject3);

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2, _domainObject3 }));
    }

    [Test]
    public void Initialization_SetsUpCopyOnWrite_ForCopiedCollection ()
    {
      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));

      _copiedData.Add(_domainObject3);

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void CopyOnWrite_SetsUpCopy ()
    {
      _copyOnWriteData.CopyOnWrite();

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));

      _underlyingCopiedData.Add(_domainObject3);

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }

    [Test]
    public void CopyOnWrite_Twice ()
    {
      _copyOnWriteData.CopyOnWrite();
      var data1 = DomainObjectCollectionDataTestHelper.GetWrappedData(_copyOnWriteData);

      _copyOnWriteData.CopyOnWrite();
      var data2 = DomainObjectCollectionDataTestHelper.GetWrappedData(_copyOnWriteData);

      Assert.That(data1, Is.SameAs(data2));
    }

    [Test]
    public void IsContentsCopied_False ()
    {
      Assert.That(_copyOnWriteData.IsContentsCopied, Is.False);
    }

    [Test]
    public void IsContentsCopied_True ()
    {
      _copyOnWriteData.CopyOnWrite();
      Assert.That(_copyOnWriteData.IsContentsCopied, Is.True);
    }

    [Test]
    public void RevertToCopiedData ()
    {
      _copyOnWriteData.CopyOnWrite();
      _underlyingCopiedData.Remove(_domainObject1);

      _copyOnWriteData.RevertToCopiedData();

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject2 }));

      _underlyingCopiedData.Add(_domainObject3);
      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject2, _domainObject3 }));
    }

    [Test]
    public void OnDataChange_PerformsCopyOperation ()
    {
      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));

      _copyOnWriteData.Add(_domainObject3);

      Assert.That(_copyOnWriteData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2, _domainObject3 }));
      Assert.That(_copiedData.ToArray(), Is.EqualTo(new[] { _domainObject1, _domainObject2 }));
    }
  }
}
