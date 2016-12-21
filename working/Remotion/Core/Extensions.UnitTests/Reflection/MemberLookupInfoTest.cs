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
using Remotion.Reflection;

namespace Remotion.Extensions.UnitTests.Reflection
{
  [TestFixture]
  public class MemberLookupInfoTest
  {
    [Test]
    public void GetParameterTypes ()
    {
      var info = new MemberLookupInfo ("Foo");

      var parameterTypes = info.GetParameterTypes (typeof (Func<int, string, object>));

      var expected = new[] { typeof (int), typeof (string) };
      Assert.That (parameterTypes, Is.EqualTo (expected));
    }

    [Test]
    public void GetSignature ()
    {
      var info = new MemberLookupInfo ("Foo");

      var signature = info.GetSignature (typeof (Func<int, string, object>));

      var expected = Tuple.Create (new[] { typeof (int), typeof (string) }, typeof (object));
      Assert.That (signature.Item1, Is.EqualTo (expected.Item1));
      Assert.That (signature.Item2, Is.EqualTo (expected.Item2));
    }

  }
}