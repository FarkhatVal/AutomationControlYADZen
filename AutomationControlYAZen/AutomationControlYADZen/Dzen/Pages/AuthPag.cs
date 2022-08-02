using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using AutomationControlYADZen.Dzen.RefreshBlockChannel;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.Pages;

public class AuthPage : Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;
    private readonly string _url = UrlPassportZen + Endpoints.Auth;
    
    public AuthPage(WebDriver driver)
    {
        driver.Url = _url;
    }
    
    public AuthPage(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }
    
    public AuthPage Open()
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(_url);
        return this;
    }
    
    public async Task<bool> Auth(int? i, string Login, string Password,
         string idNum, string? NewPassword, int? a1)
        {
            _driver.Navigate().GoToUrl(_url);
            Thread.Sleep(1500);
            if (_driver.FindElements(By.XPath("//h2[contains(text(), 'Ваш аккаунт готов!')]")).Count > 0)
            {
                _driver.Navigate().GoToUrl(Page.UrlZen);
                //Console.WriteLine("вошел сразу");
            }
            else if (_driver.FindElements(By.XPath("//button[@aria-label='Меню профиля']")).Count > 0)
            {
                return true;
            }
            else
            {
                if (_driver.FindElements(
                        By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]")).Count > 0)
                {
                    _driver.FindElement(
                        By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]")).Click();
                }
                Thread.Sleep(500); 
        //Если открылось поле ввода пароля другого аккаунта
                if (_driver.FindElements(
                        By.XPath("//span[@class='CurrentAccount-login']")).Count > 0)
                {
                    var quitLogin = _wait.Until(d => d.FindElement(By.XPath("//span[@class='CurrentAccount-login']")));
                    quitLogin.Click();
                    Thread.Sleep(500);
                    var addNewLogin =
                        _wait.Until(d => d.FindElement(By.XPath("//span[@class='AddAccountButton-text']")));
                    addNewLogin.Click();
                }
                Thread.Sleep(500);
        //Если добавлены 2 аккаунта
                if (_driver.FindElements(
                        By.XPath("//span[contains(text(), 'другой аккаунт')]")).Count > 0)
                {
                    var addLogin =
                        _wait.Until(d => d.FindElement(By.XPath("//span[contains(text(), 'другой аккаунт')]")));
                    addLogin.Click();
                }
        //Если при входе открылось поле для ввода телефона, нажимаем на кнопку для перехода в ввод логина
                if (_driver.FindElements(By.XPath("//input[@placeholder='+7 123 456-78-90']")).Count > 0)
                {
                    var inpuLogin1 =
                        _driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_clear']"));
                    inpuLogin1.Click();
                }
                var inputLogin = _wait.Until(d => d.FindElement(By.XPath("//input[@data-t='field:input-login']")));
                inputLogin.SendKeys(Login);
                var inputLoginOk = _driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputLoginOk.Click();
                Thread.Sleep(3000);
                if (_driver.FindElements(
                            By.XPath("//div[@id='field:input-login:hint'][contains(text(), 'Такого аккаунта нет')]"))
                        .Count > 0)
                {
                    if (a1 == null)
                    {
                        Assert.Fail($"Аккаунт {Login} еще не зарегистрирован");
                    }
                    else
                    {
                        Console.WriteLine($"Аккаунт {Login} еще не зарегистрирован");
                    }
                    return false;
                }

        //Если просит подтвердить номер телефона 
                if (_driver.FindElements(By.XPath(
                                "//div[@class='auth-challenge-descr']/*[contains(text(), 'Пожалуйста, подтвердите номер телефона,')]"))
                        .Count > 0)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (_driver.FindElements(
                                By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Count > 0)
                        {
                            _driver.FindElement(By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Click();
                        }

                        var telConfirm1 = _driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                        telConfirm1.Click();
                        string apiTelStatusGetNewSms1 = $"/setStatus/?apiKey={UiTestBase.ApiKey}&status=send&idNum={idNum}";
                        var addressTelStatusGetNewSms1 = new Uri(UiTestBase.Host + apiTelStatusGetNewSms1);
                        var telStatusGetNewSms1 = new HttpClient() { BaseAddress = addressTelStatusGetNewSms1 };
                        var responseTelStatusGetNewSms1 =
                            await telStatusGetNewSms1.GetAsync(addressTelStatusGetNewSms1, new CancellationToken());
                        //Получаем новый СМС
                        Thread.Sleep(30000);
                        UiTestBase.ResponseGetPhoneNomber = await GetSmsCodee.GetTelephoneNomber(UiTestBase.Host, UiTestBase.ApiGetPhoneNomber);
                        string codeNew1 = UiTestBase.ResponseGetSmsCode.SmsCode;
                        //ВВодим СМС код
                        var inputNewSms1 = _driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                        inputNewSms1.SendKeys(codeNew1);
                        var codeConfirm = _driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                        codeConfirm.Click();
                        Thread.Sleep(1000);
                        if (_driver.FindElements(
                                    By.XPath(
                                        "//div[@role ='alert'][contains(text(), 'Неправильный код подтверждения')]"))
                                .Count <
                            0) break;
                    }
                }
                //Thread.Sleep(3000);
                var inputPassword = _driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                inputPassword.SendKeys(Password);
                var inputPasswordOk = _driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputPasswordOk.Click();
                Thread.Sleep(3000);
        //Если просит подтвердить номер при входе
                if (_driver.FindElements(
                        By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = _driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }
                if (_driver.FindElements(
                        By.XPath("//div[@id='field:input-passwd:hint'][contains(text(), 'Неверный пароль')]")).Count >
                    0)
                {
                    if (NewPassword != null)
                    {
                        var inputPassword1 = _driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                        inputPassword1.SendKeys(NewPassword);
                        var inputPasswordOk1 =
                            _driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                        inputPasswordOk1.Click();
                        //Thread.Sleep(1000);
                        if (idNum != null) Console.WriteLine($"\tПароль уже иземнен на {NewPassword}");
                    }
                    else
                    {
                        Assert.Fail($"\tНе верный пароль {Password}");
                    }
                }
        //Если просит ввести капчу
                if (_driver.FindElements(By.XPath("//img[@class='captcha__image']")).Count > 0)
                {
                    await UiTestBase.InputCaptchaCode("//img[@class='captcha__image']");
                    var inputPassword1 = _driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                    inputPassword1.SendKeys("\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b" + NewPassword);
                    var captchaCodeOk = _driver.FindElement(By.XPath(
                        "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit']"));
                    captchaCodeOk.Click();
                }
                Thread.Sleep(3000);
        //Если просит доп. почту
                if (_driver.FindElements(By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = _driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }
        //Если просит ответ на КВ
                if (_driver.FindElements(By.XPath("//input[@data-t='field:input-question']")).Count > 0)
                {
                    var inputKv = _driver.FindElement(By.XPath("//input[@data-t='field:input-question']"));
                    inputKv.SendKeys(UiTestBase.KvAnswer);
                    var kvok = _driver.FindElement(By.XPath(
                        "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit']"));
                    kvok.Click();
                }
        //Если просит подтвердить номер при входе
                if (_driver.FindElements(By.XPath(
                                "//div[@class='auth-challenge-form-hint'][contains(text(), 'Ваш номер телефона:')]"))
                        .Count > 0)
                {
                    if (idNum == null) return false;
                    GetSmsCodee.TelNomberStatusSend(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                    _driver.FindElement(By.XPath("//button[@data-t='button:action']")).Click();
                    //Получаем новый СМС
                    Thread.Sleep(30000);
                    string codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                    if (codeNew == null)
                    {
                        for (int n = 1; n <= 3; n++)
                        {
                            Thread.Sleep(5000);
                            codeNew = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                            Console.WriteLine($"\t\tбыло первых попыток для подтверждения номера {n}");
                            if (codeNew != null) break;
                        }
                    }
                    //ВВодим СМС код
                    var inputNewSms1 = _driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                    inputNewSms1.SendKeys(codeNew);
                    var codeConfirm = _driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                    codeConfirm.Click();
                }
        //Если доступ временно ограничен
                if (_driver.FindElements(
                        By.XPath("//h1[@data-t='title'][contains(text(), 'Доступ временно ограничен')]")).Count > 0)
                {
                    await BasicOperationForRefreshChannel.RefreshChannel(Login, UiTestBase.KvAnswer, null, null);
                }
            }
            return true;
        }
}