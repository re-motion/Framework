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
using Moq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public static class DomainObjectMockEventReceiver
  {
    public static Mock<IDomainObjectMockEventReceiver> CreateMock (MockBehavior mockBehavior, DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      var mock = new Mock<IDomainObjectMockEventReceiver>(mockBehavior);

      domainObject.Committed += mock.Object.Committed;
      domainObject.Committing += mock.Object.Committing;
      domainObject.RolledBack += mock.Object.RolledBack;
      domainObject.RollingBack += mock.Object.RollingBack;
      domainObject.Deleted += mock.Object.Deleted;
      domainObject.Deleting += mock.Object.Deleting;
      domainObject.PropertyChanged += mock.Object.PropertyChanged;
      domainObject.PropertyChanging += mock.Object.PropertyChanging;
      domainObject.RelationChanged += mock.Object.RelationChanged;
      domainObject.RelationChanging += mock.Object.RelationChanging;

      return mock;
    }
  }
}
