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
using NUnit.Framework;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  [TestFixture]
  public class BusinessObjectPropertyPathPropertyEnumeratorBaseTest : BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    [Test]
    public void MoveNext_NeverWithSingleProperty_CurrentThrows_HasNextIsTrue ()
    {
      var classStub = CreateClassStub();
      CreatePropertyStub(classStub, "FirstProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty");

      Assert.That(() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration has not started. Call MoveNext."));
      Assert.That(enumerator.HasNext, Is.True);
    }

    [Test]
    public void MoveNext_OnceWithSingleProperty_ReturnsTrue_CurrentIsSetToProperty_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      var propertyStub = CreatePropertyStub(classStub, "FirstProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty");

      Assert.That(enumerator.MoveNext(classStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(propertyStub.Object));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_TwiceWithSingleProperty_ReturnsFalse_CurrentThrows_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      CreatePropertyStub(classStub, "FirstProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty");

      Assert.That(enumerator.MoveNext(classStub.Object), Is.True);

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_TwiceWithSingleProperty_LastPropertyIsReferenceProperty_ReturnsFalse_CurrentThrows_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      var referenceClassStub = CreateClassStub();
      CreateReferencePropertyStub(classStub, "FirstProperty", referenceClassStub);

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty");

      Assert.That(enumerator.MoveNext(classStub.Object), Is.True);

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_WithMultipleProperties_ReturnsFalseAfterLastProperty_CurrentThrows_HasNextIsFalse ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreatePropertyStub(secondClassStub, "SecondProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty:SecondProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.True);

      Assert.That(enumerator.MoveNext(secondClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(secondPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.False);

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_WithMultipleProperties_LastPropertyIsReferenceProperty_ReturnsFalseAfterLastProperty_CurrentThrows_HasNextIsFalse ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      var thirdClassStub = CreateClassStub();

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreateReferencePropertyStub(secondClassStub, "SecondProperty", thirdClassStub);

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty:SecondProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.True);

      Assert.That(enumerator.MoveNext(secondClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(secondPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.False);

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_WithMultipleProperties_GetsPropertyPathSeparatorFromCurrentClass ()
    {
      var firstClassStub = CreateClassStub();

      var secondClassStub = new Mock<IBusinessObjectClass>();
      secondClassStub.Setup(_ => _.BusinessObjectProvider).Returns(new Mock<IBusinessObjectProvider>().Object);
      Mock.Get(secondClassStub.Object.BusinessObjectProvider).Setup(_ => _.GetPropertyPathSeparator()).Returns('|');

      var thirdClassStub = CreateClassStub();

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreateReferencePropertyStub(secondClassStub, "SecondProperty", thirdClassStub);
      var thirdPropertyStub = CreatePropertyStub(thirdClassStub, "ThirdProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty:SecondProperty|ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));

      Assert.That(enumerator.MoveNext(secondClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(secondPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.True);

      Assert.That(enumerator.MoveNext(thirdClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(thirdPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.False);

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
      Assert.That(enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_WithMissingProperty_CallsTemplateMehtod_SetsCurrentNull_HasNextIsTrue ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Setup(_ => _.Identifier).Returns("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty:Missing:ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.True);

      Assert.That(
          () => enumerator.MoveNext(secondClassStub.Object),
          Throws.Exception.With.Message.EqualTo("HandlePropertyNotFound, class: SecondClass, property: Missing"));
      Assert.That(enumerator.Current, Is.Null);
      Assert.That(enumerator.HasNext, Is.True);
    }

    [Test]
    public void MoveNext_WithNonLastPropertyNotReferenceProperty_CallsTemplateMehtod_SetsCurrentNull_HasNextIsTrue ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Setup(_ => _.Identifier).Returns("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      CreatePropertyStub(secondClassStub, "SecondProperty");

      var enumerator = new TestableBusinessObjectPropertyPathPropertyEnumeratorBase("FirstProperty:SecondProperty:ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));
      Assert.That(enumerator.HasNext, Is.True);

      Assert.That(
          () => enumerator.MoveNext(secondClassStub.Object),
          Throws.Exception.With.Message.EqualTo("HandlePropertyNotLastPropertyAndNotReferenceProperty, class: SecondClass, property: SecondProperty"));
      Assert.That(enumerator.Current, Is.Null);
      Assert.That(enumerator.HasNext, Is.True);
    }
  }
}
