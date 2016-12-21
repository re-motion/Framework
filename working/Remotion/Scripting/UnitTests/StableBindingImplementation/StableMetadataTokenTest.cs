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
  public class StableMetadataTokenTest
  {
    [Test]
    public void Equals_Null ()
    {
      var method0 = typeof (Proxied).GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var stableMetadateToken0 = new StableMethodMetadataToken (method0);

      Assert.That (stableMetadateToken0.Equals(null), Is.False);
    }

    [Test]
    public void EqualsAndGetHashCode ()
    {
      var method0 = typeof (Proxied).GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var method1 = typeof (ProxiedChild).GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var method2 = typeof (ProxiedChild).GetPublicInstanceMethods ("PrependName", typeof (string)).Last ();

      Assert.That (method0.MetadataToken, Is.Not.EqualTo (method1.MetadataToken));
      Assert.That (method0.MetadataToken, Is.Not.EqualTo (method2.MetadataToken));

      var stableMetadateToken0 = new StableMethodMetadataToken (method0);
      var stableMetadateToken1 = new StableMethodMetadataToken (method1);
      var stableMetadateToken2 = new StableMethodMetadataToken (method2);

      Assert.That (stableMetadateToken0, Is.EqualTo (stableMetadateToken0));
      Assert.That (stableMetadateToken0, Is.EqualTo (stableMetadateToken1));
      Assert.That (stableMetadateToken1, Is.EqualTo (stableMetadateToken1));

      Assert.That (stableMetadateToken2, Is.EqualTo (stableMetadateToken2));
      Assert.That (stableMetadateToken0, Is.Not.EqualTo (stableMetadateToken2));
      Assert.That (stableMetadateToken1, Is.Not.EqualTo (stableMetadateToken2));

      Assert.That (stableMetadateToken0.GetHashCode (), Is.EqualTo (stableMetadateToken0.GetHashCode ()));
      Assert.That (stableMetadateToken0.GetHashCode (), Is.EqualTo (stableMetadateToken1.GetHashCode ()));
      Assert.That (stableMetadateToken1.GetHashCode (), Is.EqualTo (stableMetadateToken1.GetHashCode ()));

      Assert.That (stableMetadateToken2.GetHashCode (), Is.EqualTo (stableMetadateToken2.GetHashCode ()));
      Assert.That (stableMetadateToken0.GetHashCode (), Is.Not.EqualTo (stableMetadateToken2.GetHashCode ()));
      Assert.That (stableMetadateToken1.GetHashCode (), Is.Not.EqualTo (stableMetadateToken2.GetHashCode ()));
    }


 
  }
}
