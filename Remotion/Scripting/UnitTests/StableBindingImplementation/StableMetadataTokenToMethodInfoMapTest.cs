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
using Remotion.Scripting.StableBindingImplementation;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests.StableBindingImplementation
{
  [TestFixture]
  public class StableMetadataTokenToMethodInfoMapTest
  {
    [Test]
    public void GetMethod ()
    {
      var type = typeof (Proxied);
      var method0 = type.GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var method1 = type.GetPublicInstanceMethods ("PrependName", typeof (string)).Last ();
      var stableMetadateToken0 = new StableMethodMetadataToken (method0);
      var stableMetadateToken1 = new StableMethodMetadataToken (method1);

      var map = new StableMetadataTokenToMethodInfoMap (type);
      Assert.That (map.GetMethod (stableMetadateToken0), Is.EqualTo (method0));
      Assert.That (map.GetMethod (stableMetadateToken1), Is.EqualTo (method1));
    }
  }
}
