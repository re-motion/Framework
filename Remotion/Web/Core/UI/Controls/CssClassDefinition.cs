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
namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Contains all the CSS class definitions needed throughout UI control rendering.
  /// </summary>
  public static class CssClassDefinition
  {
    /// <summary> Gets the CSS-Class applied to an UI control when it is displayed in read-only mode. </summary>
    /// <remarks> 
    ///   <para> Class: <c>readOnly</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.readOnly</c> as a selector. </para>
    /// </remarks>
    public static string ReadOnly
    {
      get { return "readOnly"; }
    }

    /// <summary> Gets the CSS-Class applied to an UI control when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.bocTextValue.disabled</c> as a selector.</para>
    /// </remarks>
    public static string Disabled
    {
      get { return "disabled"; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to an UI control when it should become scrollable.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>scrollable</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public static string Scrollable
    {
      get { return "remotion-scrollable"; } // also change in Utilities.ts class CssClassDefinition
    }

    /// <summary>
    /// Gets the CSS-Class applied to an UI control when itself and child elements
    /// that are standard browser controls (e.g. input elements) should be styled in the current theme.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>remotion-themed</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public static string Themed
    {
      get { return "remotion-themed"; } // also change in Utilities.ts class CssClassDefinition 
    }

    /// <summary>
    /// Gets the CSS-Class applied to a button when it should be styled to indicate a primary action.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>primary</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public static string ButtonTypePrimary
    {
      get { return "primary"; }
    }

    /// <summary>
    /// Gets the CSS-Class applied to a button when it should be styled to indicate a supplemental action.
    /// </summary>
    /// <remarks> 
    ///   <para> Class: <c>supplemental</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class.</para>
    /// </remarks>
    public static string ButtonTypeSupplemental
    {
      get { return "supplemental"; }
    }

    /// <summary> Gets the CSS-Class applied to elements only visible to screen readers. </summary>
    /// <remarks> Class: <c>screenReaderText</c> </remarks>
    public static string ScreenReaderText
    {
      get { return "screenReaderText"; }
    }
  }
}
