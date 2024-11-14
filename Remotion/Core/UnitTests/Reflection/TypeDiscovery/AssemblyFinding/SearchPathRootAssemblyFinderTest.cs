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
using Moq;
using NUnit.Framework;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class SearchPathRootAssemblyFinderTest
  {
    private Mock<IAssemblyLoader> _loaderStub;

    [SetUp]
    public void SetUp ()
    {
      _loaderStub = new Mock<IAssemblyLoader>();
    }

    [Test]
    public void CreateCombinedFinder_HoldsBaseDirectory ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", false);
      var finderDirectories = GetDirectoriesForCombinedFinder(finder);

      Assert.That(finderDirectories, Has.Member("baseDirectory"));
    }

    [Test]
    public void CreateCombinedFinder_HoldsRelativeSearchPath ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", false);
      var finderDirectories = GetDirectoriesForCombinedFinder(finder);

      Assert.That(finderDirectories, Has.Member("relativeSearchPath"));
    }

    [Test]
    public void CreateCombinedFinder_HoldsRelativeSearchPath_Split ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath1;relativeSearchPath2", false);
      var finderDirectories = GetDirectoriesForCombinedFinder(finder);

      Assert.That(finderDirectories, Has.Member("relativeSearchPath1"));
      Assert.That(finderDirectories, Has.Member("relativeSearchPath2"));
    }

    [Test]
    public void CreateCombinedFinder_Specifications ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", false);
      var finderSpecs = CheckCombinedFinderAndGetSpecifications(finder);

      Assert.That(finderSpecs, Is.EquivalentTo(new[] {
          new FilePatternSpecification("*.dll", FilePatternSpecificationKind.IncludeFollowReferences) }));
    }

    [Test]
    public void CreateCombinedFinder_SearchService ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", false);
      var finderService = GetSearchServiceForCombinedFinder(finder);

      Assert.That(finderService, Is.InstanceOf(typeof(FileSystemSearchService)));
    }

    [Test]
    public void CreateCombinedFinder_ConsiderDynamicDirectory_False ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", false);
      var finderDirectories = GetDirectoriesForCombinedFinder(finder);

      Assert.That(finderDirectories, Has.No.Member("dynamicDirectory"));
    }

    [Test]
    public void CreateCombinedFinder_ConsiderDynamicDirectory_True ()
    {
      var finder = CreateRootAssemblyFinder("relativeSearchPath", true);
      var finderDirectories = GetDirectoriesForCombinedFinder(finder);
      var finderSpecs = CheckCombinedFinderAndGetSpecifications(finder);
      var finderService = GetSearchServiceForCombinedFinder(finder);

      Assert.That(finderDirectories, Has.Member("dynamicDirectory"));
      Assert.That(finderSpecs, Is.EquivalentTo(new[] {
          new FilePatternSpecification("*.dll", FilePatternSpecificationKind.IncludeFollowReferences) }));
      Assert.That(finderService, Is.InstanceOf(typeof(FileSystemSearchService)));
    }

    [Test]
    public void FindRootAssemblies_UsesCombinedFinder ()
    {
      var innerFinderStub = new Mock<IRootAssemblyFinder>();
      var rootAssembly = new RootAssembly(typeof(object).Assembly, true);
      innerFinderStub.Setup(stub => stub.FindRootAssemblies()).Returns(new[] { rootAssembly });

      var finderMock = new Mock<SearchPathRootAssemblyFinder>(
          "baseDirectory",
          "relativeSearchPath",
          false,
          "dynamicDirectory",
          _loaderStub.Object)
          { CallBase = true };
      finderMock.Setup(mock => mock.CreateCombinedFinder()).Returns(new CompositeRootAssemblyFinder(new[] { innerFinderStub.Object })).Verifiable();

      var result = finderMock.Object.FindRootAssemblies();
      Assert.That(result, Is.EqualTo(new[] { rootAssembly }));

      finderMock.Verify();
    }

    [Test]
    public void CreateForCurrentAppDomain ()
    {
      var considerDynamicDirectory = BooleanObjectMother.GetRandomBoolean();

      var finder = SearchPathRootAssemblyFinder.CreateForCurrentAppDomain(considerDynamicDirectory, _loaderStub.Object);

      Assert.That(finder.BaseDirectory, Is.EqualTo(AppContext.BaseDirectory));
      Assert.That(finder.RelativeSearchPath, Is.EqualTo(AppDomain.CurrentDomain.RelativeSearchPath));
      Assert.That(finder.DynamicDirectory, Is.EqualTo(AppDomain.CurrentDomain.DynamicDirectory));
      Assert.That(finder.AssemblyLoader, Is.SameAs(_loaderStub.Object));
      Assert.That(finder.ConsiderDynamicDirectory, Is.EqualTo(considerDynamicDirectory));
    }

    private string[] GetDirectoriesForCombinedFinder (SearchPathRootAssemblyFinder finder)
    {
      var combinedFinder = finder.CreateCombinedFinder();
      return combinedFinder.InnerFinders.Cast<FilePatternRootAssemblyFinder>().Select(f => f.SearchPath).ToArray();
    }

    private FilePatternSpecification[] CheckCombinedFinderAndGetSpecifications (SearchPathRootAssemblyFinder finder)
    {
      var innerFinders = finder.CreateCombinedFinder()
          .InnerFinders
          .Cast<FilePatternRootAssemblyFinder>();

      foreach (var innerFinder in innerFinders)
        Assert.That(innerFinder.AssemblyLoader, Is.SameAs(_loaderStub.Object));

      return innerFinders
          .SelectMany(inner => inner.Specifications)
          .Distinct()
          .ToArray();
    }

    private IFileSearchService GetSearchServiceForCombinedFinder (SearchPathRootAssemblyFinder finder)
    {
      return finder.CreateCombinedFinder()
          .InnerFinders
          .Cast<FilePatternRootAssemblyFinder>()
          .Select(inner => inner.FileSearchService)
          .Distinct()
          .Single();
    }

    private SearchPathRootAssemblyFinder CreateRootAssemblyFinder (string relativeSearchPath, bool considerDynamicDirectory)
    {
      return new SearchPathRootAssemblyFinder("baseDirectory", relativeSearchPath, considerDynamicDirectory, "dynamicDirectory", _loaderStub.Object);
    }
  }
}
