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
using System.Collections.Specialized;
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class ExtendedProviderBaseTest
  {
    [Test]
    public void Initialize()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new StubExtendedProvider ("Provider", config);

      Assert.That (provider.Name, Is.EqualTo ("Provider"));
      Assert.That (provider.Description, Is.EqualTo ("The Description"));
    }

    [Test]
    public void GetAndRemoveNonEmptyStringAttribute ()
    {
      StubExtendedProvider provider = new StubExtendedProvider ("Provider", new NameValueCollection());
      NameValueCollection config = new NameValueCollection ();
      config.Add ("Name", "Value");
      config.Add ("Other", "OtherValue");

      Assert.That (provider.GetAndRemoveNonEmptyStringAttribute (config, "Name", "Provider", true), Is.EqualTo ("Value"));
      Assert.That (config.Get ("Other"), Is.EqualTo ("OtherValue"));
      Assert.That (config["Name"], Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         ExpectedMessage = "The attribute 'Name' is missing in the configuration of the 'Provider' provider.")]
    public void GetAndRemoveNonEmptyStringAttribute_WithMissingAttributeAndRequired ()
    {
      StubExtendedProvider provider = new StubExtendedProvider ("Provider", new NameValueCollection ());
      NameValueCollection config = new NameValueCollection ();

      provider.GetAndRemoveNonEmptyStringAttribute (config, "Name", "Provider", true);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         ExpectedMessage = "The attribute 'Name' is missing in the configuration of the 'Provider' provider.")]
    public void GetAndRemoveNonEmptyStringAttribute_WithEmptyAttributeAndRequired ()
    {
      StubExtendedProvider provider = new StubExtendedProvider ("Provider", new NameValueCollection ());
      NameValueCollection config = new NameValueCollection ();
      config.Add ("Name", string.Empty);

      try
      {
        provider.GetAndRemoveNonEmptyStringAttribute (config, "Name", "Provider", true);
      }
      catch
      {
        Assert.That (config.AllKeys.Length, Is.EqualTo (1));
        throw;
      }
    }

    [Test]
    public void GetAndRemoveNonEmptyStringAttribute_WithMissingAttributeAndNotRequired ()
    {
      StubExtendedProvider provider = new StubExtendedProvider ("Provider", new NameValueCollection ());
      NameValueCollection config = new NameValueCollection ();

      Assert.That (provider.GetAndRemoveNonEmptyStringAttribute (config, "Name", "Provider", false), Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
         ExpectedMessage = "The attribute 'Name' is missing in the configuration of the 'Provider' provider.")]
    public void GetAndRemoveNonEmptyStringAttribute_WithEmptyAttributeAndNotRequired ()
    {
      StubExtendedProvider provider = new StubExtendedProvider ("Provider", new NameValueCollection ());
      NameValueCollection config = new NameValueCollection ();
      config.Add ("Name", string.Empty);

      provider.GetAndRemoveNonEmptyStringAttribute (config, "Name", "Provider", false);
    }
  }
}
