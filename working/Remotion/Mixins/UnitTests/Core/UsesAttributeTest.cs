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
using NUnit.Framework;
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.FluentBuilders;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  public class UsesAttributeTest
  {
    private readonly Type _userType = typeof (object);

    private MockRepository _mockRepository;
    private MixinConfigurationBuilder _configurationBuilderMock;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository();
      _configurationBuilderMock = _mockRepository.StrictMock<MixinConfigurationBuilder> ((MixinConfiguration) null);
    }

    [Test]
    public void Initialization_Defaults ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      Assert.That (attribute.AdditionalDependencies, Is.Empty);
      Assert.That (attribute.SuppressedMixins, Is.Empty);
      Assert.That (attribute.MixinType, Is.EqualTo (typeof (string)));
      Assert.That (attribute.IntroducedMemberVisibility, Is.EqualTo (MemberVisibility.Private));
    }

    [Test]
    public void IgnoresDuplicates ()
    {
      var attribute = new UsesAttribute (typeof (string));
      Assert.That (attribute.IgnoresDuplicates, Is.False);
    }

    [Test]
    public void Apply ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Apply_AdditionalDependencies ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.AdditionalDependencies = new[] { typeof (int) };

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Apply_SuppressedMixins ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Apply_PrivateVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Private;

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Private,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void Apply_PublicVisibility ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      attribute.SuppressedMixins = new[] { typeof (double) };
      attribute.IntroducedMemberVisibility = MemberVisibility.Public;

      _configurationBuilderMock
          .Expect (
              mock => mock.AddMixinToClass (
                  MixinKind.Used,
                  _userType,
                  typeof (string),
                  MemberVisibility.Public,
                  attribute.AdditionalDependencies,
                  attribute.SuppressedMixins,
                  CreateExpectedOrigin (attribute)))
          .Return (null);

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Text")]
    public void Apply_InvalidOperation ()
    {
      UsesAttribute attribute = new UsesAttribute (typeof (string));
      _configurationBuilderMock
          .Expect (mock => mock.AddMixinToClass (MixinKind.Used, null, null, MemberVisibility.Private, null, null, null))
          .IgnoreArguments()
          .Throw (new InvalidOperationException ("Text"));

      _mockRepository.ReplayAll();
      attribute.Apply (_configurationBuilderMock, _userType);
    }

    private MixinContextOrigin CreateExpectedOrigin (UsesAttribute attribute, Type userType = null)
    {
      return MixinContextOrigin.CreateForCustomAttribute (attribute, userType ?? _userType);
    }
  }
}