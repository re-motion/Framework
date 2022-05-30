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

using Remotion.Data.DomainObjects.DomainImplementation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain
{
  [ClassID("CustomDataType_ClassWithCustomDataType")]
  [DBTable]
  public class ClassWithCustomDataType : DomainObject
  {
    public static ClassWithCustomDataType NewObject ()
    {
      return DomainObject.NewObject<ClassWithCustomDataType>();
    }

    public static ClassWithCustomDataType GetObject (ClientTransaction clientTransaction, ObjectID id)
    {
      return (ClassWithCustomDataType)LifetimeService.GetObject(clientTransaction, id, false);
    }

    protected ClassWithCustomDataType ()
    {
    }

    public virtual CompoundDataType CompoundDataTypeValue { get; set; }

    [SimpleDataTypeProperty(MaximumLength = 100)]
    public virtual SimpleDataType SimpleDataTypeValue { get; set; }
  }
}
