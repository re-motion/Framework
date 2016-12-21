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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.ConcreteInheritance;
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain.SingleInheritance;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class UpcastIntegrationTest : IntegrationTestBase
  {
    private TableInheritanceDomainObjectIDs _concreteObjectIDs;

    public override void SetUp ()
    {
      base.SetUp ();
      _concreteObjectIDs = new TableInheritanceDomainObjectIDs (Configuration);
    }

    [Test]
    public void AccessingProperties_OfDerivedClass ()
    {
      var queryWithSingleTableInheritance =
          from c in QueryFactory.CreateLinqQuery<Company>()
          where
              c is Customer
              && (((Customer) c).Type == Customer.CustomerType.Standard || ((Customer) c).Orders.Select (o => o.ID.Value).Contains (DomainObjectIDs.Order4.Value))
          select c;
      CheckQueryResult (queryWithSingleTableInheritance, DomainObjectIDs.Customer1, DomainObjectIDs.Customer4);

      var queryWithConcreteTableInheritance =
          from fsi in QueryFactory.CreateLinqQuery<TIFileSystemItem>()
          where
              fsi is TIFolder
              && (((TIFolder) fsi).CreatedAt == new DateTime (2006, 2, 1) && ((TIFolder) fsi).FileSystemItems.Any (i => i.Name == "Datei im Root"))
          select fsi;
      CheckQueryResult (queryWithConcreteTableInheritance, _concreteObjectIDs.FolderRoot);
    }

    [Test]
    public void AccessingMixinProperties_OfDerivedClass ()
    {
      SetDatabaseModifyable();

      var singleInheritanceFirstDerivedClass1 = SingleInheritanceFirstDerivedClass.NewObject ();
      ((ISingleInheritancePersistentMixin) singleInheritanceFirstDerivedClass1).PersistentProperty = "value 1";
      var singleInheritanceFirstDerivedClass2 = SingleInheritanceFirstDerivedClass.NewObject ();
      ((ISingleInheritancePersistentMixin) singleInheritanceFirstDerivedClass2).PersistentProperty = "value 2";

      var singleInheritanceSecondDerivedClass1 = SingleInheritanceSecondDerivedClass.NewObject ();
      ((ISingleInheritancePersistentMixin) singleInheritanceSecondDerivedClass1).PersistentProperty = "value 1";
      var singleInheritanceSecondDerivedClass2 = SingleInheritanceSecondDerivedClass.NewObject ();
      ((ISingleInheritancePersistentMixin) singleInheritanceSecondDerivedClass2).PersistentProperty = "value 2";

      ClientTransaction.Current.Commit();

      var queryWithSingleTableInheritance =
          from obj in QueryFactory.CreateLinqQuery<SingleInheritanceBaseClass> ()
          where
              (obj is SingleInheritanceFirstDerivedClass || obj is SingleInheritanceSecondDerivedClass) 
                  && (((ISingleInheritancePersistentMixin) obj).PersistentProperty == "value 1")
          select obj;
      CheckQueryResult (queryWithSingleTableInheritance, singleInheritanceFirstDerivedClass1.ID, singleInheritanceSecondDerivedClass1.ID);

      var concreteInheritanceFirstDerivedClass1 = ConcreteInheritanceFirstDerivedClass.NewObject ();
      ((IConcreteInheritancePersistentMixin) concreteInheritanceFirstDerivedClass1).PersistentProperty = "value 1";
      var concreteInheritanceFirstDerivedClass2 = ConcreteInheritanceFirstDerivedClass.NewObject ();
      ((IConcreteInheritancePersistentMixin) concreteInheritanceFirstDerivedClass2).PersistentProperty = "value 2";

      var concreteInheritanceSecondDerivedClass1 = ConcreteInheritanceSecondDerivedClass.NewObject ();
      ((IConcreteInheritancePersistentMixin) concreteInheritanceSecondDerivedClass1).PersistentProperty = "value 1";
      var concreteInheritanceSecondDerivedClass2 = ConcreteInheritanceSecondDerivedClass.NewObject ();
      ((IConcreteInheritancePersistentMixin) concreteInheritanceSecondDerivedClass2).PersistentProperty = "value 2";

      ClientTransaction.Current.Commit ();

      var queryWithConcreteTableInheritance =
          from obj in QueryFactory.CreateLinqQuery<ConcreteInheritanceBaseClass> ()
          where
              (obj is ConcreteInheritanceFirstDerivedClass || obj is ConcreteInheritanceSecondDerivedClass)
              && (((IConcreteInheritancePersistentMixin) obj).PersistentProperty == "value 1")
          select obj;
      CheckQueryResult (queryWithConcreteTableInheritance, concreteInheritanceFirstDerivedClass1.ID, concreteInheritanceSecondDerivedClass1.ID);
    }
 }
}