using System;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.Pages;

public class RegistrationPage : Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;
    private readonly string _url = UrlPassportZen + Endpoints.RegistrationPage;
    
    public RegistrationPage(WebDriver driver)
    {
        driver.Url = _url;
    }
    
    public RegistrationPage(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }
    
    public RegistrationPage Open()
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(_url);
        return this;
    }
    
    public bool InputLoginAndPassword(string FirstName, int i, string Surname, string Login, string Password, int? a1)
    {
        string newLogin = Login + i;
        var interLogin = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-login']")));
        interLogin.SendKeys(newLogin);
        //Если логин занят
        Thread.Sleep(2000);
        if (_driver.FindElements(By.XPath("//strong[@class='suggest__status-text error-message'][contains(text(), 'логин занят')]")).Count > 0)
        {
            if (a1 == null)
            {
                Assert.Fail($"Логин {newLogin} занят");
                return true;
            }
            else 
            {
                Console.WriteLine($"Логин {newLogin} занят");
                return false;
            }
        }
        var interName = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-firstname']")));
        interName.SendKeys(FirstName+i);
        var interSurname = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-lastname']")));
        interSurname.SendKeys(Surname+i);
        var interPassword = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-password']")));
        interPassword.SendKeys(Password);
        var interConfirmPassword = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-password_confirm']")));
        interConfirmPassword.SendKeys(Password);
        return true;
    }
    
   
}