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
using Remotion.Data.DomainObjects.UnitTests.MixedDomains.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests
{
  [TestFixture]
  public class MixinAddedPropertyIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void PropertyDeclaredByMixin_AppliedToSameObject ()
    {
      var mixins = (from t in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) t).PersistentProperty == 99
                    select t);

      CheckQueryResult (mixins, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) m).PersistentProperty == 199
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.DerivedTargetClassForPersistentMixin1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseObject_WithInterfaceIntroducedByTwoMixins ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedTargetClassWithDerivedMixinWithInterface> ()
                    where ((IMixinAddingPersistentProperties) m).PersistentProperty == 199
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.DerivedTargetClassWithDerivedMixinWithInterface1);
    }

    [Test]
    public void PropertyDeclaredByMixin_AppliedToBaseBaseObject ()
    {
      var mixins = (from m in QueryFactory.CreateLinqQuery<DerivedDerivedTargetClassForPersistentMixin> ()
                    where ((IMixinAddingPersistentProperties) m).PersistentProperty == 299
                    select m);

      CheckQueryResult (mixins, DomainObjectIDs.DerivedDerivedTargetClassForPersistentMixin1);
    }

    [Test]
    public void RelationWithForeignKeyAddedByMixin ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
          where ((IMixinAddingPersistentProperties) o).RelationProperty != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    public void RelationWithoutForeignKeyAddedByMixin ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).VirtualRelationProperty != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    public void RelationWithForeignKeyAddedByMixin_PropertyOfRelatedObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).RelationProperty.ID != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    public void RelationWithoutForeignKeyAddedByMixin_PropertyOfRelatedObject ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          where ((IMixinAddingPersistentProperties) o).VirtualRelationProperty.ID != null
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins2);
    }

    [Test]
    public void CollectionValuedRelationAddedByMixin_UsedInFromExpression ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin>()
          from related in ((IMixinAddingPersistentProperties) o).CollectionProperty1Side
          select related;
      CheckQueryResult (query, DomainObjectIDs.RelationTargetForPersistentMixin3);
    }

    [Test]
    public void MixedProperties_AccessedViaCastProperty ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
          where o.MixedMembers.PersistentProperty == 99
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }

    [Test]
    public void MixedProperties_AccessedViaCastExtensionMethod ()
    {
      var query =
          from o in QueryFactory.CreateLinqQuery<TargetClassForPersistentMixin> ()
          where o.GetMixedMembers().PersistentProperty == 99
          select o;
      CheckQueryResult (query, DomainObjectIDs.TargetClassForPersistentMixins1);
    }
  }
}