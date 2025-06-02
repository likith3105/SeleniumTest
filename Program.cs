using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Threading;

class CloudQAShadowAndNestedForm
{
    static void Main()
    {
        IWebDriver driver = new ChromeDriver();
        driver.Manage().Window.Maximize();

        try
        {
            driver.Navigate().GoToUrl("https://app.cloudqa.io/home/AutomationPracticeForm");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            // Attempt recursive iframe switching
            SwitchToFormFrame(driver);

            // ✅ Handle standard fields (if found directly)
            TryFillStandardField(driver, "First Name", "Likith");
            TryFillStandardField(driver, "Mobile", "9876543210");
            TryFillStandardField(driver, "Email", "likith@example.com");

            // ✅ Try accessing a Shadow DOM field (sample logic)
            // Note: Adjust shadow host selector as per real HTML (below is generic)
            string js = @"
                const shadowHost = document.querySelector('shadow-host-selector'); // Replace with actual selector
                const shadowRoot = shadowHost.shadowRoot;
                const input = shadowRoot.querySelector('input[type=""text""]'); 
                input.value = 'ShadowInputValue';";
            ((IJavaScriptExecutor)driver).ExecuteScript(js);

            Console.WriteLine("Form interaction complete.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
        finally
        {
            driver.Quit();
        }
    }

    // Recursive iframe switcher
    static void SwitchToFormFrame(IWebDriver driver)
    {
        ReadOnlyCollection<IWebElement> iframes = driver.FindElements(By.TagName("iframe"));
        foreach (var iframe in iframes)
        {
            try
            {
                driver.SwitchTo().Frame(iframe);
                if (driver.FindElements(By.XPath("//*[contains(text(), 'First Name')]")).Count > 0)
                {
                    Console.WriteLine("Switched to correct iframe.");
                    return;
                }
                // Recursive check inside child iframes
                SwitchToFormFrame(driver);
                return;
            }
            catch
            {
                driver.SwitchTo().DefaultContent();
            }
        }
    }

    // Helper to fill input field by label
    static void TryFillStandardField(IWebDriver driver, string labelText, string value)
    {
        try
        {
            var input = driver.FindElement(By.XPath($"//label[contains(text(),'{labelText}')]/following-sibling::input"));
            input.Clear();
            input.SendKeys(value);
            Console.WriteLine($"Filled '{labelText}' with '{value}'.");
        }
        catch
        {
            Console.WriteLine($"Could not locate input for '{labelText}'.");
        }
    }
}
