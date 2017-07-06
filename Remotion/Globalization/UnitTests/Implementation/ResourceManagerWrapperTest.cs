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
using System.Globalization;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.Implementation
{
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