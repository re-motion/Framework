// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Reflection;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain
{
  public class FakeMemberInfo : MemberInfo
  {
    private readonly Type? _declaringType;
    private readonly int _metadataToken;
    private Module _module;

    public FakeMemberInfo (Type? declaringType, int metadataToken, Module module)
    {
      _declaringType = declaringType;
      _metadataToken = metadataToken;
      _module = module;
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override MemberTypes MemberType
    {
      get { throw new NotImplementedException(); }
    }

    public override string Name
    {
      get { throw new NotImplementedException(); }
    }

    public override Type? DeclaringType
    {
      get { return _declaringType; }
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override int MetadataToken
    {
      get { return _metadataToken; }
    }

    public override Module Module
    {
      get { return _module; }
    }
  }
}
