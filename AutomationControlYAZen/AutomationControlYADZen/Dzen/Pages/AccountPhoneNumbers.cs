using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.Pages;

public class AccountPhoneNumbers: Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;
    private readonly string _url = UrlPassportZen + Endpoints.ChangeTelNumber;
    
    public AccountPhoneNumbers(WebDriver driver)
    {
        driver.Url = _url;
    }
    
    public AccountPhoneNumbers(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }
    
    public AccountPhoneNumbers Open()
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(_url);
        return this;
    }
    public async Task DeleteTelNomber(string Password, string idNum, string NewLogin)
    {
        var telDelet = _wait.Until( d => d.FindElement(By.XPath("//button[@data-t='phones.default-restore-phone']")));
        telDelet.Click();
        var telDelet1 = _wait.Until( d => d.FindElement(By.XPath("//button[@data-t='phones.settings.restore.remove']")));
        telDelet1.Click();
        var telDelet2 = _driver.FindElement(By.XPath("//button[@data-t='phones:button:confirm.phone']"));
        telDelet2.Click();
        // ввод пароля
        var interPassword1 = _wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-password']")));
        interPassword1.SendKeys(Password);
        //ввести код из смс
        //Обновляем статус номера чтобы получить новый смс
        await GetSmsCodee.TelNomberStatusSend(UiTestBase.Host, UiTestBase.ApiKey, idNum);
        //Получаем новый СМС
        Thread.Sleep(5000);
        string codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
        if (codeNew == null)
        {
            for (int i = 1; i <= 3; i++)
            {
                Thread.Sleep(5000);
                codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                Console.WriteLine($"\t\tбыло первых попыток для удаления номера {i}");
                if (codeNew != null) break;
            }
        }
        if (codeNew == null | _driver.FindElements(
                By.XPath("//div[@role='alert'][('код, попробуйте ещё раз')]")).Count > 0)
        {
            Thread.Sleep(21000);
            _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@data-t='phones:button:retry-to-request-code']"))).Click();
            Console.WriteLine($"\t\tповторная отправка кода для удаления номера");
            await GetSmsCodee.TelNomberStatusSend(UiTestBase.Host, UiTestBase.ApiKey, idNum);
            codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
            for (int n = 1; n <= 3; n++)
            {
                Thread.Sleep(10000);
                Console.WriteLine($"\t\t\tбыло вторых попыток для удаления номера {n}");
                codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                //Console.WriteLine($"пароль: {codeNew}");
                if (codeNew != null) break;
            }
        }
        if (codeNew == null)
        {
            Console.WriteLine($"\tне пришли СМС-коды для удаления номера");
        }
        //ВВодим СМС код
        var inputNewSms = _driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
        inputNewSms.SendKeys(codeNew);
        //Thread.Sleep(3000);
        var telDeletOk = _wait.Until( d => d.FindElement(By.XPath("//button[@data-t='phones:button']")));
        telDeletOk.Click();
        Thread.Sleep(3000);
        Console.WriteLine(_driver.FindElements(
                By.XPath(
                    "//div[@class='UniversalTile-title UniversalTile-title_isBigTitle'][contains(text(), 'Добавьте номер для защиты аккаунта')]"))
            .Count > 0
            ? $"\tНомер удален"
            : "\tНомер не удален");
    }
}