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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration.IntegrationTests.MixinTypeCodeGeneration
{
  [TestFixture]
  public class PublicWrapperTest : CodeGenerationBaseTest
  {
    [Test]
    public void PublicWrapperGeneratedForProtectedOverrider ()
    {
      Type type = ((IMixinTarget) CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider))).Mixins[0].GetType ();
      MethodInfo wrappedMethod = typeof (MixinWithProtectedOverrider).GetMethod ("VirtualMethod", BindingFlags.NonPublic | BindingFlags.Instance);

      MethodInfo wrapper = GetWrapper (type, wrappedMethod);
      Assert.That (wrapper, Is.Not.Null);
    }

    [Test]
    public void NoPublicWrapperGeneratedForInfrastructureMembers ()
    {
      Type type = ((IMixinTarget) CreateMixedObject<BaseType1> (typeof (MixinWithProtectedOverrider))).Mixins[0].GetType ();
      IEnumerable<MethodInfo> wrappedMethods = 
          from method in type.GetMethods (BindingFlags.Instance | BindingFlags.Public)
          let attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (method, false)
          let declaringType = attribute != null ? attribute.ResolveReferencedMethod ().DeclaringType : null
          let declaringTypeDefinition = declaringType != null && declaringType.IsGenericType ? declaringType.GetGenericTypeDefinition() : declaringType
          where attribute != null && (declaringTypeDefinition == typeof (Mixin<>) || declaringTypeDefinition == typeof (Mixin<,>))
          select method;

      Assert.That (wrappedMethods.ToArray (), Is.Empty);
    }

    private MethodInfo GetWrapper (Type type, MethodInfo wrappedMethod)
    {
      return (from method in type.GetMethods (BindingFlags.Instance | BindingFlags.Public)
              let attribute = AttributeUtility.GetCustomAttribute<GeneratedMethodWrapperAttribute> (method, false)
              where attribute != null && attribute.ResolveReferencedMethod ().Equals (wrappedMethod)
              select method).Single ();
    }
  }
}
