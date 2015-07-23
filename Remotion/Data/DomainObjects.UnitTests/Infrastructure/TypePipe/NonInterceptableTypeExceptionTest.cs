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
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Infrastructure.TypePipe
{
  [TestFixture]
  public class NonInterceptableTypeExceptionTest
  {
    [Test]
    public void Serializable ()
    {
      var instance = new NonInterceptableTypeException ("Mess", typeof (string), new InvalidOperationException ("Inner"));

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.Message, Is.EqualTo ("Mess"));
      Assert.That (deserializedInstance.Type, Is.SameAs (typeof (string)));
      Assert.That (deserializedInstance.InnerException, Is.TypeOf<InvalidOperationException>());
    }
  }
}