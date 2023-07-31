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
using System.Globalization;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;
using Remotion.Validation.IntegrationTests.TestDomain.ComponentA;
using Remotion.Validation.Results;

namespace Remotion.Validation.IntegrationTests
{
  [TestFixture]
  public class LocalizedValidationMessagesGlobalizationIntegrationTest : IntegrationTestBase
  {
    [Test]
    public void LocalizedValidatorErrorMessages ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "Test";

      var validator = ValidationBuilder.BuildValidator<Person>();

      var result = validator.Validate(person);

      Assert.That(result.IsValid, Is.False);
      Assert.That(
          result.Errors
              .Where(e => e.ValidatedProperties.Count != 0)
              .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
          Is.EquivalentTo(new[] { "FirstName: Enter a value.", "LastName: Enter a valid value." }));
    }

    [Test]
    public void CultureIsSetBeforeValidatorIsCreated_UsesNewCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      using (new CultureScope("de-AT"))
      {
        var validator = ValidationBuilder.BuildValidator<Person>();

        var result = validator.Validate(person);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors
                .Where(e => e.ValidatedProperties.Count != 0)
                .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
            Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
      }
    }

    [Test]
    public void CultureIsSetAfterValidatorIsCreated_UsesNewCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      Assert.That(Thread.CurrentThread.CurrentCulture, Is.EqualTo(CultureInfo.InvariantCulture));

      var validator = ValidationBuilder.BuildValidator<Person>();

      using (new CultureScope("de-AT"))
      {
        var result = validator.Validate(person);

        Assert.That(result.IsValid, Is.False);
        Assert.That(
            result.Errors
                .Where(e => e.ValidatedProperties.Count != 0)
                .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
            Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
      }
    }

    [Test]
    public void CultureIsSetAfterValidationResultIsCreated_UsesPreviousCultureForLocalization ()
    {
      var person = new Person();
      person.FirstName = null;
      person.LastName = "value";

      var validator = ValidationBuilder.BuildValidator<Person>();

      using (new CultureScope("de-AT"))
      {
        var result = validator.Validate(person);

        // ValidationError is immutable.

        using (new CultureScope(""))
        {
          Assert.That(result.IsValid, Is.False);
          Assert.That(
              result.Errors
                  .Where(e => e.ValidatedProperties.Count != 0)
                  .SelectMany(e => e.ValidatedProperties.Select(vp =>$"{vp.Property.Name}: {e.LocalizedValidationMessage}")),
              Is.EquivalentTo(new[] { "FirstName: Geben Sie einen Wert ein." }));
        }
      }
    }

    [Test]
    [Ignore("RM-5906: Build test for each validator type and evaluate message for all supported cultures.")]
    public void LengthValidator_CanBuildFormattedValidationMessage ()
    {

    }
  }
}
