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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;
using Remotion.Development.UnitTesting.IsolatedCodeRunner;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class AssemblyFinderTest
  {
    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;

    private ILoggerFactory _bootstrapLoggerFactory;

    [OneTimeSetUp]
    public void OneTimeSetUp ()
    {
      _bootstrapLoggerFactory = BootstrapServiceConfiguration.GetLoggerFactory();
    }

    [OneTimeTearDown]
    public void TeastFixtureTearDown ()
    {
      Assert.That(BootstrapServiceConfiguration.GetLoggerFactory(), Is.SameAs(_bootstrapLoggerFactory));
    }

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = typeof(Enumerable).Assembly;
      _assembly2 = typeof(AssemblyFinder).Assembly;
      _assembly3 = typeof(AssemblyFinderTest).Assembly;
    }

    [Test]
    public void FindAssemblies_FindsRootAssemblies ()
    {
      var loaderStub = new Mock<IAssemblyLoader>();

      var rootAssemblyFinderMock = new Mock<IRootAssemblyFinder>();
      rootAssemblyFinderMock
          .Setup(mock => mock.FindRootAssemblies())
          .Returns(new[] { new RootAssembly(_assembly1, true), new RootAssembly(_assembly2, true) })
          .Verifiable();

      var finder = new AssemblyFinder(rootAssemblyFinderMock.Object, loaderStub.Object);
      var result = finder.FindAssemblies();

      rootAssemblyFinderMock.Verify();
      Assert.That(result, Is.EquivalentTo(new[] { _assembly1, _assembly2 }));
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies ()
    {
      var loaderMock = new Mock<IAssemblyLoader>();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly1), _assembly3.FullName))
          .Returns(_assembly1)
          .Verifiable();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly2), _assembly3.FullName))
          .Returns(_assembly2)
          .Verifiable();

      var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
      rootAssemblyFinderStub.Setup(stub => stub.FindRootAssemblies()).Returns(new[] { new RootAssembly(_assembly3, true) });

      var finder = new AssemblyFinder(rootAssemblyFinderStub.Object, loaderMock.Object);
      var result = finder.FindAssemblies();

      loaderMock.Verify();
      Assert.That(result, Is.EquivalentTo(new[] { _assembly1, _assembly2, _assembly3 }));
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_Transitive ()
    {
      const string buildOutputDirectory = "Reflection.AssemblyFinderTest.FindAssemblies_FindsReferencedAssemblies_Transitive";
      string sourceDirectoryRoot = Path.Combine(TestContext.CurrentContext.TestDirectory, @"Reflection\TypeDiscovery\AssemblyFinding\TestAssemblies\AssemblyFinderTest");

      using (var outputManager = new AssemblyCompilerBuildOutputManager(buildOutputDirectory, true, sourceDirectoryRoot))
      {
        // dependency chain: mixinSamples -> remotion -> logging
        var loggingAbstractionsAssembly = typeof(Microsoft.Extensions.Logging.ILogger).Assembly;
        var remotionAssembly = typeof(AssemblyFinder).Assembly;
        var referencingAssemblyFullPath = CompileReferencingAssembly(outputManager, remotionAssembly);
        var isolatedCodeRunner = new IsolatedCodeRunner(TestMain);
        isolatedCodeRunner.Run(referencingAssemblyFullPath);

        static void TestMain (string[] args)
        {
          BootstrapServiceConfiguration.SetLoggerFactory(NullLoggerFactory.Instance);

          var loggingAbstractionsAssembly = typeof(Microsoft.Extensions.Logging.ILogger).Assembly;
          var remotionAssembly = typeof(AssemblyFinder).Assembly;
          Test(loggingAbstractionsAssembly, remotionAssembly, Assembly.LoadFile(args[0]));
        }
      }

      static void Test (Assembly loggingAbstractionsAssembly, Assembly remotionAssembly, Assembly referencingAssembly)
      {
        var loaderMock = new Mock<IAssemblyLoader>();
        loaderMock
            .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(remotionAssembly), referencingAssembly.FullName))
            // load re-motion via samples
            .Returns(remotionAssembly)
            .Verifiable();
        loaderMock
            .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(loggingAbstractionsAssembly), remotionAssembly.FullName))
            // load logging via re-motion
            .Returns(loggingAbstractionsAssembly)
            .Verifiable();

        var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
        rootAssemblyFinderStub
            .Setup(stub => stub.FindRootAssemblies())
            .Returns(new[] { new RootAssembly(referencingAssembly, true) });

        var finder = new AssemblyFinder(rootAssemblyFinderStub.Object, loaderMock.Object);
        var result = finder.FindAssemblies();

        loaderMock.Verify();
        Assert.That(result, Is.EquivalentTo(new[] { referencingAssembly, remotionAssembly, loggingAbstractionsAssembly }));
      }
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_Transitive_NotTwice ()
    {
      // dependency chain: _assembly3 -> _assembly2 -> _assembly1; _assembly3 -> _assembly1
      Assert.That(IsAssemblyReferencedBy(_assembly2, _assembly3), Is.True);
      Assert.That(IsAssemblyReferencedBy(_assembly1, _assembly3), Is.True);
      Assert.That(IsAssemblyReferencedBy(_assembly1, _assembly2), Is.True);

      // because _assembly1 is already loaded via _assembly3, it's not tried again via _assembly2

      var loaderMock = new Mock<IAssemblyLoader>();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly2), _assembly3.FullName)) // load _assembly2 via _assembly3
          .Returns(_assembly2)
          .Verifiable();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly1), _assembly3.FullName)) // load _assembly1 via _assembly3
          .Returns((Assembly)null)
          .Verifiable();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly1), _assembly2.FullName)) // _assembly1 already loaded, no second time
          .Verifiable();

      var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
      rootAssemblyFinderStub.Setup(stub => stub.FindRootAssemblies()).Returns(new[] { new RootAssembly(_assembly3, true) });

      var finder = new AssemblyFinder(rootAssemblyFinderStub.Object, loaderMock.Object);
      finder.FindAssemblies();

      loaderMock.Verify(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly2), _assembly3.FullName));
      loaderMock.Verify(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly1), _assembly3.FullName));
      loaderMock.Verify(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(_assembly1), _assembly2.FullName), Times.Never());
    }

    [Test]
    public void FindAssemblies_FindsReferencedAssemblies_NoFollowRoot ()
    {
      var loaderMock = new Mock<IAssemblyLoader>();

      var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
      rootAssemblyFinderStub.Setup(stub => stub.FindRootAssemblies()).Returns(new[] { new RootAssembly(_assembly3, false) });

      var finder = new AssemblyFinder(rootAssemblyFinderStub.Object, loaderMock.Object);
      var result = finder.FindAssemblies();

      loaderMock.Verify(mock => mock.TryLoadAssembly(It.IsAny<AssemblyName>(), It.IsAny<string>()), Times.Never());
      Assert.That(result, Is.EquivalentTo(new[] { _assembly3 }));
    }

    [Test]
    public void FindAssemblies_NoDuplicates ()
    {
      var assembly4 = typeof(TestFixtureAttribute).Assembly;
      var loaderMock = new Mock<IAssemblyLoader>();
      loaderMock
          .Setup(mock => mock.TryLoadAssembly(ArgReferenceMatchesDefinition(assembly4), _assembly3.FullName))
          .Returns(assembly4)
          .Verifiable();

      var rootAssemblyFinderStub = new Mock<IRootAssemblyFinder>();
      rootAssemblyFinderStub
          .Setup(stub => stub.FindRootAssemblies())
          .Returns(new[] { new RootAssembly(_assembly3, true), new RootAssembly(_assembly2, true) });

      var finder = new AssemblyFinder(rootAssemblyFinderStub.Object, loaderMock.Object);
      var result = finder.FindAssemblies().ToArray();

      loaderMock.Verify();
      Assert.That(result, Is.EquivalentTo(new[] { _assembly2, _assembly3, assembly4 }));
      Assert.That(result.Length, Is.EqualTo(3));
    }

    private static AssemblyName ArgReferenceMatchesDefinition (Assembly referencedAssembly)
    {
      return It.Is<AssemblyName>(name => AssemblyName.ReferenceMatchesDefinition(name, referencedAssembly.GetName()));
    }

    private bool IsAssemblyReferencedBy (Assembly referenced, Assembly origin)
    {
      return origin.GetReferencedAssemblies()
          .Where(assemblyName => AssemblyName.ReferenceMatchesDefinition(assemblyName, referenced.GetName()))
          .Any();
    }

    private static string CompileReferencingAssembly (AssemblyCompilerBuildOutputManager outputManager, Assembly remotionAssembly)
    {
      var referencingAssemblyRelativePath = outputManager.Compile("RemotionCoreReferencingAssembly.dll", remotionAssembly.Location);
      return Path.GetFullPath(referencingAssemblyRelativePath);
    }
  }
}
