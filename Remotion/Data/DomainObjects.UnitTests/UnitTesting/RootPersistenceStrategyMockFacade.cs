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
using JetBrains.Annotations;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.DataManagement;
using Remotion.Data.DomainObjects.UnitTests.DomainImplementation.Transport;
using Remotion.Mixins;
using Remotion.Utilities;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace Remotion.Data.UnitTests.UnitTesting
{
  /// <summary>
  /// Mocks the <see cref="IPersistenceStrategy"/> used by root <see cref="ClientTransaction"/> and provides utility methods to set up the mock.
  /// </summary>
  public class RootPersistenceStrategyMockFacade
  {
    public static RootPersistenceStrategyMockFacade CreateWithStrictMock ()
    {
      var persistenceStrategyMock = MockRepository.GenerateStrictMock<IFetchEnabledPersistenceStrategy>();
      var facade = new RootPersistenceStrategyMockFacade (persistenceStrategyMock);
      return facade;
    }

    private readonly IFetchEnabledPersistenceStrategy _mock;

    public RootPersistenceStrategyMockFacade (IFetchEnabledPersistenceStrategy mock)
    {
      ArgumentUtility.CheckNotNull ("mock", mock);
      _mock = mock;
    }

    public IFetchEnabledPersistenceStrategy Mock
    {
      get { return _mock; }
    }

    /// <summary>
    /// Opens a scope in which every newly created <see cref="ClientTransaction"/> will use the <see cref="Mock"/> strategy for loading
    /// objects into its root transaciton.
    /// </summary>
    public IDisposable CreateScope ()
    {
      return RootClientTransactionComponentFactoryMixin.CreatePersistenceStrategyScope (Mock);
    }

    public IMethodOptions<IEnumerable<ILoadedObjectData>> ExpectLoadObjectData (IEnumerable<ObjectID> loadedObjectIDs)
    {
      return Mock
          .Expect (mock => mock.LoadObjectData (Arg<IEnumerable<ObjectID>>.List.Equal (loadedObjectIDs)))
          .Return (loadedObjectIDs.Select (id => (ILoadedObjectData) new FreshlyLoadedObjectData (DataContainerObjectMother.CreateExisting (id))));
    }

    public class RootClientTransactionComponentFactoryMixin : Mixin<RootClientTransactionComponentFactory>
    {
      public static IDisposable CreatePersistenceStrategyScope (IFetchEnabledPersistenceStrategy persistenceStrategy)
      {
        var mixinConfigurationScope = MixinConfiguration.BuildFromActive ()
                                                        .ForClass<RootClientTransactionComponentFactory> ()
                                                        .AddMixin<RootClientTransactionComponentFactoryMixin> ()
                                                        .EnterScope ();
        s_persistenceStrategy = persistenceStrategy;
        return new PostActionDisposableDecorator (mixinConfigurationScope, () => { s_persistenceStrategy = null; });
      }

      [ThreadStatic]
      private static IFetchEnabledPersistenceStrategy s_persistenceStrategy;

      public RootClientTransactionComponentFactoryMixin ()
      {
      }

      [OverrideTarget]
      public IPersistenceStrategy CreatePersistenceStrategy ([UsedImplicitly] ClientTransaction constructedTransaction)
      {
        if (s_persistenceStrategy == null)
          throw new InvalidOperationException ("No persistence strategy has been given.");
        return s_persistenceStrategy;
      }
    }
  }
}