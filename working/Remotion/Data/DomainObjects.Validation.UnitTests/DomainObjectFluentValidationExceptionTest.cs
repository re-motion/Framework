using System;
using System.Linq;
using FluentValidation.Results;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.UnitTests.Testdomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.Validation.UnitTests
{
  [TestFixture]
  public class DomainObjectFluentValidationExceptionTest
  {
    [Test]
    public void Serialization ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var domainObjects = new[] { TestDomainObject.NewObject (), TestDomainObject.NewObject () };
        var inner = new InvalidOperationException ("Test");
        var validationFailures = new[] { new ValidationFailure ("Test1", "Error1"), new ValidationFailure ("Test1", "Error1") };
        var exception = new DomainObjectFluentValidationException (domainObjects, validationFailures, "Msg", inner);

        var deserializedException = Serializer.SerializeAndDeserialize (exception);

        Assert.That (deserializedException.AffectedObjects, Is.Not.Null);
        Assert.That (deserializedException.AffectedObjects.Select (d => d.ID), Is.EqualTo (domainObjects.Select (d => d.ID)));

        Assert.That (deserializedException.ValidationFailures, Is.Not.Null);
        Assert.That (
            deserializedException.ValidationFailures.Select (v => v.ErrorMessage),
            Is.EqualTo (validationFailures.Select (v => v.ErrorMessage)));

        Assert.That (deserializedException.Message, Is.Not.Null);
        Assert.That (deserializedException.Message, Is.EqualTo ("Msg"));

        Assert.That (deserializedException.InnerException, Is.Not.Null);
        Assert.That (deserializedException.InnerException, Is.TypeOf (typeof (InvalidOperationException)));
      }
    } 
  }
}