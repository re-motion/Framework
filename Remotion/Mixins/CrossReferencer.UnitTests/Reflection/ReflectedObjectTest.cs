// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Text;
using MixinXRef.Reflection;
using MixinXRef.UnitTests.TestDomain;
using NUnit.Framework;

namespace MixinXRef.UnitTests.Reflection
{
  [TestFixture]
  public class ReflectedObjectTest
  {
    [Test]
    public void Constructor_ArgumentException ()
    {
      try
      {
        new ReflectedObject (new ReflectedObject (new object()));
        Assert.Fail ("expected exception not thrown");
      }
      catch (ArgumentException argumentException)
      {
        Assert.That (argumentException.Message, Is.EqualTo ("There is no point in wrapping an instance of 'MixinXRef.Reflection.ReflectedObject'."));
      }
    }

    [Test]
    public void To_CorrespondingType ()
    {
      var reflectedObject = new ReflectedObject ("string");
      String output = reflectedObject.To<String>();

      Assert.That (output, Is.EqualTo ("string"));
    }

    [Test]
    public void To_InvalidCast ()
    {
      var reflectedObject = new ReflectedObject ("string");
      try
      {
        reflectedObject.To<IDisposable>();
        Assert.Fail ("expected exception not thrown");
      }
      catch (InvalidCastException invalidCastException)
      {
        Assert.That (invalidCastException.Message, Is.EqualTo ("Unable to cast object of type 'System.String' to type 'System.IDisposable'."));
      }
    }

    [Test]
    public void CallMethod_ExistingMethod ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      var output = reflectedObject.CallMethod ("IndexOf", 't');

      Assert.That (output.To<int>(), Is.EqualTo (1));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Void methods are not supported.")]
    public void CallMethod_ExistingMethod_Void ()
    {
      // TargetDoSomething has method: public void DoSomething()
      var reflectedObject = new ReflectedObject (new TargetDoSomething());
      var output = reflectedObject.CallMethod ("DoSomething");

      Assert.That (output, Is.Null);
    }

    [Test]
    public void CallMethod_ExistingMethod_WithReflectedObject ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      var output = reflectedObject.CallMethod ("IndexOf", new ReflectedObject ('t'));

      Assert.That (output.To<int>(), Is.EqualTo (1));
    }

    [Test]
    public void CallMethod_NonExistingMethod ()
    {
      var reflectedObject = new ReflectedObject ("stringContent");
      try
      {
        reflectedObject.CallMethod ("nonExistingMethod");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Method 'nonExistingMethod' not found on type 'System.String'."));
      }
    }

    [Test]
    public void GetProperty_ExistingProperty ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = reflectedObject.GetProperty ("Length");

