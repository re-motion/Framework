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

namespace Remotion.Security.UnitTests
{
  public class AccessTypeExtensionsTest
  {
    [Test]
    public void Contains_MatchingAllowedAccessType_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.Contains (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Edit)
          },
          AccessType.Get (GeneralAccessTypes.Edit));

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void Contains_NotMatchingAllowedAccessType_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.Contains (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Edit)
          },
          AccessType.Get (GeneralAccessTypes.Create));

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void IsSubsetOf_WithSingleRequiredAccessType_MatchingSingleAllowedAccessType_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[] { AccessType.Get (GeneralAccessTypes.Edit) },
          new[] { AccessType.Get (GeneralAccessTypes.Edit) });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void IsSubsetOf_WithSingleRequiredAccessType_NotMatchingSingleAllowedAccessType_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[] { AccessType.Get (GeneralAccessTypes.Edit) },
          new[] { AccessType.Get (GeneralAccessTypes.Create) });

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void IsSubsetOf_WithSingleRequiredAccessType_NotPartOfAllowedAccessTypes_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[] { AccessType.Get (GeneralAccessTypes.Read) },
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void IsSubsetOf_WithAllRequiredAccessTypes_PartOfAllowedAccessTypes_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Create)
          },
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void IsSubsetOf_WithoutAllRequiredAccessTypes_PartOfAllowedAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Create),
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          },
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Read)
          });

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void IsSubsetOf_WithEmptySetOfAllowedAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Find)
          },
          new AccessType[0]);

      Assert.That (hasAccess, Is.EqualTo (false));
    }

    [Test]
    public void IsSubsetOf_WithEmptySetOfRequiredAccessTypes_ReturnsFalse ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new AccessType[0],
          new[]
          {
              AccessType.Get (GeneralAccessTypes.Delete),
              AccessType.Get (GeneralAccessTypes.Find)
          });

      Assert.That (hasAccess, Is.EqualTo (true));
    }

    [Test]
    public void IsSubsetOf_WithBothSetsEmpty_ReturnsTrue ()
    {
      bool hasAccess = AccessTypeExtensions.IsSubsetOf (
          new AccessType[0],
          new AccessType[0]);

      Assert.That (hasAccess, Is.EqualTo (true));
    }
  }
}