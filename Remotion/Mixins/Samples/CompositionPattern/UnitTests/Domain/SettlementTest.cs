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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain;
using Remotion.Mixins.Samples.CompositionPattern.Core.Framework;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.Domain
{
  [TestFixture]
  public class SettlementTest
  {
    [Test]
    public void NewObject ()
    {
      var instance = Settlement.NewObject ();
      Assert.That (instance, Is.InstanceOf (typeof (Settlement)));
    }

    [Test]
    public void SettlementProperties ()
    {
      var instance = Settlement.NewObject ();
      
      instance.SettlementKind = "Ordinary";
      Assert.That (instance.SettlementKind, Is.EqualTo ("Ordinary"));
    }

    [Test]
    public void DocumentProperties ()
    {
      var instance = Settlement.NewObject ();

      instance.Title = "MySettlement";
      Assert.That (instance.Title, Is.EqualTo ("MySettlement"));
    }

    [Test]
    public void TenantBoundProperties ()
    {
      var instance = Settlement.NewObject ();

      instance.Tenant = "SpecialTenant";
      Assert.That (instance.Tenant, Is.EqualTo ("SpecialTenant"));
    }

    [Test]
    public void Commit_WithoutSettlementKind ()
    {
      var instance = Settlement.NewObject();
      instance.Tenant = "SpecialTenant";
      Assert.That (instance.SettlementKind, Is.Null);

      Commit (instance);

      Assert.That (instance.SettlementKind, Is.EqualTo ("Ordinary"));
    }

    [Test]
    public void Commit_WithoutTitle ()
    {
      var instance = Settlement.NewObject ();
      instance.Tenant = "SpecialTenant";
      Assert.That (instance.Title, Is.Null);

      Commit (instance);

      Assert.That (instance.Title, Is.EqualTo ("(unnamed document of SpecialTenant)"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot commit tenant-bound object .* without a tenant.", 
        MatchType = MessageMatch.Regex)]
    public void Commit_WithoutTenant ()
    {
      var instance = Settlement.NewObject ();
      Assert.That (instance.Tenant, Is.Null);

      Commit (instance);
    }

    [Test]
    public void ToString_UsingThisProperty_AndSerialNumber ()
    {
      var instance = Settlement.NewObject ();
      instance.Tenant = "TheTenant";
      instance.SettlementKind = "Special";
      instance.Title = "The White Book";

      var result = instance.ToString ();

      Assert.That (result, Is.EqualTo ("0: Settlement: The White Book (Special); Tenant: TheTenant"));
    }

    private void Commit (IDomainObject instance)
    {
      instance.Events.OnCommitting ();
    }
  }
}