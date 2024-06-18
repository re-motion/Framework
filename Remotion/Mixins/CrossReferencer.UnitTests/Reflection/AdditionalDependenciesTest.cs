// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System.Linq;
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class AdditionalDependenciesTest
  {

    [Test]
    public void AdditionalDependencies ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew ().ForClass<TargetClass> ()
          .AddMixin<Mixin1> ()
          .AddMixin<Mixin2> ()
          .AddMixin<Mixin3> ()
          .WithDependencies<Mixin1, Mixin2>()
          .BuildConfiguration ();

      var explicitDependencies = mixinConfiguration.ClassContexts.Single().Mixins.Last().ExplicitDependencies;

      Assert.That (explicitDependencies.Count, Is.EqualTo(2));
      Assert.That (explicitDependencies.First (), Is.EqualTo (typeof(Mixin1)));
      Assert.That (explicitDependencies.Last (), Is.EqualTo (typeof (Mixin2)));
      }

    #region TestDomain for AdditionalDependenciesTest

    public class TargetClass
    {
    }

    public class Mixin1{}

    public class Mixin2{}

    public class Mixin3 { }

    #endregion
  }
}