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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationValidationTest
  {
    [Test]
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<NullTarget>().Clear().AddMixins(typeof(NullMixin)).EnterScope())
        {
          var data = MixinConfiguration.ActiveConfiguration.Validate();
          Assert.That(data.GetNumberOfSuccesses() > 0, Is.True);
          Assert.That(data.GetNumberOfFailures(), Is.EqualTo(0));
        }
      }
    }

    [Test]
    public void ValidateWithErrors ()
    {
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<int>().Clear().AddMixins(typeof(NullMixin)).EnterScope())
        {
          var data = MixinConfiguration.ActiveConfiguration.Validate();
          Assert.That(data.GetNumberOfFailures() > 0, Is.True);
        }
      }
    }

    class UninstantiableGeneric<T>
      where T : IServiceProvider
    {
    }

    [Test]
    public void ValidateWithGenerics_IgnoresGenerics ()
    {
      using (MixinConfiguration.BuildNew()
          .ForClass(typeof(KeyValuePair<,>)).Clear().AddMixins(typeof(NullMixin))
          .ForClass(typeof(UninstantiableGeneric<>)).Clear().AddMixins(typeof(NullMixin))
          .EnterScope())
      {
        var data = MixinConfiguration.ActiveConfiguration.Validate();
        Assert.That(data.GetNumberOfFailures(), Is.EqualTo(0));
      }
    }
  }
}
