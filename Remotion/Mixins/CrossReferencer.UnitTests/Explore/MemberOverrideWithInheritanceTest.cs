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
using NUnit.Framework;
using Remotion.Mixins;

namespace MixinXRef.UnitTests.Explore
{
  [TestFixture]
  public class MemberOverrideWithInheritanceTest
  {
    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.ResetMasterConfiguration();
    }

    [Test]
    public void InheritanceBehavior_OverrideMixin ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<BaseClass>().AddMixin<CustomMixin>().BuildConfiguration();
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      var subClass = (IAlgorithm) ObjectFactory.Create<SubClass>().With();
      var output = subClass.AlgorithmMethod();

      Assert.That (output, Is.EqualTo ("2, complex algorithm"));
    }

    #region TestDomain for InheritanceBehavior_OverrideMixin

    public class BaseClass
    {
      public virtual string DoSomething ()
      {
        return "1";
      }
    }

    public class SubClass : BaseClass
    {
      [OverrideMixin]
      public override string DoSomething ()
      {
        return "2";
      }
    }

    public class CustomMixin : Mixin<BaseClass>, IAlgorithm
    {
      public virtual string DoSomething ()
      {
        return "standard impl";
      }

      public string AlgorithmMethod ()
      {
        return DoSomething() + ", complex algorithm";
      }
    }

    public interface IAlgorithm
    {
      string AlgorithmMethod ();
    }

    #endregion

    [Test]
    public void InheritanceBehavior_OverrideTarget ()
    {
      var mixinConfiguration = MixinConfiguration.BuildNew().ForClass<TargetClass>().AddMixin<SubMixin>().BuildConfiguration();
      MixinConfiguration.SetActiveConfiguration (mixinConfiguration);

      var targetClass = ObjectFactory.Create<TargetClass>().With();
      var output = targetClass.DoSomething();

      Assert.That (output, Is.EqualTo("2"));
    }

    #region TestDomain for InheritanceBehavior_OverrideTarget

    public class TargetClass
    {
      public virtual string DoSomething ()
      {
        return "target class";
      }
    }

    public class BaseMixin
    {
      [OverrideTarget]
      public virtual string DoSomething ()
      {
        return "1";
      }
    }

    public class SubMixin : BaseMixin
    {
      public override string DoSomething ()
      {
        return "2";
      }
    }

    #endregion
  }
}