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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Remotion.UnitTests;

/// <summary>
/// Tests that certain C# language features are supported at compile time/at runtime in .NET Framework.
/// </summary>
[TestFixture]
public class LanguageFeaturesTest
{
  private record struct MyRecordStruct (int A);

  [Test]
  public void RecordStruct ()
  {
    // C# 10 Record structs
    var recordStruct = new MyRecordStruct(13);

    Assert.That(recordStruct.A, Is.EqualTo(13));
    Assert.That(recordStruct, Is.EqualTo(new MyRecordStruct(13)));
    Assert.That(recordStruct.ToString(), Is.EqualTo("MyRecordStruct { A = 13 }"));
  }


  [AttributeUsage(AttributeTargets.Class)]
  private class MyAttr<T> : Attribute;

  [MyAttr<int>]
  private class AttrTarget;

  [Test]
  public void GenericAttribute ()
  {
    // C# 11 Generic Attributes
    // -> don't work under .NET Framework

    var attributes = typeof(AttrTarget).GetCustomAttributes().ToArray();

    Assert.That(attributes.Length, Is.EqualTo(1));
    Assert.That(attributes[0].GetType(), Is.EqualTo(typeof(MyAttr<int>)));
  }


  [Test]
  public void RawStringLiterals ()
  {
    // C# 11 Raw string literals
    var value = """
                my super
                  cool raw literal
                """;

    Assert.That(
        value,
        Is.EqualTo(
            @"my super
  cool raw literal"));
  }


  // C# 11 Required members: Compile-time test to ensure all the required attributes are available
  // ReSharper disable once UnusedType.Local
  private record MyRecordWithRequiredInitOnlyMember
  {
    public required string Name { get; init; }

    public required int Age { get; init; }

    [SetsRequiredMembers]
    public MyRecordWithRequiredInitOnlyMember (string name)
    {
      Name = name;
    }
  }

  // C# 12 Primary constructor: Compile test to ensure primary constructors can be used
  private readonly struct StructWithPrimaryConstructor (string name)
  {
    public readonly string Name = name;
  }

  // ReSharper disable once UnusedType.Local
  private class ClassWithPrimaryConstructor (string name)
  {
    public string Name { get; } = name;
  }

  [Test]
  public void CollectionExpressions ()
  {
    // C# 12 collection expressions
    int[] array = [1, 2, 3];
    var list = new List<int> { 5, 6, 7 };

    int[] result = [..array, 4, ..list];
    Assert.That(result, Is.EqualTo((int[])[1, 2, 3, 4, 5, 6, 7]));
  }
}
