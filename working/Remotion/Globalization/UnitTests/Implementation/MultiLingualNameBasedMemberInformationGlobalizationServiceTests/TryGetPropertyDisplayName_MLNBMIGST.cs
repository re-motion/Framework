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
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedMemberInformationGlobalizationServiceTests
{
  [TestFixture]
  public class TryGetPropertyDisplayName_MultiLingualNameBasedMemberInformationGlobalizationServiceTest
  {
    [Test]
    public void Test_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithoutAssembly_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (null);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithoutType_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (null);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithoutNeutralResourcesLanguageAttribute_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.Without.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name invariant", ""),
                  new MultiLingualNameAttribute ("The Name en", "en")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheNeutralCultureAsFallbackForSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.En.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("en-US", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.EnUS.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheNeutralCultureAsFallbackForSpecficCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.EnUS.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.EnUS.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name en", "en"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB"),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();


      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-GB"));
      }
    }

    [Test]
    public void Test_UsesNeutralResourcesLanguageAttributeFromOriginalDeclaration ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.EnUS.Value);

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "en-US")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("en", "en"))
      {
        string multiLingualName;

        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name"));
      }
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesNotMatchingTheNeutralResourcesLanguageAttribute_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.FullName).Return ("The.Full.Type.Name");
      typeInformationStub.Stub (_ => _.Assembly).Return (TestAssemblies.EnUS.Value);

      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.DeclaringType).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.Name).Return ("TheProperty");

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.StringStarting (
                "The property 'TheProperty' declared on type 'The.Full.Type.Name' has no MultiLingualNameAttribute "
                + "for the assembly's neutral resource language ('en-US') applied."));
      }
    }

    [Test]
    public void Test_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.FullName).Return ("The.Full.Type.Name");

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute ("The Name en-GB", "en-GB")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.DeclaringType).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.Name).Return ("TheProperty");

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
                "The property 'TheProperty' declared on type 'The.Full.Type.Name' has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes."));
      }
    }

    [Test]
    public void Test_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (new MultiLingualNameAttribute[0]);
      Assert.That (typeof (object).BaseType, Is.Null, "Defined behavior for BaseType of Object is to return null");
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void Test_WithOverridenProperty_ReturnsTheLocalizedNameForTheOriginalDeclaration ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var basePropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      basePropertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (true))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (basePropertyInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void Test_WithOverridenPropertyHavingMultiLingualNameAttributes_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var basePropertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      basePropertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub.Stub (_ => _.FullName).Return ("The.Full.Type.Name");

      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (true))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Other Name", ""),
                  new MultiLingualNameAttribute ("The Name", "")
              });

      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (basePropertyInformationStub);
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaringType()).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.DeclaringType).Return (typeInformationStub);
      propertyInformationStub.Stub (_ => _.Name).Return ("TheProperty");

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      Assert.That (
          () => service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName),
          Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
              "The property 'TheProperty' overridden on type 'The.Full.Type.Name' has one or more MultiLingualNameAttributes applied via a property override. "
              + "The MultiLingualNameAttributes maybe only be applied to the original declaration of a property."));
    }

    [Test]
    public void Test_WithMultipleCalls_UsesCacheToRetrieveTheLocalizedName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      bool wasCalled = false;
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      propertyInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", ""),
                  new MultiLingualNameAttribute ("The Name en-US", "en-US")
              })
          .WhenCalled (
              mi =>
              {
                Assert.That (wasCalled, Is.False);
                wasCalled = true;
              });
      propertyInformationStub.Stub (_ => _.GetOriginalDeclaration()).Return (propertyInformationStub);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      using (new CultureScope ("", "en-US"))
      {
        string multiLingualName;
        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }

      using (new CultureScope ("", "fr-FR"))
      {
        string multiLingualName;
        var result = service.TryGetPropertyDisplayName (propertyInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name"));
      }
    }
  }
}