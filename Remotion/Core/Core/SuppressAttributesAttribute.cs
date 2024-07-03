// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Reflection;

namespace Remotion
{
  /// <summary>
  /// When applied to a class, causes certain custom attributes to be filtered out by 
  /// <see cref="M:Remotion.Utilities.AttributeUtility.GetCustomAttributes(System.Type,System.Type,System.Boolean)"/> and 
  /// <see cref="M:Remotion.Utilities.AttributeUtility.IsDefined(System.Reflection.MemberInfo,System.Type,System.Boolean)"/>.
  /// </summary>
  /// <remarks>
  /// All attributes of the given base type or derived from it are filtered unless they are declared on the same level as the 
  /// <see cref="SuppressAttributesAttribute"/>. Consider the following example:
  /// <example>
  /// <code>
  /// [A ("Base")]
  /// [B ("Base")]
  /// [C ("Base")]
  /// class Base
  /// {
  /// }
  /// 
  /// [A ("Derived")]
  /// [B ("Derived")]
  /// [C ("Derived")]
  /// [SuppressAttributes (typeof (A))]
  /// class Derived : Base
  /// {
  /// }
  /// 
  /// [A ("DerivedDerived")]
  /// [B ("DerivedDerived")]
  /// [C ("DerivedDerived")]
  /// class DerivedDerived : Derived
  /// {
  /// }
  /// 
  /// class A : Attribute { ... }
  /// class B : A { ... }
  /// class C : Attribute { ... }
  /// </code>
  /// <para>
  /// In this example, a call to GetCustomAttributes on typeof (DerivedDerived) would yield the following attributes:
  /// [C ("Base"), A ("Derived"), B ("Derived"), C ("Derived"), C ("DerivedDerived")].
  /// </para>
  /// <para>
  /// [A ("Base")] and [A ("DerivedDerived")] are suppressed due
  /// to the <see cref="SuppressAttributesAttribute"/> on Derived's declaration. [B ("Base")] and [B ("DerivedDerived")] are also suppressed, because
  /// they inherit from A, which is suppressed. The C attributes are not suppressed because they are not related to A; [A ("Derived")] and 
  /// [B ("Derived")] are not suppressed because they are declared on the same level as the <see cref="SuppressAttributesAttribute"/>.
  /// </para>
  /// Do not suppress instances of <see cref="SuppressAttributesAttribute"/>.
  /// </example>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
  public class SuppressAttributesAttribute : Attribute
  {
    private readonly Type _attributeBaseType;

    public SuppressAttributesAttribute (Type attributeBaseType)
    {
      _attributeBaseType = attributeBaseType;
    }

    public Type AttributeBaseType
    {
      get { return _attributeBaseType; }
    }

    public bool IsSuppressed (Type attributeType, ICustomAttributeProvider declaringEntity, ICustomAttributeProvider suppressingEntity)
    {
      return AttributeBaseType.IsAssignableFrom(attributeType) && declaringEntity != suppressingEntity;
    }
  }
}
