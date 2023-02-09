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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.UnitTests.Validators
{
  [TestFixture]
  public class NotEmptyCollectionValidatorTest : ValidatorTestBase
  {
    [Test]
    public void Validate_WithPropertyValueNull_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(null);
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext);

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyCollection_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new ArrayList());
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be empty."));
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyGenericCollection_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new GenericCollection<object>());
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be empty."));
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyReadOnlyCollection_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new ReadOnlyCollection<object>());
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be empty."));
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyEnumerable_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new Enumerable<object>()); // Enumerable.Empty<T> actually returns an array of T.
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsNonEmptyCollection_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new SimpleCollection { "someValue" });
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsNonEmptyGenericCollection_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new GenericCollection<string> { "someValue" });
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsNonEmptyReadOnlyCollection_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new ReadOnlyCollection<string> { "someValue" });
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyString_ReturnsNoValidationFailures ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(string.Empty);
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    [Test]
    public void Validate_WithPropertyValueIsEmptyBinary_ReturnSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(Array.Empty<byte>());
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures.Length, Is.EqualTo(1));
      Assert.That(validationFailures[0].ErrorMessage, Is.EqualTo("The value must not be empty."));
    }

    [Test]
    public void Validate_WithPropertyValueIsObject_ReturnsSingleValidationFailure ()
    {
      var propertyValidatorContext = CreatePropertyValidatorContext(new object());
      var validator = new NotEmptyCollectionValidator(new InvariantValidationMessage("Fake Message"));

      var validationFailures = validator.Validate(propertyValidatorContext).ToArray();

      Assert.That(validationFailures, Is.Empty);
    }

    private class Enumerable<T> : IEnumerable<T>
    {
      private readonly IEnumerable<T> _enumerable = Enumerable.Empty<T>();

      public IEnumerator<T> GetEnumerator () => _enumerable.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }

    private class SimpleCollection : ICollection
    {
      private readonly ArrayList _list = new();

      public IEnumerator GetEnumerator () => _list.GetEnumerator();

      public void CopyTo (Array array, int index) => _list.CopyTo(array, index);

      public void Add (object item) => _list.Add(item);

      public int Count => _list.Count;
      public bool IsSynchronized => true;
      public object SyncRoot { get; } = new();
    }

    private class GenericCollection<T> : ICollection<T>
    {
      private readonly List<T> _list = new();

      public IEnumerator GetEnumerator () => _list.GetEnumerator();

      public int Count => _list.Count;

      IEnumerator<T> IEnumerable<T>.GetEnumerator () => _list.GetEnumerator();

      public void Add (T item) => _list.Add(item);

      public void Clear () => _list.Clear();

      public bool Contains (T item) => _list.Contains(item);

      public void CopyTo (T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

      public bool Remove (T item) => _list.Remove(item);

      public bool IsReadOnly => false;
    }

    private class ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
      private readonly List<T> _list = new();

      public IEnumerator GetEnumerator () => _list.GetEnumerator();

      public void Add (T item) => _list.Add(item);

      public int Count => _list.Count;

      IEnumerator<T> IEnumerable<T>.GetEnumerator () => _list.GetEnumerator();
    }
  }
}
