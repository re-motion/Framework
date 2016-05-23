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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.IntegrationTests
{
  [TestFixture]
  public class PropertyTest : ClientTransactionBaseTest
  {
    [Test]
    public void SetValidEnumValue ()
    {
      var instance = ClassWithAllDataTypes.NewObject();
      instance.EnumProperty = ClassWithAllDataTypes.EnumType.Value0;
      Assert.That (instance.EnumProperty, Is.EqualTo (ClassWithAllDataTypes.EnumType.Value0));
    }

    [Test]
    [ExpectedException (typeof (InvalidEnumValueException))]
    public void SetInvalidEnumValue ()
    {
      var instance = ClassWithAllDataTypes.NewObject();
      instance.EnumProperty = (ClassWithAllDataTypes.EnumType) (-1);
    }

    [Test]
    public void EnumNotDefiningZero ()
    {
      var instance = ClassWithEnumNotDefiningZero.NewObject();
      Assert.That (instance.EnumValue, Is.EqualTo (TestDomain.EnumNotDefiningZero.First));
    }

    [Test]
    public void UpdateStructuralEquatableValueWithIdenticalValue_RecognizeObjectAsUnchanged ()
    {
      var instance = ClassWithPropertyTypeImplementingIStructuralEquatable.NewObject();
      instance.StructuralEquatableValue = Tuple.Create ("Value", 50);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.EnsureDataAvailable();
        Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
        instance.StructuralEquatableValue = Tuple.Create ("Value", 50);
        Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
      }
    }

    [Test]
    public void UpdateStructuralEquatableValueWithChangedValue_RecognizeObjectAsChanged ()
    {
      var instance = ClassWithPropertyTypeImplementingIStructuralEquatable.NewObject();
      instance.StructuralEquatableValue = Tuple.Create ("Value", 50);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.EnsureDataAvailable();

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          instance.EnsureDataAvailable();
          Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
          instance.StructuralEquatableValue = Tuple.Create ("Other", 100);
          Assert.That (instance.State, Is.EqualTo (StateType.Changed));

          ClientTransaction.Current.Commit();
          Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
        }

        Assert.That(instance.StructuralEquatableValue, Is.EqualTo(Tuple.Create ("Other", 100)));
        Assert.That (instance.State, Is.EqualTo (StateType.Changed));
      }
    }

    [Test]
    public void RollbackStructuralEquatableValue_ResetsChangedValue ()
    {
      var instance = ClassWithPropertyTypeImplementingIStructuralEquatable.NewObject();
      instance.StructuralEquatableValue = Tuple.Create ("Value", 50);

      using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
      {
        instance.EnsureDataAvailable();

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          instance.EnsureDataAvailable();
          Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
          instance.StructuralEquatableValue = Tuple.Create ("Other", 100);
          Assert.That (instance.State, Is.EqualTo (StateType.Changed));

          ClientTransaction.Current.Rollback();
          Assert.That(instance.StructuralEquatableValue, Is.EqualTo(Tuple.Create ("Value", 50)));
          Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
        }

        Assert.That(instance.StructuralEquatableValue, Is.EqualTo(Tuple.Create ("Value", 50)));
        Assert.That (instance.State, Is.EqualTo (StateType.Unchanged));
      }
    }
  }
}