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
using NUnit.Framework;
using Remotion.Mixins.Utilities;

namespace Remotion.Mixins.UnitTests.Core.Utilities
{
  [TestFixture]
  public class MixedObjectInstantiationScopeTest
  {
    [SetUp]
    public void SetUp ()
    {
      MixedObjectInstantiationScope.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      MixedObjectInstantiationScope.SetCurrent (null);
    }
    
    [Test]
    public void ScopeInitializedOnDemand ()
    {
      Assert.That (MixedObjectInstantiationScope.HasCurrent, Is.False);
      new MixedObjectInstantiationScope ();
      Assert.That (MixedObjectInstantiationScope.HasCurrent, Is.True);
    }

    [Test (Description = "Checks (in conjunction with ScopeInitializedOnDemand) whether this test fixture correctly resets the scope.")]
    public void CurrentIsReset ()
    {
      Assert.That (MixedObjectInstantiationScope.HasCurrent, Is.False);
      new MixedObjectInstantiationScope ();
      Assert.That (MixedObjectInstantiationScope.HasCurrent, Is.True);
    }

    [Test]
    public void DefaultMixinInstancesEmpty ()
    {
      Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances, Is.Not.Null);
      Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (0));
    }

    [Test]
    public void InstancesCanBeSuppliedInScopes ()
    {
      Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (0));
      using (new MixedObjectInstantiationScope ("1", "2"))
      {
        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (2));
        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0], Is.EqualTo ("1"));
        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[1], Is.EqualTo ("2"));

        using (new MixedObjectInstantiationScope ("a"))
        {
          Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (1));
          Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0], Is.EqualTo ("a"));

          using (new MixedObjectInstantiationScope ())
          {
            Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (0));
          }

          Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (1));
          Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0], Is.EqualTo ("a"));
        }

        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (2));
        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[0], Is.EqualTo ("1"));
        Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances[1], Is.EqualTo ("2"));
      }
      Assert.That (MixedObjectInstantiationScope.Current.SuppliedMixinInstances.Length, Is.EqualTo (0));
    }
  }
}
