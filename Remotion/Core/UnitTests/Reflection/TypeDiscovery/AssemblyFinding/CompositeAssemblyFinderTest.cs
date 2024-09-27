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
using Moq;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class CompositeAssemblyFinderTest
  {
    private class AssemblyStub (string name) : Assembly
    {
      public override string FullName { get; } = name;
    }

    private Assembly _assembly1;
    private Assembly _assembly2;
    private Assembly _assembly3;

    [SetUp]
    public void SetUp ()
    {
      _assembly1 = new AssemblyStub("Assembly1");
      _assembly2 = new AssemblyStub("Assembly2");
      _assembly3 = new AssemblyStub("Assembly3");
    }

    [Test]
    public void FindAssemblies_NoInnerFinders ()
    {
      var finder = new CompositeAssemblyFinder(Array.Empty<IAssemblyFinder>());

      var assemblies = finder.FindAssemblies();

      Assert.That(assemblies, Is.Empty);
    }

    [Test]
    public void FindAssemblies_InnerFinder ()
    {
      IAssemblyFinder innerFinderStub = CreateInnerFinderStub(_assembly1, _assembly2);
      var finder = new CompositeAssemblyFinder([innerFinderStub]);

      var assemblies = finder.FindAssemblies();
      Assert.That(assemblies, Is.EquivalentTo(new[] { _assembly1, _assembly2 }));
    }

    [Test]
    public void FindAssemblies_MultipleInnerFinders ()
    {
      IAssemblyFinder innerFinderStub1 = CreateInnerFinderStub(_assembly1, _assembly2);
      IAssemblyFinder innerFinderStub2 = CreateInnerFinderStub(_assembly3);

      var finder = new CompositeAssemblyFinder([innerFinderStub1, innerFinderStub2]);

      var assemblies = finder.FindAssemblies();
      Assert.That(assemblies, Is.EquivalentTo(new[] { _assembly1, _assembly2, _assembly3 }));
    }

    private IAssemblyFinder CreateInnerFinderStub (params Assembly[] assemblies)
    {
      var innerFinderStub = new Mock<IAssemblyFinder>();
      innerFinderStub.Setup(stub => stub.FindAssemblies()).Returns(assemblies);

      return innerFinderStub.Object;
    }
  }
}
