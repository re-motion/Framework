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
type Dictionary<T> = { [key: string]: T | undefined };
type NonReadonly<T> = { -readonly [P in keyof T]: T[P]; };
type Nullable<T> = T | null;
type Optional<T> = T | undefined;

type AnyFunction = (...args: any[]) => any;
type Constructor<T> = { new(...args: any[]): T };

type EventHandler<TArgs> = (args: TArgs) => void;

type Result<T, E> = Success<T> | Failure<E>;
type Failure<E> = { success: false, error: E };
type Success<T> = { success: true, value: T };

type NotNullNorUndefined = string | number | boolean | symbol | bigint | object;
type NotNull = NotNullNorUndefined | undefined;
type NotUndefined = NotNullNorUndefined | null;

type CssSelectorOrElement<TElement extends Element> = string | TElement;
type CssSelectorOrElements<TElement extends Element> = string | TElement | TElement[];

type DeepPartial<T> = {
  [P in keyof T]?: DeepPartial<T[P]>;
};
type PartialWithRequiredProperties<T, K extends keyof T> = Omit<T, Exclude<keyof T, K>> & Partial<Pick<T, Exclude<keyof T, K>>>;

type OutBox<T> = { Value: Nullable<T> };

// StyleUtility
declare var StyleUtility:
{
  IsNovaGray?: boolean;
};

// Enhance the ASP.NET typings
declare namespace Sys.WebForms
{ 
  type PostBackSettings = {
    async: boolean;
    asyncTarget?: Nullable<string>;
    panelsToUpdate?: Nullable<string[]>;
    sourceElement?: HTMLElement;
  };

  interface PageRequestManagerInternals
  {
    _scriptDisposes: { [key: string]: string };
    _elementContains: (container: HTMLElement, target: HTMLElement) => boolean;
  }

  interface PageRequestManagerInternalPrototype
  {
    _updatePanel: (this: Sys.WebForms.PageRequestManagerInternals, updatePanelElement: HTMLElement, rendering: string) => void;
  }

  type DoPostBack = (eventTarget: string, eventArgument: string) => void;
}

interface Window
{
  __doPostBack: Sys.WebForms.DoPostBack;
}

interface HTMLFormElement
{
  __EVENTTARGET: HTMLInputElement;
  __EVENTARGUMENT: HTMLInputElement;
}
