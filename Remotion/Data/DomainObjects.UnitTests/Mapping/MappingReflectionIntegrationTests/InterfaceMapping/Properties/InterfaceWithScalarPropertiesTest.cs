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
  public class InterfaceWithScalarPropertiesTest : ReferenceMappingTestBase
  {
    private interface INotInMapping
    {
      int Value1 { get; set; }

      int Value2 { get; set; }
    }

    private interface IAgeable : IDomainObject
    {
      DateTime Birthday { get; set; }

      [StorageClassNone]
      int Age { get; set; }
    }

    private interface IPerson : IAgeable
    {
      string Name { get; set; }
    }

    private interface INumbered : IDomainObject
    {
      int Number { get; set; }

      int Secret { get; set; }
    }

    [DBTable]
    public class Person : DomainObject, IPerson, INumbered, INotInMapping
    {
      /// <summary>
      /// No property in <see cref="Person"/> as it is implicitly implemented from <see cref="IPerson"/> and it declares the property.
      /// </summary>
      public virtual string Name { get; set; }

      /// <summary>
      /// No property in <see cref="Person"/>  as it is implicitly implemented from <see cref="IAgeable"/> and it declares the property.
      /// </summary>
      public virtual DateTime Birthday { get; set; }

      /// <summary>
      /// No property in <see cref="Person"/> as it is excluded in the interface using the <see cref="StorageClassNoneAttribute"/> on <see cref="IAgeable"/>.
      /// </summary>
      public int Age { get; set; }

      /// <summary>
      /// No property in <see cref="Person"/> as it is implicitly implemented from <see cref="IPerson"/> and it declares the property.
      /// </summary>
      public virtual int Number { get; set; }

      /// <summary>
      /// No property in <see cref="Person"/> as it is explicitly implemented from <see cref="INumbered"/> and it declares the property.
      /// </summary>
      int INumbered.Secret { get; set; }

      /// <summary>
      /// Normal property in <see cref="Person"/>.
      /// </summary>
      public virtual int MyOwnProperty { get; set; }

      /// <summary>
      /// Normal property <see cref="Person"/> as it is implicitly implemented from an interface that is not mapped.
      /// </summary>
      public int Value1 { get; set; }

      /// <summary>
      /// No property in <see cref="Person"/> as it is explicitly implemented.
      /// </summary>
      int INotInMapping.Value2 { get; set; }
    }

    [Test]
    public void Verify ()
    {
      RunVerificationAgainstReferenceTypeDefinitions();
    }

    /// <inheritdoc />
    protected override void CreateReferenceTypeDefinitions (ReferenceTypeDefinitionCollectionBuilder builder)
    {
      builder.InterfaceDefinitionFor<IAgeable>()
          .PersistentProperty(e => e.Birthday);

      builder.InterfaceDefinitionFor<IPerson>()
          .Extends<IAgeable>()
          .PersistentProperty(e => e.Name, c => c.SetIsNullable());

      builder.InterfaceDefinitionFor<INumbered>()
          .PersistentProperty(e => e.Number)
          .PersistentProperty(e => e.Secret);

      builder.ClassDefinitionFor<Person>()
          .Implements<IPerson>()
          .Implements<IAgeable>()
          .Implements<INumbered>()
          .PersistentProperty(e => e.MyOwnProperty)
          .PersistentProperty(e => e.Value1);
    }
  }
}
