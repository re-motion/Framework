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
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class FilePatternRootAssemblyFinderTest
  {
    private IFileSearchService _searchServiceStub;
    private IAssemblyLoader _loaderMock;
    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;

    [SetUp]
    public void SetUp ()
    {
      _searchServiceStub = MockRepository.GenerateStub<IFileSearchService> ();
      _loaderMock = MockRepository.GenerateMock<IAssemblyLoader> ();
      _assembly1 = typeof (object).Assembly;
      _assembly2 = typeof (FilePatternRootAssemblyFinder).Assembly;
      _assembly3 = typeof (FilePatternRootAssemblyFinderTest).Assembly;
    }

    [Test]
    public void FindAssemblies ()
    {
      var specification1 = new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeFollowReferences);

      StubSearchService ("*.dll", "1.dll", "2.dll");
      StubSearchService ("*.exe", "1.exe");

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (_assembly2);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly3);
      _loaderMock.Replay();

      var finder = CreateRootAssemblyFinder (specification1, specification2);

      var rootAssemblies = finder.FindRootAssemblies ().Select (ra => ra.Assembly).ToArray();

      _loaderMock.VerifyAllExpectations();
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _assembly1, _assembly2, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_Exclude ()
    {
      var specification1 = new FilePatternSpecification ("*.1", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.2", FilePatternSpecificationKind.Exclude);

      StubSearchService ("*.1", "1.dll", "2.dll"); // included
      StubSearchService ("*.2", "2.dll", "3.dll"); // excluded

      var finder = CreateRootAssemblyFinder (specification1, specification2);

      finder.FindRootAssemblies ().ForceEnumeration();

      _loaderMock.AssertWasCalled (mock => mock.TryLoadAssembly ("1.dll"));
      _loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly ("2.dll"));
      _loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly ("3.dll"));
    }

    [Test]
    public void FindAssemblies_Exclude_WithDifferentFollowKinds ()
    {
      var specification1 = new FilePatternSpecification ("*.1", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.2", FilePatternSpecificationKind.IncludeNoFollow);
      var specification3 = new FilePatternSpecification ("*.3", FilePatternSpecificationKind.Exclude);

      StubSearchService ("*.1", "1.dll", "2.dll"); // included
      StubSearchService ("*.2", "3.dll", "4.dll"); // included
      StubSearchService ("*.3", "2.dll", "3.dll"); // excluded

      var finder = CreateRootAssemblyFinder (specification1, specification2, specification3);

      finder.FindRootAssemblies ().ForceEnumeration();

      _loaderMock.AssertWasCalled (mock => mock.TryLoadAssembly ("1.dll"));
      _loaderMock.AssertWasCalled (mock => mock.TryLoadAssembly ("4.dll"));
      _loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly ("2.dll"));
      _loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly ("3.dll"));
    }

    [Test]
    public void FindAssemblies_Exclude_OnlyAffectsPreviousIncludes ()
    {
      var specification1 = new FilePatternSpecification ("*.1", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.2", FilePatternSpecificationKind.Exclude);
      var specification3 = new FilePatternSpecification ("*.3", FilePatternSpecificationKind.IncludeFollowReferences);

      StubSearchService ("*.1", "1.dll", "2.dll"); // included
      StubSearchService ("*.2", "1.dll", "2.dll"); // excluded
      StubSearchService ("*.3", "2.dll", "3.dll"); // included

      var finder = CreateRootAssemblyFinder (specification1, specification2, specification3);

      finder.FindRootAssemblies ().ForceEnumeration();

      _loaderMock.AssertWasCalled (mock => mock.TryLoadAssembly ("2.dll"));
      _loaderMock.AssertWasCalled (mock => mock.TryLoadAssembly ("3.dll"));
      _loaderMock.AssertWasNotCalled (mock => mock.TryLoadAssembly ("1.dll"));
    }

    [Test]
    public void FindAssemblies_NullsRemoved ()
    {
      var specification1 = new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeFollowReferences);

      StubSearchService ("*.dll", "1.dll", "2.dll");
      StubSearchService ("*.exe", "1.exe", "2.exe");

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (null);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly3);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.exe")).Return (null);
      _loaderMock.Replay ();

      var finder = CreateRootAssemblyFinder (specification1, specification2);

      var rootAssemblies = finder.FindRootAssemblies ().Select (ra => ra.Assembly).ToArray ();

      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies.Length, Is.EqualTo (2));
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _assembly1, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_DuplicatesAreNotRemoved ()
    {
      var specification1 = new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeFollowReferences);

      StubSearchService ("*.dll", "1.dll", "2.dll");
      StubSearchService ("*.exe", "1.exe", "2.exe");

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.dll")).Return (_assembly2);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("2.exe")).Return (_assembly2);
      _loaderMock.Replay ();

      var finder = CreateRootAssemblyFinder (specification1, specification2);

      var rootAssemblies = finder.FindRootAssemblies ().Select (ra => ra.Assembly).ToArray ();

      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies.Length, Is.EqualTo (4));
      Assert.That (rootAssemblies.Distinct(), Is.EquivalentTo (new[] { _assembly1, _assembly2 }));
    }

    [Test]
    public void FindAssemblies_FollowReferences ()
    {
      var specification1 = new FilePatternSpecification ("*.dll", FilePatternSpecificationKind.IncludeFollowReferences);
      var specification2 = new FilePatternSpecification ("*.exe", FilePatternSpecificationKind.IncludeNoFollow);

      StubSearchService ("*.dll", "1.dll");
      StubSearchService ("*.exe", "1.exe");

      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.dll")).Return (_assembly1);
      _loaderMock.Expect (mock => mock.TryLoadAssembly ("1.exe")).Return (_assembly2);
      _loaderMock.Replay ();

      var finder = CreateRootAssemblyFinder (specification1, specification2);

      var rootAssemblies = finder.FindRootAssemblies ().ToDictionary (ra => ra.Assembly);

      _loaderMock.VerifyAllExpectations ();
      Assert.That (rootAssemblies[_assembly1].FollowReferences, Is.True);
      Assert.That (rootAssemblies[_assembly2].FollowReferences, Is.False);
    }

    private FilePatternRootAssemblyFinder CreateRootAssemblyFinder (params FilePatternSpecification[] specifications)
    {
      return new FilePatternRootAssemblyFinder ("searchPath", specifications.AsOneTime (), _searchServiceStub, _loaderMock);
    }

    private void StubSearchService (string expectedPattern, params string[] fakeFiles)
    {
      _searchServiceStub.Stub (stub => stub.GetFiles ("searchPath", expectedPattern, SearchOption.TopDirectoryOnly)).Return (fakeFiles);
    }
  }
}
