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
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Compilation;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using Remotion.Development.UnitTesting.IsolatedCodeRunner;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyLoading
{
  [TestFixture]
  public class ApplicationAssemblyLoaderFilterTest
  {
    [SetUp]
    public void SetUp ()
    {
      ApplicationAssemblyLoaderFilter.Instance.Reset();
    }

    [TearDown]
    public void TearDown ()
    {
      ApplicationAssemblyLoaderFilter.Instance.Reset();
    }

    [Test]
    public void ApplicationAssemblyMatchExpression ()
    {
      ApplicationAssemblyLoaderFilter filter = ApplicationAssemblyLoaderFilter.Instance;
      Assert.That(
          filter.SystemAssemblyMatchExpression,
          Is.EqualTo(
              @"^((mscorlib)|(System)|(System\..*)|(Microsoft\..*)"
              + @"|(CoreForms\.Web)|(CoreForms\.Web\..*)"
              + @"|(Moq)"
              + @"|(netstandard)"
              + @"|(NUnit\..*)|(NUnit3\..*)"
              + @"|(Remotion\..*\.Generated\..*)"
              + @"|(Rhino.Mocks)"
              + @"|(testcentric\.engine\.metadata)"
              + @"|(TypePipe_.*Generated.*))$"));
    }

    [Test]
    public void ApplicationAssemblyConsidering ()
    {
      ApplicationAssemblyLoaderFilter filter = ApplicationAssemblyLoaderFilter.Instance;
      Assert.That(filter.ShouldConsiderAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(typeof(TestFixtureAttribute).Assembly.GetName()), Is.True);
      Assert.That(filter.ShouldConsiderAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly.GetName()), Is.True);

      Assert.That(filter.ShouldConsiderAssembly(typeof(object).Assembly.GetName()), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("System")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Microsoft.Something.Whatever")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Moq")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("netstandard")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("NUnit.Something.Whatever")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("NUnit3.Something.Whatever")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Remotion.Mixins.Generated.Unsigned")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Remotion.Mixins.Generated.Signed")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Remotion.Data.DomainObjects.Generated.Signed")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Remotion.Data.DomainObjects.Generated.Unsigned")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("Rhino.Mocks")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("CoreForms.Web")), Is.False);
      Assert.That(filter.ShouldConsiderAssembly(new AssemblyName("CoreForms.Web.Services")), Is.False);
    }

    [Test]
    public void AddIgnoredAssembly ()
    {
      ApplicationAssemblyLoaderFilter filter = ApplicationAssemblyLoaderFilter.Instance;
      Assert.That(filter.ShouldConsiderAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly.GetName()), Is.True);
      filter.AddIgnoredAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly.GetName().Name);
      Assert.That(filter.ShouldConsiderAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly.GetName()), Is.False);
    }

    [Test]
    public void ApplicationAssemblyInclusion_DependsOnAttribute ()
    {
      string compiledAssemblyPath = Path.Combine(AppContext.BaseDirectory, "NonApplicationMarkedAssembly.dll");
      try
      {
        var isolatedCodeRunner = new IsolatedCodeRunner(Test);
        isolatedCodeRunner.Run(compiledAssemblyPath);
      }
      finally
      {
        if (File.Exists(compiledAssemblyPath))
          FileUtility.DeleteAndWaitForCompletion(compiledAssemblyPath);
      }
    }

    private static void Test (string[] args)
    {
      var path = args[0];

      ApplicationAssemblyLoaderFilter filter = ApplicationAssemblyLoaderFilter.Instance;
      Assert.That(filter.ShouldIncludeAssembly(typeof(AttributeAssemblyLoaderFilterTest).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(TestFixtureAttribute).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(ApplicationAssemblyLoaderFilter).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(object).Assembly), Is.True);
      Assert.That(filter.ShouldIncludeAssembly(typeof(Uri).Assembly), Is.True);

      var assemblyCompiler = new AssemblyCompiler(
          @"Reflection\TypeDiscovery\TestAssemblies\NonApplicationMarkedAssembly",
          path,
          typeof(NonApplicationAssemblyAttribute).Assembly.Location);
      assemblyCompiler.Compile();
      Assert.That(filter.ShouldIncludeAssembly(assemblyCompiler.CompiledAssembly), Is.False);
    }
  }
}
