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
using Remotion.Configuration.ServiceLocation;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.Configuration.ServiceLocation
{
  [TestFixture]
  public class ServiceLocationConfigurationTest
  {
    private const string _xmlFragmentDefault = @"<serviceLocation xmlns=""..."" />";
    private const string _xmlFragmentWithServiceLocatorProvider = @"<serviceLocation xmlns=""..."">
        <serviceLocatorProvider type=""Remotion.UnitTests::Configuration.ServiceLocation.FakeServiceLocatorProvider"" />
      </serviceLocation>";

    [Test]
    public void Deserialization_Default ()
    {
      var section = Deserialize (_xmlFragmentDefault);
      Assert.That (section.ServiceLocatorProvider.Type, Is.SameAs (typeof (DefaultServiceLocatorProvider)));
    }

    [Test]
    public void Deserialization_SpecificProvider ()
    {
      var section = Deserialize (_xmlFragmentWithServiceLocatorProvider);
      Assert.That (section.ServiceLocatorProvider.Type, Is.SameAs (typeof (FakeServiceLocatorProvider)));
    }

    [Test]
    public void CreateServiceLocatorProvider_Default ()
    {
      var section = Deserialize (_xmlFragmentDefault);
      Assert.That (section.CreateServiceLocatorProvider (), Is.TypeOf<DefaultServiceLocatorProvider> ());
    }

    [Test]
    public void CreateServiceLocatorProvider_SpecificProvider ()
    {
      var section = Deserialize (_xmlFragmentWithServiceLocatorProvider);
      Assert.That (section.CreateServiceLocatorProvider (), Is.TypeOf<FakeServiceLocatorProvider>());
    }

    private ServiceLocationConfiguration Deserialize (string xmlFragment)
    {
      var section = new ServiceLocationConfiguration ();
      ConfigurationHelper.DeserializeSection (section, xmlFragment);
      return section;
    }
  }
}
