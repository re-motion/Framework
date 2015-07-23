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
function BocReferenceValueBase()
{
}

BocReferenceValueBase._nullIconUrl = null;

BocReferenceValueBase.InitializeGlobals = function (nullIconUrl)
{
  BocReferenceValueBase._nullIconUrl = nullIconUrl;
};

BocReferenceValueBase.UpdateCommand = function (oldCommand, businessObject, iconServiceUrl, iconContext, commandInfo, onFailure)
{
  ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
  ArgumentUtility.CheckTypeIsString('businessObject', businessObject);
  ArgumentUtility.CheckTypeIsString('iconServiceUrl', iconServiceUrl);
  ArgumentUtility.CheckTypeIsObject('iconContext', iconContext);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
  ArgumentUtility.CheckTypeIsFunction('onFailure', onFailure);

  var newCommand = BocReferenceValueBase.CreateCommand(oldCommand, commandInfo, businessObject);

  var oldIcon = oldCommand.find('img');
  var newIcon = BocReferenceValueBase.CreateEmptyIcon(oldIcon, newCommand.prop('title'));
  newCommand.append(newIcon);

  oldCommand.replaceWith(newCommand);
  BocReferenceValueBase.FixLayout(newCommand);

  if (iconServiceUrl != null && iconContext != null)
  {
    var pageRequestManager = Sys.WebForms.PageRequestManager.getInstance();
    if (pageRequestManager.get_isInAsyncPostBack())
      return oldCommand;

    var params = { businessObject: businessObject };
    for (var propertyName in iconContext)
      params[propertyName] = iconContext[propertyName];

    Sys.Net.WebServiceProxy.invoke(
        iconServiceUrl,
        'GetIcon',
        false,
        params,
        function (result, context, methodName)
        {
          BocReferenceValueBase.UpdateIconFromWebService(newCommand, newIcon, result);
          BocReferenceValueBase.FixLayout(newCommand);
        },
        function (err, context, methodName)
        {
          onFailure(err);
          BocReferenceValueBase.ResetCommand(newCommand);
          BocReferenceValueBase.FixLayout(newCommand);
        });
  }

  return newCommand;
};

BocReferenceValueBase.CreateCommand = function (oldCommand, commandInfo, businessObject)
{
  ArgumentUtility.CheckNotNull('oldCommand', oldCommand);
  ArgumentUtility.CheckTypeIsObject('commandInfo', commandInfo);
  ArgumentUtility.CheckTypeIsString('businessObject', businessObject);

  var newCommand = $('<a/>');

  var tempCommandInfo = {};
  if (commandInfo != null)
  {
    tempCommandInfo = jQuery.extend(true, {}, commandInfo);
    if (businessObject == null)
    {
      tempCommandInfo.href = null;
      tempCommandInfo.target = null;
      tempCommandInfo.onClick = null;
      tempCommandInfo.title = null;
    }
    else if (commandInfo.href != null)
    {
      tempCommandInfo.href = String.format(commandInfo.href, businessObject);
    }
  }

  var oldCommandAttributes = oldCommand[0].attributes;

  for (var i = 0; i < oldCommandAttributes.length; i++)
  {
    var value = oldCommandAttributes[i].nodeValue;
    if (value != null && value != '')
      newCommand.attr(oldCommandAttributes[i].nodeName, value);
  }

  for (var property in tempCommandInfo)
  {
    var value = tempCommandInfo[property];
    if (value == null)
      newCommand.removeAttr(property);
    else
      newCommand.attr(property, value);
  }

  newCommand.removeClass('hasIcon');

  return newCommand;
};

BocReferenceValueBase.CreateEmptyIcon = function (oldIcon, title)
{
  ArgumentUtility.CheckNotNull('oldIcon', oldIcon);
  ArgumentUtility.CheckTypeIsString('title', title);

  var newIcon = oldIcon.clone();
  newIcon.attr({ src: BocReferenceValueBase._nullIconUrl, alt: '' });
  newIcon.removeAttr('title');
  if (!StringUtility.IsNullOrEmpty(title))
    newIcon.attr({ title: title });
  newIcon.css({ width: oldIcon.width(), height: oldIcon.height() });

  return newIcon;
};

BocReferenceValueBase.UpdateIconFromWebService = function (command, icon, iconInformation)
{
  ArgumentUtility.CheckNotNull('icon', icon);
  ArgumentUtility.CheckTypeIsObject('iconInformation', iconInformation);

  if (iconInformation == null)
    return;

  icon.attr({ src: iconInformation.Url });

  icon.attr({ alt: '' });
  if (!StringUtility.IsNullOrEmpty(iconInformation.AlternateText))
    icon.attr({ alt: iconInformation.AlternateText });

  if (!StringUtility.IsNullOrEmpty(iconInformation.ToolTip) && StringUtility.IsNullOrEmpty(icon.prop('title')))
    icon.attr({ title: iconInformation.ToolTip });

  icon.css({ width: iconInformation.Width, height: iconInformation.Height });

  command.addClass('hasIcon');
};

BocReferenceValueBase.ResetCommand = function (command)
{
  ArgumentUtility.CheckTypeIsObject('command', command);
  command.removeAttr('href');
  command.removeAttr('onclick');
  command.removeAttr('title');
  command.removeAttr('target');
  command.removeClass('hasIcon');
};

BocReferenceValueBase.FixLayout = function (command)
{
  // IE7 and IE8 cannot detect a change of the '+' CSS selector condition without a forced refresh
  if (BrowserUtility.GetIEVersion() < 9)
  {
    var nextSibling = command.next();
    if (nextSibling.length == 0)
      return;
    nextSibling[0].className = nextSibling[0].className;
  }
};
