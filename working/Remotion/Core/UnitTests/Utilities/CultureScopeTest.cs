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
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class CultureScopeTest
  {
    [Test]
    public void CultureScopeByNameTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope ("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
        using (new CultureScope ("en-GB", "fr-MC"))
        {
          Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("en-GB"));
          Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("fr-MC"));
        }
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
      }
    }

    [Test]
    public void CultureScopeByCultureInfoTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (new CultureScope ("de-AT", "uz-Cyrl-UZ"))
      {
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
        using (new CultureScope (new CultureInfo ("en-GB", false), new CultureInfo ("fr-MC", false)))
        {
          Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("en-GB"));
          Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("fr-MC"));
        }
        Assert.That (currentThread.CurrentCulture.Name, Is.EqualTo ("de-AT"));
        Assert.That (currentThread.CurrentUICulture.Name, Is.EqualTo ("uz-Cyrl-UZ"));
      }
    }


    [Test]
    public void CreateInvariantCultureScopeTest ()
    {
      Thread currentThread = Thread.CurrentThread;
      using (CultureScope.CreateInvariantCultureScope ())
      {
        Assert.That (currentThread.CurrentCulture, Is.EqualTo (CultureInfo.InvariantCulture));
        Assert.That (currentThread.CurrentUICulture, Is.EqualTo (CultureInfo.InvariantCulture));
      }    
    
    }
  }
}