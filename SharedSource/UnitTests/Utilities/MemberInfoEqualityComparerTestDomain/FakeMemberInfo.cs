// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.MemberInfoEqualityComparerTestDomain
{
  public class FakeMemberInfo : MemberInfo
  {
    private readonly Type _declaringType;
    private readonly int _metadataToken;
    private Module _module;

    public FakeMemberInfo (Type declaringType, int metadataToken, Module module)
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

    public override Type DeclaringType
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