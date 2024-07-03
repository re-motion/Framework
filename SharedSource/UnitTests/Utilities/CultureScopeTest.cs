// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

#nullable enable
namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CultureScopeTest
  {
    [Test]
    public void CultureScopeByNameTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope("en-GB", "fr-MC"))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("en-GB"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("fr-MC"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CultureScopeByCultureInfoTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope(new CultureInfo("en-GB", false), new CultureInfo("fr-MC", false)))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("en-GB"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("fr-MC"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateCultureScopeWithEmptyNames ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope("", ""))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo(""));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo(""));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateCultureScopeWithNullForCultureName_DoesNotChangeCulture ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope(null, "fr-MC"))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("fr-MC"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateCultureScopeWithNullForUICultureName_DoesNotChangeUICulture ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope("en-GB", null))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("en-GB"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateCultureScopeWithNullForCultureInfo_DoesNotChangeCulture ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope(null, new CultureInfo("fr-MC", false)))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("fr-MC"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateCultureScopeWithNullForUICultureInfo_DoesNotChangeUICulture ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        using (new CultureScope(new CultureInfo("en-GB", false), null))
        {
          Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("en-GB"));
          Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
        }
        Assert.That(currentThread.CurrentCulture.Name, Is.EqualTo("de-AT"));
        Assert.That(currentThread.CurrentUICulture.Name, Is.EqualTo("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CreateInvariantCultureScopeTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (CultureScope.CreateInvariantCultureScope())
      {
        Assert.That(currentThread.CurrentCulture, Is.EqualTo(CultureInfo.InvariantCulture));
        Assert.That(currentThread.CurrentUICulture, Is.EqualTo(CultureInfo.InvariantCulture));
      }

    }
  }
}
