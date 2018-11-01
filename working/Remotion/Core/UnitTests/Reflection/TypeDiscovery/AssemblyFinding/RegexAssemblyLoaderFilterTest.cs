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
using System.Reflection;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class RegexAssemblyLoaderFilterTest
  {
    [Test]
    public void RegexConsidering_SimpleName ()
    {
      var filter = new RegexAssemblyLoaderFilter ("^Remotion.*$", RegexAssemblyLoaderFilter.MatchTargetKind.SimpleName);
      Assert.That (filter.MatchExpressionString, Is.EqualTo ("^Remotion.*$"));
      Assert.That (filter.ShouldConsiderAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly.GetName()), Is.True);
      Assert.That (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()), Is.False);
      Assert.That (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()), Is.False);
      Assert.That (filter.ShouldConsiderAssembly (new AssemblyName ("this is not a Remotion assembly")), Is.False);
    }

    [Test]
    public void RegexConsidering_FullName ()
    {
      var filter = new RegexAssemblyLoaderFilter (
          typeof (object).Assembly.FullName,
          RegexAssemblyLoaderFilter.MatchTargetKind.FullName);
      Assert.That (filter.MatchExpressionString.StartsWith ("mscorlib"), Is.True);
      Assert.That (filter.ShouldConsiderAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly.GetName()), Is.False);
      Assert.That (filter.ShouldConsiderAssembly (typeof (TestFixtureAttribute).Assembly.GetName()), Is.False);
      Assert.That (filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName()), Is.True);
      Assert.That (filter.ShouldConsiderAssembly (new AssemblyName ("this is not mscorlib")), Is.False);
    }

    [Test]
    public void RegexInclusion_AlwaysTrue ()
    {
      var filter = new RegexAssemblyLoaderFilter ("spispopd", RegexAssemblyLoaderFilter.MatchTargetKind.SimpleName);
      Assert.That (filter.MatchExpressionString, Is.EqualTo ("spispopd"));
      Assert.That (filter.ShouldIncludeAssembly (typeof (AttributeAssemblyLoaderFilterTest).Assembly), Is.True);
      Assert.That (filter.ShouldIncludeAssembly (typeof (TestFixtureAttribute).Assembly), Is.True);
      Assert.That (filter.ShouldIncludeAssembly (typeof (object).Assembly), Is.True);
      Assert.That (filter.ShouldIncludeAssembly (typeof (Uri).Assembly), Is.True);
    }
  }
}
