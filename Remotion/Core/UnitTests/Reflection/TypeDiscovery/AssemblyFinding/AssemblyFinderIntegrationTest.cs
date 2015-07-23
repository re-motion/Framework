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
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino_Is = Rhino.Mocks.Constraints.Is;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  [Serializable]
  public class AssemblyFinderIntegrationTest
  {
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

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      var searchPathForDlls = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dlls");
      var searchPathForExes = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Exes");
      var dynamicBase = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "Reflection.AssemblyFinderIntegrationTest.Dynamic");

      _baseDirectoryBuildOutputManager = CreateAssemblyCompilerBuildOutputManager (AppDomain.CurrentDomain.BaseDirectory);
      _dynamicDirectoryBuildOutputManager = CreateAssemblyCompilerBuildOutputManager (dynamicBase);
      _searchPathForDllsBuildOutputManager = CreateAssemblyCompilerBuildOutputManager (searchPathForDlls);
      _searchPathForExesBuildOutputManager = CreateAssemblyCompilerBuildOutputManager (searchPathForExes);

      _markedReferencedAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain ("MarkedReferencedAssembly.dll");
      _markedAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain ("MarkedAssembly.dll", _markedReferencedAssemblyPath);
      _markedExeAssemblyPath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain ("MarkedExeAssembly.dll");
      _markedAssemblyWithDerivedAttributePath = _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain ("MarkedAssemblyWithDerivedAttribute.dll");
      _baseDirectoryBuildOutputManager.CompileInSeparateAppDomain ("UnmarkedAssembly.dll");

      _markedAssemblyInSearchPathPath = _searchPathForDllsBuildOutputManager.CompileInSeparateAppDomain ("MarkedAssemblyInRelativeSearchPath.dll");
      _markedExeAssemblyInSearchPathPath =
          _searchPathForExesBuildOutputManager.CompileInSeparateAppDomain ("MarkedExeAssemblyInRelativeSearchPath.exe");

      _markedAssemblyInSearchPathWithNameMismatchPath =
          _searchPathForDllsBuildOutputManager.CompileInSeparateAppDomain ("MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");
      _markedAssemblyInSearchPathWithNameMismatchPath = _searchPathForDllsBuildOutputManager.RenameGeneratedAssembly (
          "MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll", "_MarkedAssemblyWithOtherFilenameInRelativeSearchPath.dll");

      _markedAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.CompileInSeparateAppDomain ("MarkedAssemblyInDynamicDirectory.dll");
      _markedExeAssemblyInDynamicDirectoryPath = _dynamicDirectoryBuildOutputManager.CompileInSeparateAppDomain (
          "MarkedExeAssemblyInDynamicDirectory.exe");
    }

    [TestFixtureTearDown]
    public void TeastFixtureTearDown ()
    {
      _baseDirectoryBuildOutputManager.Dispose();
      _dynamicDirectoryBuildOutputManager.Dispose();
      _searchPathForDllsBuildOutputManager.Dispose ();
      _searchPathForExesBuildOutputManager.Dispose ();
    }

    [Test]
    public void FindRootAssemblies_ForAppDomain_WithConsiderDynamicDirectoryTrue ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory ("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory ("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory ("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory ();

        var assemblyLoader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies ();

        Assert.That (rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That (rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That (
            rootAssemblies.Select (root => root.Assembly).ToArray(),
            Is.EquivalentTo (
                new[]
                {
                    Load (_markedAssemblyPath),
                    Load (_markedExeAssemblyPath),
                    Load (_markedAssemblyWithDerivedAttributePath),
                    Load (_markedReferencedAssemblyPath),
                    Load (_markedAssemblyInSearchPathPath),
                    Load (_markedExeAssemblyInSearchPathPath),
                    Load (_markedAssemblyInDynamicDirectoryPath),
                    Load (_markedExeAssemblyInDynamicDirectoryPath),
                    LoadFile (_markedAssemblyInSearchPathWithNameMismatchPath)
                }));
      });
    }

    [Test]
    public void FindRootAssemblies_WithConsiderDynamicDirectoryFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly firstInMemoryAssembly = CompileTestAssemblyInMemory ("FirstInMemoryAssembly", _markedReferencedAssemblyPath);
        Assembly secondInMemoryAssembly = CompileTestAssemblyInMemory ("SecondInMemoryAssembly");
        CompileTestAssemblyInMemory ("UnmarkedInMemoryAssembly");

        InitializeDynamicDirectory ();

        var assemblyLoader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (false, assemblyLoader);
        var rootAssemblies = rootAssemblyFinder.FindRootAssemblies ();

        Assert.That (rootAssemblies, Has.No.Member(firstInMemoryAssembly));
        Assert.That (rootAssemblies, Has.No.Member(secondInMemoryAssembly));

        Assert.That (
            rootAssemblies.Select (root => root.Assembly).ToArray (),
            Is.EquivalentTo (
                new[]
                {
                    Load (_markedAssemblyPath),
                    Load (_markedExeAssemblyPath),
                    Load (_markedAssemblyWithDerivedAttributePath),
                    Load (_markedReferencedAssemblyPath),
                    Load (_markedAssemblyInSearchPathPath),
                    Load (_markedExeAssemblyInSearchPathPath),
                    LoadFile (_markedAssemblyInSearchPathWithNameMismatchPath)
                }));
      });
    }

    [Test]
    public void FindAssemblies_References ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        Assembly markedAssembly = Load (_markedAssemblyPath);

        var loader = CreateLoaderForMarkedAssemblies ();
        var rootAssemblyFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
        rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (new[] { new RootAssembly (markedAssembly, true) });
        rootAssemblyFinderStub.Replay ();

        var assemblyFinder = new AssemblyFinder (rootAssemblyFinderStub, loader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();
        Assert.That (assemblies, Is.EquivalentTo (new[] { markedAssembly, Load (_markedReferencedAssemblyPath) }));
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        InitializeDynamicDirectory ();

        var filterMock = new MockRepository ().StrictMock<IAssemblyLoaderFilter> ();
        filterMock.Expect (mock => mock.ShouldConsiderAssembly (Arg<AssemblyName>.Is.Anything)).Return (false).Repeat.AtLeastOnce ();
        filterMock.Replay ();

        var assemblyLoader = new FilteringAssemblyLoader (filterMock);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.VerifyAllExpectations ();
        Assert.That (assemblies, Is.Empty);
      });
    }

    [Test]
    public void FindAssemblies_WithSpecificFiler_ConsiderAssemblyTrueIncludeAssemblyFalse ()
    {
      ExecuteInSeparateAppDomain (delegate
      {
        InitializeDynamicDirectory ();

        var filterMock = new MockRepository ().StrictMock<IAssemblyLoaderFilter> ();
        filterMock.Expect (mock => mock.ShouldConsiderAssembly (Arg<AssemblyName>.Is.NotNull)).Return (true).Repeat.AtLeastOnce ();
        filterMock.Expect (mock => mock.ShouldIncludeAssembly (Arg<Assembly>.Is.NotNull)).Return (false).Repeat.AtLeastOnce ();
        filterMock.Replay ();

        var assemblyLoader = new FilteringAssemblyLoader (filterMock);
        var rootAssemblyFinder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain (true, assemblyLoader);
        var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);

        Assembly[] assemblies = assemblyFinder.FindAssemblies().ToArray();

        filterMock.VerifyAllExpectations();
        Assert.That (assemblies, Is.Empty);
      });
    }

    private void ExecuteInSeparateAppDomain (CrossAppDomainDelegate test)
    {
      AppDomain appDomain = null;

      try
      {
        var appDomainSetup = new AppDomainSetup ();
        appDomainSetup.ApplicationName = "Test";
        appDomainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        appDomainSetup.PrivateBinPath = _searchPathForDllsBuildOutputManager.BuildOutputDirectory + ";" + _searchPathForExesBuildOutputManager.BuildOutputDirectory;
        appDomainSetup.DynamicBase = _dynamicDirectoryBuildOutputManager.BuildOutputDirectory;
        appDomainSetup.ShadowCopyFiles = AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles;

        appDomain = AppDomain.CreateDomain ("Test", null, appDomainSetup);

        appDomain.DoCallBack (test);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);
      }
    }

    private void InitializeDynamicDirectory ()
    {
      _dynamicDirectoryBuildOutputManager.CopyAllGeneratedAssembliesToNewDirectory (AppDomain.CurrentDomain.DynamicDirectory);
    }

    private Assembly CompileTestAssemblyInMemory (string assemblyName, params string[] referencedAssemblies)
    {
      AssemblyCompiler assemblyCompiler = AssemblyCompiler.CreateInMemoryAssemblyCompiler (
          c_testAssemblySourceDirectoryRoot + "\\" + assemblyName,
          ArrayUtility.Combine (new[] { typeof (MarkerAttribute).Module.Name }, 
          referencedAssemblies));
      assemblyCompiler.Compile ();
      return assemblyCompiler.CompiledAssembly;
    }

    private AssemblyCompilerBuildOutputManager CreateAssemblyCompilerBuildOutputManager (string buildOutputDirectory)
    {
      var createBuildOutputDirectory = buildOutputDirectory != AppDomain.CurrentDomain.BaseDirectory;
      return new AssemblyCompilerBuildOutputManager (
          buildOutputDirectory, createBuildOutputDirectory, c_testAssemblySourceDirectoryRoot, typeof (MarkerAttribute).Module.Name);
    }

    private FilteringAssemblyLoader CreateLoaderForMarkedAssemblies ()
    {
      var markerAttributeType = typeof (MarkerAttribute);
      var attributeFilter = new AttributeAssemblyLoaderFilter (markerAttributeType);
      return new FilteringAssemblyLoader (attributeFilter);
    }

    private Assembly Load (string assemblyPath)
    {
      var assemblyName = Path.GetFileNameWithoutExtension (assemblyPath);
      return Assembly.Load (assemblyName);
    }

    private Assembly LoadFile (string assemblyPath)
    {
      return Assembly.LoadFile (assemblyPath);
    }
  }
}
