using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.IE;
using System.Threading;
using System;
using System.Collections.Generic;
namespace MultiThreading
{
    class BrowserStackOptions : DriverOptions
    {
        public BrowserStackOptions(String browser_name, String browser_version)
        {
            this.BrowserName = browser_name;
            this.BrowserVersion = browser_version;
        }
        [Obsolete]
        public override void AddAdditionalCapability(string capabilityName, object capabilityValue)
        {
            this.AddAdditionalOption(capabilityName, capabilityValue);
        }
        public override ICapabilities ToCapabilities()
        {
            IWritableCapabilities capabilities = this.GenerateDesiredCapabilities(true);
            return capabilities.AsReadOnly();
        }
    }
    class MultiThreading
    {
        static void Main(string[] args)
        {
            Dictionary<string, object> cap1 = new Dictionary<string, object>();
            cap1.Add("browserName", "chrome");
            cap1.Add("browserVersion", "103.0");
            cap1.Add("os", "Windows");
            cap1.Add("osVersion", "10");

            //Creating Threads and defining the browser and OS combinations where the test will run
            Thread t1 = new Thread(obj => sampleTestCase(cap1));
            Thread t2 = new Thread(obj => executeTestWithCaps2(cap1));

            //Executing the methods
            t1.Start();

            t1.Join();

        }
        static void sampleTestCase(Dictionary<string, object> cap)
        {
            // Update your credentials
            String BROWSERSTACK_USERNAME = "gt_Ps7Yb8";
            String BROWSERSTACK_ACCESS_KEY = "vKeJXsPUb51ytMjm5w72";
            String BUILD_NAME = "browserstack-build-1";
            cap.Add("userName", BROWSERSTACK_USERNAME);
            cap.Add("accessKey", BROWSERSTACK_ACCESS_KEY);
            cap.Add("buildName", BUILD_NAME);
            String browserVersion = cap.ContainsKey("browserVersion") == true ? (string)cap["browserVersion"] : "";
            var browserstackOptions = new BrowserStackOptions((string)cap["browserName"], browserVersion);
            browserstackOptions.AddAdditionalOption("bstack:options", cap);
            executeTestWithCaps(browserstackOptions);
        }
        //executeTestWithCaps function takes capabilities from 'sampleTestCase' function and executes the test
        static void executeTestWithCaps(DriverOptions capability)
        {
            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub.browserstack.com/wd/hub/"), capability);
            try
            {
                driver.Navigate().GoToUrl("https://www.duckduckgo.com");
                IWebElement query = driver.FindElement(By.Name("q"));
                query.SendKeys("BrowserStack");
                query.Submit();
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                wait.Until(localDriver => localDriver.Title.ToLower().Contains("browserstack"));
                // Setting the status of test as 'passed' or 'failed' based on the condition; if title of the web page starts with 'BrowserStack'
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"passed\", \"reason\": \" Title matched!\"}}");
            }
            catch (WebDriverTimeoutException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Title not matched \"}}");
            }
            Console.WriteLine(driver.Title);
            driver.Quit();
        }
        static void executeTestWithCaps2(DriverOptions capability)
        {
            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub.browserstack.com/wd/hub/"), capability);
            try
            {
                driver.Navigate().GoToUrl("https://www.duckduckgo.com");
                IWebElement query = driver.FindElement(By.Id("logo_homepage_link"));
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                query.Text.Contains("About DuckDuckGo");
            }
            catch (WebDriverTimeoutException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Title not matched \"}}");
            }
            Console.WriteLine(driver.Title);
            driver.Quit();
        }
        static void executeTestWithCaps3(DriverOptions capability)
        {
            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub.browserstack.com/wd/hub/"), capability);
            try
            {
                driver.Navigate().GoToUrl("https://www.duckduckgo.com");
                IWebElement query = driver.FindElement(By.ClassName("badge-link__title"));
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                query.Text.Contains("Tired of being tracked online? ");
            }
            catch (WebDriverTimeoutException)
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"failed\", \"reason\": \" Title not matched \"}}");
            }
            Console.WriteLine(driver.Title);
            driver.Quit();
        }
    }

}
