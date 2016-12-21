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
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  public static class DomainObjectTestHelper
  {
    public static T ExecuteInReferenceInitializing_NewObject<T> (Func<Order, T> func)
    {
      T result = default(T);
      EventHandler handler = (sender, args) => result = func ((Order) sender);

      Order.StaticInitializationHandler += handler;
      try
      {
        Order.NewObject (); // indirect call of FinishReferenceInitialization
      }
      finally
      {
        Order.StaticInitializationHandler -= handler;
      }

      return result;
    }

    public static T ExecuteInReferenceInitializing_LoadedObject<T> (Func<Order, T> func)
    {
      T result = default (T);
      EventHandler handler = (sender, args) => result = func ((Order) sender);

      Order.StaticInitializationHandler += handler;
      try
      {
        new DomainObjectIDs (MappingConfiguration.Current).Order1.GetObject<Order> (); // indirect call of FinishReferenceInitialization
      }
      finally
      {
        Order.StaticInitializationHandler -= handler;
      }

      return result;
    }
  }
}