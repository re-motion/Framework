﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableLoadedObjectDataRegistrationAgentFake : ILoadedObjectDataRegistrationAgent
  {
    public void RegisterIfRequired (ILoadedObjectData loadedObjectData, bool throwOnNotFound)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ILoadedObjectData> RegisterIfRequired (IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound)
    {
      throw new System.NotImplementedException();
    }

    public IEnumerable<ILoadedObjectData> BeginRegisterIfRequired (IEnumerable<ILoadedObjectData> loadedObjects, bool throwOnNotFound, LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector)
    {
      throw new NotImplementedException();
    }

    public void EndRegisterIfRequired (LoadedObjectDataPendingRegistrationCollector pendingLoadedObjectDataCollector)
    {
      throw new NotImplementedException();
    }
  }
}