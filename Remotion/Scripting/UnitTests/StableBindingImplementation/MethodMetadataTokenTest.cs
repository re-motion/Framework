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
  public class MetadataTokenTest
  {
    [Test]
    public void Ctor ()
    {
      var method0 = typeof (Proxied).GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var metadateToken0 = new MethodMetadataToken (method0);
      Assert.That (metadateToken0.Token, Is.EqualTo (method0.MetadataToken));
      Assert.That (metadateToken0.MethodInfo, Is.EqualTo (method0));
    }


    [Test]
    public void Equals_Null ()
    {
      var method0 = typeof (Proxied).GetPublicInstanceMethods ("OverrideMe", typeof (string)).Last ();
      var stableMetadateToken0 = new MethodMetadataToken (method0);

      Assert.That (stableMetadateToken0.Equals (null), Is.False);
    }
  }
}
