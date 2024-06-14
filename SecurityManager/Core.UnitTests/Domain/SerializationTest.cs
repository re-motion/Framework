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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.TypePipe;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SerializationTest
  {
    [Test]
    public void DomainObjectsAreSerializable ()
    {
      Assert2.IgnoreIfFeatureSerializationIsDisabled();

      CheckDomainObjectSerializability(delegate { return AccessControlEntry.NewObject(); });
      CheckDomainObjectSerializability(delegate { return StatefulAccessControlList.NewObject(); });
      CheckDomainObjectSerializability(delegate { return Permission.NewObject(); });
      CheckDomainObjectSerializability(delegate { return StateCombination.NewObject(); });
      CheckDomainObjectSerializability(delegate { return StateUsage.NewObject(StateDefinition.NewObject()); });
      CheckDomainObjectSerializability(delegate { return AbstractRoleDefinition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return AccessTypeDefinition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return AccessTypeReference.NewObject(); });
      CheckDomainObjectSerializability(delegate { return Culture.NewObject("DE-DE"); });
      CheckDomainObjectSerializability(delegate { return SecurableClassDefinition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return LocalizedName.NewObject("foo", Culture.NewObject("DE-DE"), SecurableClassDefinition.NewObject()); });
      CheckDomainObjectSerializability(delegate { return StateDefinition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return StatePropertyDefinition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return StatePropertyReference.NewObject(); });
      CheckDomainObjectSerializability(delegate { return (Group)LifetimeService.NewObject(ClientTransaction.Current, typeof(Group), ParamList.Empty); });
      CheckDomainObjectSerializability(delegate { return (GroupType)LifetimeService.NewObject(ClientTransaction.Current, typeof(GroupType), ParamList.Empty); });
      CheckDomainObjectSerializability(delegate { return GroupTypePosition.NewObject(); });
      CheckDomainObjectSerializability(delegate { return (Position)LifetimeService.NewObject(ClientTransaction.Current, typeof(Position), ParamList.Empty); });
      CheckDomainObjectSerializability(delegate { return Role.NewObject(); });
      CheckDomainObjectSerializability(delegate { return (Tenant)LifetimeService.NewObject(ClientTransaction.Current, typeof(Tenant), ParamList.Empty); });
      CheckDomainObjectSerializability(delegate { return (User)LifetimeService.NewObject(ClientTransaction.Current, typeof(User), ParamList.Empty); });
    }

    private void CheckDomainObjectSerializability<T> (Func<T> creator)
        where T: DomainObject
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
      {
        T instance = creator();

        Tuple<T, ClientTransaction> deserializedTuple = Serializer.SerializeAndDeserialize(Tuple.Create(instance, ClientTransaction.Current));
        T deserializedT = deserializedTuple.Item1;
        Assert.That(deserializedT, Is.Not.Null);

        IBusinessObject bindableOriginal = (IBusinessObject)instance;
        IBusinessObject bindableDeserialized = (IBusinessObject)deserializedT;

        foreach (IBusinessObjectProperty property in bindableOriginal.BusinessObjectClass.GetPropertyDefinitions())
        {
          Assert.That(bindableDeserialized.BusinessObjectClass.GetPropertyDefinition(property.Identifier), Is.Not.Null);

          object value = null;
          bool propertyCanBeRetrieved;
          try
          {
            value = bindableOriginal.GetProperty(property);
            propertyCanBeRetrieved = true;
          }
          catch (Exception)
          {
            propertyCanBeRetrieved = false;
          }

          if (propertyCanBeRetrieved)
          {
            object newValue;
            using (deserializedTuple.Item2.EnterNonDiscardingScope())
            {
              newValue = bindableDeserialized.GetProperty(property);
            }
            if (value != null && ReflectionUtility.IsDomainObject(property.PropertyType))
              Assert.That(((DomainObject)newValue).ID, Is.EqualTo(((DomainObject)value).ID));
            else
              Assert.That(newValue, Is.EqualTo(value));
          }
        }
      }
    }
  }
}
