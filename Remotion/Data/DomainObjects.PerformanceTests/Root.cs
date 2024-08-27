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
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  /// <summary>
  /// This class is the root to execute a single test in the profiler.
  /// Switch the project type to a ConsoleApplication in order to use it.
  /// </summary>
  public class Root
  {
    private Root ()
    {
    }
/*
    [STAThread]
    public static void Main (string[] args)
    {
      // LogManager.Initialize();

      var setUpFixture = new SetUpFixture();
      setUpFixture.OneTimeSetUp();

      // Have all xml files loaded, so if the code is instrumented by a profiler,
      // the loading does not falsify the method run times during the first call of GetObject.
      Dev.Null = MappingConfiguration.Current;

      //RunLoadObjectsTest();

      //RunHasRelationChangedTest();
      //RunCommitTest ();

      PropertyTest();
      //BindableObjectWithSecurityTest ();
      //BindableObjectWithoutSecurityTest ();

      //LinqTest();
      //InstantiationTest ();

      // RelationQuerySyncSpike ();

      //ClassDefinitionTest();

      setUpFixture.OneTimeTearDown();

      Console.WriteLine("Tests complete");
      //Console.ReadLine();
    }
*/
    private static void RunHasRelationChangedTest ()
    {
      var test = new HasRelationChangedTest();
      test.OneTimeSetUp();

      test.SetUp();
      test.AskChanged();
      test.TearDown();

      test.TestFixtureTearDown();
    }

    private static void RunCommitTest ()
    {
      var test = new CommitTest();
      test.OneTimeSetUp();

      test.SetUp();
      test.CommitSubTransaction_Relations();
      test.CommitSubTransaction_ValueProperties();
      test.CommitTransactionToDatabase_NewProperties();
      test.TearDown();

      test.TestFixtureTearDown();
    }

    private static void RunLoadObjectsTest ()
    {
      var test = new LoadObjectsTest();
      test.OneTimeSetUp();

      test.SetUp();
      test.LoadObjectsOverRelationTest();
      test.TearDown();

      test.SetUp();
      test.LoadObjectsOverRelationWithAbstractBaseClass();
      test.TearDown();

      test.TestFixtureTearDown();
    }

    private static void PropertyTest ()
    {
      var test = new PropertyTest();

      test.SetUp();
      test.String_GetProperty();
      test.Unidirectional_GetProperty();
      test.Bidirectional_OneToOne_RealEndPoint_GetProperty();
      test.Bidirectional_OneToOne_VirtualEndPoint_GetProperty();
      test.Bidirectional_OneToMany_RealEndPoint_GetProperty();
      test.Bidirectional_OneToMany_CollectionEndPoint_GetProperty();
      test.String_SetProperty();
      test.Unidirectional_SetProperty();
      test.Bidirectional_OneToOne_RealEndPoint_SetProperty();
      test.Bidirectional_OneToOne_VirtualEndPoint_SetProperty();
      test.Bidirectional_OneToMany_RealEndPoint_SetProperty();
      test.Bidirectional_OneToMany_CollectionEndPoint_SetProperty();
      test.TearDown();
    }

    private static void BindableObjectWithSecurityTest ()
    {
      var test = new BindableObjectWithSecurityTest();

      test.SetUp();
      test.BusinessObject_Property_IsAccessible();
      test.BusinessObject_GetProperty();
      test.DynamicMethod_GetProperty();
      test.DomainObject_GetProperty();
      test.BusinessObject_SetProperty();
      test.DomainObject_SetProperty();
      test.TearDown();
    }

    private static void BindableObjectWithoutSecurityTest ()
    {
      var test = new BindableObjectWithoutSecurityTest();

      test.SetUp();
      test.BusinessObject_Property_IsAccessible();
      test.BusinessObject_GetProperty();
      test.DynamicMethod_GetProperty();
      test.DomainObject_GetProperty();
      test.BusinessObject_SetProperty();
      test.DomainObject_SetProperty();
      test.TearDown();
    }

    private static void LinqTest ()
    {
      var test = new LinqTest();
      test.SetUp();
      test.WithTinyResultSet();
      test.WithSmallResultSet();
      test.WithLargeResultSet();
      test.WithComplexQuery_Subqueries();
      test.WithComplexQuery_SubqueriesInSecondFromClauseAndMultiplyOrderByClauses();
      test.WithComplexQuery_JoinsAndSubquery();
      test.WithComplexQuery_GroupBy();
      test.TearDown();
    }

    private static void InstantiationTest ()
    {
      var test = new InstantiationTest();
      test.SetUp();
      test.GetObjectReference();
      test.TearDown();
    }

    private static void ClassDefinitionTest ()
    {
      var test = new ClassDefinitionTest();
      test.SetUp();
      test.GetOppositeClassDefinition();
      test.GetMandatoryOppositeClassDefinition();
      test.GetOppositeEndPointDefinition();
      test.GetMandatoryOppositeEndPointDefinition();
      test.TearDown();
    }
  }
}