      Assert.That (output.To<int>(), Is.EqualTo (6));
    }

    [Test]
    public void GetProperty_NonExistingProperty ()
    {
      var reflectedObject = new ReflectedObject ("string");

      try
      {
        reflectedObject.GetProperty ("nonExistingProperty");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Method 'get_nonExistingProperty' not found on type 'System.String'."));
      }
    }

    [Test]
    public void IEnumerableFunctionality_OnEnumerableWrappedObject ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = new StringBuilder (6);

      foreach (var reflectedCharacter in reflectedObject)
        output.Append (reflectedCharacter.To<char>());
      Assert.That (output.ToString(), Is.EqualTo ("string"));
    }

    [Test]
    public void IEnumerableFunctionality_OnNonEnumerableWrappedObject ()
    {
      var reflectedObject = new ReflectedObject (42);

      try
      {
        reflectedObject.GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable ()
    {
      var reflectedObject = new ReflectedObject ("string");
      var output = new StringBuilder (6);

      foreach (var character in reflectedObject.AsEnumerable<char>())
        output.Append (character);

      Assert.That (output.ToString(), Is.EqualTo ("string"));
    }

    [Test]
    public void AsEnumerable_NonEnumerable ()
    {
      var reflectedObject = new ReflectedObject (42);

      try
      {
        reflectedObject.AsEnumerable<object>().GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (NotSupportedException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("The reflected object 'System.Int32' is not enumerable."));
      }
    }

    [Test]
    public void AsEnumerable_EnumerableButWrongType ()
    {
      var reflectedObject = new ReflectedObject ("string");

      try
      {
        // 'char' is convertible to 'int'!
        reflectedObject.AsEnumerable<float>().GetEnumerator().MoveNext();
        Assert.Fail ("expected exception not thrown");
      }
      catch (InvalidCastException notSupportedException)
      {
        Assert.That (notSupportedException.Message, Is.EqualTo ("Specified cast is not valid."));
      }
    }

    [Test]
    public void Create_DefaultConstructor ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (int).Assembly, "System.Int32");
      Assert.That (reflectedObject.To<int>(), Is.EqualTo (0));
    }

    [Test]
    public void Create_ConstructorWithArguments ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (string).Assembly, "System.String", 'x', 5);
      var expectedOutput = new String ('x', 5);
      Assert.That (reflectedObject.To<string>(), Is.EqualTo (expectedOutput));
    }

    [Test]
    public void Create_ConstructorWithInvalidArguments ()
    {
      try
      {
        ReflectedObject.Create (typeof (string).Assembly, "System.String", "string constructor is not overloaded with string");
        Assert.Fail ("expected exception not thrown");
      }
      catch (MissingMethodException missingMethodException)
      {
        Assert.That (missingMethodException.Message, Is.EqualTo ("Constructor on type 'System.String' not found."));
      }
    }

    [Test]
    public void Create_ConstructorWithWrappedArguments ()
    {
      var reflectedObject = ReflectedObject.Create (typeof (string).Assembly, "System.String", 'x', new ReflectedObject (5));
      var expectedOutput = new String ('x', 5);
      Assert.That (reflectedObject.To<string>(), Is.EqualTo (expectedOutput));
    }

    [Test]
    public void CallMethod_Static ()
    {
      // public static bool string.IsNullOrEmpty (string aString);
      var output = ReflectedObject.CallMethod (typeof (string), "IsNullOrEmpty", "notEmpty");

      Assert.That (output.To<bool>(), Is.False);
    }

    [Test]
    public void ToString_Test ()
    {
      const string content = "toString() for string";
      var reflectedObject = new ReflectedObject (content);

      Assert.That (reflectedObject.ToString(), Is.EqualTo (content));
    }

    [Test]
    public void Equals_False_Unsymetric ()
    {
      var reflectedObject1 = new ReflectedObject ("string");

      // would be nice but does not follow the equals contract (symmetric)
      // Assert.That (reflectedObject1, Is.EqualTo ("string"));
      Assert.That (reflectedObject1, Is.Not.EqualTo ("string"));
    }

    [Test]
    public void Equals_True_Unwrap ()
    {
      var reflectedObject1 = new ReflectedObject ("string");
      var reflectedObject2 = new ReflectedObject ("string");

      Assert.True(reflectedObject1.Equals(reflectedObject2));
    }

    [Test]
    public void Equals_False ()
    {
      var reflectedObject1 = new ReflectedObject ("string");
      var reflectedObject2 = new ReflectedObject ("anotherString");

      Assert.False (reflectedObject1.Equals (reflectedObject2));
    }

    [Test]
    public void GetHashCode_Same ()
    {
      var reflectedObject1 = new ReflectedObject ("string");
      var reflectedObject2 = new ReflectedObject ("string");

      Assert.That (reflectedObject1.GetHashCode(), Is.EqualTo (reflectedObject2.GetHashCode()));
    }

    [Test]
    public void GetHashCode_Different ()
    {
      var reflectedObject1 = new ReflectedObject ("string");
      var reflectedObject2 = new ReflectedObject ("anotherString");

      Assert.That (reflectedObject1.GetHashCode(), Is.Not.EqualTo (reflectedObject2.GetHashCode()));
    }
  }
}