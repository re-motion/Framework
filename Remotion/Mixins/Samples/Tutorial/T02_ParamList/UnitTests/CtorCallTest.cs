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
using NUnit.Framework;
using Remotion.Mixins.Samples.Tutorial.T02_ParamList.Core;
using Remotion.TypePipe;

namespace Remotion.Mixins.Samples.Tutorial.T02_ParamList.UnitTests
{
  [TestFixture]
  public class CtorCallTest
  {
    [Test]
    public void Activator_BrokenCalls_ArrayPassed ()
    {
      Assert.That(
          // looks for a ctor with five string arguments
          () => (TheClass)Activator.CreateInstance(typeof(TheClass), new[] { "my", "home", "is", "my", "castle" }),
          Throws.InstanceOf<MissingMethodException>());
    }

    [Test]
    public void Activator_BrokenCalls_SingleNullPassed ()
    {
      Assert.That(
          // looks for a default ctor
          () => (TheClass)Activator.CreateInstance(typeof(TheClass), null),
          Throws.InstanceOf<MissingMethodException>());
    }

    [Test]
    public void Activator_BrokenCalls_AmbiguousNullPassed ()
    {
      Assert.That(
          () => (TheClass)Activator.CreateInstance(typeof(TheClass), new object[] { null }),
          Throws.InstanceOf<AmbiguousMatchException>());
    }

    [Test]
    public void ParamList_ArrayPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass>(ParamList.Create(new[] { "my", "home", "is", "my", "castle" }));
      Assert.That(theObject1.ConstructionInfo, Is.EqualTo("Many strings: my, home, is, my, castle"));
    }

    [Test]
    public void ParamList_SingleNullPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass>(ParamList.Create((string)null));
      Assert.That(theObject1.ConstructionInfo, Is.EqualTo("Not even one string."));
    }

    [Test]
    public void ParamList_OtherNullPassed ()
    {
      var theObject1 = TheObjectFactory.Create<TheClass>(ParamList.Create((string[])null));
      Assert.That(theObject1.ConstructionInfo, Is.EqualTo("Not many strings."));
    }

  }
}
