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
using System.Configuration.Provider;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class ProviderHelperBaseTest
  {
    private StubProviderHelper _providerHelper;
    private ConfigurationPropertyCollection _propertyCollection;
    private StubExtendedConfigurationSection _stubConfigurationSection;

    [SetUp]
    public void SetUp ()
    {
      _stubConfigurationSection = new StubExtendedConfigurationSection ("WellKnown", "defaultProvider", "Default Value", "providers");
      _providerHelper = _stubConfigurationSection.GetStubProviderHelper();
      _propertyCollection = _stubConfigurationSection.GetProperties();
      _providerHelper.InitializeProperties (_propertyCollection);
    }

    [Test]
    public void Initialize ()
    {
      Assert.That (_propertyCollection.Count, Is.EqualTo (2));

      ConfigurationProperty defaultProviderProperty = _propertyCollection["defaultProvider"];
      Assert.That (defaultProviderProperty, Is.Not.Null);
      Assert.That (defaultProviderProperty.Type, Is.EqualTo (typeof (string)));
      Assert.That (defaultProviderProperty.DefaultValue, Is.EqualTo ("Default Value"));
      Assert.That (defaultProviderProperty.IsRequired, Is.False);
      Assert.IsInstanceOf (typeof (StringValidator), defaultProviderProperty.Validator);

      ConfigurationProperty providersProperty = _propertyCollection["providers"];
      Assert.That (providersProperty, Is.Not.Null);
      Assert.That (providersProperty.Type, Is.EqualTo (typeof (ProviderSettingsCollection)));
      Assert.That (providersProperty.DefaultValue, Is.Null);
      Assert.That (providersProperty.IsRequired, Is.False);
      Assert.IsInstanceOf (typeof (DefaultValidator), providersProperty.Validator);
    }

    [Test]
    public void GetProviders ()
    {
      string xmlFragment =
          @"
          <stubConfigSection>
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.That (_providerHelper.Providers.Count, Is.EqualTo (2));
      Assert.IsInstanceOf (typeof (FakeProvider), _providerHelper.Providers["Fake"]);
    }

    [Test]
    public void GetProvider ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""Fake"">
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Assert.IsInstanceOf (typeof (FakeProvider), _providerHelper.Provider);
      Assert.That (_providerHelper.Provider, Is.SameAs (_providerHelper.Providers["Fake"]));
    }

    [Test]
    public void GetProvider_WithWellKnownProvider ()
    {
      string xmlFragment = @"<stubConfigSection defaultProvider=""WellKnown"" />";
      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
      Assert.IsInstanceOf (typeof (FakeWellKnownProvider), _providerHelper.Provider);
    }

    [Test]
    public void GetProvider_WithoutDefaultProvider ()
    {
      StubExtendedConfigurationSection stubConfigurationSection =
          new StubExtendedConfigurationSection ("WellKnown", "defaultProvider", null, "providers");
      StubProviderHelper providerHelper = stubConfigurationSection.GetStubProviderHelper ();
      providerHelper.InitializeProperties (_stubConfigurationSection.GetProperties ());

      Assert.That (providerHelper.Provider, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultProvider does not exist in the providers collection.")]
    public void GetProvider_WithInvalidProviderName ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""Invalid"">
            <providers>
              <add name=""Fake"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);

      Dev.Null = _providerHelper.Provider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'WellKnown' identifies a well known provider and cannot be reused for custom providers.")]
    public void PostDeserialize_DuplicateWellKnownProvider ()
    {
      string xmlFragment =
          @"
          <stubConfigSection defaultProvider=""WellKnown"">
            <providers>
              <add name=""WellKnown"" type=""Remotion.UnitTests::Configuration.FakeProvider"" />
            </providers>
          </stubConfigSection>";

      ConfigurationHelper.DeserializeSection (_stubConfigurationSection, xmlFragment);
    }

    [Test]
    public void GetType_Test ()
    {
      Type type = _providerHelper.GetType (
          _propertyCollection["defaultProvider"],
          typeof (FakeProvider).Assembly.GetName(),
          "Remotion.UnitTests.Configuration.FakeProvider");

      Assert.That (type, Is.SameAs (typeof (FakeProvider)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage =
            "The current value of property 'defaultProvider' requires that the assembly 'Invalid' is placed within the CLR's probing path for this application."
        )]
    public void GetType_WithInvalidAssemblyName ()
    {
      _providerHelper.GetType (
          _propertyCollection["defaultProvider"],
          new AssemblyName ("Invalid"),
          "Remotion.UnitTests.Configuration.FakeProvider");
    }

    [Test]
    public void GetTypeWithMatchingVersionNumber ()
    {
      Type type = _providerHelper.GetTypeWithMatchingVersionNumber (
          _propertyCollection["defaultProvider"],
          "Remotion.UnitTests",
          "Remotion.UnitTests.Configuration.FakeProvider");

      Assert.That (type, Is.SameAs (typeof (FakeProvider)));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void GetTypeWithMatchingVersionNumber_WithInvalidAssemblyName ()
    {
      _providerHelper.GetTypeWithMatchingVersionNumber (
          _propertyCollection["defaultProvider"],
          "Invalid",
          "Remotion.UnitTests.Configuration.FakeProvider");
    }

    [Test]
    public void InstantiateProvider ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProvider");
      providerSettings.Parameters.Add ("description", "The Description");

      ProviderBase providerBase = _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.That (providerBase, Is.Not.Null);
      Assert.IsInstanceOf (typeof (FakeProvider), providerBase);
      Assert.That (providerBase.Name, Is.EqualTo ("Custom"));
      Assert.That (providerBase.Description, Is.EqualTo ("The Description"));
    }


    [Test]
    public void InstantiateProvider_WithConstructorException ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.ThrowingFakeProvider");
      providerSettings.Parameters.Add ("description", "The Description");

      try
      {
        _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));
        Assert.Fail ("Expected ConfigurationErrorsException.");
      }
      catch (ConfigurationErrorsException ex)
      {
        Assert.IsInstanceOf (typeof (TargetInvocationException), ex.InnerException);
        Assert.IsInstanceOf (typeof (ConstructorException), ex.InnerException.InnerException);
        Assert.That (ex.Message, Is.EqualTo ("A message from the constructor."));
      }
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException), ExpectedMessage = "Type name must be specified for this provider.")]
    public void InstantiateProvider_WithMissingTypeName ()
    {
      _providerHelper.InstantiateProvider (new ProviderSettings(), typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the class 'Remotion.UnitTests.Configuration.FakeProviderBase'.")]
    public void InstantiateProvider_WithTypeNotDerivedFromRequiredBaseType ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeOtherProvider");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the interface 'Remotion.UnitTests.Configuration.IFakeProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface ()
    {
      ProviderSettings providerSettings = new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProviderBase");
      _providerHelper.InstantiateProvider (providerSettings, typeof (FakeProviderBase), typeof (IFakeProvider));
    }

    [Test]
    public void InstantiateProviders ()
    {
      ProviderSettingsCollection providerSettingsCollection = new ProviderSettingsCollection();
      providerSettingsCollection.Add (new ProviderSettings ("Custom", "Remotion.UnitTests::Configuration.FakeProvider"));
      ProviderCollection providerCollection = new ProviderCollection();

      _providerHelper.InstantiateProviders (providerSettingsCollection, providerCollection, typeof (FakeProviderBase), typeof (IFakeProvider));

      Assert.That (providerCollection.Count, Is.EqualTo (1));
      ProviderBase providerBase = providerCollection["Custom"];
      Assert.IsInstanceOf (typeof (FakeProvider), providerBase);
      Assert.That (providerBase.Name, Is.EqualTo ("Custom"));
    }
  }
}
