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

[assembly: AppliedTestMarkerAttribute]

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading
{
  public class AppliedTestMarkerAttribute : Attribute { }
  public class UnusedTestMarkerAttribute : Attribute { }

  [TestFixture]
  public class AttributeAssemblyLoaderFilterTest
  {
    [Test]
    public void AttributeConsidering ()
    {
      var filter = new AttributeAssemblyLoaderFilter(typeof(Attribute)); // attribute type doesn't matter here
      Assert.That(filter.ShouldConsiderAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(typeof(TestFixtureAttribute).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(typeof(object).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("name does not matter")), Is.True);
    }

    [Test]
    public void AttributeInclusion ()
    {
      var filter = new AttributeAssemblyLoaderFilter(typeof(AppliedTestMarkerAttribute));
      Assert.That(filter.ShouldIncludeAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(TestFixtureAttribute).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(object).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(Uri).Assembly), Is.False);

      filter = new AttributeAssemblyLoaderFilter(typeof(CLSCompliantAttribute));
      Assert.That(filter.ShouldIncludeAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(TestFixtureAttribute).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(object).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(Uri).Assembly), Is.True);

      filter = new AttributeAssemblyLoaderFilter(typeof(UnusedTestMarkerAttribute));
      Assert.That(filter.ShouldIncludeAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(TestFixtureAttribute).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(object).Assembly), Is.False);
      Assert.That(filter.ShouldIncludeAssembly(typeof(Uri).Assembly), Is.False);
    }
  }
}
