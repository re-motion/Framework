using System.Globalization;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class ResourceManagerWrapperTest
  {
    [Test]
    public void GetAvailableStrings_WithID1_DoesNotReturnFallbackValues_DoesNotReturnLocalizationsForCulturesNotDefinedViaAvailableResourcesLanguagesAttribute ()
    {
      var factory = new ResourceAttributeBasedResourceManagerFactory();
      var type = typeof (ClassWithResources);
      var resourceManager = factory.CreateResourceManager (type);

      var id ="property:Value1";

      // load de-at localization so the resourcemanager has to load them
      var expected = "Value 1";
      ValidateDeAtLocalization (resourceManager, id, expected);

      // now load all available localizations for the property and there should be only the neutral one
      // GetAvailableStrings should not return fallback values for localizations
      var result = resourceManager.GetAvailableStrings (id);
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[CultureInfo.InvariantCulture], Is.EqualTo (expected));

      // validate de-at loclization again to check if fallback to neutral still works
      ValidateDeAtLocalization (resourceManager, id, expected);
    }

    [Test]
    public void GetAvailableStrings_WithID2_DoesNotReturnFallbackValues_DoesNotReturnLocalizationsForCulturesNotDefinedViaAvailableResourcesLanguagesAttribute ()
    {
      var factory = new ResourceAttributeBasedResourceManagerFactory();
      var type = typeof (ClassWithResources);
      var resourceManager = factory.CreateResourceManager (type);

      var id = "type:ClassWithShortResourceIdentifier";

      // load de-at localization so the resourcemanager has to load them
      var expected = "Kurze Type ID";
      ValidateDeAtLocalization (resourceManager, id, expected);

      // now load all available localizations for the property and there should be only the neutral one
      // GetAvailableStrings should not return fallback values for localizations
      var result = resourceManager.GetAvailableStrings (id);
      Assert.That (result.Count, Is.EqualTo (5));
      Assert.That (result[CultureInfo.InvariantCulture], Is.EqualTo ("Short Type ID"));
      Assert.That (result[new CultureInfo ("de-AT")], Is.EqualTo (expected));
      Assert.That (result[new CultureInfo ("fr")], Is.EqualTo ("fr: Kurze Type ID"));
      Assert.That (result[new CultureInfo ("fr-CA")], Is.EqualTo ("fr-CA: Kurze Type ID"));
      Assert.That (result[new CultureInfo ("fr-CH")], Is.EqualTo ("fr-CH: Kurze Type ID"));

      // validate de-at loclization again to check if fallback to neutral still works
      ValidateDeAtLocalization (resourceManager, id, expected);
    }

    [Test]
    public void GetAvailableStrings_NeutralResourcesLanguageAttributeIsNotInvariantCultureAndSpecifiedCultureIsNotListedInAvailableResourcesLanguagesList ()
    {
      var factory = new ResourceAttributeBasedResourceManagerFactory();
      var type = TestAssemblies.En.Value.GetType ("ClassWithResources", true, false);
      var resourceManager = factory.CreateResourceManager (type);

      var id = "type:ClassWithShortResourceIdentifier";

      var result = resourceManager.GetAvailableStrings (id);
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[CultureInfo.InvariantCulture], Is.EqualTo ("Short Type ID"));
      Assert.That (result[new CultureInfo ("en")], Is.EqualTo ("Short Type ID"));
    }

    private void ValidateDeAtLocalization (IResourceManager resourceManager, string shortName, string expected)
    {
      using (new CultureScope ("de-AT"))
      {
        string result;
        Assert.That (resourceManager.TryGetString (shortName, out result), Is.True);
        Assert.That (result, Is.EqualTo (expected));
      }
    }
  }
}