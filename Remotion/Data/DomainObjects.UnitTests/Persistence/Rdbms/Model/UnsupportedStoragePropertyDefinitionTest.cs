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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class UnsupportedStoragePropertyDefinitionTest
  {
    private UnsupportedStoragePropertyDefinition _unsupportedStorageProperty;
    private Exception _innerException;

    [SetUp]
    public void SetUp ()
    {
      _innerException = new Exception ("Inner!");
      _unsupportedStorageProperty = new UnsupportedStoragePropertyDefinition (typeof (int), "Message", _innerException);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_unsupportedStorageProperty.Message, Is.EqualTo ("Message"));
      Assert.That (_unsupportedStorageProperty.PropertyType, Is.SameAs (typeof (int)));
      Assert.That (_unsupportedStorageProperty.InnerException, Is.SameAs (_innerException));
    }

    [Test]
    public void Initialization_WithNullInnerException ()
    {
      var columnDefinition = new UnsupportedStoragePropertyDefinition (typeof (int), "Message", null);

      Assert.That (columnDefinition.InnerException, Is.Null);
      Assert.That (
          () => columnDefinition.GetColumns(), 
          Throws.TypeOf<NotSupportedException>().With.InnerException.Null);
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (
          () => _unsupportedStorageProperty.GetColumns (),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      Assert.That (
          () => _unsupportedStorageProperty.GetColumnsForComparison(), 
          Throws.TypeOf<NotSupportedException>()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValue ()
    {
      Assert.That (
          () => _unsupportedStorageProperty.SplitValue (null),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      Assert.That (
          () => { _unsupportedStorageProperty.SplitValueForComparison (null); },
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      Assert.That (
          () => _unsupportedStorageProperty.SplitValuesForComparison (null),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void CombineValue ()
    {
      Assert.That (
          () => _unsupportedStorageProperty.CombineValue (MockRepository.GenerateStub<IColumnValueProvider> ()),
          Throws.TypeOf<NotSupportedException> ()
              .With.Message.EqualTo ("This operation is not supported because the storage property is invalid. Reason: Message")
              .And.InnerException.SameAs (_innerException));
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var property1 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", new Exception());
      var property2 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", new Exception());
      var property3 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", new Exception());

      var result = property1.UnifyWithEquivalentProperties (new[] { property2, property3 });

      Assert.That (
          result, Is.TypeOf<UnsupportedStoragePropertyDefinition>().With.Property ("PropertyType").SameAs (typeof (int)));
      Assert.That (((UnsupportedStoragePropertyDefinition) result).Message, Is.EqualTo ("x"));
      Assert.That (((UnsupportedStoragePropertyDefinition) result).InnerException, Is.SameAs (property1.InnerException));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ();

      Assert.That (
          () => _unsupportedStorageProperty.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'UnsupportedStoragePropertyDefinition', and the given property has "
              + "type 'SimpleStoragePropertyDefinition'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentPropertyType ()
    {
      var exception = new Exception();
      var property1 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", exception);
      var property2 = new UnsupportedStoragePropertyDefinition (typeof (string), "x", exception);

      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has property type 'System.Int32', and the given property has "
              + "property type 'System.String'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentMessage ()
    {
      var exception = new Exception ();
      var property1 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", exception);
      var property2 = new UnsupportedStoragePropertyDefinition (typeof (int), "y", exception);
      
      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has message 'x', and the given property has "
              + "message 'y'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentExceptionType ()
    {
      var property1 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", new InvalidOperationException());
      var property2 = new UnsupportedStoragePropertyDefinition (typeof (int), "x", new ArgumentException());

      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has inner exception type 'System.InvalidOperationException', and the "
              + "given property has inner exception type 'System.ArgumentException'.\r\nParameter name: equivalentProperties"));
    }
  }
}