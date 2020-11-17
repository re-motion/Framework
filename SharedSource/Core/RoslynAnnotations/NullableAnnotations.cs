// NOTE: This file was taken from the dotnet runtime repository (https://github.com/dotnet/runtime/blob/52d4d8c057d24a77b2cf6708d495cf549a8bee9a/src/libraries/System.Private.CoreLib/src/System/Diagnostics/CodeAnalysis/NullableAttributes.cs)
// and is part of the dotnet/runtime project of the .NET Foundation.
// It has since been modified for use in the re-motion framework development.
//
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the
// License for the specific language governing permissions and limitations
// under the License.
//
// Original license header by dotnet/runtime:
//
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
//
// Original license text by dotnet/runtime:
//
// The MIT License (MIT)
//
// Copyright (c) .NET Foundation and Contributors
//
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

#define INTERNAL_NULLABLE_ATTRIBUTES

#pragma warning disable MA0048 // File name must match type name

// ReSharper disable once CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
  /// <summary>Specifies that null is allowed as an input even if the corresponding type disallows it.</summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
  sealed partial class AllowNullAttribute : Attribute
  {
  }

  /// <summary>Specifies that null is disallowed as an input even if the corresponding type allows it.</summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = false)]
  sealed partial class DisallowNullAttribute : Attribute
  {
  }

  /// <summary>Specifies that an output may be null even if the corresponding type disallows it.</summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
  sealed partial class MaybeNullAttribute : Attribute
  {
  }

  /// <summary>Specifies that an output will not be null even if the corresponding type allows it.</summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
  sealed partial class NotNullAttribute : Attribute
  {
  }

  /// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter may be null even if the corresponding type disallows it.</summary>
  [AttributeUsage (AttributeTargets.Parameter, Inherited = false)]
  sealed partial class MaybeNullWhenAttribute : Attribute
  {
    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter may be null.
    /// </param>
    public MaybeNullWhenAttribute (bool returnValue) => ReturnValue = returnValue;

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }
  }

  /// <summary>Specifies that when a method returns <see cref="ReturnValue"/>, the parameter will not be null even if the corresponding type allows it.</summary>
  [AttributeUsage (AttributeTargets.Parameter, Inherited = false)]
  sealed partial class NotNullWhenAttribute : Attribute
  {
    /// <summary>Initializes the attribute with the specified return value condition.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    public NotNullWhenAttribute (bool returnValue) => ReturnValue = returnValue;

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }
  }

  /// <summary>Specifies that the output will be non-null if the named parameter is non-null.</summary>
  [AttributeUsage (AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
  sealed partial class NotNullIfNotNullAttribute : Attribute
  {
    /// <summary>Initializes the attribute with the associated parameter name.</summary>
    /// <param name="parameterName">
    /// The associated parameter name.  The output will be non-null if the argument to the parameter specified is non-null.
    /// </param>
    public NotNullIfNotNullAttribute (string parameterName) => ParameterName = parameterName;

    /// <summary>Gets the associated parameter name.</summary>
    public string ParameterName { get; }
  }

  /// <summary>Applied to a method that will never return under any circumstance.</summary>
  [AttributeUsage (AttributeTargets.Method, Inherited = false)]
  sealed partial class DoesNotReturnAttribute : Attribute
  {
  }

  /// <summary>Specifies that the method will not return if the associated Boolean parameter is passed the specified value.</summary>
  [AttributeUsage (AttributeTargets.Parameter, Inherited = false)]
  sealed partial class DoesNotReturnIfAttribute : Attribute
  {
    /// <summary>Initializes the attribute with the specified parameter value.</summary>
    /// <param name="parameterValue">
    /// The condition parameter value. Code after the method will be considered unreachable by diagnostics if the argument to
    /// the associated parameter matches this value.
    /// </param>
    public DoesNotReturnIfAttribute (bool parameterValue) => ParameterValue = parameterValue;

    /// <summary>Gets the condition parameter value.</summary>
    public bool ParameterValue { get; }
  }

  /// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values.</summary>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  sealed partial class MemberNotNullAttribute : Attribute
  {
    /// <summary>Initializes the attribute with a field or property member.</summary>
    /// <param name="member">
    /// The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullAttribute (string member) => Members = new[] { member };

    /// <summary>Initializes the attribute with the list of field and property members.</summary>
    /// <param name="members">
    /// The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullAttribute (params string[] members) => Members = members;

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }
  }

  /// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values when returning with the specified return value condition.</summary>
  [AttributeUsage (AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  sealed partial class MemberNotNullWhenAttribute : Attribute
  {
    /// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="member">
    /// The field or property member that is promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute (bool returnValue, string member)
    {
      ReturnValue = returnValue;
      Members = new[] { member };
    }

    /// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
    /// <param name="returnValue">
    /// The return value condition. If the method returns this value, the associated parameter will not be null.
    /// </param>
    /// <param name="members">
    /// The list of field and property members that are promised to be not-null.
    /// </param>
    public MemberNotNullWhenAttribute (bool returnValue, params string[] members)
    {
      ReturnValue = returnValue;
      Members = members;
    }

    /// <summary>Gets the return value condition.</summary>
    public bool ReturnValue { get; }

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }
  }
}