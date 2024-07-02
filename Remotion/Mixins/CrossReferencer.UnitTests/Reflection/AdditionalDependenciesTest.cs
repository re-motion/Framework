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

namespace Remotion.Mixins.CrossReferencer.UnitTests.Reflection
{
  [TestFixture]
  public class AdditionalDependenciesTest
  {
    [Test]
    public void AdditionalDependencies ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass>()
          .AddMixin<Mixin1>()
          .AddMixin<Mixin2>()
          .AddMixin<Mixin3>()
          .WithDependencies<Mixin1, Mixin2>()
          .BuildConfiguration();

      var explicitDependencies = mixinConfiguration.ClassContexts.Single().Mixins.Last().ExplicitDependencies;

      Assert.That(explicitDependencies.Count, Is.EqualTo(2));
      Assert.That(explicitDependencies.First(), Is.EqualTo(typeof(Mixin1)));
      Assert.That(explicitDependencies.Last(), Is.EqualTo(typeof(Mixin2)));
    }

    #region TestDomain for AdditionalDependenciesTest

    public class TargetClass
    {
    }

    public class Mixin1
    {
    }

    public class Mixin2
    {
    }

    public class Mixin3
    {
    }

    #endregion
  }
}
