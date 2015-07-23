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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.RdbmsPersistenceModelLoaderTestDomain;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model.Building
{
  // Test Domain:
  //
  //                 BaseBase
  //                     |
  //                   Base
  //                 /      \
  //            Table1       Table2
  //                         /    \
  //                   Derived1  Derived2
  //                                |
  //                          DerivedDerived
  //                                |
  //                       DerivedDerivedDerived
  //
  // All Base classes are persisted as UnionViewDefinitions, all Tables as TableDefinitions, all Derived as FilterViewDefinitions.
  
  public class RdbmsPersistenceModelLoaderTestHelper
  {
    private readonly ClassDefinition _baseBaseClassDefinition;
    private readonly ClassDefinition _baseClassDefinition;
    private readonly ClassDefinition _tableClassDefinition1;
    private readonly ClassDefinition _tableClassDefinition2;
    private readonly ClassDefinition _derivedClassDefinition1;
    private readonly ClassDefinition _derivedClassDefinition2;
    private readonly ClassDefinition _derivedDerivedClassDefinition;
    private readonly ClassDefinition _derivedDerivedDerivedClassDefinition;
    private readonly PropertyDefinition _baseBasePropertyDefinition;
    private readonly PropertyDefinition _basePropertyDefinition;
    private readonly PropertyDefinition _tablePropertyDefinition1;
    private readonly PropertyDefinition _tablePropertyDefinition2;
    private readonly PropertyDefinition _derivedPropertyDefinition1;
    private readonly PropertyDefinition _derivedPropertyDefinition2;
    private readonly PropertyDefinition _derivedDerivedPropertyDefinition;

    public RdbmsPersistenceModelLoaderTestHelper ()
    {
      _baseBaseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("BaseBaseClass", typeof (BaseBaseClass));
      _baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("BaseClass", typeof (BaseClass), baseClass: _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition ("Table1Class", typeof (Table1Class), baseClass: _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition ("Table2Class", typeof (Table2Class), baseClass: _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition ("Derived1Class", typeof (Derived1Class), baseClass: _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition ("Derived2Class", typeof (Derived2Class), baseClass: _tableClassDefinition2);
      _derivedDerivedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("DerivedDerivedClass", typeof (DerivedDerivedClass), baseClass: _derivedClassDefinition2);
      _derivedDerivedDerivedClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("DerivedDerivedDerivedClass", typeof (DerivedDerivedDerivedClass), baseClass: _derivedDerivedClassDefinition);

      _baseBaseClassDefinition.SetDerivedClasses (new[] { _baseClassDefinition });
      _baseClassDefinition.SetDerivedClasses (new[] { _tableClassDefinition1, _tableClassDefinition2 });
      _tableClassDefinition2.SetDerivedClasses (new[] { _derivedClassDefinition1, _derivedClassDefinition2 });
      _derivedClassDefinition2.SetDerivedClasses (new[] { _derivedDerivedClassDefinition });
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinition[0]);
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinition[0]);
      _derivedDerivedClassDefinition.SetDerivedClasses (new[] { _derivedDerivedDerivedClassDefinition });
      _derivedDerivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinition[0]);

      _baseBaseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _baseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      
      _baseBasePropertyDefinition = CreateAndAddPropertyDefinition (_baseBaseClassDefinition, "BaseBaseProperty");
      _basePropertyDefinition = CreateAndAddPropertyDefinition (_baseClassDefinition, "BaseProperty");
      _tablePropertyDefinition1 = CreateAndAddPropertyDefinition (_tableClassDefinition1, "TableProperty1");
      _tablePropertyDefinition2 = CreateAndAddPropertyDefinition (_tableClassDefinition2, "TableProperty2");
      _derivedPropertyDefinition1 = CreateAndAddPropertyDefinition (_derivedClassDefinition1, "DerivedProperty1");
      _derivedPropertyDefinition2 = CreateAndAddPropertyDefinition (_derivedClassDefinition2, "DerivedProperty2");
      _derivedDerivedPropertyDefinition = CreateAndAddPropertyDefinition (_derivedDerivedClassDefinition, "DerivedDerivedProperty");
      _derivedDerivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
    }

    public ClassDefinition BaseBaseClassDefinition
    {
      get { return _baseBaseClassDefinition; }
    }

    public ClassDefinition BaseClassDefinition
    {
      get { return _baseClassDefinition; }
    }

    public ClassDefinition TableClassDefinition1
    {
      get { return _tableClassDefinition1; }
    }

    public ClassDefinition TableClassDefinition2
    {
      get { return _tableClassDefinition2; }
    }

    public ClassDefinition DerivedClassDefinition1
    {
      get { return _derivedClassDefinition1; }
    }

    public ClassDefinition DerivedClassDefinition2
    {
      get { return _derivedClassDefinition2; }
    }

    public ClassDefinition DerivedDerivedClassDefinition
    {
      get { return _derivedDerivedClassDefinition; }
    }

    public ClassDefinition DerivedDerivedDerivedClassDefinition
    {
      get { return _derivedDerivedDerivedClassDefinition; }
    }

    public PropertyDefinition BaseBasePropertyDefinition
    {
      get { return _baseBasePropertyDefinition; }
    }

    public PropertyDefinition BasePropertyDefinition
    {
      get { return _basePropertyDefinition; }
    }

    public PropertyDefinition TablePropertyDefinition1
    {
      get { return _tablePropertyDefinition1; }
    }

    public PropertyDefinition TablePropertyDefinition2
    {
      get { return _tablePropertyDefinition2; }
    }

    public PropertyDefinition DerivedPropertyDefinition1
    {
      get { return _derivedPropertyDefinition1; }
    }

    public PropertyDefinition DerivedPropertyDefinition2
    {
      get { return _derivedPropertyDefinition2; }
    }

    public PropertyDefinition DerivedDerivedPropertyDefinition
    {
      get { return _derivedDerivedPropertyDefinition; }
    }

    public static PropertyDefinition CreateAndAddPropertyDefinition (ClassDefinition classDefinition, string propertyName)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition, propertyName);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }

    public static PropertyDefinition CreateAndAddPropertyDefinition (ClassDefinition classDefinition, string propertyName, IPropertyInformation propertyInformation)
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForPropertyInformation (classDefinition, propertyName, propertyInformation);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }
  }
}