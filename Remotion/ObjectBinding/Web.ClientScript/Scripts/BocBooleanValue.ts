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

//  BocBooleanValue.js contains client side scripts used by BocBooleanValue.

class BocBooleanValue_Resource
{
  constructor(
    private readonly _trueValue: string,
    private readonly _falseValue: string,
    private readonly _nullValue: string,
    private readonly _trueDescription: string,
    private readonly _falseDescription: string,
    private readonly _nullDescription: string,
    private readonly _trueIconUrl: string,
    private readonly _falseIconUrl: string,
    private readonly _nullIconUrl: string,
    private readonly _trueHoverIconUrl: string,
    private readonly _falseHoverIconUrl: string,
    private readonly _nullHoverIconUrl: string) 
  {
  }

  public SelectNextCheckboxValue (
    checkboxSpan: HTMLElement,
    icon: HTMLImageElement,
    label: Nullable<HTMLElement>,
    hiddenField: HTMLInputElement,
    isRequired: boolean,
    trueDescription: Nullable<string>,
    falseDescription: Nullable<string>,
    nullDescription: Nullable<string>) 
  {
    const trueValue = this._trueValue;
    const falseValue = this._falseValue;
    const nullValue = this._nullValue;

    const oldValue = hiddenField.value;
    let newValue;

    //  Select the next value.
    //  true -> false -> null -> true
    if (isRequired) {
        if (oldValue == falseValue)
            newValue = trueValue;
        else if (oldValue == nullValue)
            newValue = trueValue;
        else
            newValue = falseValue;
    }
    else {
        if (oldValue == falseValue)
            newValue = nullValue;
        else if (oldValue == nullValue)
            newValue = trueValue;
        else
            newValue = falseValue;
    }

    // Update the controls
    hiddenField.value = newValue;
    let checkedState: Optional<string>;
    let iconSrc: Optional<string>;
    let iconHoverSrc: Optional<string>;
    let description: Optional<string>;

    if (newValue == falseValue) {
        checkedState = 'false';
        iconSrc = this._falseIconUrl;
        iconHoverSrc = this._falseHoverIconUrl;
        if (falseDescription == null)
            description = this._falseDescription;
        else
            description = falseDescription;
    }
    else if (newValue == nullValue) {
        checkedState = 'mixed';
        iconSrc = this._nullIconUrl;
        iconHoverSrc = this._nullHoverIconUrl;
        if (nullDescription == null)
            description = this._nullDescription;
        else
            description = nullDescription;
    }
    else if (newValue == trueValue) {
        checkedState = 'true';
        iconSrc = this._trueIconUrl;
        iconHoverSrc = this._trueHoverIconUrl;
        if (trueDescription == null)
            description = this._trueDescription;
        else
            description = trueDescription;
    } // RM-7676: Handle invalid check box states in BocBooleanValue_Resource.SelectNextCheckboxValue


    checkboxSpan.setAttribute('aria-checked', checkedState!);

    const isHover = icon.src == icon.dataset['srcHover'];
    icon.dataset["src"] = iconSrc!;
    icon.dataset['srcHover'] = iconHoverSrc!;
    icon.src = isHover ? icon.dataset["src"] : icon.dataset['srcHover'];

    if (label == null)
    {
      checkboxSpan.title = StringUtility.GetPlainTextFromHtml(description!);
    }
    else
    {
      label.innerHTML = description!;
    }
    hiddenField.dispatchEvent(new Event('change'))
  }
}

class BocBooleanValue
{
  private static _bocBooleanValue_Resources: Dictionary<BocBooleanValue_Resource> = {};

  public static InitializeGlobals(
      key: string,
      trueValue: string, 
      falseValue: string, 
      nullValue: string, 
      trueDescription: string,
      falseDescription: string,
      nullDescription: string,
      trueIconUrl: string, 
      falseIconUrl: string, 
      nullIconUrl: string,
      trueHoverIconUrl: string,
      falseHoverIconUrl: string,
      nullHoverIconUrl: string): void
  {
    BocBooleanValue._bocBooleanValue_Resources[key] = new BocBooleanValue_Resource(
        trueValue,
        falseValue,
        nullValue,
        trueDescription,
        falseDescription,
        nullDescription,
        trueIconUrl,
        falseIconUrl,
        nullIconUrl,
        trueHoverIconUrl,
        falseHoverIconUrl,
        nullHoverIconUrl);
  }

  // Selected the next value of the tri-state checkbox, skipping the null value if isRequired is true.
  // checkboxSpan: The span tag representing the clickable area.
  // icon: The icon representing the tri-state checkbox.
  // label: The label containing the description for the value. null for no description.
  // hiddenField: The hidden input field used to store the value between postbacks.
  // isRequired: true to enqable the null value, false to limit the choices to true and false.
  public static SelectNextCheckboxValue (
    key: string,
    checkboxSpan: HTMLElement,
    icon: HTMLImageElement,
    label: Nullable<HTMLElement>,
    hiddenField: HTMLInputElement,
    isRequired: boolean,
    trueDescription: Nullable<string>,
    falseDescription: Nullable<string>,
    nullDescription: Nullable<string>): void
  {
    const resource = BocBooleanValue._bocBooleanValue_Resources[key]!;
    resource.SelectNextCheckboxValue(
    checkboxSpan,
    icon,
    label,
    hiddenField,
    isRequired,
    trueDescription,
    falseDescription,
    nullDescription);
  }

  public static OnKeyDown (context: HTMLElement): void
  {
    // TODO RM-7677: Pass event objects into the handler methods instead of using the ambient event variable
    function typeOverride <T>(value: unknown): asserts value is T {};
    typeOverride<KeyboardEvent>(event);

    if (event.keyCode == 32)
    {
      context.parentElement!.click();
      event.cancelBubble = true;
      event.returnValue = false;
    }
  }

  public static OnMouseOver (context: HTMLElement): void
  {
    const img = context.querySelector('img')!;
    img.src = img.dataset['srcHover']!;
  }

  public static OnMouseOut (context: HTMLElement): void
  {
    const img = context.querySelector('img')!;
    img.src = img.dataset["src"]!;
  }
}
