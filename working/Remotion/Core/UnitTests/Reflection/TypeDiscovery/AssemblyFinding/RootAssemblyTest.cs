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
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class RootAssemblyTest
  {
    [Test]
    public void Equals_True ()
    {
      var rootAsm1 = new RootAssembly (typeof (object).Assembly, true);
      var rootAsm2 = new RootAssembly (typeof (object).Assembly, false);

      Assert.That (rootAsm1, Is.EqualTo (rootAsm2));
    }

    [Test]
    public void Equals_False ()
    {
      var rootAsm1 = new RootAssembly (typeof (object).Assembly, true);
      var rootAsm2 = new RootAssembly (typeof (RootAssemblyTest).Assembly, true);

      Assert.That (rootAsm1, Is.Not.EqualTo (rootAsm2));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var rootAsm1 = new RootAssembly (typeof (object).Assembly, true);
      var rootAsm2 = new RootAssembly (typeof (object).Assembly, false);

      Assert.That (rootAsm1.GetHashCode (), Is.EqualTo (rootAsm2.GetHashCode ()));
    }

    [Test]
    public void ToString_WithFollowReferencesSetToTrue ()
    {
      var rootAsm = new RootAssembly (typeof (object).Assembly, true);

      Assert.That (rootAsm.ToString(), Is.EqualTo (typeof(object).Assembly.FullName + ", including references"));
    }

    [Test]
    public void ToString_WithFollowReferencesSetToFalse ()
    {
      var rootAsm = new RootAssembly (typeof (object).Assembly, false);

      Assert.That (rootAsm.ToString(), Is.EqualTo (typeof(object).Assembly.FullName));
    }
  }
}
