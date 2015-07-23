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
using System.Text;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  [TestFixture]
  public class AllSelectedColumnsSpecificationTest : StandardMappingTest
  {
    [Test]
    public void AppendProjection_StringBuilderEmpty ()
    {
      var sb = new StringBuilder();

      AllSelectedColumnsSpecification.Instance.AppendProjection (sb, MockRepository.GenerateStub<ISqlDialect>());

      Assert.That (sb.ToString(), Is.EqualTo ("*"));
    }

    [Test]
    public void Union ()
    {
      var instance = AllSelectedColumnsSpecification.Instance;

      Assert.That (instance.Union (new ColumnDefinition[0]), Is.SameAs (instance));
    }

    [Test]
    public void AdjustForTable ()
    {
      var instance = AllSelectedColumnsSpecification.Instance;
      Assert.That (
          () => instance.AdjustForTable (TableDefinitionObjectMother.Create (TestDomainStorageProviderDefinition)),
          Throws.TypeOf<NotSupportedException>());
    }
  }
}