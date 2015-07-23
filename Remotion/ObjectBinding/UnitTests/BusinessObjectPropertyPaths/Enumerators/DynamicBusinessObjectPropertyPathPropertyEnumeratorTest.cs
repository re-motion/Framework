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
using Remotion.ObjectBinding.BusinessObjectPropertyPaths.Enumerators;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.BusinessObjectPropertyPaths.Enumerators
{
  [TestFixture]
  public class DynamicBusinessObjectPropertyPathPropertyEnumeratorTest : BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    [Test]
    public void MoveNext_WithMultipleProperties ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      var thirdClassStub = CreateClassStub();

      var firstPropertyStub = CreateReferencePropertyStub (firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreateReferencePropertyStub (secondClassStub, "SecondProperty", thirdClassStub);
      var thirdPropertyStub = CreatePropertyStub (thirdClassStub, "ThirdProperty");

      var enumerator = new DynamicBusinessObjectPropertyPathPropertyEnumerator ("FirstProperty:SecondProperty:ThirdProperty");

      Assert.That (enumerator.MoveNext (firstClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (firstPropertyStub));

      Assert.That (enumerator.MoveNext (secondClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (secondPropertyStub));
      
      Assert.That (enumerator.MoveNext (thirdClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (thirdPropertyStub));

      Assert.That (enumerator.MoveNext (MockRepository.GenerateStub<IBusinessObjectClass>()), Is.False);
      Assert.That (()=>enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
    }

    [Test]
    public void MoveNext_WithMissingProperty_ContinuesEnumeration_SetsCurrentNull ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Stub (_ => _.Identifier).Return ("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub (firstClassStub, "FirstProperty", secondClassStub);

      var enumerator = new DynamicBusinessObjectPropertyPathPropertyEnumerator ("FirstProperty:Missing:ThirdProperty");

      Assert.That (enumerator.MoveNext (firstClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (firstPropertyStub));

      Assert.That (enumerator.MoveNext (secondClassStub), Is.True);
      Assert.That (enumerator.Current, Is.Null);
    }

    [Test]
    public void MoveNext_WithNonLastPropertyNotReferenceProperty_ContinuesEnumeration_SetsCurrentNull ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      secondClassStub.Stub (_ => _.Identifier).Return ("SecondClass");

      var firstPropertyStub = CreateReferencePropertyStub (firstClassStub, "FirstProperty", secondClassStub);

      var enumerator = new DynamicBusinessObjectPropertyPathPropertyEnumerator ("FirstProperty:SecondProperty:ThirdProperty");

      Assert.That (enumerator.MoveNext (firstClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (firstPropertyStub));

      Assert.That (enumerator.MoveNext (secondClassStub), Is.True);
      Assert.That (enumerator.Current, Is.Null);
    }
  }
}