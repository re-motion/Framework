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
using MixinXRef.Utility;
using NUnit.Framework;
using System.Linq;

namespace MixinXRef.UnitTests.Utility
{
  [TestFixture]
  public class ErrorAggregatorTest
  {
    [Test]
    public void AddException_RetrieveWithExceptionsProperty()
    {
      var errorAggregator = new ErrorAggregator<Exception>();

      var exception1 = new Exception("test exception");
      var exception2 = new Exception ("another text exception");

      errorAggregator.AddException(exception1);
      errorAggregator.AddException(exception2);

      Assert.That(new[] { exception1, exception2 }, Is.EqualTo(errorAggregator.Exceptions.ToList()));
    }
  }
}