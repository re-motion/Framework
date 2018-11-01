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
using Remotion.TypePipe;

namespace Remotion.Mixins
{
  /// <summary>
  /// Acts as a convenience base class for domain objects in the mixin-based composition pattern. Provides a <see cref="This"/> property that 
  /// allows access to the composed interface, and a <see cref="NewObject{TComposite}"/> factory method for subclasses.
  /// </summary>
  /// <typeparam name="TComposedInterface">The composed interface of the derived class. This interface defines the members available via
  /// the <see cref="This"/> property. See the Remarks section for details. Each composed interface can only be associated with one single subclass
  /// of <see cref="ComposedObject{TComposedInterface}"/>.
  /// </typeparam>
  /// <remarks>
  /// <para>
  /// When a class inherits members provided by mixins, those additional members are only available by casting the class instance to the respective 
  /// interfaces introduced by the mixins. This can be cumbersome, so the concept of composed interfaces was added. 
  /// A composed interface combines public members of the
  /// mixed class with members introduced by mixins. The target class members are added to the composed interface either by having the
  /// composed interface extend an interface also implemented by the target class, or by simply redeclaring the members of the target class on the
  /// composed interface. The mixin members are added to the composed interface by having the composed interface extend the interfaces 
  /// introduced by the mixins. For an example, see the documentation for the <see cref="ComposedInterfaceAttribute"/>.
  /// </para>
  /// <para>
  /// While composed interfaces provide easy access to the members added by mixins, they still require one cast (from the target class instance to
  /// the composed interface). To remove the need for this cast, classes implementing mixin-based composition can use the 
  /// <see cref="ComposedObject{TComposedInterface}"/> base class. When a class derives from <see cref="ComposedObject{TComposedInterface}"/>, 
  /// it defines a composed interface for itself and all
  /// the mixins it composes via the <see cref="UsesAttribute"/>. That composed interface is called the <typeparamref name="TComposedInterface"/>.
  /// </para>
  /// <para>
  /// The <see cref="ComposedObject{TComposedInterface}"/> base class associates the <typeparamref name="TComposedInterface"/> with the derived class 
  /// and provides a <see cref="This"/> property allowing access to all the members provided by the class and the composed mixins. Use the
  /// <see cref="This"/> property as the full public API of the class.
  /// </para>
  /// <para>
  /// The base class checks that the derived class is always instantiated by the mixin engine, and it will throw an exception from its constructor 
  /// if this is not the case.
  /// </para>
  /// </remarks>
  public abstract class ComposedObject<TComposedInterface> : IHasComposedInterface<TComposedInterface>
      where TComposedInterface: class
  {
    /// <summary>
    /// Used to create instances of the class derived from <see cref="ComposedObject{TComposedInterface}"/>.
    /// </summary>
    /// <typeparam name="TComposite">The type of the composite domain object to create. This must be a subclass of 
    /// <see cref="ComposedObject{TComposedInterface}"/>.</typeparam>
    /// <param name="ctorArgs">The constructor arguments.</param>
    /// <returns>An instance of <typeparamref name="TComposite"/>, accessed via the <typeparamref name="TComposedInterface"/>.</returns>
    protected static TComposedInterface NewObject<TComposite> (ParamList ctorArgs)
        where TComposite: ComposedObject<TComposedInterface>
    {
      return ObjectFactory.Create<TComposite> (ctorArgs).This;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComposedObject{TComposedInterface}"/> class and checks that the instance was created by
    /// the mixin engine.
    /// </summary>
    /// <exception cref="InvalidOperationException">The instance was not created by the mixin engine. Use the <see cref="NewObject{TComposite}"/>
    /// factory method, the <see cref="ObjectFactory"/>, or the <see cref="TypeFactory"/> to instantiate subclasses of 
    /// <see cref="ComposedObject{TComposedInterface}"/>.</exception>
    protected ComposedObject ()
    {
      if (!(this is TComposedInterface))
      {
        var message = string.Format (
            "Type '{0}' is not associated with the composed interface '{1}'. You should instantiate the class via the ObjectFactory class or the "
            + "NewObject method. If you manually created a mixin configuration, don't forget to add the composed interface.",
            GetType(),
            typeof (TComposedInterface).Name);
        throw new InvalidOperationException (message);
      }
    }

    /// <summary>
    /// Gets this <see cref="ComposedObject{TComposedInterface}"/> instance via the <typeparamref name="TComposedInterface"/> interface type. 
    /// This enables callers to access the members of all composed mixins without explicit casts.
    /// </summary>
    /// <value>This instance, accessed via the <typeparamref name="TComposedInterface"/>.</value>
    public TComposedInterface This
    {
      get { return (TComposedInterface) (object) this; }
    }
  }
}