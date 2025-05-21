// MIT License
//
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// Copyright (c) 2018 Adrian Longley & Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Coypu.Drivers.Selenium
{
    internal class SeleniumWindowManager
    {
        private readonly IWebDriver _webDriver;
        private IWebDriver _switchedToFrame;
        private IWebElement _switchedToFrameElement;

        public SeleniumWindowManager(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public bool SwitchedToAFrame => _switchedToFrame != null;

        public string LastKnownWindowHandle { get; private set; }

        public IWebDriver SwitchToFrame(IWebElement webElement)
        {
            if (Equals(_switchedToFrameElement, webElement))
                return _switchedToFrame;

            var frame = _webDriver.SwitchTo()
                                  .Frame(webElement);

            _switchedToFrameElement = webElement;
            _switchedToFrame = frame;

            return frame;
        }

        public void SwitchToWindow(string windowName)
        {
            if (LastKnownWindowHandle != windowName || SwitchedToAFrame)
            {
                _webDriver.SwitchTo().Window(windowName);

                // Fix for https://bugzilla.mozilla.org/show_bug.cgi?id=1305822
                if (_webDriver is FirefoxDriver)
                {
                    _webDriver.SwitchTo().DefaultContent();
                }

                LastKnownWindowHandle = windowName;
            }

            _switchedToFrame = null;
            _switchedToFrameElement = null;
        }
    }
}