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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class ConcurrencyViolationExceptionTest : StandardMappingTest
  {
    [Test]
    public void Initialize_WithoutObjectIDs_HasEmptyIDsCollection ()
    {
      var exception = new ConcurrencyViolationException("The Message", new ObjectID[0], null);

      Assert.That(exception.Message, Is.EqualTo("The Message"));
      Assert.That(exception.IDs, Is.Empty);
      Assert.That(exception.InnerException, Is.Null);
    }

    [Test]
    public void Initialize_WithoutObjectIDs_HasIDsInCollection ()
    {
      IEnumerable<ObjectID> expectedIDs = new[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 };
      var exception = new ConcurrencyViolationException(expectedIDs);

      Assert.That(
          exception.Message,
          Is.EqualTo(
              string.Format(
                  "Concurrency violation encountered. One or more object(s) have already been changed by someone else: '{0}', '{1}'",
                  DomainObjectIDs.Order1,
                  DomainObjectIDs.Order3)));
      Assert.That(exception.IDs, Is.Not.SameAs(expectedIDs));
      Assert.That(exception.IDs, Is.EqualTo(expectedIDs));
    }
  }
}
