// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
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