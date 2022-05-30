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
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  [Serializable]
#if !NETFRAMEWORK
  [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
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

      _markedReferencedAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain("MarkedReferencedAssembly.dll");
      _markedAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain("MarkedAssembly.dll", _markedReferencedAssemblyPath);
      _markedExeAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain("MarkedExeAssembly.dll");
      _markedAssemblyWithDerivedAttributePath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain("MarkedAssemblyWithDerivedAttribute.dll");
      _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain("UnmarkedAssembly.dll");

      _markedAssemblyInSearchPathPath = _searchPathForDllsBuildOutputManager.CompileInSeparateAppDomain("MarkedAssemblyInRelativeSearchPath.dll");
      _markedExeAssemblyInSearchPathPath =
          _searchPathForExesBuildOutputManager.CompileInSeparateAppDomain("MarkedExeAssemblyInRelativeSearchPath.exe");

      _markedAssemblyInSearchPathWithNameMismatchPath =
          _searchPathForDllsBuildOutputManager.CompileInSeparateAppDomain("MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");
      _markedAssemblyInSearchPathWithNameMismatchPath = _searchPathForDllsBuildOutputManager.RenameGeneratedAssembly(
          "MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll", "_MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");

      _markedAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.CompileInSeparateAppDomain("MarkedAssemblyInDynamicDirectory.dll");
      _markedExeAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.CompileInSeparateAppDomain(
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
      ExecuteInSeparateAppDomain(delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory();

        var assemblyLoader = CreateLoaderForMarkedAssemblies();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies();

        Assert.That(rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That(rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That(
            rootAssemblies.Select(root => root.Assembly).ToArray(),
            Is.EquivalentTo(
                new[]
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
                }));
      });
    }

    [Test]
    public void FindRootAssemblies_WithConsiderDynamicDirectoryFalse ()
    {
      ExecuteInSeparateAppDomain(delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory();

        var assemblyLoader = CreateLoaderForMarkedAssemblies();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(false, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies();

        Assert.That(rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That(rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That(
            rootAssemblies.Select(root => root.Assembly).ToArray(),
            Is.EquivalentTo(
                new[]
                {
                    Load(_markedAssemblyPath),
                    Load(_markedExeAssemblyPath),
                    Load(_markedAssemblyWithDerivedAttributePath),
                    Load(_markedReferencedAssemblyPath),
                    Load(_markedAssemblyInSearchPathPath),
                    Load(_markedExeAssemblyInSearchPathPath),
                    LoadFile(_markedAssemblyInSearchPathWithNameMismatchPath)
                }));
      });
    }

    [Test]
    public void FindAssemblies_References ()
    {
      ExecuteInSeparateAppDomain(delegate
      {
        Assembly markedAssembly = Load(_markedAssemblyPath);

        var loader = CreateLoaderForMarkedAssemblies();
        var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
        rootAssemblyFinderStub.Setup(stub => stub.FindRootAssemblies()).Returns(new[] { new RootAssembly(markedAssembly, true) });

        var assemblyFinder = new AssemblyFinder(rootAssemblyFinderStub.Object, loader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();
        Assert.That(assemblies, Is.EquivalentTo(new[] { markedAssembly, Load(_markedReferencedAssemblyPath) }));
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain(delegate
      {
        InitializeDynamicDirectory();

        var filterMock = new Mock<IAssemblyLoaderFilter>(MockBehavior.Strict);
        filterMock.Setup(mock => mock.ShouldConsiderAssembly(It.IsAny<AssemblyName>())).Returns(false).Verifiable();

        var assemblyLoader = new FilteringAssemblyLoader(filterMock.Object);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder(rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.Verify(mock => mock.ShouldConsiderAssembly(It.IsAny<AssemblyName>()), Times.AtLeastOnce());
        Assert.That(assemblies, Is.Empty);
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyTrueIncludeAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain(delegate
      {
        InitializeDynamicDirectory();

        var filterMock = new Mock<IAssemblyLoaderFilter>(MockBehavior.Strict);
        filterMock.Setup(mock => mock.ShouldConsiderAssembly(It.IsNotNull<AssemblyName>())).Returns(true).Verifiable();
        filterMock.Setup(mock => mock.ShouldIncludeAssembly(It.IsNotNull<Assembly>())).Returns(false).Verifiable();

        var assemblyLoader = new FilteringAssemblyLoader(filterMock.Object);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder(rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.Verify(mock => mock.ShouldIncludeAssembly(It.IsNotNull<Assembly>()), Times.AtLeastOnce());
        Assert.That(assemblies, Is.Empty);
      });
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
      _dynamicDirectoryBuildOutputManager.CopyAllGeneratedAssembliesToNewDirectory(
#if NETFRAMEWORK
          AppDomain.CurrentDomain.DynamicDirectory
#else
          null
#endif
      );
    }

    private Assembly CompileTestAssemblyInMemory (string assemblyName, params string[] referencedAssemblies)
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

    private FilteringAssemblyLoader CreateLoaderForMarkedAssemblies ()
    {
      var markerAttributeType = typeof(MarkerAttribute);
      var attributeFilter = new AttributeAssemblyLoaderFilter(markerAttributeType);
      return new FilteringAssemblyLoader(attributeFilter);
    }

    private Assembly Load (string assemblyPath)
    {
      var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
      return Assembly.Load(assemblyName);
    }

    private Assembly LoadFile (string assemblyPath)
    {
      return Assembly.LoadFile(assemblyPath);
    }
  }
}
