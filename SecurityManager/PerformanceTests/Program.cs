using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Logging;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.AccessControl.AccessEvaluation;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.PerformanceTests
{
  class Program
  {
    private static void Main (string[] args)
    {
      var defaultServiceLocator = DefaultServiceLocator.Create();

      defaultServiceLocator.Register(
          typeof(Remotion.Data.DomainObjects.IClientTransactionExtensionFactory),
          typeof(Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory),
          LifetimeKind.Singleton,
          RegistrationType.Single);
      defaultServiceLocator.Register(
          typeof(Remotion.Data.DomainObjects.Tracing.IPersistenceExtensionFactory),
          typeof(Remotion.Data.DomainObjects.UberProfIntegration.LinqToSqlExtensionFactory),
          LifetimeKind.Singleton,
          RegistrationType.Single);

      ServiceLocator.SetLocatorProvider(() => defaultServiceLocator);

      LogManager.Initialize();

      var provider = new SecurityService(
          SafeServiceLocator.Current.GetInstance<IAccessControlListFinder>(),
          SafeServiceLocator.Current.GetInstance<ISecurityTokenBuilder>(),
          SafeServiceLocator.Current.GetInstance<IAccessResolver>());
      var context =
          new SimpleSecurityContext(
              "ActaNova.Federal.Domain.File, ActaNova.Federal.Domain",
              "ServiceUser",
              string.Empty,
              "SystemTenant",
              false,
              new Dictionary<string, EnumWrapper>
              {
                  {
                      "CommonFileState",
                      EnumWrapper.Get("Work|ActaNova.Domain.CommonFile+CommonFileStateType, ActaNova.Domain")
                  }
              },
              new EnumWrapper[0]);
      ISecurityPrincipal user = new SecurityPrincipal("ServiceUser", null, null, null);

      LoadMapping();
      LoadObjectBinding();
      CreateFirstDomainObject();
      InitializeLinq();
      ExecuteFirstLinqQuery();

      Console.WriteLine("done init");
      Console.ReadLine();

      TestSimpleQueryAsString();
      TestSimpleLinqQuery();
      TestSimpleLinqQueryWithCustomProjection_MappingInitialization();
      TestSimpleLinqQueryWithCustomProjection();
      TestComplexLinqQuery();

      Console.WriteLine();

      Console.WriteLine("Initializing query cache...");
      new SecurityContextRepository(new RevisionProvider(), new UserNamesRevisionProvider()).GetTenant("SystemTenant");
      new SecurityPrincipalRepository(new UserRevisionProvider()).GetUser("ServiceUser");
      Console.WriteLine("Query cache initialized");
      Console.WriteLine();

      using (StopwatchScope.CreateScope("First access check took {elapsed:ms} ms."))
      {
        provider.GetAccess(context, user);
      }
      Console.WriteLine("Init done");
      //Console.ReadKey();

      Stopwatch stopwatch = Stopwatch.StartNew();
      int dummy = 0;
      int count = 1;
      for (int i = 0; i < count; i++)
      {
        dummy += provider.GetAccess(context, user).Length;
      }
      stopwatch.Stop();
      Console.WriteLine("Time taken: {0}ms", ((decimal)stopwatch.ElapsedMilliseconds) / count);

      provider.GetAccess(
          new SimpleSecurityContext(
              "ActaNova.Federal.Domain.File, ActaNova.Federal.Domain",
              null,
              null,
              null,
              true,
              new Dictionary<string, EnumWrapper>(),
              new EnumWrapper[0]),
          new SecurityPrincipal("jou", null, null, null));

      Trace.Write(dummy);
      Console.ReadKey();
    }

    private static void LoadMapping ()
    {
      using (StopwatchScope.CreateScope("Loading Mapping took {elapsed:ms} ms."))
        MappingConfiguration.Current.GetTypeDefinitions();
    }

    private static void LoadObjectBinding ()
    {
      using (StopwatchScope.CreateScope("Loading object binding took {elapsed:ms} ms."))
        BindableObjectProvider.GetProviderForBindableObjectType(typeof(Tenant)).GetBindableObjectClass(typeof(Tenant));
    }

    private static void CreateFirstDomainObject ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      using (StopwatchScope.CreateScope("First DomainObject {elapsed:ms} ms."))
        new OrganizationalStructureFactory().CreatePosition();
    }

    private static void InitializeLinq ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        using (StopwatchScope.CreateScope("Initializing Linq took {elapsed:ms} ms."))
          QueryFactory.CreateLinqQuery<Tenant>();
      }
    }

    private static void ExecuteFirstLinqQuery ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
       using (StopwatchScope.CreateScope("Executing first Linq query took {elapsed:ms} ms."))
       QueryFactory.CreateLinqQuery<Position>().ToList();
      }
    }

    private static void TestSimpleLinqQuery ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        using (StopwatchScope.CreateScope("Simple Linq query took {elapsed:ms} ms"))
        {
          QueryFactory.CreateLinqQuery<Position>().ToList();
        }
      }
    }

    private static void TestSimpleLinqQueryWithCustomProjection_MappingInitialization ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        QueryFactory.CreateLinqQuery<Tenant>().Select(p => new { Value = p.ID, Key = p.UniqueIdentifier }).ToList();
      }
    }

    private static void TestSimpleLinqQueryWithCustomProjection ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        using (StopwatchScope.CreateScope("Simple Linq query with custom projection took {elapsed:ms} ms"))
        {
          QueryFactory.CreateLinqQuery<Tenant>().Select(p => new { Value = p.ID, Key = p.UniqueIdentifier }).ToList();
        }
      }
    }

    private static void TestSimpleQueryAsString ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        using (StopwatchScope.CreateScope("Simple query from string took {elapsed:ms} ms"))
        {
          var query =
              QueryFactory.CreateQuery(
                  new QueryDefinition(
                      "id",
                      DomainObjectsConfiguration.Current.Storage.StorageProviderDefinitions["SecurityManager"],
                      "select * from PositionView",
                      QueryType.Collection));
          ClientTransaction.Current.QueryManager.GetCollection<Position>(query);
        }
      }
    }
    private static void TestComplexLinqQuery ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        using (StopwatchScope.CreateScope("Complex query took {elapsed:ms} ms"))
        {
          var result = from acl in QueryFactory.CreateLinqQuery<StatefulAccessControlList>()
                       from sc in acl.GetStateCombinationsForQuery()
                       from usage in sc.GetStateUsagesForQuery().DefaultIfEmpty()
                       from propertyReference in acl.GetClassForQuery().GetStatePropertyReferencesForQuery().DefaultIfEmpty()
                       select new
                              {
                                  Class = acl.GetClassForQuery().ID,
                                  Acl = acl.ID.GetHandle<StatefulAccessControlList>(),
                                  HasState = propertyReference != null,
                                  StatePropertyID = propertyReference.StateProperty.ID.Value,
                                  StatePropertyClassID = propertyReference.StateProperty.ID.ClassID,
                                  StatePropertyName = propertyReference.StateProperty.Name,
                                  StateValue = usage.StateDefinition.Name
                              };

          result.Count();
        }
      }
    }
  }
}
