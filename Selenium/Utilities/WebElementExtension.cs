﻿using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Runtime.Remoting;
using System.Windows.Forms;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using OpenQA.Selenium.Interactions;

namespace Selenium
{
    public static class WebElementExtensions
    {
        public static void ClickCustom(this IWebElement element, IWebDriver driver, bool js = false)
        {
            try
            {
                element.ScreenBusy(driver);
                element.ElementToBeClickable(driver);
                element.HighlightElement(driver);
                if (!js)
                    element.Click();
                else
                    JavaScriptExecutor(JSOperator.Click, element);
                element.ScreenBusy(driver);
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver))
                    element.ClickCustom(driver);
                else
                {
                    Console.WriteLine($"[Root Cause] : While Performing Click On [{element.GetLocator()}]");
                    throw new Exception(e.Message);
                }
            }

        }

        public static void ClearAndPaste(this IWebElement element, string text, IWebDriver driver)
        {
            try
            {
                ScreenBusy(driver);
                element.HighlightElement(driver);
                element.ClearManual();
                Thread.Sleep(100);
                element.PasteFromClipboard(text);
                ScreenBusy(driver);
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver))
                    element.ClearAndPaste(text, driver);
                else
                {
                    Console.WriteLine($"[Root Cause] : While Performing Clear Paste On [{element.GetLocator()}]");
                    throw new Exception(e.Message);
                }
                    
            }
        }

        public static void SendKeysWrapper(this IWebElement element, string text, IWebDriver driver)
        {
            try
            {
                ScreenBusy(driver);
                element.HighlightElement(driver);
                element.Clear();
                Thread.Sleep(50);
                element.SendKeys(text);
                ScreenBusy(driver);
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver)) element.SendKeysWrapper(text, driver);
                else
                {
                    Console.WriteLine($"[Root Cause] : While Sending Text On [{element.GetLocator()}]");
                    throw new Exception(e.Message);
                }                    
            }
        }

        public static void SelectDropDown(this IWebElement element, IWebDriver driver,string option, bool js = false)
        {
            int count = 0;
            try
            {
                ScreenBusy(driver);
                Thread.Sleep(20);
                bool selected = false;                
                if (!js)
                {
                    for (int i = 0; i < 25; i++)
                    {
                        count = i;
                        element.ClickCustom(driver);
                        var options = element.FindElements(By.TagName("option"));
                        Wait((d => d.FindElements(By.TagName("option")).Count() > 0), driver, 1);
                        count = i;
                        foreach (var a in options)
                        {
                            if (a.Text.Trim() == option)
                            {
                                a.Click();
                                selected = true;
                                return;
                            }
                        }
                        Thread.Sleep(3000);
                    }
                }
                else
                {
                    Thread.Sleep(3000);
                    JavaScriptExecutor(string.Format(JSOperator.DropDown, option), element);
                }
                
                if (!selected)
                    throw new Exception($"[Root Cause] : Unable to Secting DropDown Option [{option}] On [{element.GetLocator()}] ## [Retry Count] : {count }");

            }
            catch (Exception e)
            {
                if (HandlePopUp(driver)) element.SelectDropDown(driver, option);
                else
                {
                    Console.WriteLine($"[Root Cause] : Unable to Secting DropDown Option [{option}] On [{element.GetLocator()}]");
                    throw new Exception(e.Message);
                }   
            }            
        }

        public static void SelectByIndex(this IWebElement element, IWebDriver driver,int index=1)
        {
            int count = 0;
            try
            {
                ScreenBusy(driver);
                bool selected = false;
                Thread.Sleep(20);
                IList<IWebElement> options = null;                
                Wait((d => d.FindElements(By.TagName("option")).Count() > 1), driver, 3);
                for (int i = 0; i < 25; i++)
                {
                    options = element.FindElements(By.TagName("option"));
                    count = i;
                    element.ClickCustom(driver);
                    if (options.Count() > 1)
                    {                        
                        options[index].Click();
                        selected = true;
                        break;
                    }
                    Thread.Sleep(3000);
                }                
                if (!selected)
                    throw new Exception($"[Root Cause] : Unable to Secting DropDown Index [{index}] On [{element.GetLocator()}] ## [Retry Count] : {count }");
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver)) element.SelectByIndex(driver, index);
                else
                {
                    Console.WriteLine($"[Root Cause] : Unable to Secting DropDown Index [{index}] On [{element.GetLocator()}]");
                    throw new Exception(e.Message);
                }
            }            

        }

        public static void ScreenBusy(this IWebElement element,IWebDriver driver, int timeout = 120)
        {
            try {
                Thread.Sleep(100);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//div[@class='modal-backdrop fade in']")));
                Thread.Sleep(50);
            }
            catch { }
            
        }

        public static void ScreenBusy(IWebDriver driver, int timeout = 120)
        {
            try {
                Thread.Sleep(100);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.XPath("//div[@class='modal-backdrop fade in']")));
            }
            catch { }
        }

        public static By GetLocator(this IWebElement element)
        {
            Thread.Sleep(20);
            var elementProxy = RemotingServices.GetRealProxy(element);
            var bysFromElement = (IReadOnlyList<object>)elementProxy
                .GetType()
                .GetProperty("Bys", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?
                .GetValue(elementProxy);
            return (By)bysFromElement[0];
        }

        public static By GetLocator(this IList<IWebElement> element)
        {
            var elementProxy = RemotingServices.GetRealProxy(element);
            var bysFromElement = (IReadOnlyList<object>)elementProxy
                .GetType()
                .GetProperty("Bys", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?
                .GetValue(elementProxy);
            return (By)bysFromElement[0];
        }        

        public static string GetText(this IWebElement element,IWebDriver driver)
        {
            try
            {
                ScreenBusy(driver);
                return element.Text;
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver)) element.GetText(driver);
                else
                {
                    Console.WriteLine("[Locator] :" + element.GetLocator());
                    throw new Exception($"[Error] : Unable To GetText From  element [{element}] was unsuccessfull");
                }
                return null;
            }

        }

        public static List<string> GetText(this IList<IWebElement> element,IWebDriver driver)
        {
            List<string> list=null;
            try
            {
                Thread.Sleep(50);
                ScreenBusy(driver);
                Thread.Sleep(50);
                list = element.Select(e => e.Text).ToList();
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver))
                    element.GetText(driver);
                else
                    throw new Exception("[Error] : While  Handling Popup & [Message] :" + e.Message);
            }
            return list;
        }

        public static void SendTextAndSelect(this IWebElement element, string text, IWebDriver driver,bool js = false)
        {
            try
            {
                ScreenBusy(driver);
                element.HighlightElement(driver);
                if (!js)
                {
                    element.Click();
                    element.ClearManual();
                    Thread.Sleep(100);
                    for (int i = 0; i < text.Length; i++)
                        element.SendKeys(text[i].ToString());
                }
                else
                {
                    JavaScriptExecutor(JSOperator.Clear, element);
                    JavaScriptExecutor(string.Format(JSOperator.SetValue, text), element);
                }
            }
            catch (Exception e)
            {
                if (HandlePopUp(driver))
                    element.ClearAndPaste(text, driver);
                else
                    throw new Exception("[Error] : While Sending Text & [Message] : [" + e.Message + "]");
            }
        }        

        public static void ClearManual(this IWebElement element)
        {
            element.SendKeys(OpenQA.Selenium.Keys.Control + "a");
            element.SendKeys(OpenQA.Selenium.Keys.Delete);
        }

        public static void PasteFromClipboard(this IWebElement element, string textToCopy)
        {
            Thread thread = new Thread(() => Clipboard.SetText(textToCopy));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();
            element.SendKeys(OpenQA.Selenium.Keys.Control + "v");
            var a = new Thread(() => Clipboard.Clear());
            a.SetApartmentState(ApartmentState.STA);
            a.Start();
            a.Join();
        }

        public static IWebDriver GetWrappedDriver(this IWebElement element)
        {
            IWebDriver instance = null;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Thread.Sleep(100);
                    instance = (element as IWrapsDriver ?? (IWrapsDriver)((IWrapsElement)element).WrappedElement).WrappedDriver;
                    break;
                }
                catch { continue; }
            }
            if (instance != null)
                return instance;
            else
                throw new Exception("[Info : WebDriver instace is not created]");
        }

        public static void ScrollElement(this IWebElement element, IWebDriver driver)
        {
            try {
                Thread.Sleep(20);
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                Thread.Sleep(20);
            } catch { }
            
        }        

        public static bool HandlePopUp(IWebDriver driver)
        {
            try
            {
                string xpath = "//button[contains(text(),'Yes') or contains(text(),'OK')]";
                Thread.Sleep(300);
                ScreenBusy(driver,30);
                var e = driver.FindElement(By.XPath(xpath));
                Wait(ExpectedConditions.ElementToBeClickable(e),driver,5);
                e.ClickCustom(driver);
                Thread.Sleep(100);
                Console.WriteLine("************* Handling Un Expected Popup *************");
                Console.WriteLine("             [Performed] : Click On 'OK/Yes'          ");
                Console.WriteLine("******************************************************");
                ScreenBusy(driver,30);
                return true;
            }
            catch
            {
                return false;
            }

        }
        
        private static void JavaScriptExecutor(string pattern, IWebElement element)
        {
            var js = element.GetWrappedDriver() as IJavaScriptExecutor;
            js.ExecuteScript(pattern, element);
        }

        public static void Wait<TResult>(Func<IWebDriver, TResult> condition, IWebDriver driver, int seconds = 20)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));
            wait.PollingInterval = TimeSpan.FromMilliseconds(500);
            wait.IgnoreExceptionTypes(typeof(Exception));
            wait.Until(condition);
        }

        public static void ElementToBeClickable(this IWebElement element, IWebDriver driver,int timeOut =10)
        {
            try
            {
                Wait(ExpectedConditions.ElementToBeClickable(element), driver, timeOut);
            }
            catch { Console.WriteLine($"    Element Not Clickable [Locator] : {element.GetLocator()}"); }

        }

        public static void HighlightElement(this IWebElement element,IWebDriver driver)
        {
            for (int i = 0; i < 2; i++)
            {
                Thread.Sleep(30);
                (driver as IJavaScriptExecutor).ExecuteScript("arguments[0].setAttribute('style',arguments[1]);", element, "border: 3px solid blue;");
                (driver as IJavaScriptExecutor).ExecuteScript("arguments[0].setAttribute('style',arguments[1]);", element, "border: 0px solid blue;");
            }

        }
        
        private static class JSOperator
        {
            public static string Click { get { return "arguments[0].click();"; } }
            public static string Clear { get { return "arguments[0].value = '';"; } }
            public static string SetValue { get { return "arguments[0].value = '{0}';"; } }
            public static string IsDisplayed { get { return "if(parseInt(arguments[0].offsetHeight) > 0 && parseInt(arguments[0].offsetWidth) > 0) return true; return false;"; } }
            public static string ValidateAttribute { get { return "return arguments[0].getAttribute('{0}');"; } }
            public static string ScrollToElement { get { return "arguments[0].scrollIntoView(true);"; } }
            public static string DropDown { get { return "var length = arguments[0].options.length;  for (var i=0; i<length; i++){{  if (arguments[0].options[i].text == '{0}'){{ arguments[0].selectedIndex = i; break; }} }}"; } }

        }

    }
}
