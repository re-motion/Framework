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
  public class ResolvedBusinessObjectPropertyPathPropertyEnumeratorTest : BusinessObjectPropertyPathPropertyEnumeratorTestBase
  {
    [Test]
    public void MoveNext_NeverWithSingleProperty_CurrentThrows_HasNextIsTrue ()
    {
      var classStub = CreateClassStub();
      var propertyStub = CreatePropertyStub (classStub, "FirstProperty");

      var enumerator = new ResolvedBusinessObjectPropertyPathPropertyEnumerator (new[] { propertyStub });

      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration has not started. Call MoveNext."));
      Assert.That (enumerator.HasNext, Is.True);
    }

    [Test]
    public void MoveNext_OnceWithSingleProperty_ReturnsTrue_CurrentIsSetToProperty_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      var propertyStub = CreatePropertyStub (classStub, "FirstProperty");

      var enumerator = new ResolvedBusinessObjectPropertyPathPropertyEnumerator (new[] { propertyStub });

      Assert.That (enumerator.MoveNext (classStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (propertyStub));
      Assert.That (enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_TwiceWithSingleProperty_ReturnsFalse_CurrentThrows_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      var propertyStub = CreatePropertyStub (classStub, "FirstProperty");

      var enumerator = new ResolvedBusinessObjectPropertyPathPropertyEnumerator (new[] { propertyStub });

      Assert.That (enumerator.MoveNext (classStub), Is.True);

      Assert.That (enumerator.MoveNext (MockRepository.GenerateStub<IBusinessObjectClass>()), Is.False);
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
      Assert.That (enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_AgainAfterEnumerationFinished_ReturnsFalse_CurrentThrows_HasNextIsFalse ()
    {
      var classStub = CreateClassStub();
      var propertyStub = CreatePropertyStub (classStub, "FirstProperty");

      var enumerator = new ResolvedBusinessObjectPropertyPathPropertyEnumerator (new[] { propertyStub });

      Assert.That (enumerator.MoveNext (classStub), Is.True);

      Assert.That (enumerator.MoveNext (MockRepository.GenerateStub<IBusinessObjectClass>()), Is.False);
      Assert.That (enumerator.MoveNext (MockRepository.GenerateStub<IBusinessObjectClass>()), Is.False);
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
      Assert.That (enumerator.HasNext, Is.False);
    }

    [Test]
    public void MoveNext_WithMultipleProperties_ReturnsFalseAfterLastProperty_CurrentThrows_HasNextIsFalse ()
    {
      var firstClassStub = CreateClassStub();
      var secondClassStub = CreateClassStub();
      var firstPropertyStub = CreateReferencePropertyStub (firstClassStub, "FirstProperty", secondClassStub);
      var secondPropertyStub = CreatePropertyStub (secondClassStub, "SecondProperty");

      var enumerator = new ResolvedBusinessObjectPropertyPathPropertyEnumerator (new[] { firstPropertyStub, secondPropertyStub });

      Assert.That (enumerator.MoveNext (firstClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (firstPropertyStub));
      Assert.That (enumerator.HasNext, Is.True);

      Assert.That (enumerator.MoveNext (secondClassStub), Is.True);
      Assert.That (enumerator.Current, Is.SameAs (secondPropertyStub));
      Assert.That (enumerator.HasNext, Is.False);

      Assert.That (enumerator.MoveNext (MockRepository.GenerateStub<IBusinessObjectClass>()), Is.False);
      Assert.That (() => enumerator.Current, Throws.InvalidOperationException.With.Message.EqualTo ("Enumeration already finished."));
      Assert.That (enumerator.HasNext, Is.False);
    }
  }
}