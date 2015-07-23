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
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.UI.Controls
{
  [TypeConverter (typeof (IconInfoConverter))]
  public sealed class IconInfo
  {
    public static IconInfo CreateSpacer (IResourceUrlFactory resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      var infrastructureType = SafeServiceLocator.Current.GetInstance<IInfrastructureResourceUrlFactory>().GetType();

      string url = resourceUrlFactory.CreateResourceUrl (infrastructureType, ResourceType.Image, "Spacer.gif").GetUrl();
      return new IconInfo (url) { AlternateText = "" };
    }

    public static bool ShouldSerialize (IconInfo icon)
    {
      if (icon == null)
        return false;
      else if (string.IsNullOrEmpty (icon.Url)
               && string.IsNullOrEmpty (icon.AlternateText)
               && string.IsNullOrEmpty (icon.ToolTip)
               && icon.Height.IsEmpty
               && icon.Width.IsEmpty)
        return false;
      else
        return true;
    }

    private string _url;
    private string _alternateText;
    private string _toolTip;
    private Unit _width;
    private Unit _height;

    public IconInfo (string url, string alternateText, string toolTip, Unit width, Unit height)
    {
      Url = url;
      AlternateText = alternateText;
      ToolTip = toolTip;
      Width = width;
      Height = height;
    }

    public IconInfo (string url, Unit width, Unit height)
        : this (url, null, null, width, height)
    {
    }

    public IconInfo (string url, string alternateText, string toolTip, string width, string height)
        : this (url, alternateText, toolTip, new Unit (width), new Unit (height))
    {
    }

    public IconInfo (string url, string width, string height)
        : this (url, null, null, width, height)
    {
    }

    public IconInfo (string url)
        : this (url, null, null, Unit.Empty, Unit.Empty)
    {
    }

    public IconInfo ()
        : this (string.Empty)
    {
    }

    [Editor (typeof (ImageUrlEditor), typeof (UITypeEditor))]
    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string Url
    {
      get { return _url; }
      set { _url = value ?? string.Empty; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string AlternateText
    {
      get { return _alternateText; }
      set { _alternateText = value ?? string.Empty; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue ("")]
    [NotifyParentProperty (true)]
    public string ToolTip
    {
      get { return _toolTip; }
      set { _toolTip = value ?? string.Empty; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue (typeof (Unit), "")]
    [NotifyParentProperty (true)]
    public Unit Width
    {
      get { return _width; }
      set { _width = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [DefaultValue (typeof (Unit), "")]
    [NotifyParentProperty (true)]
    public Unit Height
    {
      get { return _height; }
      set { _height = value; }
    }

    public override string ToString ()
    {
      return _url;
    }

    public void Render (HtmlTextWriter writer, IControl container)
    {
      ArgumentUtility.CheckNotNull ("container", container);
      ArgumentUtility.CheckNotNull ("writer", writer);

      string url = container.ResolveClientUrl (_url);
      if (string.IsNullOrEmpty (url))
        throw new InvalidOperationException (string.Format ("Icon url '{0}' resolved to null or empty. Icons without a source are not supported.", _url));

      writer.AddAttribute (HtmlTextWriterAttribute.Src, url);

      AddDimensionToAttributes (writer, HtmlTextWriterAttribute.Width, _width);
      AddDimensionToAttributes (writer, HtmlTextWriterAttribute.Height, _height);

      writer.AddAttribute ("class", "Icon");

      writer.AddAttribute (HtmlTextWriterAttribute.Alt, _alternateText ?? string.Empty);

      if (! string.IsNullOrEmpty (_toolTip))
        writer.AddAttribute (HtmlTextWriterAttribute.Title, _toolTip);

      writer.RenderBeginTag (HtmlTextWriterTag.Img);
      writer.RenderEndTag();
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public bool HasRenderingInformation
    {
      get { return ! string.IsNullOrEmpty (_url); }
    }

    public void Reset ()
    {
      _url = string.Empty;
      _alternateText = string.Empty;
      _toolTip = string.Empty;
      _width = Unit.Empty;
      _height = Unit.Empty;
    }

    public void LoadResources (IResourceManager resourceManager)
    {
      if (resourceManager == null)
        return;

      string key;
      key = ResourceManagerUtility.GetGlobalResourceKey (Url);
      if (! string.IsNullOrEmpty (key))
        Url = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (AlternateText);
      if (! string.IsNullOrEmpty (key))
        AlternateText = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (ToolTip);
      if (! string.IsNullOrEmpty (key))
        ToolTip = resourceManager.GetString (key);
    }

    private void AddDimensionToAttributes (HtmlTextWriter writer, HtmlTextWriterAttribute attribute, Unit attributeValue)
    {
      if (attributeValue.IsEmpty)
        return;

      writer.AddStyleAttribute (GetDimensionAsStyleAttribute (attribute), attributeValue.ToString());
    }

    private HtmlTextWriterStyle GetDimensionAsStyleAttribute (HtmlTextWriterAttribute attribute)
    {
      switch (attribute)
      {
        case HtmlTextWriterAttribute.Height:
          return HtmlTextWriterStyle.Height;
        case HtmlTextWriterAttribute.Width:
          return HtmlTextWriterStyle.Width;
        default:
          throw new InvalidOperationException ("Invalid value for attribute. Only Height and Width are supported.");
      }
    }
  }

  public class IconInfoConverter : ExpandableObjectConverter
  {
    public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
    {
      if (context == null // Requried to circumvent the Designer
          && sourceType == typeof (string))
        return true;
      return base.CanConvertFrom (context, sourceType);
    }

    public override bool CanConvertTo (ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof (string))
        return true;
      return base.CanConvertTo (context, destinationType);
    }

    public override object ConvertFrom
        (ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      if (value == null)
        return null;
      if (value is string)
      {
        string stringValue = (string) value;
        IconInfo icon = new IconInfo();
        if (stringValue != string.Empty)
        {
          string[] valueParts = stringValue.Split (new char[] { '\0' }, 5);
          icon.Url = valueParts[0];
          if (valueParts[1] != string.Empty)
            icon.Width = Unit.Parse (valueParts[1]);
          if (valueParts[2] != string.Empty)
            icon.Height = Unit.Parse (valueParts[2]);
          icon.AlternateText = valueParts[3];
          icon.ToolTip = valueParts[4];
        }
        return icon;
      }

      return base.ConvertFrom (context, culture, value);
    }

    public override object ConvertTo
        (ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof (string))
      {
        if (value == null)
          return null;
        if (value is IconInfo)
        {
          IconInfo icon = (IconInfo) value;
          if (context == null) // Requried to circumvent the Designer
          {
            if (IconInfo.ShouldSerialize (icon))
            {
              StringBuilder serializedValue = new StringBuilder();
              serializedValue.Append (icon.Url).Append ("\0");
              serializedValue.Append (icon.Width.ToString()).Append ("\0");
              serializedValue.Append (icon.Height.ToString()).Append ("\0");
              serializedValue.Append (icon.AlternateText).Append ("\0");
              serializedValue.Append (icon.ToolTip);
              return serializedValue.ToString();
            }
            else
              return string.Empty;
          }
          else
            return icon.Url;
        }
      }
      return base.ConvertTo (context, culture, value, destinationType);
    }
  }
}