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
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class CompositeRootAssemblyFinderTest
  {
    private RootAssembly _root1;
    private RootAssembly _root2;
    private RootAssembly _root3;

    [SetUp]
    public void SetUp ()
    {
      _root1 = new RootAssembly (typeof (object).Assembly, true);
      _root2 = new RootAssembly (typeof (CompositeRootAssemblyFinder).Assembly, true);
      _root3 = new RootAssembly (typeof (CompositeRootAssemblyFinderTest).Assembly, true);
    }

    [Test]
    public void FindRootAssemblies_NoInnerFinders ()
    {
      var finder = new CompositeRootAssemblyFinder (new IRootAssemblyFinder[0]);

      var rootAssemblies = finder.FindRootAssemblies();
      Assert.That (rootAssemblies, Is.Empty);
    }

    [Test]
    public void FindRootAssemblies_InnerFinder ()
    {
      IRootAssemblyFinder innerFinderStub = CreateInnerFinderStub (_root1, _root2);
      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub });

      var rootAssemblies = finder.FindRootAssemblies ();
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _root1, _root2 }));
    }

    [Test]
    public void FindRootAssemblies_MultipleInnerFinders ()
    {
      IRootAssemblyFinder innerFinderStub1 = CreateInnerFinderStub (_root1, _root2);
      IRootAssemblyFinder innerFinderStub2 = CreateInnerFinderStub (_root3);

      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub1, innerFinderStub2 });

      var rootAssemblies = finder.FindRootAssemblies ();
      Assert.That (rootAssemblies, Is.EquivalentTo (new[] { _root1, _root2, _root3 }));
    }

    [Test]
    public void FindRootAssemblies_RemovesAreNotDuplicates ()
    {
      IRootAssemblyFinder innerFinderStub1 = CreateInnerFinderStub (_root1, _root2, _root2);
      IRootAssemblyFinder innerFinderStub2 = CreateInnerFinderStub (_root3, _root2, _root1);

      var finder = new CompositeRootAssemblyFinder (new[] { innerFinderStub1, innerFinderStub2 });

      var rootAssemblies = finder.FindRootAssemblies ().ToArray();
      Assert.That (rootAssemblies.Length, Is.EqualTo (6)); 
      Assert.That (rootAssemblies.Distinct(), Is.EquivalentTo (new[] { _root1, _root2, _root3 }));
    }

    private IRootAssemblyFinder CreateInnerFinderStub (params RootAssembly[] assemblies)
    {
      var innerFinderStub = MockRepository.GenerateStub<IRootAssemblyFinder> ();
      innerFinderStub.Stub (stub => stub.FindRootAssemblies ()).Return (assemblies);
      innerFinderStub.Replay ();
      return innerFinderStub;
    }
  }
}
