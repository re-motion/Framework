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
using System.Globalization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.TypePipe;
using Remotion.ServiceLocation;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.TypeAssembly.Implementation;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.TypePipe.PerformanceTests
{
  [Explicit("Performance measurement for caching")]
  [TestFixture]
  public class CachePerformanceTest
  {
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      var storageSettingsFactory = StorageSettingsFactory.CreateForSqlServer("Integrated Security=SSPI;Initial Catalog=TestDatabase;Data Source=localhost");
      serviceLocator.RegisterSingle(() => storageSettingsFactory);

      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    [TearDown]
    public void TearDown ()
    {
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void TypePipe ()
    {
      var participants = new IParticipant[]
                         {
                           new MixinParticipant(
                               SafeServiceLocator.Current.GetInstance<IConfigurationProvider>(),
                               SafeServiceLocator.Current.GetInstance<IMixinTypeProvider>(),
                               SafeServiceLocator.Current.GetInstance<ITargetTypeModifier>(),
                               SafeServiceLocator.Current.GetInstance<IConcreteTypeMetadataImporter>()),
                           new DomainObjectParticipant(
                               SafeServiceLocator.Current.GetInstance<ITypeDefinitionProvider>(),
                               SafeServiceLocator.Current.GetInstance<IInterceptedPropertyFinder>())
                         };
      var pipelineFactory = new RemotionPipelineFactory();

      var pipeline = pipelineFactory.Create("CachePerformanceTest", participants);
      var typeCache = (ITypeCache)PrivateInvoke.GetNonPublicField(pipeline.ReflectionService, "_typeCache");
      var constructorCallCache = (IConstructorCallCache)PrivateInvoke.GetNonPublicField(pipeline.ReflectionService, "_constructorCallCache");
      var typeAssembler = (ITypeAssembler)PrivateInvoke.GetNonPublicField(pipeline, "_typeAssembler");
      var typeID = typeAssembler.ComputeTypeID(typeof(DomainType));

      Func<Type> typeCacheFunc = () => typeCache.GetOrCreateType(typeID);
      Func<Delegate> constructorDelegateCacheFunc = () => constructorCallCache.GetOrCreateConstructorCall(typeID, typeof(Func<object>), true);

      TimeThis("TypePipe_Types", typeCacheFunc);
      TimeThis("TypePipe_ConstructorDelegates", constructorDelegateCacheFunc);
    }

    private static void TimeThis<T> (string testName, Func<T> func)
        where T : notnull
    {
      // Warmup and cache population.
      func();

      const int startPow = 3;
      const int maxPow = 6;
      int hc = 0;

      Console.WriteLine(testName);
      for (int i = startPow; i <= maxPow; ++i)
      {
        GC.Collect(2, GCCollectionMode.Forced);
        GC.WaitForPendingFinalizers();
        GC.Collect(2, GCCollectionMode.Forced);

        long requestedInstanceCount = (long)Math.Pow(10, i);

        StopwatchScope.MeasurementAction measurementAction =
            (c, s) => Console.WriteLine(
                "{0}: {1}ms, per call: {2}",
                requestedInstanceCount.ToString(CultureInfo.InvariantCulture).PadLeft(8),
                s.ElapsedTotal.TotalMilliseconds.ToString("0.0000").PadLeft(10),
                (s.ElapsedTotal.TotalMilliseconds / requestedInstanceCount).ToString("0.000000"));

        using (StopwatchScope.CreateScope(measurementAction))
        {
          for (int j = 0; j < requestedInstanceCount; ++j)
          {
            var obj = func();
            hc = hc >> 1;
            hc ^= obj.GetHashCode();
          }
        }
      }
      Console.WriteLine();
    }

    [DBTable]
    [Uses(typeof(object))]
    public class DomainType : DomainObject { }
  }
}
