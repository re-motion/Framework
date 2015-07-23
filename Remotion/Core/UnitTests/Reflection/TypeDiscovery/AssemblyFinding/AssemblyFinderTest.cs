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
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class AssemblyFinderTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = typeof (object).Assembly;
      _assembly2 = typeof (AssemblyFinder).Assembly;
      _assembly3 = typeof (AssemblyFinderTest).Assembly;
    }

    [Test]
    public void FindAssemblies_FindsRootAssemblies ()
    {
      var loaderStub = MockRepository.GenerateStub<IAssemblyLoader> ();
      loaderStub.Replay ();

      var rootAssemblyFinderMock = MockRepository.GenerateMock<IRootAssemblyFinder> ();
      rootAssemblyFinderMock
          .Expect (mock => mock.FindRootAssemblies ())
          .Return (new[] { new RootAssembly (_assembly1, true), new RootAssembly(_assembly2, true) });
      rootAssemblyFinderMock.Replay ();
      
      var finder = new AssemblyFinder (rootAssemblyFinderMock, loaderStub);
      var result = finder.FindAssemblies ();

      rootAssemblyFinderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EquivalentTo (new[] { _assembly1, _assembly2 }));
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies ()
    {
      var loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (_assembly1), Arg.Is (_assembly3.FullName)))
          .Return (_assembly1);
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (_assembly2), Arg.Is (_assembly3.FullName)))
          .Return (_assembly2);
      loaderMock.Replay ();
      
      var rootAssemblyFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
      rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (new[] { new RootAssembly (_assembly3, true) });
      rootAssemblyFinderStub.Replay ();
      
      var finder = new AssemblyFinder (rootAssemblyFinderStub, loaderMock);
      var result = finder.FindAssemblies ();

      loaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EquivalentTo (new[] { _assembly1, _assembly2, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_Transitive ()
    {
      const string buildOutputDirectory = "Reflection.AssemblyFinderTest.FindAssemblies_FindsReferencedAssemblies_Transitive";
      const string sourceDirectoryRoot = @"Reflection\TypeDiscovery\AssemblyFinding\TestAssemblies\AssemblyFinderTest";

      Action<object[]> testAction = delegate (object[] args)
      {
        var outputManagerWithoutClosure = (AssemblyCompilerBuildOutputManager) args[0];

        // dependency chain: mixinSamples -> remotion -> log4net
        var log4NetAssembly = typeof (log4net.LogManager).Assembly;
        var remotionAssembly = typeof (AssemblyFinder).Assembly;
        var referencingAssembly = CompileReferencingAssembly (outputManagerWithoutClosure, remotionAssembly);

        var loaderMock = MockRepository.GenerateMock<IAssemblyLoader>();
        loaderMock
            .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (remotionAssembly), Arg.Is (referencingAssembly.FullName)))
            // load re-motion via samples
            .Return (remotionAssembly);
        loaderMock
            .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (log4NetAssembly), Arg.Is (remotionAssembly.FullName)))
            // load log4net via re-motion
            .Return (log4NetAssembly);
        loaderMock.Replay();

        var rootAssemblyFinderStub = MockRepository.GenerateMock<IRootAssemblyFinder>();
        rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (
            new[] { new RootAssembly (referencingAssembly, true) });
        rootAssemblyFinderStub.Replay();

        var finder = new AssemblyFinder (rootAssemblyFinderStub, loaderMock);
        var result = finder.FindAssemblies();

        loaderMock.VerifyAllExpectations();
        Assert.That (result, Is.EquivalentTo (new[] { referencingAssembly, remotionAssembly, log4NetAssembly }));
      };

      // Run test action in separate AppDomain
      using (var outputManager = new AssemblyCompilerBuildOutputManager (buildOutputDirectory, true, sourceDirectoryRoot))
      {
        AppDomainRunner.Run (testAction, outputManager);
      }
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_Transitive_NotTwice ()
    {
      // dependency chain: _assembly3 -> _assembly2 -> _assembly1; _assembly3 -> _assembly1
      Assert.That (IsAssemblyReferencedBy (_assembly2, _assembly3), Is.True);
      Assert.That (IsAssemblyReferencedBy (_assembly1, _assembly3), Is.True);
      Assert.That (IsAssemblyReferencedBy (_assembly1, _assembly2), Is.True);

      // because _assembly1 is already loaded via _assembly3, it's not tried again via _assembly2

      var loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (_assembly2), Arg.Is (_assembly3.FullName))) // load _assembly2 via _assembly3
          .Return (_assembly2);
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (_assembly1), Arg.Is (_assembly3.FullName))) // load _assembly1 via _assembly3
          .Return (null);
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (_assembly1), Arg.Is (_assembly2.FullName))) // _assembly1 already loaded, no second time
          .Repeat.Never ()
          .Return (_assembly2);
      loaderMock.Replay ();
      
      var rootAssemblyFinderStub = MockRepository.GenerateMock<IRootAssemblyFinder> ();
      rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (new[] { new RootAssembly (_assembly3, true) });
      rootAssemblyFinderStub.Replay ();

      var finder = new AssemblyFinder (rootAssemblyFinderStub, loaderMock);
      finder.FindAssemblies ();

      loaderMock.VerifyAllExpectations ();
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_NoFollowRoot ()
    {
      var loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();

      var rootAssemblyFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
      rootAssemblyFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (new[] { new RootAssembly(_assembly3, false) });
      rootAssemblyFinderStub.Replay ();

      var finder = new AssemblyFinder (rootAssemblyFinderStub, loaderMock);
      var result = finder.FindAssemblies ();

      loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly (Arg<AssemblyName>.Is.Anything, Arg<string>.Is.Anything));
      Assert.That (result, Is.EquivalentTo (new[] { _assembly3 }));
    }

    [Test]
    public void FindAssemblies_NoDuplicates ()
    {
      var assembly4 = typeof (TestFixtureAttribute).Assembly;
      var loaderMock = MockRepository.GenerateMock<IAssemblyLoader>();
      loaderMock
          .Expect (mock => mock.TryLoadAssembly (ArgReferenceMatchesDefinition (assembly4), Arg.Is (_assembly3.FullName)))
          .Return (assembly4);
      loaderMock.Replay();

      var rootAssemblyFinderStub = MockRepository.GenerateMock<IRootAssemblyFinder> ();
      rootAssemblyFinderStub
          .Stub (stub => stub.FindRootAssemblies ())
          .Return (new[] { new RootAssembly (_assembly3, true), new RootAssembly (_assembly2, true) });
      rootAssemblyFinderStub.Replay ();

      var finder = new AssemblyFinder (rootAssemblyFinderStub, loaderMock);
      var result = finder.FindAssemblies ().ToArray();

      loaderMock.VerifyAllExpectations();
      Assert.That (result, Is.EquivalentTo (new[] { _assembly2, _assembly3, assembly4 }));
      Assert.That (result.Length, Is.EqualTo (3));
    }

    private static AssemblyName ArgReferenceMatchesDefinition (Assembly referencedAssembly)
    {
      return Arg<AssemblyName>.Matches (name => AssemblyName.ReferenceMatchesDefinition (name, referencedAssembly.GetName()));
    }

    private bool IsAssemblyReferencedBy (Assembly referenced, Assembly origin)
    {
      return origin.GetReferencedAssemblies ()
          .Where (assemblyName => AssemblyName.ReferenceMatchesDefinition (assemblyName, referenced.GetName ()))
          .Any ();
    }

    private static Assembly CompileReferencingAssembly (AssemblyCompilerBuildOutputManager outputManager, Assembly remotionAssembly)
    {
      var referencingAssemblyRelativePath = outputManager.CompileInSeparateAppDomain ("RemotionCoreReferencingAssembly.dll", remotionAssembly.Location);
      var referencingAssemblyFullPath = Path.GetFullPath (referencingAssemblyRelativePath);
      return Assembly.LoadFile (referencingAssemblyFullPath);
    }
  }
}
