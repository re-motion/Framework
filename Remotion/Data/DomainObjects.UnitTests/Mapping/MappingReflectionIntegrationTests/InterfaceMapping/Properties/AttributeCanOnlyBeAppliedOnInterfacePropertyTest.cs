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

namespace Remotion.Data.DomainObjects.UnitTests.Mapping.MappingReflectionIntegrationTests.InterfaceMapping.Properties
{
  public class AttributeCanOnlyBeAppliedOnInterfacePropertyTest : ReferenceMappingTestBase
  {
    private interface IPerson : IDomainObject
    {
      [StringProperty(IsNullable = false)]
      string FirstName { get; set; }

      string LastName { get; set; }
    }

    [DBTable]
    public class Person : DomainObject, IPerson
    {
      /// <summary>
      /// Not-null because the interface property's attribute says so.
      /// </summary>
      public virtual string FirstName { get; set; }

      /// <summary>
      /// Nullable because the attribute on this property is not considered as it is an interface property.
      /// </summary>
      [StringProperty(IsNullable = false)]
      public virtual string LastName { get; set; }
    }

    [Test]
    public void Verify ()
    {
      RunVerificationAgainstReferenceTypeDefinitions();
    }

    /// <inheritdoc />
    protected override void CreateReferenceTypeDefinitions (ReferenceTypeDefinitionCollectionBuilder builder)
    {
      builder.InterfaceDefinitionFor<IPerson>()
          .PersistentProperty(e => e.FirstName)
          .PersistentProperty(e => e.LastName, c => c.SetIsNullable());

      builder.ClassDefinitionFor<Person>()
          .Implements<IPerson>();
    }
  }
}
