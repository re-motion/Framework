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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class NullLoadedObjectDataTest
  {
    private NullLoadedObjectData _loadedObjectData;

    [SetUp]
    public void SetUp ()
    {
      _loadedObjectData = new NullLoadedObjectData();
    }

    [Test]
    public void ObjectID ()
    {
      Assert.That (_loadedObjectData.ObjectID, Is.Null);
    }

    [Test]
    public void GetDomainObjectReference ()
    {
      var reference = _loadedObjectData.GetDomainObjectReference ();

      Assert.That (reference, Is.Null);
    }

    [Test]
    public void Accept ()
    {
      var visitorMock = MockRepository.GenerateStrictMock<ILoadedObjectVisitor>();
      visitorMock.Expect (mock => mock.VisitNullLoadedObject (_loadedObjectData));
      visitorMock.Replay ();

      _loadedObjectData.Accept (visitorMock);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((INullObject) _loadedObjectData).IsNull, Is.True);
    }
  }
}