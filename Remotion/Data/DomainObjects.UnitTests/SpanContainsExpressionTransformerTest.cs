// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.UnitTests.Linq.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests;

public class SpanContainsExpressionTransformerTest : IntegrationTestBase
{
  // Asserts that the minimum lang version is set to C# 14 by using null-conditional assigment.
  // This is a tripwire as the tests in this fixture only make sense with C# 14 enabled.
  // ReSharper disable once UnusedMember.Local
  private static void MinLangVersion14Assertion (Ceo ceo) => ceo?.Name = "34";

  /// <summary>
  /// Verifies the implementation assertion that equality comparer are not supported in <c>Contains</c>.
  /// By using an explicit <c>.AsEnumerable()</c>, the implicit span cast is prevented.
  /// </summary>
  [Test]
  public void Assertion_ExplicitEnumerableCaseContainsWithEqualityComparer_ThrowsNotSupportedException ()
  {
    var ceos = new[] { "asd" };
    Assert.That(
        () => QueryFactory.CreateLinqQuery<Company>()
            .Where(e => ceos.AsEnumerable().Contains(e.Name, StringComparer.OrdinalIgnoreCase))
            .ToArray(),
        Throws.TypeOf<NotSupportedException>()
            .With.Message.EqualTo(
                "There was an error generating SQL for the query 'from Company e in DomainObjectQueryable<Company>"
                + " where value(System.String[]).Contains([e].Name, value(System.OrdinalIgnoreCaseComparer)) select [e]'."
                + " The method 'System.Linq.Enumerable.Contains' is not supported by this code generator, and no custom"
                + " transformer has been registered. Expression: '[asd] AS Arg0.Contains([t0].[Name] AS Arg1,"
                + " value(System.OrdinalIgnoreCaseComparer) AS Arg2)'"));
  }

  [Test]
  public void Contains_WithNonStringType_IsTransformedAndWorks ()
  {
    var ceos = new[] { Guid.Empty };
    var result = QueryFactory.CreateLinqQuery<Company>()
        // ReSharper disable once CSharp14OverloadResolutionWithSpanBreakingChange
        .Where(e => ceos.Contains((Guid)e.ID.Value))
        .ToArray();

    Assert.That(result, Is.Not.Null);
  }

  /// <remarks>
  /// Explicit test for string as that might use a different code path than other types.
  /// According to the docs <see cref="MemoryExtensions"/>.<see cref="MemoryExtensions.AsSpan(string)"/>
  /// is used for string conversions.
  /// It does not seem like this is the case now, but this test ensure that we catch the problem if it changes in the future.
  /// </remarks>
  [Test]
  public void Contains_WithStringType_IsTransformedAndWorks ()
  {
    var ceos = new[] { "asd" };
    var result = QueryFactory.CreateLinqQuery<Company>()
        // ReSharper disable once CSharp14OverloadResolutionWithSpanBreakingChange
        .Where(e => ceos.Contains(e.Name))
        .ToArray();

    Assert.That(result, Is.Not.Null);
  }

  [Test]
  public void Contains_WithNullEqualityComparer ()
  {
    var ceos = new[] { "asd" };
    var result = QueryFactory.CreateLinqQuery<Company>()
        // ReSharper disable once CSharp14OverloadResolutionWithSpanBreakingChange
        .Where(e => ceos.Contains(e.Name, null))
        .ToArray();

    Assert.That(result, Is.Not.Null);
  }

  [Test]
  public void Contains_WithEqualityComparer_IsTransformedAndThrowsNotSupportedException ()
  {
    var ceos = new[] { "asd" };
    Assert.That(
        () => QueryFactory.CreateLinqQuery<Company>()
            // ReSharper disable once CSharp14OverloadResolutionWithSpanBreakingChange
            .Where(e => ceos.Contains(e.Name, StringComparer.OrdinalIgnoreCase))
            .ToArray(),
        Throws.TypeOf<NotSupportedException>()
            .With.Message.EqualTo(
                "There was an error generating SQL for the query 'from Company e in DomainObjectQueryable<Company>"
                + " where value(System.String[]).Contains([e].Name, value(System.OrdinalIgnoreCaseComparer)) select [e]'."
                + " The method 'System.Linq.Enumerable.Contains' is not supported by this code generator, and no custom"
                + " transformer has been registered. Expression: '[asd] AS Arg0.Contains([t0].[Name] AS Arg1,"
                + " value(System.OrdinalIgnoreCaseComparer) AS Arg2)'"));
  }
}
