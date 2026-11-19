// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SortingOptimization;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SortingOptimization;

public abstract class SortingOptimizationTestBase
{
  protected class ExpectedPropertySpecification
  {
    public ExpectedPropertySpecification (ClassDefinition classDefinition, PropertyInfo propertyInfo, bool hasForeignKeyConstraint, ForeignKeyCycleBreakHint cycleBreakHint)
    {
      ClassDefinition = classDefinition;
      PropertyInfo = propertyInfo;
      HasForeignKeyConstraint = hasForeignKeyConstraint;
      CycleBreakHint = cycleBreakHint;
    }

    public ClassDefinition ClassDefinition { get; }
    public PropertyInfo PropertyInfo { get; }
    public bool HasForeignKeyConstraint { get; }
    public ForeignKeyCycleBreakHint CycleBreakHint { get; }
  }

  public MappingConfiguration GetMappingConfiguration (params Type[] typesToUse)
  {
    var fixedTypeDiscoveryService = new FixedTypeDiscoveryService(typesToUse);
    var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

    var mappingConfiguration = MappingConfiguration.Create(
        MappingReflectorObjectMother.CreateMappingReflector(fixedTypeDiscoveryService),
        new PersistenceModelLoader(storageSettings),
        new SortingOptimizationNodeFactory(new DomainModelConstraintProvider()),
        SafeServiceLocator.Current.GetInstance<ILoggerFactory>());
    return mappingConfiguration;
  }

  protected TableDefinition GetTableDefinition (ClassDefinition classDefinition)
  {
    return InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition>(
        (IRdbmsStorageEntityDefinition)classDefinition.StorageEntityDefinition,
        (table, _) => table,
        (filterView, continuation) => continuation(filterView.BaseEntity),
        (unionView, _) => { throw new AssertionException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); },
        (emptyView, _) => { throw new AssertionException($"Could not determine {nameof(TableDefinition)} for {classDefinition.ID}"); });
  }

  protected void AssertForeignKeyPropertySpecifications (
      ClassDefinition classDefinition,
      IEnumerable<ExpectedPropertySpecification> expectedSpecifications,
      IEnumerable<SortingOptimizationObjectIDPropertySpecification> actualProperties)
  {
    Assert.That(actualProperties, Has.Count.EqualTo(expectedSpecifications.Count()));
    foreach (var expectedSpecification in expectedSpecifications)
    {
      var expectedPropertyDefinition = classDefinition.GetMandatoryPropertyDefinition(expectedSpecification.PropertyInfo.DeclaringType!.FullName + "." + expectedSpecification.PropertyInfo.Name);
      var actualSpec = actualProperties.FirstOrDefault(p => p.PropertyDefinition == expectedPropertyDefinition);
      Assert.That(actualSpec, Is.Not.Null);
      Assert.That(actualSpec.HasForeignKeyConstraint, Is.EqualTo(expectedSpecification.HasForeignKeyConstraint));
      Assert.That(actualSpec.CycleBreakHint, Is.EqualTo(expectedSpecification.CycleBreakHint));
    }
  }

  protected static PropertyInfo GetPropertyInfo<TSourceObject> (Expression<Func<TSourceObject, object>> propertyExpression)
  {
    return MemberInfoFromExpressionUtility.GetProperty(propertyExpression);
  }
}
