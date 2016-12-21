﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Development.UnitTesting;

namespace Remotion.ObjectBinding.UnitTests
{
  [TestFixture]
  public class BusinessObjectPropertyAccessExceptionTest
  {
    [Test]
    public void Initialize ()
    {
      Exception expectedException = new Exception("The Exception");
      var exception = new BusinessObjectPropertyAccessException ("The Message", expectedException);

      Assert.That (exception.Message, Is.EqualTo ("The Message"));
      Assert.That (exception.InnerException, Is.SameAs (expectedException));
    }

    [Test]
    public void Serialization ()
    {
      Exception expectedException = new Exception("The Exception");
      var exception = new BusinessObjectPropertyAccessException ("The Message", expectedException);

      var deserialized = Serializer.SerializeAndDeserialize (exception);

      Assert.That (deserialized.Message, Is.EqualTo ("The Message"));
      Assert.That (deserialized.InnerException, Is.Not.Null);
      Assert.That (deserialized.InnerException.Message, Is.EqualTo ("The Exception"));
    }
  }
}