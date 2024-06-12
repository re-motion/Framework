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
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Security.UnitTests.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests
{
  [TestFixture]
  public class SecurityContextTest
  {
    [Test]
    public void CreateSecurityContextWithAbstractRole ()
    {
      Enum[] abstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles(abstractRoles);

      Assert.That(
          context.AbstractRoles,
          Is.EquivalentTo(new[] { EnumWrapper.Get(TestAbstractRoles.QualityEngineer), EnumWrapper.Get(TestAbstractRoles.Developer) }));
    }

    [Test]
    public void CreateSecurityContextWithInvalidAbstractRole ()
    {
      // SimpleEnum does not have AbstractRoleAttribute
      Enum[] abstractRoles = new Enum[] { SimpleEnum.First };
      Assert.That(
          () => CreateTestSecurityContextWithAbstractRoles(abstractRoles),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Enumerated Type 'Remotion.Security.UnitTests.SampleDomain.SimpleEnum' cannot be used as an abstract role. "
                  + "Valid abstract roles must have the Remotion.Security.AbstractRoleAttribute applied.", "abstractRoles"));
    }

    [Test]
    public void CreateSecurityContextWithoutAbstractRoles ()
    {
      SecurityContext context = CreateTestSecurityContextWithAbstractRoles(new Enum[0]);
      Assert.That(context.AbstractRoles, Is.Empty);
    }

    [Test]
    public void CreateSecurityContextWithState ()
    {
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum>();
      testStates.Add("Confidentiality", TestSecurityState.Public);
      testStates.Add("State", TestSecurityState.Secret);

      SecurityContext context = CreateTestSecurityContextWithStates(testStates);

      Assert.That(context.GetState("Confidentiality"), Is.EqualTo(EnumWrapper.Get(TestSecurityState.Public)));
      Assert.That(context.GetState("State"), Is.EqualTo(EnumWrapper.Get(TestSecurityState.Secret)));
    }

    [Test]
    public void CreateSecurityContextWithInvalidState ()
    {
      // SimpleEnum does not have SecurityStateAttribute
      Dictionary<string, Enum> testStates = new Dictionary<string, Enum>();
      testStates.Add("Confidentiality", TestSecurityState.Public);
      testStates.Add("State", SimpleEnum.Second);
      Assert.That(
          () => CreateTestSecurityContextWithStates(testStates),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Enumerated Type 'Remotion.Security.UnitTests.SampleDomain.SimpleEnum' cannot be used as a security state. "
                  + "Valid security states must have the Remotion.Security.SecurityStateAttribute applied.", "states"));
    }

    [Test]
    public void CreateSecurityContextWithInvalidType ()
    {
      Assert.That(
          () => CreateTestSecurityContextForType(typeof(SimpleType)),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Parameter 'type' is a 'Remotion.Security.UnitTests.SampleDomain.SimpleType', "
                  + "which cannot be assigned to type 'Remotion.Security.ISecurableObject'.",
                  "type"));
    }

    [Test]
    public void CreateSecurityContextFromEnumWrappers ()
    {
      var abstractRoles = new[] { EnumWrapper.Get(TestAbstractRoles.QualityEngineer), EnumWrapper.Get(SimpleEnum.Second), EnumWrapper.Get("Role") };
      var states = new Dictionary<string, EnumWrapper>();
      states.Add("property1", EnumWrapper.Get(TestSecurityState.Confidential));
      states.Add("property2", EnumWrapper.Get(SimpleEnum.First));
      states.Add("property3", EnumWrapper.Get("State"));

      SecurityContext context = SecurityContext.Create(typeof(ISecurableObject), "owner", "group", "tenant", states, abstractRoles);

      Assert.That(context.Owner, Is.EqualTo("owner"));
      Assert.That(context.OwnerGroup, Is.EqualTo("group"));
      Assert.That(context.OwnerTenant, Is.EqualTo("tenant"));

      Assert.That(context.AbstractRoles, Is.Not.SameAs(abstractRoles));
      Assert.That(context.AbstractRoles, Is.EquivalentTo(abstractRoles));

      Assert.That(context.GetNumberOfStates(), Is.EqualTo(3));
      Assert.That(context.GetState("property1"), Is.EqualTo(states["property1"]));
      Assert.That(context.GetState("property2"), Is.EqualTo(states["property2"]));
      Assert.That(context.GetState("property3"), Is.EqualTo(states["property3"]));
    }

    [Test]
    public void GetClassName ()
    {
      SecurityContext context = CreateTestSecurityContext();
      Assert.That(context.Class, Is.EqualTo("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain"));
    }

    [Test]
    public void IsStateless_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates(states);

      Assert.That(context.IsStateless, Is.False);
    }

    [Test]
    public void IsStateless_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContextWithStates(new Dictionary<string, Enum>());

      Assert.That(context.IsStateless, Is.False);
    }

    [Test]
    public void IsStateless_Stateless ()
    {
      SecurityContext context = CreateStatelessTestSecurityContext();

      Assert.That(context.IsStateless, Is.True);
    }

    [Test]
    public void ContainsState_ContextContainsDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates(states);

      Assert.That(context.ContainsState("Confidentiality"), Is.True);
    }

    [Test]
    public void ContainsState_ContextDoesNotContainDemandedState ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates(states);

      Assert.That(context.ContainsState("State"), Is.False);
    }

    [Test]
    public void GetNumberOfStates_WithStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Confidentiality", TestSecurityState.Public);

      SecurityContext context = CreateTestSecurityContextWithStates(states);

      Assert.That(context.GetNumberOfStates(), Is.EqualTo(1));
    }

    [Test]
    public void GetNumberOfStates_Stateless ()
    {
      SecurityContext context = CreateStatelessTestSecurityContext();

      Assert.That(context.GetNumberOfStates(), Is.EqualTo(0));
    }

    [Test]
    public void GetNumberOfStates_WithoutStates ()
    {
      SecurityContext context = CreateTestSecurityContext();

      Assert.That(context.GetNumberOfStates(), Is.EqualTo(0));
    }


    [Test]
    public void Equals_WithNull ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof(SecurableObject));

      Assert.That(context.Equals(null), Is.False);
    }

    [Test]
    public void Equals_WithSame ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof(File));

      Assert.That(context.Equals(context), Is.True);
    }

    [Test]
    public void Equals_FullyQualified ()
    {
      Dictionary<string, Enum> leftStates = new Dictionary<string, Enum>();
      leftStates.Add("State", TestSecurityState.Public);
      leftStates.Add("Confidentiality", TestSecurityState.Public);
      Enum[] leftAbstractRoles = new Enum[] { TestAbstractRoles.Developer, TestAbstractRoles.QualityEngineer };
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", leftStates, leftAbstractRoles);

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum>();
      rightStates.Add("Confidentiality", TestSecurityState.Public);
      rightStates.Add("State", TestSecurityState.Public);
      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", rightStates, rightAbstractRoles);

      Assert.That(left.Equals(right), Is.True);
      Assert.That(right.Equals(left), Is.True);
    }

    [Test]
    public void Equals_WithDifferentClasses ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());
      SecurityContext right = SecurityContext.Create(
          typeof(PaperFile), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentOwners ()
    {
      SecurityContext left = SecurityContext.Create(
          typeof(File), "owner1", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());
      SecurityContext right = SecurityContext.Create(
          typeof(File), "owner2", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentOwnerGroups ()
    {
      SecurityContext left = SecurityContext.Create(
          typeof(File), "owner", "ownerGroup1", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());
      SecurityContext right = SecurityContext.Create(
          typeof(File), "owner", "ownerGroup2", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentOwnerTenants ()
    {
      SecurityContext left = SecurityContext.Create(
          typeof(File), "owner", "ownerGroup", "ownerTenant1", CreateTwoStates(), CreateTwoAbstractRoles());
      SecurityContext right = SecurityContext.Create(
          typeof(File), "owner", "ownerGroup", "ownerTenant2", CreateTwoStates(), CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentStatePropertyLength ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum>();
      rightStates.Add("Confidentiality", TestSecurityState.Public);
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentStatePropertyNames ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum>();
      rightStates.Add("Confidentiality", TestSecurityState.Public);
      rightStates.Add("State1", TestSecurityState.Public);
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentStatePropertyValues ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum>();
      rightStates.Add("Confidentiality", TestSecurityState.Public);
      rightStates.Add("State", TestSecurityState.Confidential);
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", rightStates, CreateTwoAbstractRoles());

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithDifferentAbstractRoleLength ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer };
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), rightAbstractRoles);

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }


    [Test]
    public void Equals_WithDifferentAbstractRoles ()
    {
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), CreateTwoAbstractRoles());

      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Manager };
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", CreateTwoStates(), rightAbstractRoles);

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void Equals_WithStatefulAndStateless ()
    {
      SecurityContext left = SecurityContext.CreateStateless(typeof(File));
      SecurityContext right = SecurityContext.Create(typeof(File), null, null, null, new Dictionary<string, Enum>(), new Enum[0]);

      Assert.That(left.Equals(right), Is.False);
      Assert.That(right.Equals(left), Is.False);
    }

    [Test]
    public void EqualsObject_WithEqual ()
    {
      SecurityContext left = SecurityContext.CreateStateless(typeof(SecurableObject));
      SecurityContext right = SecurityContext.CreateStateless(typeof(SecurableObject));

      Assert.That(left.Equals((object)right), Is.True);
    }

    [Test]
    public void EqualsObject_WithNull ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof(SecurableObject));

      Assert.That(context.Equals((object)null), Is.False);
    }

    [Test]
    public void EqualsObject_WithObject ()
    {
      SecurityContext context = SecurityContext.CreateStateless(typeof(SecurableObject));

      Assert.That(context.Equals(new object()), Is.False);
    }

    [Test]
    public void TestGetHashCode ()
    {
      Dictionary<string, Enum> leftStates = new Dictionary<string, Enum>();
      leftStates.Add("State", TestSecurityState.Public);
      leftStates.Add("Confidentiality", TestSecurityState.Public);
      Enum[] leftAbstractRoles = new Enum[] { TestAbstractRoles.Developer, TestAbstractRoles.QualityEngineer };
      SecurityContext left = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", leftStates, leftAbstractRoles);

      Dictionary<string, Enum> rightStates = new Dictionary<string, Enum>();
      rightStates.Add("Confidentiality", TestSecurityState.Public);
      rightStates.Add("State", TestSecurityState.Public);
      Enum[] rightAbstractRoles = new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
      SecurityContext right = SecurityContext.Create(typeof(File), "owner", "ownerGroup", "ownerTenant", rightStates, rightAbstractRoles);

      Assert.That(right.GetHashCode(), Is.EqualTo(left.GetHashCode()));
    }

    private static Dictionary<string, Enum> CreateTwoStates ()
    {
      Dictionary<string, Enum> states = new Dictionary<string, Enum>();
      states.Add("Confidentiality", TestSecurityState.Public);
      states.Add("State", TestSecurityState.Secret);
      return states;
    }

    private static Enum[] CreateTwoAbstractRoles ()
    {
      return new Enum[] { TestAbstractRoles.QualityEngineer, TestAbstractRoles.Developer };
    }

    private SecurityContext CreateTestSecurityContextForType (Type type)
    {
      return CreateTestSecurityContext(type, new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateTestSecurityContextWithStates (IDictionary<string, Enum> states)
    {
      return CreateTestSecurityContext(states, new Enum[0]);
    }

    private SecurityContext CreateTestSecurityContextWithAbstractRoles (ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext(new Dictionary<string, Enum>(), abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext ()
    {
      return CreateTestSecurityContext(new Dictionary<string, Enum>(), new Enum[0]);
    }

    private SecurityContext CreateTestSecurityContext (IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return CreateTestSecurityContext(typeof(File), states, abstractRoles);
    }

    private SecurityContext CreateTestSecurityContext (Type type, IDictionary<string, Enum> states, ICollection<Enum> abstractRoles)
    {
      return SecurityContext.Create(type, "owner", "group", "tenant", states, abstractRoles);
    }

    private SecurityContext CreateStatelessTestSecurityContext ()
    {
      return SecurityContext.CreateStateless(typeof(File));
    }
  }
}
