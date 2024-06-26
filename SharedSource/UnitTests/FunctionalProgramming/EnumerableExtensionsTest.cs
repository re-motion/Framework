// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.UnitTests.FunctionalProgramming.TestDomain;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.FunctionalProgramming
{
  [TestFixture]
  public class EnumerableExtensionsTest
  {
    private IEnumerable<string> _enumerableWithoutValues = default!;
    private IEnumerable<string> _enumerableWithOneValue = default!;
    private IEnumerable<string> _enumerableWithThreeValues = default!;

    private IEnumerable<string?> _enumerableWithOneNullableReferenceValue = default!;
    private IEnumerable<int?> _enumerableWithOneNullableValueTypeValue = default!;
    private IEnumerable<int> _enumerableWithOneValueTypeValue = default!;

    [SetUp]
    public void SetUp ()
    {
      _enumerableWithoutValues = new string[0];
      _enumerableWithOneValue = new[] { "test" };
      _enumerableWithThreeValues = new[] { "test1", "test2", "test3" };

      _enumerableWithOneNullableReferenceValue = new string?[] { null };
      _enumerableWithOneNullableValueTypeValue = new int?[] { null };
      _enumerableWithOneValueTypeValue = new[] { 1 };
    }

    [Test]
    public void First_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.First(() => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test"));
    }

    [Test]
    public void First_ThrowCustomException_WithThreeValues ()
    {
      string actual = _enumerableWithThreeValues.First(() => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test1"));
    }

    [Test]
    public void First_ThrowCustomException_Empty ()
    {
      Assert.That(
          () => _enumerableWithoutValues.First(() => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.First(s => s == "test", () => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_WithThreeValues ()
    {
      string actual = _enumerableWithThreeValues.First(s => s == "test2", () => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test2"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_Empty ()
    {
      Assert.That(
          () => _enumerableWithoutValues.First(s => s == "test2", () => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void First_WithPredicate_ThrowCustomException_NoMatch ()
    {
      Assert.That(
          () => _enumerableWithThreeValues.First(s => s == "invalid", () => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void Single_ThrowCustomException_WithOneValue ()
    {
      string actual = _enumerableWithOneValue.Single(() => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test"));
    }

    [Test]
    public void Single_ThrowCustomException_WithThreeValues ()
    {
      Assert.That(
          () => _enumerableWithThreeValues.Single(() => new ApplicationException("ExpectedText")),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Sequence contains more than one element."));
    }

    [Test]
    public void Single_ThrowCustomException_Empty ()
    {
      Assert.That(
          () => _enumerableWithoutValues.Single(() => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void Single_WithOneNullableReferenceType ()
    {
      string? actual = _enumerableWithOneNullableReferenceValue.Single(() => new ApplicationException("ExpectedText"));
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void Single_WithOneNullableValueType ()
    {
      int? actual = _enumerableWithOneNullableValueTypeValue.Single(() => new ApplicationException("ExpectedText"));
      Assert.That(actual, Is.Null);
    }

    [Test]
    public void Single_WithOneValueType ()
    {
      int? actual = _enumerableWithOneValueTypeValue.Single(() => new ApplicationException("ExpectedText"));
      Assert.That(actual, Is.EqualTo(1));
    }

    [Test]
    public void Single_WithPredicate_ThrowCustomException_WithThreeValuesAndSingleMatch ()
    {
      string actual = _enumerableWithThreeValues.Single(s => s == "test2", () => new ApplicationException("ExpectedText"));

      Assert.That(actual, Is.EqualTo("test2"));
    }

    [Test]
    public void Single_WithPredicate_ThrowCustomException_WithThreeValuesAndMultipleMatches ()
    {
      Assert.That(
          () => _enumerableWithThreeValues.Single(s => true, () => new ApplicationException("ExpectedText")),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Sequence contains more than one matching element."));
    }

    [Test]
    public void Single_WithPredicate_ThrowCustomException_Empty ()
    {
      Assert.That(
          () => _enumerableWithoutValues.Single(s => s == "test2", () => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void Single_WithPredicate_ThrowCustomException_NoMatch ()
    {
      Assert.That(
          () => _enumerableWithThreeValues.Single(s => s == "invalid", () => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void CreateSequence_WhileNotNull ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      IEnumerable<Element> actual = fourth.CreateSequence(e => e.Parent);
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { fourth, third, second, first }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue ()
    {
      var first = new Element(1, new Element(0, null));
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      IEnumerable<Element> actual = fourth.CreateSequence(e => e.Parent, e => e != first);
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { fourth, third, second }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_WithNull ()
    {
      IEnumerable<Element> actual = ((Element?)null).CreateSequence(e => e.Parent, e => e != null);
      Assert.That(actual.ToArray(), Is.Empty);
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_WithSingleElement ()
    {
      var element = new Element(0, null);

      IEnumerable<Element> actual = element.CreateSequence(e => e.Parent, e => e != null);
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { element }));
    }

    [Test]
    public void CreateSequence_WhilePredicateEvaluatesTrue_StopsOnNull ()
    {
      IEnumerable<string> actual = "ABCD".CreateSequence(e =>
      {
        var nextElement = e.Substring(0, e.Length - 1);
        return nextElement.Length == 0 ? null : nextElement;
      }, e => true);

      Assert.That(actual.ToArray(), Is.EqualTo(new[] { "ABCD", "ABC", "AB", "A" }));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WhileNotNull ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(e => e.Parent, e => new Exception());
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { fourth, third, second, first }));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WhilePredicateEvaluatesTrue ()
    {
      var first = new Element(1, new Element(0, null));
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(e => e.Parent, e => e != first, null, e => new Exception());
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { fourth, third, second }));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WhilePredicateEvaluatesTrue_WithNull ()
    {
      IEnumerable<Element> actual = ((Element?)null).CreateSequenceWithCycleCheck(e => e.Parent, e => e != null, null, e => new Exception());
      Assert.That(actual.ToArray(), Is.Empty);
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WhilePredicateEvaluatesTrue_WithSingleElement ()
    {
      var element = new Element(0, null);

      IEnumerable<Element> actual = element.CreateSequenceWithCycleCheck(e => e.Parent, e => e != null, null, e => new Exception());
      Assert.That(actual.ToArray(), Is.EqualTo(new[] { element }));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WhilePredicateEvaluatesTrue_StopsOnNull ()
    {
      IEnumerable<string> actual = "ABCD".CreateSequenceWithCycleCheck(e =>
      {
        var nextElement = e.Substring(0, e.Length - 1);
        return nextElement.Length == 0 ? null : nextElement;
      }, e => true, null, e => new Exception());

      Assert.That(actual.ToArray(), Is.EqualTo(new[] { "ABCD", "ABC", "AB", "A" }));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WithCycle_Throws ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      first.SetParent(fourth);

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(
          e => e.Parent,
          e => e != null,
          EqualityComparer<Element>.Default,
          e => new Exception(string.Format("element: '{0}'", e)));

      Assert.That(() => actual.Take(10).ToArray(), Throws.Exception.With.Message.EqualTo(string.Format("element: '{0}'", fourth)));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_ElementIsOwnParent_Throws ()
    {
      var first = new Element(1, null);
      first.SetParent(first);

      IEnumerable<Element> actual = first.CreateSequenceWithCycleCheck(
          e => e.Parent,
          e => e != null,
          EqualityComparer<Element>.Default,
          e => new Exception(string.Format("element: '{0}'", e)));

      Assert.That(() => actual.Take(10).ToArray(), Throws.Exception.With.Message.EqualTo(string.Format("element: '{0}'", first)));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WithCycleAboveRoot_Throws ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      first.SetParent(third);

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(
          e => e.Parent,
          e => e != null,
          EqualityComparer<Element>.Default,
          e => new Exception(string.Format("element: '{0}'", e)));

      Assert.That(() => actual.Take(10).ToArray(), Throws.Exception.With.Message.EqualTo(string.Format("element: '{0}'", third)));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_WithCycleAboveRoot_ElementIsOwnParent_Throws ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);

      first.SetParent(first);

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(
          e => e.Parent,
          e => e != null,
          EqualityComparer<Element>.Default,
          e => new Exception(string.Format("element: '{0}'", e)));

      Assert.That(() => actual.Take(10).ToArray(), Throws.Exception.With.Message.EqualTo(string.Format("element: '{0}'", first)));
    }

    [Test]
    public void CreateSequenceWithCycleCheck_UsesEqualityComparer ()
    {
      var first = new Element(1, null);
      var second = new Element(2, first);
      var third = new Element(3, second);
      var fourth = new Element(4, third);


      var fakeComparer = new FakeElementEqualityComparer((x, y) => second.Equals(x) || second.Equals(y));

      IEnumerable<Element> actual = fourth.CreateSequenceWithCycleCheck(
          e => e.Parent,
          e => e != null,
          fakeComparer,
          e => new Exception(string.Format("element: '{0}'", e)));

      Assert.That(() => actual.Take(10).ToArray(), Throws.Exception.With.Message.EqualTo(string.Format("element: '{0}'", second)));
    }

    [Test]
    public void SetEquals_True ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 1, 2, 3 };
      Assert.That(first.SetEquals(second), Is.True);
    }

    [Test]
    public void SetEquals_True_Empty ()
    {
      IEnumerable<int> first = Enumerable.Empty<int>();
      IEnumerable<int> second = Enumerable.Empty<int>();
      Assert.That(first.SetEquals(second), Is.True);
    }

    [Test]
    public void SetEquals_True_DifferentOrder ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 3, 1, 2 };
      Assert.That(first.SetEquals(second), Is.True);
    }

    [Test]
    public void SetEquals_DifferentCount ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3, 1, 2, 2 };
      IEnumerable<int> second = new[] { 1, 2, 3 };
      Assert.That(first.SetEquals(second), Is.True);
    }

    [Test]
    public void SetEquals_False_FirstNotInSecond ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<int> second = new[] { 1, 2 };
      Assert.That(first.SetEquals(second), Is.False);
    }

    [Test]
    public void SetEquals_False_SecondNotInFirst ()
    {
      IEnumerable<int> first = new[] { 1 };
      IEnumerable<int> second = new[] { 1, 2 };
      Assert.That(first.SetEquals(second), Is.False);
    }

#if NETFRAMEWORK
    [Test]
    public void Zip_Tuples ()
    {
      IEnumerable<int> first = new[] { 1, 2, 3 };
      IEnumerable<string> second = new[] { "a", "b" };

      var result = first.Zip(second);

      Assert.That(result, Is.EqualTo(new[] { Tuple.Create(1, "a"), Tuple.Create(2, "b") }));
    }
#endif

    [Test]
    public void Interleave ()
    {
      IEnumerable<string> first = new[] { "a", "b" };
      IEnumerable<string> second = new[] { "x", "y" };

      var result = first.Interleave(second);

      Assert.That(result, Is.EqualTo(new[] { "a", "x", "b", "y" }));
    }

    [Test]
    public void Interleave_DifferentLength ()
    {
      IEnumerable<string> first = new[] { "a", "b" };
      IEnumerable<string> second = new[] { "x" };

      var result1 = first.Interleave(second).ToArray();
      var result2 = second.Interleave(first);

      Assert.That(result1, Is.EqualTo(new[] { "a", "x", "b" }));
      Assert.That(result2, Is.EqualTo(new[] { "x", "a", "b" }));
    }

    [Test]
    public void ConvertToCollection_WithCollection ()
    {
      var collection = new[] { 1, 2, 3 };

      ICollection<int> result = collection.ConvertToCollection();

      Assert.That(result, Is.SameAs(collection));
    }

    [Test]
    public void ConvertToCollection_WithNonCollection ()
    {
      var enumerable = Enumerable.Range(1, 3).Reverse();

      ICollection<int> result = enumerable.ConvertToCollection();

      Assert.That(result, Is.Not.SameAs(enumerable));
      Assert.That(result, Is.EqualTo(enumerable));
    }

    [Test]
    public void Concat_WithSingleElement ()
    {
      var result = Enumerable.Range(1, 3).Concat(4);
      Assert.That(result, Is.EqualTo(new[] { 1, 2, 3, 4 }));
    }

    [Test]
    public void Concat_WithSingleElement_NullItem ()
    {
      var result = new[] { "test" }.Concat((string?)null);
      Assert.That(result, Is.EqualTo(new[] { "test", null }));
    }

    [Test]
    public void SingleOrDefault_CustomException ()
    {
      var referenceTypeInput = new[] { "a" };
      var valuTypeInput = new[] { 1 };

      var referenceTypeResult = referenceTypeInput.SingleOrDefault(() => new Exception());
      var valueTypeResult = valuTypeInput.SingleOrDefault(() => new Exception());

      Assert.That(referenceTypeResult, Is.EqualTo("a"));
      Assert.That(valueTypeResult, Is.EqualTo(1));
    }

    [Test]
    public void SingleOrDefault_CustomException_Empty ()
    {
      var result = new string[0].SingleOrDefault(() => new Exception());

      Assert.That(result, Is.Null);
    }

    [Test]
    public void SingleOrDefault_CustomException_MultipleElements ()
    {
      var input = new[] { "a", "b" };
      Assert.That(
          () => input.SingleOrDefault(() => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void SingleOrDefault_WithPredicate_CustomException ()
    {
      var input = new[] { 1, 2 };

      var result = input.SingleOrDefault(x => x == 2, () => new Exception());

      Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void SingleOrDefault_WithPredicate_CustomException_Empty ()
    {
      var input = new[] { 1, 2 };

      var result = input.SingleOrDefault(x => false, () => new Exception());

      Assert.That(result, Is.EqualTo(0));
    }

    [Test]
    public void SingleOrDefault_WithPredicate_CustomException_MultipleElements ()
    {
      var input = new[] { 2, 1, 2 };
      Assert.That(
          () => input.SingleOrDefault(x => x == 2, () => new ApplicationException("ExpectedText")),
          Throws.InstanceOf<ApplicationException>()
              .With.Message.EqualTo("ExpectedText"));
    }

    [Test]
    public void ApplySideEffect ()
    {
      var sourceSequence = new[] { 1, 2, 3 };

      int sum = 0;
      var resultSequence = sourceSequence.ApplySideEffect(i => sum += i);

      using (var enumerator = resultSequence.GetEnumerator())
      {
        Assert.That(sum, Is.EqualTo(0));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(sum, Is.EqualTo(1));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(sum, Is.EqualTo(3));

        Assert.That(enumerator.MoveNext(), Is.True);
        Assert.That(sum, Is.EqualTo(6));

        Assert.That(enumerator.MoveNext(), Is.False);
      }
    }

  }
}
