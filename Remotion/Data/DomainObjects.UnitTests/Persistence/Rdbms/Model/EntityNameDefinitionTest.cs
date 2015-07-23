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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class EntityNameDefinitionTest
  {
    [Test]
    public void Initialization ()
    {
      var definition = new EntityNameDefinition ("schema", "entity");
      Assert.That (definition.SchemaName, Is.EqualTo ("schema"));
      Assert.That (definition.EntityName, Is.EqualTo ("entity"));
    }

    [Test]
    public void Initialization_NullSchemaName ()
    {
      var definition = new EntityNameDefinition (null, "entity");
      Assert.That (definition.SchemaName, Is.Null);
      Assert.That (definition.EntityName, Is.EqualTo ("entity"));
    }

    [Test]
    public void Initialization_NullEntityName ()
    {
      Assert.That (() => new EntityNameDefinition ("schema", null), Throws.TypeOf<ArgumentNullException> ());
    }

    [Test]
    public void Initialization_EmptyNames ()
    {
      Assert.That (
          () => new EntityNameDefinition ("", "entity"),
          Throws.ArgumentException.And.Message.EqualTo ("Parameter 'schemaName' cannot be empty.\r\nParameter name: schemaName"));
      Assert.That (
          () => new EntityNameDefinition ("schema", ""),
          Throws.ArgumentException.And.Message.EqualTo ("Parameter 'entityName' cannot be empty.\r\nParameter name: entityName"));
    }

    [Test]
    public void Equals_True ()
    {
      var one = new EntityNameDefinition ("schema", "entity");
      var two = new EntityNameDefinition ("schema", "entity");

      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_True_WithNullSchema ()
    {
      var one = new EntityNameDefinition (null, "entity");
      var two = new EntityNameDefinition (null, "entity");

      Assert.That (one, Is.EqualTo (two));
    }

    [Test]
    public void Equals_False_DifferentSchema ()
    {
      var one = new EntityNameDefinition ("schema1", "entity");
      var two = new EntityNameDefinition ("schema2", "entity");

      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void Equals_False_DifferentEntityName ()
    {
      var one = new EntityNameDefinition ("schema", "entity1");
      var two = new EntityNameDefinition ("schema", "entity2");

      Assert.That (one, Is.Not.EqualTo (two));
    }

    [Test]
    public void GetHashCode_EqualObjects ()
    {
      var one = new EntityNameDefinition ("schema", "entity");
      var two = new EntityNameDefinition ("schema", "entity");

      Assert.That (one.GetHashCode (), Is.EqualTo (two.GetHashCode ()));
    }

    [Test]
    public void GetHashCode_EqualObjects_WithNullSchema ()
    {
      var one = new EntityNameDefinition (null, "entity");
      var two = new EntityNameDefinition (null, "entity");

      Assert.That (one.GetHashCode (), Is.EqualTo (two.GetHashCode ()));
    }
  }
}