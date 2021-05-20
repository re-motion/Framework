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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;

namespace Remotion.UnitTests.Reflection.TypeDiscovery.AssemblyFinding
{
  [TestFixture]
  public class CachingAssemblyFinderDecoratorTest
  {
    private Mock<IAssemblyFinder> _innerFinder;
    private CachingAssemblyFinderDecorator _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerFinder = new Mock<IAssemblyFinder> (MockBehavior.Strict);
      _decorator = new CachingAssemblyFinderDecorator(_innerFinder.Object);
    }

    [Test]
    public void FindAssemblies_FirstTime ()
    {
      var assemblies = new[] { typeof (object).Assembly, GetType().Assembly };
      _innerFinder.Setup (mock => mock.FindAssemblies()).Returns (assemblies).Verifiable();

      var result = _decorator.FindAssemblies();

      _innerFinder.Verify();
      Assert.That (result, Is.EqualTo (assemblies));
    }

    [Test]
    public void FindAssemblies_MultipleTimes ()
    {
      var assemblies = new[] { typeof (object).Assembly, GetType ().Assembly };
      _innerFinder
          .Setup (mock => mock.FindAssemblies ())
          .Returns (assemblies)
          .Verifiable();

      var result1 = _decorator.FindAssemblies ();
      var result2 = _decorator.FindAssemblies ();

      _innerFinder.Verify (mock => mock.FindAssemblies (), Times.Once());
      Assert.That (result1, Is.EqualTo (assemblies));
      Assert.That (result2, Is.EqualTo (result1));
    }
  }
}