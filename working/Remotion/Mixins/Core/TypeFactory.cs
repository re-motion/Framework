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
using System.Runtime.Serialization;
using Remotion.Mixins.CodeGeneration;
using Remotion.ServiceLocation;
using Remotion.TypePipe.Implementation;

namespace Remotion.Mixins
{
  /// <summary>
  /// Provides support for combining mixins and target classes into concrete, "mixed", instantiable types.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When a target class should be combined with mixins, the target class (and sometimes also the mixin types) cannot be instantiated as
  /// is. Instead, a concrete type has to be created which incorporates the necessary delegation code. While the type generation is actually performed
  /// by another class, the <see cref="TypeFactory"/> provides the public API to be used when retrieving a generated type.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> should only be used if <see cref="Type"/> objects are required. If the combined type should be instantiated,
  /// the <see cref="ObjectFactory"/> class should be used instead.
  /// </para>
  /// <para>
  /// The <see cref="TypeFactory"/> class uses the mixin configuration active for the thread on which it is called.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true"/>
  public static class TypeFactory
  {
    // This class holds lazy, readonly static fields. It relies on the fact that the .NET runtime will reliably initialize fields in a nested static
    // class with a static constructor as lazily as possible on first access of the static field.
    // Singleton implementations with nested classes are documented here: http://csharpindepth.com/Articles/General/Singleton.aspx.
    static class LazyStaticFields
    {
      public static readonly ITypeFactoryImplementation TypeFactoryImplementation =
          SafeServiceLocator.Current.GetInstance<ITypeFactoryImplementation> ();

      // ReSharper disable EmptyConstructor
      // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit; this will make the static fields as lazy as possible.
      static LazyStaticFields ()
      {
      }
      // ReSharper restore EmptyConstructor
    }

    /// <summary>
    /// Retrieves a concrete, instantiable, mixed type for the given <paramref name="targetOrConcreteType"/>, or <paramref name="targetOrConcreteType"/> itself if no
    /// mixin configuration exists for the type on the current thread.
    /// </summary>
    /// <param name="targetOrConcreteType">Base type for which a mixed type should be retrieved or a concrete mixed type.</param>
    /// <returns>A concrete, instantiable, mixed type for the given <paramref name="targetOrConcreteType"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="targetOrConcreteType"/> parameter is <see langword="null"/>.</exception>
    /// <exception cref="Exception"><para>The current mixin configuration for the <paramref name="targetOrConcreteType"/> contains severe configuration problems 
    /// that make generation of a target class definition object impossible.</para><para>- or -</para><para>The current mixin configuration for 
    /// the <paramref name="targetOrConcreteType"/> violates at least one validation rule, which makes code generation impossible.</para> </exception>
    /// <remarks>
    /// <para>
    /// The type returned by this method is guaranteed to be derived from <paramref name="targetOrConcreteType"/>, but will usually not be the same as
    /// <paramref name="targetOrConcreteType"/>. It manages integration of the mixins with the given <paramref name="targetOrConcreteType"/>.
    /// </para>
    /// <para>
    /// Note that the factory will not create derived types for types not currently having a mixin configuration. This means that types returned
    /// by the factory can <b>not</b> always be treated as derived types.
    /// </para>
    /// <para>
    /// The returned type provides the same constructors as <paramref name="targetOrConcreteType"/> does and can thus be instantiated, e.g. via
    /// <see cref="Activator.CreateInstance(Type, object[])"/>. When this happens, all the mixins associated with the generated type are also
    /// instantiated and configured to be used with the target instance. If you need to supply pre-created mixin instances for an object, use
    /// a <em>MixedObjectInstantiationScope</em>. See <see cref="ObjectFactory"/> for a simpler way to immediately create instances of mixed types.
    /// </para>
    /// <para>
    /// If <paramref name="targetOrConcreteType"/> is already a generated type, this method will not subclass it again.
    /// </para>
    /// </remarks>
    public static Type GetConcreteType (Type targetOrConcreteType)
    {
      return LazyStaticFields.TypeFactoryImplementation.GetConcreteType (targetOrConcreteType);
    }

    /// <summary>
    /// Initializes a mixin target instance which was created without its constructor having been called.
    /// </summary>
    /// <param name="mixinTarget">The mixin target to initialize.</param>
    /// <param name="initializationSemantics">The semantics to apply during initialization.</param>
    /// <exception cref="ArgumentNullException">The mixin target is <see langword="null"/>.</exception>
    /// <remarks>This method is useful when a mixin target instance is created via <see cref="FormatterServices.GetSafeUninitializedObject"/>.</remarks>
    public static void InitializeUnconstructedInstance (object mixinTarget, InitializationSemantics initializationSemantics)
    {
      LazyStaticFields.TypeFactoryImplementation.InitializeUnconstructedInstance (mixinTarget, initializationSemantics);
    }
  }
}
