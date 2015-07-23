using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.Commands;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides a common interface for classes performing actions on the re-store data structures on the data management level.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Each command has five steps:
  /// <list type="bullet">
  ///   <item>
  ///     <description>
  ///       <see cref="Begin"/> raises all begin event notifications on the associated <see cref="ClientTransaction"/>.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="Begin"/> raises all begin event notifications on the objects involved in the operation.
  ///     </description>
  ///   </item>  
  ///   <item>
  ///     <description>
  ///       <see cref="Perform"/> actually performs the operation (without raising any events).
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="End"/> raises all end event notifications on the objects involved in the operation.
  ///     </description>
  ///   </item>
  ///   <item>
  ///     <description>
  ///       <see cref="End"/> raises all end event notifications on the associated <see cref="ClientTransaction"/>.
  ///     </description>
  ///   </item>
  /// </list>
  /// Not all commands have to implement all the steps, unrequired steps can be left empty.
  /// </para>
  /// <para>
  /// Some commands can be broadened to include other objects also affected by the operation via <see cref="ExpandToAllRelatedObjects"/>. For example,
  /// a relation end point modification command can be extended to include changes to all affected opposite objects via 
  /// <see cref="ExpandToAllRelatedObjects"/>.
  /// </para>
  /// <para>
  /// Some commands throw an exception when they are executed. Check whether the command can be executed via <see cref="GetAllExceptions"/>,
  /// <see cref="DataManagementCommandExtensions.CanExecute"/>, and <see cref="DataManagementCommandExtensions.EnsureCanExecute"/>.
  /// </para>
  /// <para>
  /// Execute commands immediately after retrieving them. Do not change the state of the <see cref="ClientTransaction"/> while holding on to a command,
  /// as this will cause the command to become inconsistent with the <see cref="ClientTransaction"/>. Executing a command that has become inconsistent
  /// can lead to undefined behavior and may destroy transaction consistency.
  /// </para>
  /// </remarks>
  public interface IDataManagementCommand
  {
    /// <summary>
    /// Gets a sequence of exceptions indicating why this command cannot be executed. If the command can be executed, the sequence is empty.
    /// </summary>
    /// <returns>A sequence of exceptions indicating why this command cannot be executed. If the command can be executed, the sequence is empty.</returns>
    /// <remarks>
    /// Implementations should implement this method as efficiently as possible, especially in the case of an empty exception sequence.
    /// If a lengthier calculation is needed, the calculation should be performed in the command's constructor. (Commands are supposed to be executed 
    /// immediately after construction, so the sequence returned by <see cref="GetAllExceptions"/> is not supposed to change anyway.)
    /// </remarks>
    IEnumerable<Exception> GetAllExceptions ();

    /// <summary>
    /// Notifies the client transaction that the operation is about to begin. The operation can be canceled at this point of time if an event 
    /// handler throws an exception.
    /// </summary>
    void Begin ();

    /// <summary>
    /// Actually performs the operation without raising any events.
    /// </summary>
    void Perform ();

    /// <summary>
    /// Raises all end event notifications on the associated <see cref="ClientTransaction"/>. Event handlers should not throw any exceptions at this 
    /// point of time, the operation has already been performed.
    /// </summary>
    void End ();

    /// <summary>
    /// Returns an <see cref="ExpandedCommand"/> that involves changes to all objects affected by this 
    /// <see cref="IDataManagementCommand"/>. If no other objects are involved by the change, that <see cref="ExpandedCommand"/> method contains just 
    /// this <see cref="IDataManagementCommand"/>.
    /// </summary>
    ExpandedCommand ExpandToAllRelatedObjects ();
  }
}