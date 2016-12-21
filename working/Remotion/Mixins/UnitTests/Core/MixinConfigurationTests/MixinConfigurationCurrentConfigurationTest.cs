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
using Remotion.Development.UnitTesting;

namespace Remotion.Mixins.UnitTests.Core.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationCurrentConfigurationTest
  {
    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.SetActiveConfiguration (null);
    }

    [Test]
    public void SetActiveConfiguration ()
    {
      MixinConfiguration oldConfiguration = MixinConfiguration.ActiveConfiguration;

      var newConfiguration = new MixinConfiguration();
      MixinConfiguration.SetActiveConfiguration (newConfiguration);

      Assert.That (MixinConfiguration.ActiveConfiguration, Is.Not.SameAs (oldConfiguration));
      Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (newConfiguration));
    }

    [Test]
    public void ActiveConfigurationIsThreadSpecific()
    {
      var newConfiguration = new MixinConfiguration ();
      MixinConfiguration.SetActiveConfiguration (newConfiguration);

      ThreadRunner.Run (() => Assert.That (MixinConfiguration.ActiveConfiguration, Is.Not.SameAs (newConfiguration)));
    }

    [Test]
    public void EnterScope()
    {
      var configuration1 = new MixinConfiguration ();
      var configuration2 = new MixinConfiguration ();

      MixinConfiguration.SetActiveConfiguration (null);
      Assert.That (MixinConfiguration.HasActiveConfiguration, Is.False);

      using (configuration1.EnterScope())
      {
        Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (configuration1));

        using (configuration2.EnterScope())
        {
          Assert.That (MixinConfiguration.ActiveConfiguration, Is.Not.SameAs (configuration1));
          Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (configuration2));
        }

        Assert.That (MixinConfiguration.ActiveConfiguration, Is.Not.SameAs (configuration2));
        Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (configuration1));
      }

      Assert.That (MixinConfiguration.HasActiveConfiguration, Is.False);
    }

    [Test]
    public void MasterConfigurationIsCopiedByNewThreads ()
    {
      var oldMasterConfiguration = MixinConfiguration.GetMasterConfiguration ();
      try
      {
        var newMasterConfiguration = new MixinConfiguration ();
        MixinConfiguration.SetMasterConfiguration (newMasterConfiguration);

        ThreadRunner.Run (() => Assert.That (MixinConfiguration.ActiveConfiguration, Is.SameAs (newMasterConfiguration)));
      }
      finally
      {
        MixinConfiguration.SetMasterConfiguration (oldMasterConfiguration);
      }
    }
  }
}
