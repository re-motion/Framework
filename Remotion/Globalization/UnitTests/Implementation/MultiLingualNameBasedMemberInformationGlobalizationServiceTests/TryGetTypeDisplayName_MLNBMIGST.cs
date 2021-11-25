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
using System.Reflection;
using Moq;
using NUnit.Framework;
using Remotion.Globalization.Implementation;
using Remotion.Globalization.UnitTests.TestDomain;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Globalization.UnitTests.Implementation.MultiLingualNameBasedMemberInformationGlobalizationServiceTests
{
  [TestFixture]
  public class TryGetTypeDisplayName_MultiLingualNameBasedMemberInformationGlobalizationServiceTest
  {
    [Test]
    public void Test_WithMultiLingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name", "")
              });

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

      Assert.That(result, Is.True);
      Assert.That(multiLingualName, Is.EqualTo("The Name"));
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureMatchesSpecificCulture_ReturnsForTheSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en-US"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesNeutralCulture_ReturnsForTheNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-GB", "en-GB")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en"));
      }
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesForDifferentCulturesAndCurrentUICultureOnlyMatchesInvariantCulture_ReturnsForTheInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name invariant", ""),
                  new MultiLingualNameAttribute("The Name en-GB", "en-GB")
              });

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      using (new CultureScope("it-IT", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name invariant"));
      }
    }

    [Test]
    public void Test_WithoutAssembly_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name invariant", ""),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns((Assembly)null);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("", ""))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name invariant"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithoutNeutralResourcesLanguageAttribute_UsesInvariantCultureForNeutralCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name invariant", ""),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.Without.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("", ""))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name invariant"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("", ""))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name invariant", ""),
                  new MultiLingualNameAttribute("The Name en", "en")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("", ""))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name invariant"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_ReturnsTheNeutralCultureAsFallbackForSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesNeutralCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("en-US", "en-US"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en-US"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheSpecifiedCultureAsFallbackForInvariantCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.EnUS.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("", ""))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en-US"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_ReturnsTheNeutralCultureAsFallbackForSpecficCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.EnUS.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithNeutralResourcesLanguageAttributeSpecifiesSpecificCulture_DoesNotOverrideExistingSpecificCulture ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name en", "en"),
                  new MultiLingualNameAttribute("The Name en-GB", "en-GB"),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              });
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.EnUS.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();


      using (new CultureScope("en-GB", "en-GB"))
      {
        string multiLingualName;

        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en-GB"));
      }
    }

    [Test]
#if !NETFRAMEWORK
    [Ignore("TODO RM-7808: Integrate the RoslynCodeDomProvider and renable the AssemblyCompiler tests")]
#endif
    public void Test_WithMultiLingualNameAttributesNotMatchingTheNeutralResourcesLanguageAttribute_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name en-GB", "en-GB")
              });
      typeInformationStub.Setup(_ =>_.FullName).Returns("The.Full.Type.Name");
      typeInformationStub.Setup(_ => _.Assembly).Returns(TestAssemblies.En.Value);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      using (new CultureScope("it-IT", "en-GB"))
      {
        string multiLingualName;

        Assert.That(
            () => service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.StartsWith(
                "The type 'The.Full.Type.Name' has no MultiLingualNameAttribute for the assembly's neutral resource language ('en') applied."));
      }
    }

    [Test]
    public void Test_WithMultipleMultiLingualNameAttributesForSameCulture_ThrowsInvalidOperationException ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name fr-FR", "fr-FR"),
                  new MultiLingualNameAttribute("The Name en-GB", "en-GB")
              });
      typeInformationStub.Setup(_ =>_.FullName).Returns("The.Full.Type.Name");

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      using (new CultureScope("it-IT", "en-US"))
      {
        string multiLingualName;

        Assert.That(
            () => service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName),
            Throws.TypeOf<InvalidOperationException>().With.Message.EqualTo(
                "The type 'The.Full.Type.Name' has more than one MultiLingualNameAttribute for the culture 'fr-FR' applied. "
                + "The used cultures must be unique within the set of MultiLingualNameAttributes."));
      }
    }

    [Test]
    public void Test_WithoutMultiLingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(new MultiLingualNameAttribute[0]);
      typeInformationStub.Setup(_ => _.BaseType).Returns((ITypeInformation)null);
      Assert.That(typeof(object).BaseType, Is.Null, "Defined behavior for BaseType of Object is to return null");

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

      Assert.That(result, Is.False);
      Assert.That(multiLingualName, Is.Null);
    }

    [Test]
    public void Test_WithMultiLingualNameAttributeOnlyOnBaseClass_DoesNotCheckBaseClass ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(new MultiLingualNameAttribute[0]);

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

      Assert.That(result, Is.False);
      Assert.That(multiLingualName, Is.Null);
      typeInformationStub.Verify(_ => _.BaseType, Times.Never());
    }

    [Test]
    public void Test_WithMultiLingualNameAttributesOnCurrentClass_DoesNotCheckBaseClass ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name", "")
              });

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

      Assert.That(result, Is.True);
      Assert.That(multiLingualName, Is.EqualTo("The Name"));
      typeInformationStub.Verify(_ => _.BaseType, Times.Never());
    }

    [Test]
    public void Test_WithMultipleCalls_UsesCacheToRetrieveTheLocalizedName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      bool wasCalled = false;
      var typeInformationStub = new Mock<ITypeInformation>();
      typeInformationStub
          .Setup(_ => _.GetCustomAttributes<MultiLingualNameAttribute>(false))
          .Returns(
              new[]
              {
                  new MultiLingualNameAttribute("The Name", ""),
                  new MultiLingualNameAttribute("The Name en-US", "en-US")
              })
          .Callback(
              (bool inherited) =>
              {
                Assert.That(wasCalled, Is.False);
                wasCalled = true;
              });

      var typeInformationForResourceResolutionStub = new Mock<ITypeInformation>();

      using (new CultureScope("", "en-US"))
      {
        string multiLingualName;
        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name en-US"));
      }

      using (new CultureScope("", "fr-FR"))
      {
        string multiLingualName;
        var result = service.TryGetTypeDisplayName(typeInformationStub.Object, typeInformationForResourceResolutionStub.Object, out multiLingualName);

        Assert.That(result, Is.True);
        Assert.That(multiLingualName, Is.EqualTo("The Name"));
      }
    }
  }
}
