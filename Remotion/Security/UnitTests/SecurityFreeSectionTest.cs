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
using Remotion.Development.UnitTesting;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class SecurityFreeSectionTest
  {
    [Test]
    public void Activate_IsActive_Dispose_WithSingleSecurityFreeSection ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      var scope = SecurityFreeSection.Activate();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      scope.Dispose ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Activate_IsActive_Dispose_WithNestedSecurityFreeSections ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      var scope = SecurityFreeSection.Activate();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      using (SecurityFreeSection.Activate())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      scope.Dispose ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Activate_IsActive_Dispose_WithNestedSecurityFreeSectionsUnorderd_ThrowsInvalidOperationException ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      IDisposable scope1 = SecurityFreeSection.Activate(); // scope1 is boxed to make sure no value-copy errors are introduced.

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      var scope2 = SecurityFreeSection.Activate();
      Assert.That (SecurityFreeSection.IsActive, Is.True);

      Assert.That (
          () => scope1.Dispose(),
          Throws.InvalidOperationException.With.Message.StringStarting ("Nested SecurityFreeSection scopes have been exited out-of-sequence."));

      scope2.Dispose();
      scope1.Dispose();
    }

    [Test]
    public void CreatedViaDefaultConstructor_ThrowsInvalidOperationException ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      IDisposable scope1 = new SecurityFreeSection.Scope();

      Assert.That (SecurityFreeSection.IsActive, Is.False);

      Assert.That (
          () => scope1.Dispose(),
          Throws.InvalidOperationException.With.Message.StringStarting (
              "The SecurityFreeSection scope has not been entered by invoking SecurityFreeSection.Create()."));
    }

    [Test]
    [Obsolete]
    public void Create ()
    {
      using (SecurityFreeSection.Create())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }
    }

    [Test]
    [Obsolete]
    public void Leave ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      var scope = SecurityFreeSection.Activate();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      scope.Leave ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Activate_IsActive_Dispose_NotIsActive_Activate_IsActive ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      var scope = SecurityFreeSection.Activate();

      Assert.That (SecurityFreeSection.IsActive, Is.True);

      scope.Dispose ();

      Assert.That (SecurityFreeSection.IsActive, Is.False);

      using (SecurityFreeSection.Activate())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }
    }

    [Test]
    public void Activate_IsActive_Deactivate_NotIsActive_Activate_IsActive_Dispose_NotIsActive_Dispose_IsActive_Dispose_NotIsActive ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);

      using (SecurityFreeSection.Activate())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.True);

        using (SecurityFreeSection.Deactivate())
        {
          Assert.That (SecurityFreeSection.IsActive, Is.False);

          using (SecurityFreeSection.Activate())
          {
            Assert.That (SecurityFreeSection.IsActive, Is.True);
          }

          Assert.That (SecurityFreeSection.IsActive, Is.False);
        }

        Assert.That (SecurityFreeSection.IsActive, Is.True);
      }

      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Deactivate_WithoutActiveSection_StaysDeactivated()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);

      using (SecurityFreeSection.Deactivate())
      {
        Assert.That (SecurityFreeSection.IsActive, Is.False);
      }

      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }

    [Test]
    public void Threading ()
    {
      Assert.That (SecurityFreeSection.IsActive, Is.False);
      var scope = SecurityFreeSection.Activate();
      Assert.That (SecurityFreeSection.IsActive, Is.True);

      ThreadRunner.Run (
          delegate
          {
            Assert.That (SecurityFreeSection.IsActive, Is.False);
            using (SecurityFreeSection.Activate())
            {
              Assert.That (SecurityFreeSection.IsActive, Is.True);
            }
            Assert.That (SecurityFreeSection.IsActive, Is.False);
          });

      scope.Dispose ();
      Assert.That (SecurityFreeSection.IsActive, Is.False);
    }
  }
}
