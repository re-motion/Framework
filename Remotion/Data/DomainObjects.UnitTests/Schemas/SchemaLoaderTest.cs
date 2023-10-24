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
using System.Xml.Schema;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries.ConfigurationLoader;
using Remotion.Data.DomainObjects.Schemas;

namespace Remotion.Data.DomainObjects.UnitTests.Schemas
{
  [TestFixture]
  public class SchemaLoaderTest
  {
    [Test]
    public void InitializeWithQueries ()
    {
      Assert.That(SchemaLoader.Queries.SchemaUri, Is.EqualTo(PrefixNamespace.QueryConfigurationNamespace.Uri));
    }

    [Test]
    public void LoadSchemaSetWithQueries ()
    {
      XmlSchemaSet schemaSet = SchemaLoader.Queries.LoadSchemaSet();

      Assert.That(schemaSet, Is.Not.Null);
      Assert.That(schemaSet.Count, Is.EqualTo(2));
      Assert.That(schemaSet.Contains(PrefixNamespace.QueryConfigurationNamespace.Uri), Is.True);
      Assert.That(schemaSet.Contains("http://www.re-motion.org/Data/DomainObjects/Types"), Is.True);
    }
  }
}
