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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class TestDomainObjectMother
  {
    public static ClassWithRelationProperties[] PrepareDatabaseObjectsWithRelationProperties (int numberOfObjects)
    {
      return PrepareDatabaseObjects<ClassWithRelationProperties> (numberOfObjects, CreateAndFillRelationPropertyObject);
    }

    public static ClassWithValueProperties[] PrepareDatabaseObjectsWithValueProperties (int numberOfObjects)
    {
      return PrepareDatabaseObjects<ClassWithValueProperties> (numberOfObjects, CreateAndFillValuePropertyObject);
    }

    public static T[] PrepareDatabaseObjects<T> (int numberOfObjects, Func<T> factory) where T : DomainObject
    {
      var objects = QueryFactory.CreateLinqQuery<T> ().Take (numberOfObjects).ToArray();
      if (objects.Length < numberOfObjects)
      {
        using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
        {
          for (int i = 0; i < numberOfObjects - objects.Length; i++)
            factory ();
          ClientTransaction.Current.Commit ();
        }
        objects = QueryFactory.CreateLinqQuery<T> ().Take (numberOfObjects).ToArray ();
      }
      return objects;
    }

    public static ClassWithRelationProperties[] PrepareLocalObjectsWithRelationProperties (int numberOfObjects)
    {
      return PrepareLocalObjects<ClassWithRelationProperties> (numberOfObjects, CreateAndFillRelationPropertyObject);
    }

    public static ClassWithValueProperties[] PrepareLocalObjectsWithValueProperties (int numberOfObjects)
    {
      return PrepareLocalObjects<ClassWithValueProperties> (numberOfObjects, CreateAndFillValuePropertyObject);
    }

    public static T[] PrepareLocalObjects<T> (int numberOfObjects, Func<T> factory) where T : DomainObject
    {
      var objects = ClientTransaction.Current.GetEnlistedDomainObjects().OfType<T>()
          .Where (obj => obj.State != StateType.NotLoadedYet).Take (numberOfObjects)
          .ToList ();

      var remaining = numberOfObjects - objects.Count;
      for (int i = 0; i < remaining; i++)
        objects.Add (factory ());
      return objects.ToArray();
    }

    public static ClassWithRelationProperties CreateAndFillRelationPropertyObject ()
    {
      ClassWithRelationProperties instance = SimpleDomainObject<ClassWithRelationProperties>.NewObject ();
      instance.Unary1 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary2 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary3 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary4 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary5 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary6 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary7 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary8 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary9 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();
      instance.Unary10 = SimpleDomainObject<OppositeClassWithAnonymousRelationProperties>.NewObject ();

      instance.Real1 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real2 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real3 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real4 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real5 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real6 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real7 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real8 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real9 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();
      instance.Real10 = SimpleDomainObject<OppositeClassWithVirtualRelationProperties>.NewObject ();

      instance.Virtual1 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual2 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual3 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual4 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual5 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual6 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual7 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual8 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual9 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();
      instance.Virtual10 = SimpleDomainObject<OppositeClassWithRealRelationProperties>.NewObject ();

      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());
      instance.Collection.Add (SimpleDomainObject<OppositeClassWithCollectionRelationProperties>.NewObject ());

      return instance;
    }

    public static ClassWithValueProperties CreateAndFillValuePropertyObject ()
    {
      ClassWithValueProperties instance = SimpleDomainObject<ClassWithValueProperties>.NewObject ();
      instance.BoolProperty1 = true;
      instance.BoolProperty2 = false;
      instance.BoolProperty3 = true;
      instance.BoolProperty4 = false;
      instance.BoolProperty5 = true;
      instance.BoolProperty6 = false;
      instance.BoolProperty7 = true;
      instance.BoolProperty8 = false;
      instance.BoolProperty9 = true;
      instance.BoolProperty10 = false;

      instance.IntProperty1 = 0;
      instance.IntProperty2 = 1;
      instance.IntProperty3 = 2;
      instance.IntProperty4 = 3;
      instance.IntProperty5 = 4;
      instance.IntProperty6 = 5;
      instance.IntProperty7 = 6;
      instance.IntProperty8 = 7;
      instance.IntProperty9 = 8;
      instance.IntProperty10 = 9;

      instance.DateTimeProperty1 = new DateTime (2001, 01, 01, 01, 01, 01);
      instance.DateTimeProperty2 = new DateTime (2002, 01, 01, 01, 02, 02);
      instance.DateTimeProperty3 = new DateTime (2003, 01, 01, 01, 03, 03);
      instance.DateTimeProperty4 = new DateTime (2004, 01, 01, 01, 04, 04);
      instance.DateTimeProperty5 = new DateTime (2005, 01, 01, 01, 05, 05);
      instance.DateTimeProperty6 = new DateTime (2006, 01, 01, 01, 06, 06);
      instance.DateTimeProperty7 = new DateTime (2007, 01, 01, 01, 07, 07);
      instance.DateTimeProperty8 = new DateTime (2008, 01, 01, 01, 08, 08);
      instance.DateTimeProperty9 = new DateTime (2009, 01, 01, 01, 09, 09);
      instance.DateTimeProperty10 = new DateTime (2010, 01, 01, 01, 10, 10);

      instance.StringProperty1 = "One";
      instance.StringProperty2 = "Two";
      instance.StringProperty3 = "Three";
      instance.StringProperty4 = "Four";
      instance.StringProperty5 = "Five";
      instance.StringProperty6 = "Six";
      instance.StringProperty7 = "Seven";
      instance.StringProperty8 = "Eight";
      instance.StringProperty9 = "Nine";
      instance.StringProperty10 = "Ten";
      
      return instance;
    }
  }
}