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
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.IntegrationTests
{
  [TestFixture]
  public class MultiLingualNameBasedMemberInformationGlobalizationServiceIntegrationTest
  {
    [Test]
    public void TryGetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de-AT Type Name"));
      }

      using (new CultureScope ("it", "de"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de Type Name"));
      }

      using (new CultureScope ("it", "de-DE"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de Type Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("en Type Name"));
      }

      using (new CultureScope ("it", "it"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Invariant Type Name"));
      }

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (DerivedClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de-AT Derived Type Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (DerivedClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Invariant Derived Type Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.False);
        Assert.That (resourceValue, Is.Null);
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (EnumWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (EnumWithMultiLingualNameAttribute)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Enum Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (StructWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (StructWithMultiLingualNameAttribute)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Struct Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (InterfaceWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (InterfaceWithMultiLingualNameAttribute)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Interface Name"));
      }

      // no localization for derived interface without own localization
      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (DerivedInterfaceWithoutOwnMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (DerivedInterfaceWithoutOwnMultiLingualNameAttribute)),
                out resourceValue),
            Is.False);
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (DerivedInterfaceWithOwnMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (DerivedInterfaceWithOwnMultiLingualNameAttribute)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("The Invariant Derived Interface Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithInterfaceIntroducedMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithInterfaceIntroducedMultiLingualNameAttribute)),
                out resourceValue),
            Is.False);
      }
    }

    [Test]
    public void GetTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.GetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.EqualTo ("de-AT Type Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        Assert.That (
            service.GetTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.EqualTo ("ClassWithoutMultiLingualNameAttribute"));
      }
    }

    [Test]
    public void ContainsTypeDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.ContainsTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.True);
      }
      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.ContainsTypeDisplayName (
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute)),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.False);
      }
    }

    [Test]
    public void TryGetPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de-AT Property Name"));
      }

      using (new CultureScope ("it", "de"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de Property Name"));
      }

      using (new CultureScope ("it", "it"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Invariant Property Name"));
      }

      using (new CultureScope ("it", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (DerivedClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de-AT Property Name"));
      }

      using (new CultureScope ("it", "it"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute).GetProperty ("PropertyWithoutMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                out resourceValue),
            Is.False);
        Assert.That (resourceValue, Is.Null);
      }
    }


    [Test]
    public void TryGetPropertyDisplayName_WithPropertiesDeclaredInAssemblyWithDifferentNeutralLanguage ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();
      var enUSassembly = TestAssemblies.EnUS;
      var enUSType = enUSassembly.Value.GetType ("DerivedClassWithMultiLingualNameAttributeAndDifferentNeutralLanguage");
      var declaredProperty = enUSType.GetProperty ("PropertyWithMultiLingualNameAttributeOnDerivedClass");
      var overriddenProperty = enUSType.GetProperty ("OverriddenPropertyWithMultiLingualNameAttribute");

      using (new CultureScope ("it", "it"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (declaredProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("en-US Property Name"));
      }

      using (new CultureScope ("en", "en"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (declaredProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("en Property Name"));
      }

      using (new CultureScope ("en-US", "en-US"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (declaredProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("en-US Property Name"));
      }

      using (new CultureScope ("it", "it"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (overriddenProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("Invariant Property Name"));
      }

      using (new CultureScope ("de", "de"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (overriddenProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de Property Name"));
      }

      using (new CultureScope ("de-AT", "de-AT"))
      {
        string resourceValue;
        Assert.That (
            service.TryGetPropertyDisplayName (PropertyInfoAdapter.Create (overriddenProperty), TypeAdapter.Create (enUSType), out resourceValue),
            Is.True);
        Assert.That (resourceValue, Is.EqualTo ("de-AT Property Name"));
      }
    }

    [Test]
    public void TryGetPropertyDisplayName_WithMultiLingualNameAttributeAppliedToOverride_ThrowsInvalidOperationException ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      string resourceValue;
      Assert.That (
          () =>
              service.TryGetPropertyDisplayName (
                  PropertyInfoAdapter.Create (
                      typeof (DerivedClassWithMultiLingualNameAttribute)
                          .GetProperty ("PropertyWithMultiLingualNameAttributeOnOverride")),
                  TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes)),
                  out resourceValue),
          Throws.InvalidOperationException);
    }

    [Test]
    public void GetPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.GetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.EqualTo ("de-AT Property Name"));
      }

      using (new CultureScope ("it", "en"))
      {
        Assert.That (
            service.GetPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute).GetProperty ("PropertyWithoutMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.EqualTo ("PropertyWithoutMultiLingualNameAttribute"));
      }
    }

    [Test]
    public void ContainsPropertyDisplayName ()
    {
      var service = SafeServiceLocator.Current.GetInstance<IMemberInformationGlobalizationService>();

      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.ContainsPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithMultiLingualNameAttribute).GetProperty ("PropertyWithMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.True);
      }

      using (new CultureScope ("it", "de-AT"))
      {
        Assert.That (
            service.ContainsPropertyDisplayName (
                PropertyInfoAdapter.Create (typeof (ClassWithoutMultiLingualNameAttribute).GetProperty ("PropertyWithoutMultiLingualNameAttribute")),
                TypeAdapter.Create (typeof (ClassWithoutMultiLingualResourcesAttributes))),
            Is.False);
      }
    }
  }
}