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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class ObjectsNotFoundExceptionTest : StandardMappingTest
  {
    [Test]
    public void Serialization ()
    {
      var innerException = new Exception ("Test");
      var exception = new ObjectsNotFoundException ("Message", new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }, innerException);

      var deserializedInstance = Serializer.SerializeAndDeserialize (exception);

      Assert.That (deserializedInstance.Message, Is.EqualTo ("Message"));
      Assert.That (deserializedInstance.IDs, Is.EqualTo (new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 }));
      Assert.That (deserializedInstance.InnerException, Is.Not.Null);
    }
  }
}