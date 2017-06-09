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
    private IResourceManager _resourceManager;

    [SetUp]
    public void SetUp ()
    {
      var factory = new ResourceAttributeBasedResourceManagerFactory ();
      var type = typeof (ClassWithResources);
      _resourceManager = factory.CreateResourceManager (type);
    }

    [Test]
    public void GetAvailableStrings_WithID1_DoesNotReturnFallbackValues_DoesNotReturnLocalizationsForCulturesNotDefinedViaAvailableResourcesLanguagesAttribute ()
    {
      var id ="property:Value1";

      // load de-at localization so the resourcemanager has to load them
      var expected = "Value 1";
      ValidateDeAtLocalization (_resourceManager, id, expected);

      // now load all available localizations for the property and there should be only the neutral one
      // GetAvailableStrings should not return fallback values for localizations
      var result = _resourceManager.GetAvailableStrings (id);
      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[CultureInfo.InvariantCulture], Is.EqualTo (expected));

      // validate de-at loclization again to check if fallback to neutral still works
      ValidateDeAtLocalization (_resourceManager, id, expected);
    }

    [Test]
    public void GetAvailableStrings_WithID2_DoesNotReturnFallbackValues_DoesNotReturnLocalizationsForCulturesNotDefinedViaAvailableResourcesLanguagesAttribute ()
    {
      var id = "type:ClassWithShortResourceIdentifier";

      // load de-at localization so the resourcemanager has to load them
      var expected = "Kurze Type ID";
      ValidateDeAtLocalization (_resourceManager, id, expected);

      // now load all available localizations for the property and there should be only the neutral one
      // GetAvailableStrings should not return fallback values for localizations
      var result = _resourceManager.GetAvailableStrings (id);
      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[CultureInfo.InvariantCulture], Is.EqualTo ("Short Type ID"));
      Assert.That (result[new CultureInfo ("de-AT")], Is.EqualTo (expected));

      // validate de-at loclization again to check if fallback to neutral still works
      ValidateDeAtLocalization (_resourceManager, id, expected);
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