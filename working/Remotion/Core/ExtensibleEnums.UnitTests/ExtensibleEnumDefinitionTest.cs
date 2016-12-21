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
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.ExtensibleEnums.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumDefinitionTest
  {
    private Color _red;
    private Color _green;
    private Color _blue;
    private MethodInfo _fakeMethod;

    [SetUp]
    public void SetUp ()
    {
      _red = new Color ("Red");
      _green = new Color ("Green");
      _blue = new Color ("Blue");
      _fakeMethod = typeof (ColorExtensions).GetMethod ("Red");
    }

    [Test]
    public void GetEnumType ()
    {
      var definition = CreateDefinition (_red, _green);
      Assert.That (definition.GetEnumType (), Is.SameAs (typeof (Color)));
    }

    [Test]
    public void IsDefined_String_True ()
    {
      var definition = CreateDefinition (_red, _green);
      Assert.That (definition.IsDefined (_red.ID), Is.True);
    }

    [Test]
    public void IsDefined_String_False ()
    {
      var definition = CreateDefinition (_green);
      Assert.That (definition.IsDefined (_red.ID), Is.False);
    }

    [Test]
    public void IsDefined_Value_True ()
    {
      var definition = CreateDefinition (_red, _green);
      Assert.That (definition.IsDefined (_red), Is.True);
    }

    [Test]
    public void IsDefined_Value_False_ID ()
    {
      var definition = CreateDefinition (_green);
      Assert.That (definition.IsDefined (_red), Is.False);
    }

    [Test]
    public void IsDefined_Value_False_Type ()
    {
      var definition = CreateDefinition (_red);
      
      var valueWithWrongType = new EnumWithDifferentCtors (_red.ID);
      Assert.That (valueWithWrongType.ID, Is.EqualTo (_red.ID));
      
      Assert.That (definition.IsDefined (valueWithWrongType), Is.False);
    }

    [Test]
    public void GetValueInfos ()
    {
      var infos = GetInfos (_red, _green, _blue).ToArray();
      var definition = CreateDefinition<Color> (infos);
      var valueInstances = definition.GetValueInfos().ToArray();

      Assert.That (valueInstances, Is.EquivalentTo (infos));
    }

    [Test]
    public void GetValueInfos_CachesValues ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService>();
      valueDiscoveryServiceMock
          .Expect (mock => mock.GetValueInfos (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything))
          .Return (GetInfos (_red))
          .Repeat.Once();
      valueDiscoveryServiceMock.Replay();

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      var values1 = extensibleEnumDefinition.GetValueInfos();
      var values2 = extensibleEnumDefinition.GetValueInfos();

      valueDiscoveryServiceMock.VerifyAllExpectations();
      Assert.That (values1, Is.SameAs (values2));
    }

    [Test]
    public void GetValueInfos_PassesExtensibleEnumDefinitionInstance_ToExtensibleEnumValueDiscoveryService ()
    {
      var valueDiscoveryServiceMock = MockRepository.GenerateMock<IExtensibleEnumValueDiscoveryService>();
      var infos = GetInfos (_red);
      valueDiscoveryServiceMock.Stub (mock => mock.GetValueInfos (Arg<ExtensibleEnumDefinition<Color>>.Is.Anything)).Return (infos);

      var extensibleEnumDefinition = new ExtensibleEnumDefinition<Color> (valueDiscoveryServiceMock);
      extensibleEnumDefinition.GetValueInfos();

      valueDiscoveryServiceMock.AssertWasCalled (mock => mock.GetValueInfos (extensibleEnumDefinition));
    }

    [Test]
    public void GetValueInfos_DefaultOrder_IsAlphabetic ()
    {
      var definition = CreateDefinition (_red, _blue, _green);

      var values = definition.GetValueInfos().Select (info => info.Value).ToArray();

      Assert.That (values, Is.EqualTo (new[] { _blue, _green, _red }));
    }

    [Test]
    public void GetValueInfos_ExplicitOrder_WithKeys ()
    {
      var infos = new[]  { 
          CreateInfo (new Planet ("Earth"), 1.5),
          CreateInfo (new Planet ("Mars"), 2.0),
          CreateInfo (new Planet ("Venus"), 1.0),
      };
      var definition = CreateDefinition<Planet> (infos);

      var values = definition.GetValueInfos ();

      Assert.That (values, Is.EqualTo (new[] { infos[2], infos[0], infos[1] }));
    }

    [Test]
    public void GetValueInfos_ExplicitOrder_SameKeys_DefaultToAlphabetic ()
    {
      var infos = new[]  { 
          CreateInfo (new Planet ("Saturn"), 1.0),
          CreateInfo (new Planet ("Jupiter"), 1.0)
      };
      var definition = CreateDefinition<Planet> (infos);

      var values = definition.GetValueInfos ();

      Assert.That (values, Is.EqualTo (new[] { infos[1], infos[0] }));
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValueInfos_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValueInfos();
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color' does not define any values.")]
    public void GetValueInfos_NoValues ()
    {
      var definition = CreateDefinition<Color>();
      definition.GetValueInfos();
    }

    [Test]
    public void GetValueInfoByID ()
    {
      var definition = CreateDefinition (_red, _green);
      var valueInfo = definition.GetValueInfoByID ("Red");

      Assert.That (valueInfo.Value, Is.EqualTo (_red));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException),
        ExpectedMessage = "The extensible enum type 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color' does not define a value called '?'.")]
    public void GetValueInfoByID_WrongIDThrows ()
    {
      var definition = CreateDefinition (_red, _green);
      definition.GetValueInfoByID ("?");
    }

    [Test]
    [ExpectedException (typeof (InvalidExtensibleEnumDefinitionException), ExpectedMessage =
        "Extensible enum 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color' defines two values with ID 'Red'.")]
    public void GetValueInfoByID_DuplicateIDs ()
    {
      var definition = CreateDefinition (_red, _red);
      definition.GetValueInfoByID ("ID");
    }

    [Test]
    public void TryGetValueInfoByID ()
    {
      var definition = CreateDefinition (_red, _green);

      ExtensibleEnumInfo<Color> result;
      var success = definition.TryGetValueInfoByID ("Red", out result);

      var expected = Color.Values.Red();
      Assert.That (success, Is.True);
      Assert.That (result.Value, Is.EqualTo (expected));
    }

    [Test]
    public void TryGetValueInfoByID_WrongID ()
    {
      var definition = CreateDefinition (_red, _green);

      ExtensibleEnumInfo<Color> result;
      var success = definition.TryGetValueInfoByID ("?", out result);

      Assert.That (success, Is.False);
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetCustomAttributes_NonGeneric ()
    {
      var customAttributes = Color.Values.GetCustomAttributes (typeof (SampleAttribute));

      var expected = typeof (ColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false)
          .Concat (typeof (LightColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false))
          .ToArray();
      Assert.That (customAttributes, Is.EquivalentTo (expected));
      Assert.That (customAttributes, Is.TypeOf (typeof (SampleAttribute[])));
    }

    [Test]
    public void GetCustomAttributes_NonGeneric_Interface ()
    {
      var customAttributes = Color.Values.GetCustomAttributes (typeof (ISampleAttribute));

      var expected = typeof (ColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false)
          .Concat (typeof (LightColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false))
          .ToArray ();
      Assert.That (customAttributes, Is.EquivalentTo (expected));
      Assert.That (customAttributes, Is.TypeOf (typeof (ISampleAttribute[])));
    }

    [Test]
    public void GetCustomAttributes_Generic ()
    {
      var customAttributes = Color.Values.GetCustomAttributes<SampleAttribute>();

      var expected = typeof (ColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false)
          .Concat (typeof (LightColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false))
          .ToArray ();
       Assert.That (customAttributes, Is.EquivalentTo (expected));
    }

    [Test]
    public void GetCustomAttributes_Generic_Interface ()
    {
      var customAttributes = Color.Values.GetCustomAttributes<ISampleAttribute> ();

      var expected = typeof (ColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false)
          .Concat (typeof (LightColorExtensions).GetCustomAttributes (typeof (SampleAttribute), false))
          .ToArray ();
      Assert.That (customAttributes, Is.EquivalentTo (expected));
    }

    [Test]
    public void GetCustomAttributes_EqualAttributes_ExtensionTypesAreFiltered ()
    {
      var customAttributes = ExtensibleEnumWithDuplicateAttribute.Values.GetCustomAttributes<ISampleAttribute> ();

      Assert.That (customAttributes.Length, Is.EqualTo (2));
    }

    [Test]
    public void GetValueInfos_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      ReadOnlyCollection<IExtensibleEnumInfo> valueInfos = ((IExtensibleEnumDefinition) definition).GetValueInfos();
      Assert.That (valueInfos, Is.EqualTo (definition.GetValueInfos()));
    }

    [Test]
    public void GetValueInfoByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnumInfo valueInfo = ((IExtensibleEnumDefinition) definition).GetValueInfoByID ("Red");
      Assert.That (valueInfo, Is.SameAs (definition.GetValueInfoByID ("Red")));
    }

    [Test]
    public void TryGetValueInfoByID_NonGeneric ()
    {
      var definition = CreateDefinition (_red, _green);

      IExtensibleEnumInfo valueInfo;
      bool success = ((IExtensibleEnumDefinition) definition).TryGetValueInfoByID ("Red", out valueInfo);

      ExtensibleEnumInfo<Color> expectedValueInfo;
      bool expectedSuccess = definition.TryGetValueInfoByID ("Red", out expectedValueInfo);

      Assert.That (success, Is.EqualTo (expectedSuccess));
      Assert.That (valueInfo, Is.SameAs (expectedValueInfo));
    }

    private IEnumerable<ExtensibleEnumInfo<T>> GetInfos<T> (params T[] values) 
        where T: ExtensibleEnum<T>
    {
      return values.Select (value => CreateInfo (value, 0.0));
    }

    private ExtensibleEnumInfo<T> CreateInfo<T> (T value,  double positionalKey) 
        where T: ExtensibleEnum<T>
    {
      return new ExtensibleEnumInfo<T> (value, _fakeMethod, positionalKey);
    }

    private ExtensibleEnumDefinition<T> CreateDefinition<T> (params T[] values) 
        where T: ExtensibleEnum<T>
    {
      var infos = GetInfos (values);
      return CreateDefinition (infos);
    }

    private ExtensibleEnumDefinition<T> CreateDefinition<T> (IEnumerable<ExtensibleEnumInfo<T>> infos) 
        where T: ExtensibleEnum<T>
    {
      var valueDiscoveryServiceStub = MockRepository.GenerateStub<IExtensibleEnumValueDiscoveryService>();
      var definition = new ExtensibleEnumDefinition<T> (valueDiscoveryServiceStub);

      valueDiscoveryServiceStub.Stub (stub => stub.GetValueInfos (Arg<ExtensibleEnumDefinition<T>>.Is.Anything)).Return (infos);
      return definition;
    }
  }
}