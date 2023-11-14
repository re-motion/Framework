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
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.Compilation;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
#if !NETFRAMEWORK
using Remotion.Development.UnitTesting.IsolatedCodeRunner;
#endif

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  [Serializable]
  public class AssemblyFinderIntegrationTest
  {
#if !NETFRAMEWORK
    private delegate void CrossAppDomainDelegate ();
#endif

    private const string c_testAssemblySourceDirectoryRoot = @"Reflection\TypeDiscovery\TestAssemblies";
    private AssemblyCompilerBuildOutputManager _baseDirectoryBuildOutputManager;
    private AssemblyCompilerBuildOutputManager _dynamicDirectoryBuildOutputManager;
    private AssemblyCompilerBuildOutputManager _searchPathForDllsBuildOutputManager;
    private AssemblyCompilerBuildOutputManager _searchPathForExesBuildOutputManager;

    private string _markedAssemblyPath;
    private string _markedExeAssemblyPath;
    private string _markedAssemblyWithDerivedAttributePath;
    private string _markedReferencedAssemblyPath;

    private string _markedAssemblyInSearchPathPath;
    private string _markedExeAssemblyInSearchPathPath;
    private string _markedAssemblyInSearchPathWithNameMismatchPath;

    private string _markedAssemblyInDynamicDirectoryPath;
    private string _markedExeAssemblyInDynamicDirectoryPath;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
      var searchPathForDlls = Path.Combine(AppContext.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dlls");
      var searchPathForExes = Path.Combine(AppContext.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Exes");
      var dynamicBase = Path.Combine(AppContext.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dynamic");

      _baseDirectoryBuildOutputManager = CreateAssemblyCompilerBuildOutputManager(AppContext.BaseDirectory);
      _dynamicDirectoryBuildOutputManager = CreateAssemblyCompilerBuildOutputManager(dynamicBase);
      _searchPathForDllsBuildOutputManager = CreateAssemblyCompilerBuildOutputManager(searchPathForDlls);
      _searchPathForExesBuildOutputManager = CreateAssemblyCompilerBuildOutputManager(searchPathForExes);

      _markedReferencedAssemblyPath = _baseDirectoryBuildOutputManager.Compile("MarkedReferencedAssembly.dll");
      _markedAssemblyPath = _baseDirectoryBuildOutputManager.Compile("MarkedAssembly.dll", _markedReferencedAssemblyPath);
      _markedExeAssemblyPath = _baseDirectoryBuildOutputManager.Compile("MarkedExeAssembly.dll");
      _markedAssemblyWithDerivedAttributePath = _baseDirectoryBuildOutputManager.Compile("MarkedAssemblyWithDerivedAttribute.dll");
      _baseDirectoryBuildOutputManager.Compile("UnmarkedAssembly.dll");

      _markedAssemblyInSearchPathPath = _searchPathForDllsBuildOutputManager.Compile("MarkedAssemblyInRelativeSearchPath.dll");
      _markedExeAssemblyInSearchPathPath =
          _searchPathForExesBuildOutputManager.Compile("MarkedExeAssemblyInRelativeSearchPath.exe");

      _markedAssemblyInSearchPathWithNameMismatchPath =
          _searchPathForDllsBuildOutputManager.Compile("MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");
      _markedAssemblyInSearchPathWithNameMismatchPath = _searchPathForDllsBuildOutputManager.RenameGeneratedAssembly(
          "MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll", "_MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");

      _markedAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.Compile("MarkedAssemblyInDynamicDirectory.dll");
      _markedExeAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.Compile(
          "MarkedExeAssemblyInDynamicDirectory.exe");
    }

    [OneTimeTearDown]
    public void TeastFixtureTearDown ()
    {
      _baseDirectoryBuildOutputManager.Dispose();
      _dynamicDirectoryBuildOutputManager.Dispose();
      _searchPathForDllsBuildOutputManager.Dispose();
      _searchPathForExesBuildOutputManager.Dispose();
    }

    [Test]
    public void FindRootAssemblies_ForAppDomain_WithConsiderDynamicDirectoryTrue ()
    {
#if NETFRAMEWORK
      ExecuteInSeparateAppDomain(delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory();

        Test(firstInMemoryAssembly, secondInMemoryAssembly,
            () =>
            {
              return new[]
                     {
                         Load(_markedAssemblyPath),
                         Load(_markedExeAssemblyPath),
                         Load(_markedAssemblyWithDerivedAttributePath),
                         Load(_markedReferencedAssemblyPath),
                         Load(_markedAssemblyInSearchPathPath),
                         Load(_markedExeAssemblyInSearchPathPath),
                         Load(_markedAssemblyInDynamicDirectoryPath),
                         Load(_markedExeAssemblyInDynamicDirectoryPath),
                         LoadFile(_markedAssemblyInSearchPathWithNameMismatchPath)
                     };
            });
      });
#else
      // Because unloading assemblies with AssemblyLoadContext is hard to do right, we do the test in an external process instead
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run();

      static void TestMain (string[] args)
      {
        var firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", "MarkedReferencedAssembly.dll");
        var secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        Test(firstInMemoryAssembly, secondInMemoryAssembly,
            () =>
            {
              return new[]
                     {
                         Load("MarkedAssembly.dll"),
                         Load("MarkedExeAssembly.dll"),
                         Load("MarkedAssemblyWithDerivedAttribute.dll"),
                         Load("MarkedReferencedAssembly.dll"),
                     };
            });
      }
#endif

      static void Test (Assembly firstInMemoryAssembly, Assembly secondInMemoryAssembly, Func<Assembly[]> getExpectedAssemblies)
      {
        var assemblyLoader = CreateLoaderForMarkedAssemblies();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies();

        Assert.That(rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That(rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That(rootAssemblies.Select(root => root.Assembly).ToArray(), Is.EquivalentTo(getExpectedAssemblies()));
      }
    }

    [Test]
    public void FindRootAssemblies_WithConsiderDynamicDirectoryFalse ()
    {
#if NETFRAMEWORK
      ExecuteInSeparateAppDomain(delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory();

        Test(
            firstInMemoryAssembly,
            secondInMemoryAssembly,
            () =>
            {
              return new[]
                     {
                         Load(_markedAssemblyPath),
                         Load(_markedExeAssemblyPath),
                         Load(_markedAssemblyWithDerivedAttributePath),
                         Load(_markedReferencedAssemblyPath),
                         Load(_markedAssemblyInSearchPathPath),
                         Load(_markedExeAssemblyInSearchPathPath),
                         LoadFile(_markedAssemblyInSearchPathWithNameMismatchPath)
                     };
            });
      });
#else
      // Because unloading assemblies with AssemblyLoadContext is hard to do right, we do the test in an external process instead
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run();

      static void TestMain (string[] args)
      {
        var firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", "MarkedReferencedAssembly.dll");
        var secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        Test(firstInMemoryAssembly, secondInMemoryAssembly,
            () =>
            {
              return new[]
                     {
                         Load("MarkedAssembly.dll"),
                         Load("MarkedExeAssembly.dll"),
                         Load("MarkedAssemblyWithDerivedAttribute.dll"),
                         Load("MarkedReferencedAssembly.dll"),
                     };
            });
      }
#endif

      static void Test (Assembly firstInMemoryAssembly, Assembly secondInMemoryAssembly, Func<Assembly[]> getExpectedAssemblies)
      {
        var assemblyLoader = CreateLoaderForMarkedAssemblies();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(false, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies();

        Assert.That(rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That(rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That(rootAssemblies.Select(root => root.Assembly).ToArray(), Is.EquivalentTo(getExpectedAssemblies()));
      }
    }

    [Test]
    public void FindAssemblies_References ()
    {
#if NETFRAMEWORK
      ExecuteInSeparateAppDomain(delegate
      {
        Test(_markedAssemblyPath, _markedReferencedAssemblyPath);
      });
#else
      // Because unloading assemblies with AssemblyLoadContext is hard to do right, we do the test in an external process instead
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run(_markedAssemblyPath, _markedReferencedAssemblyPath);

      static void TestMain (string[] args)
      {
        var markedAssemblyPath = args[0];
        var markedReferencedAssemblyPath = args[1];

        Test(markedAssemblyPath, markedReferencedAssemblyPath);
      }
#endif

      static void Test (string markedAssemblyPath, string markedReferencedAssemblyPath)
      {
        var markedAssembly = Load(markedAssemblyPath);

        var loader = CreateLoaderForMarkedAssemblies();
        var fixedRootAssemblyFinder = new FixedRootAssemblyFinder(new RootAssembly(markedAssembly, true));
        var assemblyFinder = new AssemblyFinder(fixedRootAssemblyFinder, loader);

        var assemblies = assemblyFinder.FindAssemblies().ToArray();
        var markedReferenceAssembly = Load(markedReferencedAssemblyPath);

        Assert.That(assemblies, Is.EquivalentTo(new[] { markedAssembly, markedReferenceAssembly }));
      }
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyFalse ()
    {
#if NETFRAMEWORK
      ExecuteInSeparateAppDomain(delegate
      {
        InitializeDynamicDirectory();

        Test();
      });
#else
      // Because unloading assemblies with AssemblyLoadContext is hard to do right, we do the test in an external process instead
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run();

      static void TestMain (string[] args)
      {
        Test();
      }
#endif

      static void Test ()
      {
        var filterMock = new Mock<IAssemblyLoaderFilter>(MockBehavior.Strict);
        filterMock.Setup(mock => mock.ShouldConsiderAssembly(It.IsAny<AssemblyName>())).Returns(false).Verifiable();

        var assemblyLoader = new FilteringAssemblyLoader(filterMock.Object);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder(rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.Verify(mock => mock.ShouldConsiderAssembly(It.IsAny<AssemblyName>()), Times.AtLeastOnce());
        Assert.That(assemblies, Is.Empty);
      }
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyTrueIncludeAssemblyFalse ()
    {
#if NETFRAMEWORK
      ExecuteInSeparateAppDomain(delegate
      {
        InitializeDynamicDirectory();

        Test();
      });
#else
      // Because unloading assemblies with AssemblyLoadContext is hard to do right, we do the test in an external process instead
      var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
      isolatedCodeRunner.Run();

      static void TestMain (string[] args)
      {
        Test();
      }
#endif

      static void Test ()
      {
        var filterMock = new Mock<IAssemblyLoaderFilter>(MockBehavior.Strict);
        filterMock.Setup(mock => mock.ShouldConsiderAssembly(It.IsNotNull<AssemblyName>())).Returns(true).Verifiable();
        filterMock.Setup(mock => mock.ShouldIncludeAssembly(It.IsNotNull<Assembly>())).Returns(false).Verifiable();

        var assemblyLoader = new FilteringAssemblyLoader(filterMock.Object);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder(rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.Verify(mock => mock.ShouldIncludeAssembly(It.IsNotNull<Assembly>()), Times.AtLeastOnce());
        Assert.That(assemblies, Is.Empty);
      }
    }

    private void ExecuteInSeparateAppDomain (CrossAppDomainDelegate test)
    {
#if NETFRAMEWORK
      AppDomain appDomain = null;

      try
      {
        var appDomainSetup = new AppDomainSetup();
        appDomainSetup.ApplicationName = "Test";
        appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        appDomainSetup.PrivateBinPath = _searchPathForDllsBuildOutputManager.BuildOutputDirectory + ";" + _searchPathForExesBuildOutputManager.BuildOutputDirectory;
        appDomainSetup.DynamicBase = _dynamicDirectoryBuildOutputManager.BuildOutputDirectory;
        appDomainSetup.ShadowCopyFiles = AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles;

        appDomain = AppDomain.CreateDomain("Test", null, appDomainSetup);

        appDomain.DoCallBack(test);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload(appDomain);
      }
#else
      throw new PlatformNotSupportedException("This API is not supported on the current platform.");
#endif
    }

    private void InitializeDynamicDirectory ()
    {
#if NETFRAMEWORK
      _dynamicDirectoryBuildOutputManager.CopyAllGeneratedAssembliesToNewDirectory(AppDomain.CurrentDomain.DynamicDirectory);
#else
      throw new PlatformNotSupportedException("Dynamic directory is not supported under .NET.");
#endif
    }

    private static Assembly CompileTestAssemblyInMemory (string assemblyName, params string[] referencedAssemblies)
    {
      AssemblyCompiler assemblyCompiler = AssemblyCompiler.CreateInMemoryAssemblyCompiler(
          c_testAssemblySourceDirectoryRoot + "\\" + assemblyName,
          ArrayUtility.Combine(new[] { typeof(MarkerAttribute).Module.Name },
          referencedAssemblies));
      assemblyCompiler.Compile();
      return assemblyCompiler.CompiledAssembly;
    }

    private AssemblyCompilerBuildOutputManager CreateAssemblyCompilerBuildOutputManager (string buildOutputDirectory)
    {
      var createBuildOutputDirectory = buildOutputDirectory != AppContext.BaseDirectory;
      return new AssemblyCompilerBuildOutputManager(
          buildOutputDirectory, createBuildOutputDirectory, c_testAssemblySourceDirectoryRoot, typeof(MarkerAttribute).Module.Name);
    }

    private static FilteringAssemblyLoader CreateLoaderForMarkedAssemblies ()
    {
      var markerAttributeType = typeof(MarkerAttribute);
      var attributeFilter = new AttributeAssemblyLoaderFilter(markerAttributeType);
      return new FilteringAssemblyLoader(attributeFilter);
    }

    private static Assembly Load (string assemblyPath)
    {
      var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
      return Assembly.Load(assemblyName);
    }

    private static Assembly LoadFile (string assemblyPath)
    {
      return Assembly.LoadFile(assemblyPath);
    }
  }
}
