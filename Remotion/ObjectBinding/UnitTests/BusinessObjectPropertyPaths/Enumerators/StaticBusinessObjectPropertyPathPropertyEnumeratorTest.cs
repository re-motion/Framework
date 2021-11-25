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
using Moq;
using NUnit.Framework;
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  [TestFixture]
  public class StaticBusinessObjectPropertyPathPropertyEnumeratorTest : BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    [Test]
    public void MoveNext_WithMultipleProperties ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      var thirdClassStub = CreateClassStub();

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreateReferencePropertyStub(secondClassStub, "SecondProperty", thirdClassStub);
      var thirdPropertyStub = CreatePropertyStub(thirdClassStub, "ThirdProperty");

      var enumerator = new StaticBusinessObjectPropertyPathPropertyEnumerator("FirstProperty:SecondProperty:ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));

      Assert.That(enumerator.MoveNext(secondClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(secondPropertyStub.Object));
      
      Assert.That(enumerator.MoveNext(thirdClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(thirdPropertyStub.Object));

      Assert.That(enumerator.MoveNext(new Mock<IBusinessObjectClass>().Object), Is.False);
      Assert.That(()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo("Enumeration already finished."));
    }

    [Test]
    public void MoveNext_WithMissingProperty_ThrowsParseException ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Setup(_ => _.Identifier).Returns("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);

      var enumerator = new StaticBusinessObjectPropertyPathPropertyEnumerator("FirstProperty:Missing:ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));

      Assert.That(
          () => enumerator.MoveNext(secondClassStub.Object),
          Throws.TypeOf<ParseException>().With.Message.EqualTo("BusinessObjectClass 'SecondClass' does not contain a property named 'Missing'."));
    }

    [Test]
    public void MoveNext_WithNonLastPropertyNotReferenceProperty_ThrowsParseException ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Setup(_ => _.Identifier).Returns("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub(firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreatePropertyStub(secondClassStub, "SecondProperty");

      var enumerator = new StaticBusinessObjectPropertyPathPropertyEnumerator("FirstProperty:SecondProperty:ThirdProperty");

      Assert.That(enumerator.MoveNext(firstClassStub.Object), Is.True);
      Assert.That(enumerator.Current, Is.SameAs(firstPropertyStub.Object));

      Assert.That(
          () => enumerator.MoveNext(secondClassStub.Object),
          Throws.TypeOf<ParseException>()
                .With.Message.EqualTo(
                    string.Format(
                        "Each property in a property path except the last one must be a reference property. "
                        + "Property 'SecondProperty' is of type '{0}'.",
                        secondPropertyStub.Object.GetType().Name)));
    }
  }
}