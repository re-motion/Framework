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
using System.Configuration;
using System.Linq;
using NUnit.Framework;
using Remotion.Configuration.TypeDiscovery;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.UnitTests.Configuration.TypeDiscovery
{
  [TestFixture]
  public class TypeDiscoveryConfigurationTest
  {
    private const string _xmlFragmentDefault = @"<typeDiscovery xmlns=""..."" />";
    private const string _xmlFragmentWithSpecificRootAssemblies = @"<typeDiscovery xmlns=""..."">
        <specificRootAssemblies>
          <byName>
            <include name=""mscorlib""/>
          </byName>
        </specificRootAssemblies>
      </typeDiscovery>";
    private const string _xmlFragmentWithSpecificEmptyRootAssemblies = @"<typeDiscovery xmlns=""..."">
        <specificRootAssemblies />
      </typeDiscovery>";

    [Test]
    public void Deserialization_Default ()
    {
      var section = Deserialize(_xmlFragmentDefault);
      Assert.That(section.SpecificRootAssemblies, Is.Not.Null);
      Assert.That(section.SpecificRootAssemblies.ByName, Is.Empty);
      Assert.That(section.SpecificRootAssemblies.ByFile, Is.Empty);
    }

    [Test]
    public void Deserialization_SpecificRootAssemblies ()
    {
      var section = Deserialize(_xmlFragmentWithSpecificRootAssemblies);
      Assert.That(section.SpecificRootAssemblies.ByName.Single().Name, Is.EqualTo("mscorlib"));
    }

    private TypeDiscoveryConfiguration Deserialize (string xmlFragment)
    {
      var section = new TypeDiscoveryConfiguration();
      ConfigurationHelper.DeserializeSection(section, xmlFragment);
      return section;
    }
  }
}
