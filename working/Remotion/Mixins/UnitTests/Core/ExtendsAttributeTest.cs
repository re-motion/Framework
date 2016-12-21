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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class ExtendsAttributeTest
  {
    private readonly Type _extenderType = typeof (List<>);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder>((MixinConfiguration) null);
    }

    [Test]
    public void Initialization_Defaults ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (string));
      Assert.That (attribute.AdditionalDependencies, Is.Empty);
      Assert.That (attribute.SuppressedMixins, Is.Empty);
      Assert.That (attribute.TargetType, Is.EqualTo (typeof (string)));
      Assert.That (attribute.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void IgnoresDuplicates ()
    {
      var attribute = new ExtendsAttribute (typeof (string));
      Assert.That (attribute.IgnoresDuplicates, Is.False);
    }

    [Test]
    public void Apply ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  _extenderType,
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Apply_SuppressedMixins ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new[] { typeof (int) };

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  _extenderType,
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Apply_AdditionalDependencies ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.AdditionalDependencies = new[] { typeof (string) };

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  _extenderType,
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Apply_PrivateVisibility ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  _extenderType,
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Apply_PublicVisibility ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  _extenderType,
                  MemberVisibility.Public,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Apply_Generic ()
    {
      ExtendsAttribute attribute = new ExtendsAttribute (typeof (object));
      attribute.SuppressedMixins = new[] { typeof (int) };
      attribute.AdditionalDependencies = new[] { typeof (string) };
      attribute.MixinTypeArguments = new[] { typeof (double) };

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (List<double>),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type"
        + " System.Collections.Generic.List`1 specified 2 generic type argument(s) when 1 argument(s) were expected.")]
    public void Apply_WrongNumberOfGenericArguments ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (double), typeof (string) };

      attribute.Apply (_configurationBuilderMock, typeof (List<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class System.Object applied to mixin type"
        + " System.Object specified 2 generic type argument(s) when 0 argument(s) were expected.")]
    public void Apply_WrongNumberOfGenericArguments_NoneExpected ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (double), typeof (string) };

      attribute.Apply (_configurationBuilderMock, typeof (object));
    }

    [Test]
    public void Apply_GenericArgumentsPossible_NoneGiven ()
    {
      var attribute = new ExtendsAttribute (typeof (object));

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Extending,
                  typeof (object),
                  typeof (List<>),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute, typeof (List<>))))
          .Return (null);

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, typeof (List<>));
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = 
        "The ExtendsAttribute for target class 'System.Object' applied to mixin type 'System.Nullable`1[T]' specified invalid generic type arguments: "
        + "GenericArguments[0], 'System.String', on 'System.Nullable`1[T]' violates the constraint of type 'T'.")]
    public void Apply_WrongKindOfGenericArguments ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (string) };

      attribute.Apply (_configurationBuilderMock, typeof (Nullable<>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = 
        "The ExtendsAttribute for target class 'System.Object' applied to mixin type 'System.Collections.Generic.List`1[System.String]' specified "
        + "generic type arguments, but the mixin type already has type arguments specified.")]
    public void Apply_GenericArgumentsAlreadyGiven ()
    {
      var attribute = new ExtendsAttribute (typeof (object));
      attribute.MixinTypeArguments = new[] { typeof (string) };

      attribute.Apply (_configurationBuilderMock, typeof (List<string>));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Foofa.")]
    public void Apply_InvalidOperation ()
    {
      var attribute = new ExtendsAttribute (typeof (object));

      _configurationBuilderMock
          .Expect (mock => mock.AddMixinToClass (MixinKind.Extending, null, null, MemberVisibility.Private, null, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Foofa."));

      _mockRepository.ReplayAll ();
      attribute.Apply (_configurationBuilderMock, _extenderType);
    }

    private MixinContextOrigin CreateExpectedOrigin (ExtendsAttribute attribute, Type extenderType = null)
    {
      return MixinContextOrigin.CreateForCustomAttribute (attribute, extenderType ?? _extenderType);
    }
  }
}
