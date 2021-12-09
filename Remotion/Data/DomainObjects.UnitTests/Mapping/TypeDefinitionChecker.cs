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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  public class TypeDefinitionChecker
  {
    public void Check (TypeDefinition expectedDefinition, TypeDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull("actualDefinition", actualDefinition);

      if (expectedDefinition is ClassDefinition expectedClassDefinition)
      {
        if (actualDefinition is not ClassDefinition actualClassDefinition)
          throw new InvalidOperationException("The two type definitions must have a matching type.");

        CheckClassDefinition(expectedClassDefinition, actualClassDefinition);
      }
      else
      {
        Assert.Fail("Unsupported type definition cannot be checked.");
      }
    }

    public void Check (
        IEnumerable<TypeDefinition> expectedDefinitions,
        IDictionary<Type, TypeDefinition> actualDefinitions,
        bool checkRelations,
        bool ignoreUnknown)
    {
      ArgumentUtility.CheckNotNull("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull("actualDefinitions", actualDefinitions);

      if (!ignoreUnknown)
        Assert.AreEqual(expectedDefinitions.Count(), actualDefinitions.Count, "Number of class definitions does not match.");

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.Type];
        if (expectedDefinition is ClassDefinition expectedClassDefinition)
        {
          if (actualDefinition is not ClassDefinition actualClassDefinition)
          {
            Assert.Fail("The two type definitions must have a matching type.");
            return;
          }

          CheckClassDefinition(expectedClassDefinition, actualClassDefinition);
          CheckDerivedClasses(expectedClassDefinition, actualClassDefinition);
        }
        else
        {
          Assert.Fail("Unsupported type definition cannot be checked.");
        }
      }

      if (checkRelations)
        CheckRelationEndPoints(expectedDefinitions, actualDefinitions);
    }

    public void CheckRelationEndPoints (IEnumerable<TypeDefinition> expectedDefinitions, IDictionary<Type, TypeDefinition> actualDefinitions)
    {
      ArgumentUtility.CheckNotNull("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull("actualDefinitions", actualDefinitions);

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.Type];
        var endPointDefinitionChecker = new RelationEndPointDefinitionChecker();
        endPointDefinitionChecker.Check(expectedDefinition.MyRelationEndPointDefinitions, actualDefinition.MyRelationEndPointDefinitions, true);
      }
    }

    public void CheckPersistenceModel (IEnumerable<TypeDefinition> expectedDefinitions, IDictionary<Type, TypeDefinition> actualDefinitions)
    {
      ArgumentUtility.CheckNotNull("expectedDefinitions", expectedDefinitions);
      ArgumentUtility.CheckNotNull("actualDefinitions", actualDefinitions);

      foreach (var expectedDefinition in expectedDefinitions)
      {
        var actualDefinition = actualDefinitions[expectedDefinition.Type];
        CheckPersistenceModel(expectedDefinition, actualDefinition);
      }
    }

    public void CheckPersistenceModel (TypeDefinition expectedDefinition, TypeDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull("actualDefinition", actualDefinition);

      Assert.AreEqual(
          expectedDefinition.StorageEntityDefinition.StorageProviderDefinition,
          actualDefinition.StorageEntityDefinition.StorageProviderDefinition,
          "StorageProviderDefinition of type definition '{0}' does not match. ",
          expectedDefinition.Type.GetFullNameSafe());

      // We can't check much since FakeMappingConfiguration always creates fake entity definitions, only the entity name is defined.
      var expectedEntityName = GetEntityName(expectedDefinition.StorageEntityDefinition);
      var actualEntityName = GetEntityName(actualDefinition.StorageEntityDefinition);

      Assert.AreEqual(
          expectedEntityName,
          actualEntityName,
          "Entity name of type definition '{0}' does not match.",
          expectedDefinition.Type.GetFullNameSafe());

      foreach (PropertyDefinition expectedPropertyDefinition in expectedDefinition.MyPropertyDefinitions)
      {
        PropertyDefinition actualPropertyDefinition = actualDefinition.MyPropertyDefinitions[expectedPropertyDefinition.PropertyName];
        Assert.IsNotNull(
            actualPropertyDefinition,
            "Type '{0}' has no property '{1}'.",
            expectedDefinition.Type.GetFullNameSafe(),
            expectedPropertyDefinition.PropertyName);

        if (expectedPropertyDefinition.StorageClass == StorageClass.Persistent)
        {
          // We can't check much since FakeMappingConfiguration always creates fake storage property definitions, only the first column name is defined.
          var expectedColumnName = GetFirstColumnName(expectedPropertyDefinition.StoragePropertyDefinition);
          var actualColumnName = GetFirstColumnName(actualPropertyDefinition.StoragePropertyDefinition);

          Assert.AreEqual(
              expectedColumnName,
              actualColumnName,
              "Column name of property definition '{0}' (type definition: '{1}') does not match.",
              expectedPropertyDefinition.PropertyName,
              actualDefinition.Type.GetFullNameSafe());
        }
      }
    }

    private void CheckClassDefinition (ClassDefinition expectedDefinition, ClassDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull("actualDefinition", actualDefinition);

      Assert.AreEqual(expectedDefinition.ID, actualDefinition.ID, "IDs of class definitions do not match.");

      Assert.AreEqual(
          expectedDefinition.Type,
          actualDefinition.Type,
          "Type of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual(
          expectedDefinition.IsTypeResolved,
          actualDefinition.IsTypeResolved,
          "IsResolved of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreEqual(
          expectedDefinition.IsAbstract,
          actualDefinition.IsAbstract,
          "IsAbstract of class definition '{0}' does not match.",
          expectedDefinition.ID);

      Assert.AreSame(
          expectedDefinition.InstanceCreator,
          actualDefinition.InstanceCreator,
          "Instance creator of class definition '{0}' is not the expected object.",
          actualDefinition.ID);

      if (expectedDefinition.BaseClass == null)
      {
        Assert.IsNull(actualDefinition.BaseClass, "actualDefinition.BaseClass of class definition '{0}' is not null.", expectedDefinition.ID);
      }
      else
      {
        Assert.IsNotNull(actualDefinition.BaseClass, "actualDefinition.BaseClass of class definition '{0}' is null.", expectedDefinition.ID);

        Assert.AreEqual(
            expectedDefinition.BaseClass.ID,
            actualDefinition.BaseClass.ID,
            "BaseClass of class definition '{0}' does not match.",
            expectedDefinition.ID);
      }

      CheckPropertyDefinitions(expectedDefinition.MyPropertyDefinitions, actualDefinition.MyPropertyDefinitions, expectedDefinition);
    }

    public void CheckDerivedClasses (ClassDefinition expectedDefinition, ClassDefinition actualDefinition)
    {
      ArgumentUtility.CheckNotNull("expectedDefinition", expectedDefinition);
      ArgumentUtility.CheckNotNull("actualDefinition", actualDefinition);

      CheckDerivedClasses(expectedDefinition.DerivedClasses, actualDefinition.DerivedClasses, expectedDefinition);
    }

    private void CheckDerivedClasses (
        IEnumerable<ClassDefinition> expectedDerivedClasses,
        IEnumerable<ClassDefinition> actualDerivedClasses,
        ClassDefinition expectedClassDefinition)
    {
      Assert.AreEqual(
          expectedDerivedClasses.Count(),
          actualDerivedClasses.Count(),
          "Number of derived classes of class definition '{0}' does not match.",
          expectedClassDefinition.ID);

      var actualDerivedClassesDictionary = actualDerivedClasses.ToDictionary(cd => cd.ID);
      foreach (ClassDefinition expectedDerivedClass in expectedDerivedClasses)
      {
        Assert.IsNotNull(
            actualDerivedClassesDictionary[expectedDerivedClass.ID],
            "Actual class definition '{0}' does not contain expected derived class '{1}'.",
            expectedClassDefinition.ID,
            expectedDerivedClass.ID);
      }
    }

    private void CheckPropertyDefinitions (
        PropertyDefinitionCollection expectedDefinitions,
        PropertyDefinitionCollection actualDefinitions,
        TypeDefinition expectedTypeDefinition)
    {
      Assert.AreEqual(
          expectedDefinitions.Count,
          actualDefinitions.Count,
          "Number of property definitions in type definition '{0}' does not match. Expected: {1}",
          expectedTypeDefinition.Type.GetFullNameSafe(),
          string.Join(", ", actualDefinitions.Select(pd => pd.PropertyName)));

      foreach (PropertyDefinition expectedDefinition in expectedDefinitions)
      {
        PropertyDefinition actualDefinition = actualDefinitions[expectedDefinition.PropertyName];
        Assert.IsNotNull(actualDefinition, "Type '{0}' has no property '{1}'.", expectedTypeDefinition.Type.GetFullNameSafe(), expectedDefinition.PropertyName);
        CheckPropertyDefinition(expectedDefinition, actualDefinition, expectedTypeDefinition);
      }
    }

    private void CheckPropertyDefinition (
        PropertyDefinition expectedDefinition,
        PropertyDefinition actualDefinition,
        TypeDefinition typeDefinition)
    {
      Assert.AreEqual(
          expectedDefinition.PropertyName,
          actualDefinition.PropertyName,
          "PropertyNames of property definitions (type definition: '{0}') do not match.",
          typeDefinition.Type.GetFullNameSafe());

      Assert.AreEqual(
          expectedDefinition.TypeDefinition.Type,
          actualDefinition.TypeDefinition.Type,
          "Type of property definition '{0}' does not match.",
          expectedDefinition.PropertyName);

      Assert.AreEqual(
          expectedDefinition.StorageClass,
          actualDefinition.StorageClass,
          "StorageClass of property definition '{0}' (type definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          typeDefinition.Type.GetFullNameSafe());

      Assert.AreEqual(
          expectedDefinition.MaxLength,
          actualDefinition.MaxLength,
          "MaxLength of property definition '{0}' (type definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          typeDefinition.Type.GetFullNameSafe());

      Assert.AreEqual(
          expectedDefinition.PropertyType,
          actualDefinition.PropertyType,
          "PropertyType of property definition '{0}' (type definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          typeDefinition.Type.GetFullNameSafe());

      Assert.AreEqual(
          expectedDefinition.IsNullable,
          actualDefinition.IsNullable,
          "IsNullable of property definition '{0}' (type definition: '{1}') does not match",
          expectedDefinition.PropertyName,
          typeDefinition.Type.GetFullNameSafe());

      Assert.AreEqual(
          expectedDefinition.IsObjectID,
          actualDefinition.IsObjectID,
          "IsObjectID of property definition '{0}' (type definition: '{1}') does not match.",
          expectedDefinition.PropertyName,
          typeDefinition.Type.GetFullNameSafe());
    }

    private string GetEntityName (IStorageEntityDefinition storageEntityDefinition)
    {
      if (storageEntityDefinition is FakeStorageEntityDefinition)
        return ((FakeStorageEntityDefinition)storageEntityDefinition).Name;
      else if (storageEntityDefinition is IRdbmsStorageEntityDefinition)
        return InlineRdbmsStorageEntityDefinitionVisitor.Visit<string>(
            (IRdbmsStorageEntityDefinition)storageEntityDefinition,
            (table, continuation) => table.TableName.EntityName,
            (filterView, continuation) => continuation(filterView.BaseEntity),
            (unionView, continuation) => null,
            (emptyView, continuation) => null);
      else
        return null;
    }

    private string GetFirstColumnName (IStoragePropertyDefinition storagePropertyDefinition)
    {
      if (storagePropertyDefinition is FakeStoragePropertyDefinition)
        return ((FakeStoragePropertyDefinition)storagePropertyDefinition).Name;
      else if (storagePropertyDefinition is SimpleStoragePropertyDefinition)
        return ((SimpleStoragePropertyDefinition)storagePropertyDefinition).ColumnDefinition.Name;
      else if (storagePropertyDefinition is ObjectIDStoragePropertyDefinition)
        return GetFirstColumnName(((ObjectIDStoragePropertyDefinition)storagePropertyDefinition).ValueProperty);
      else if (storagePropertyDefinition is ObjectIDWithoutClassIDStoragePropertyDefinition)
        return GetFirstColumnName(((ObjectIDWithoutClassIDStoragePropertyDefinition)storagePropertyDefinition).ValueProperty);
      else if (storagePropertyDefinition is SerializedObjectIDStoragePropertyDefinition)
        return GetFirstColumnName(((SerializedObjectIDStoragePropertyDefinition)storagePropertyDefinition).SerializedIDProperty);
      else
        throw new NotSupportedException(storagePropertyDefinition.GetType().Name + " is not supported.");
    }
  }
}
