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
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class MultiLingualNameBasedEnumerationGlobalizationServiceTest
  {
    private enum TestEnum
    {
      ValueWithoutMultiLingualNameAttribute,

      [MultiLingualNameAttribute ("The Name", "")]
      ValueWithMultiLingualNameAttributeForInvariantCulture,

      [MultiLingualNameAttribute ("The Name invariant", "")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name en", "en")]
      [MultiLingualNameAttribute ("The Name en-US", "en-US")]
      ValueWithMultiLingualNameAttributes,

      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      ValueWithoutInvariantCulture,

      [MultiLingualNameAttribute ("The Name invariant", "")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name fr-FR", "fr-FR")]
      [MultiLingualNameAttribute ("The Name en-US", "en-US")]
      ValueWithDuplicateMultiLingualNameAttributes
    }

    [Flags]
    private enum FlagsEnum
    {
      [MultiLingualNameAttribute ("The Value 1", "")]
      Value1 = 1,

      [MultiLingualNameAttribute ("The Value 2", "")]
      Value2 = 2
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributeForInvariantCulture, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithoutNeutralResourcesLanguageAttribute_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.En.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithInvariantAndEn");

      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.En.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUS");

      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingInvariantCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.En.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithInvariantAndEn");

      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheNeutralCultureAsFallbackForSpecificCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.En.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUS");

      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.En.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUS");

      using (new CultureScope ("en-US", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.EnUS.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUS");

      using (new CultureScope ("", ""))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheNeutralCultureAsFallbackForSpecficCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.EnUS.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUS");

      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en"));
      }
    }

    [Test]
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      var testEnumType = TestAssemblies.EnUS.Value.GetType ("TestEnum", true, false);
      var enumValue = (Enum) Enum.Parse (testEnumType, "ValueWithEnAndEnUSAndEnGB");

      using (new CultureScope ("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (enumValue, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-GB"));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "de-AT"))
      {
        string multiLingualName;

        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesNotMatchingTheNeutralResourcesLanguageAttribute_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-GB"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithoutInvariantCulture, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.StartsWith (
                "The enum value 'ValueWithoutInvariantCulture' declared on type "
                + "'Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedEnumerationGlobalizationServiceTest+TestEnum' "
                + "has no MultiLingualNameAttribute for the assembly's neutral resource language ('') applied."));
      }
    }
    
    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That (
            () => service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithDuplicateMultiLingualNameAttributes, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo (
                "The enum value 'ValueWithDuplicateMultiLingualNameAttributes' declared on type "
                + "'Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedEnumerationGlobalizationServiceTest+TestEnum' "
                + "has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes."));
      }
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithoutMultiLingualNameAttribute, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithUnknownEnumValue_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName ((TestEnum) 100, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithCombinedFlagsEnumValue_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      string multiLingualName;

      var result = service.TryGetEnumerationValueDisplayName (FlagsEnum.Value1 | FlagsEnum.Value2, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }

    [Test]
    public void TryGetEnumerationValueDisplayName_WithMultipleCalls_UsesCacheToRetrieveTheLocalizedName ()
    {
      var service = new MultiLingualNameBasedEnumerationGlobalizationService();

      using (new CultureScope ("", "en-US"))
      {
        string multiLingualName;
        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name en-US"));
      }

      using (new CultureScope ("", "fr-FR"))
      {
        string multiLingualName;
        var result = service.TryGetEnumerationValueDisplayName (TestEnum.ValueWithMultiLingualNameAttributes, out multiLingualName);

        Assert.That (result, Is.True);
        Assert.That (multiLingualName, Is.EqualTo ("The Name fr-FR"));
      }
    }
  }
}