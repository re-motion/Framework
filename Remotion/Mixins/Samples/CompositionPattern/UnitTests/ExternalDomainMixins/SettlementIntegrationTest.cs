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
using Remotion.Mixins.Samples.CompositionPattern.Core.Domain.Mixins;
using Remotion.Mixins.Samples.CompositionPattern.Core.ExternalDomainMixins;

namespace Remotion.Mixins.Samples.CompositionPattern.UnitTests.ExternalDomainMixins
{
  [TestFixture]
  public class SettlementIntegrationTest
  {
    private IDisposable _municipalMixinConfigurationScope;

    [SetUp]
    public void SetUp ()
    {
      _municipalMixinConfigurationScope = MixinConfiguration.BuildFromActive ()
          .ForClass<Settlement> ()
              .AddMixin<MunicipalSettlementMixin> ()
              .AddMixin<MunicipalDocumentMixin> ().ReplaceMixin<DocumentMixin>()
              .EnterScope ();
    }

    [TearDown]
    public void TearDown ()
    {
      _municipalMixinConfigurationScope.Dispose ();
    }

    [Test]
    public void NewObject ()
    {
      var instance = Settlement.NewObject ();
      Assert.That (instance, Is.InstanceOf (typeof (Settlement)));
    }

    [Test]
    public void MunicipalProperties ()
    {
      var instance = Settlement.NewObject ();

      instance.AsMunicipalSettlement().MunicipalityID = 7;
      Assert.That (instance.AsMunicipalSettlement ().MunicipalityID, Is.EqualTo (7));
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
      instance.AsMunicipalSettlement ().MunicipalityID = 7;

      instance.Title = "MySettlement";
      Assert.That (instance.Title, Is.EqualTo ("MySettlement (for municipality 7)"));
    }

    [Test]
    public void TenantBoundProperties ()
    {
      var instance = Settlement.NewObject ();

      instance.Tenant = "SpecialTenant";
      Assert.That (instance.Tenant, Is.EqualTo ("SpecialTenant"));
    }

    [Test]
    public void GetDescriptionForMayors ()
    {
      var instance = Settlement.NewObject ();
      instance.AsMunicipalSettlement ().MunicipalityID = 17;
      instance.Title = "Title";
      instance.SettlementKind = "Kind";

      var result = instance.AsMunicipalSettlement().GetDescriptionForMayors ();

      Assert.That (result, Is.EqualTo ("MunicipalSettlement: Title (for municipality 17) (Kind), 17"));
    }
  }
}