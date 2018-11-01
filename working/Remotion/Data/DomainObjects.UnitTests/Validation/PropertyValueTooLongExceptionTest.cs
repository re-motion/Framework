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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class PropertyValueTooLongExceptionTest : ClientTransactionBaseTest
  {
    [Test]
    public void Serialization ()
    {
      var domainObject = Order.NewObject();
      var inner = new InvalidOperationException ("Test");
      var exception = new PropertyValueTooLongException (domainObject, "xy", 10, "Msg", inner);

      var deserializedException = Serializer.SerializeAndDeserialize (exception);

      Assert.That (deserializedException.DomainObject, Is.Not.Null);
      Assert.That (deserializedException.DomainObject.ID, Is.EqualTo (domainObject.ID));

      Assert.That (deserializedException.PropertyName, Is.EqualTo ("xy"));

      Assert.That (deserializedException.MaxLength, Is.EqualTo (10));

      Assert.That (deserializedException.Message, Is.EqualTo ("Msg"));

      Assert.That (deserializedException.InnerException, Is.TypeOf (typeof (InvalidOperationException)));
    }
  }
}