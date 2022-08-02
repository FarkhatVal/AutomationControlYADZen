using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.Pages;

public class LoginPage: Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;
    private readonly string _url = UrlZen;
    
    public LoginPage(WebDriver driver)
    {
        driver.Url = _url;
    }
    
    public LoginPage(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }
    public LoginPage Open()
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(_url);
        return this;
    }
    public static async Task<bool> Auth(WebDriver driver, WebDriverWait wait, int i, string Login, string Password, string idNum, string? NewPassword, int? a1)
        {
            Thread.Sleep(1500);
            if (driver.FindElements(By.XPath("//h2[contains(text(), 'Ваш аккаунт готов!')]")).Count > 0)
            {
                driver.Navigate().GoToUrl(Page.UrlZen);
                //Console.WriteLine("вошел сразу");
            }
            else if (driver.FindElements(By.XPath("//button[@aria-label='Меню профиля']")).Count > 0)
            {
                return true;
            }
            else
            {
                
                if (driver.FindElements(
                                 By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]"))
                             .Count > 0)
                {
                    driver.FindElement(
                        By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]")).Click();
                }
                else
                {
                    var auth = driver.FindElement(By.XPath("//span[contains(text(), 'Войти')]"));
                    auth.Click();
                }
                Thread.Sleep(1000);
                //Если открылось поле ввода пароля другого аккаунта
                if (driver.FindElements(
                        By.XPath("//span[@class='CurrentAccount-login']")).Count > 0)
                {
                    var quitLogin = wait.Until(d => d.FindElement(By.XPath("//span[@class='CurrentAccount-login']")));
                    quitLogin.Click();
                    Thread.Sleep(1000);
                    var addNewLogin =
                        wait.Until(d => d.FindElement(By.XPath("//span[@class='AddAccountButton-text']")));
                    addNewLogin.Click();
                }
                Thread.Sleep(1000);
                //Если добавлены 2 аккаунта
                if (driver.FindElements(
                        By.XPath("//span[contains(text(), 'другой аккаунт')]")).Count > 0)
                {
                    var addLogin =
                        wait.Until(d => d.FindElement(By.XPath("//span[contains(text(), 'другой аккаунт')]")));
                    addLogin.Click();
                }
                //Если при входе открылось поле для ввода телефона, нажимаем на кнопку для перехода в ввод логина
                if (driver.FindElements(By.XPath("//input[@placeholder='+7 123 456-78-90']")).Count > 0)
                {
                    var inpuLogin1 =
                        driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_clear']"));
                    inpuLogin1.Click();
                }
                var inputLogin = wait.Until(d => d.FindElement(By.XPath("//input[@data-t='field:input-login']")));
                inputLogin.SendKeys(Login);
                var inputLoginOk = driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputLoginOk.Click();
                Thread.Sleep(3000);
                if (driver.FindElements(
                            By.XPath("//div[@id='field:input-login:hint'][contains(text(), 'Такого аккаунта нет')]"))
                        .Count > 0)
                {
                    if (a1 == null)
                    {
                        Assert.Fail($"Аккаунт {Login} еще не зарегистрирован");
                        return true;
                    }
                    else
                    {
                        Console.WriteLine($"Аккаунт {Login} еще не зарегистрирован");
                        return false;
                    }
                }

                //Если просит подтвердить номер телефона 
                if (driver.FindElements(By.XPath(
                                "//div[@class='auth-challenge-descr']/*[contains(text(), 'Пожалуйста, подтвердите номер телефона,')]"))
                        .Count > 0)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (driver.FindElements(
                                By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Count > 0)
                        {
                            driver.FindElement(By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Click();
                        }

                        var telConfirm1 = driver.FindElement(By.XPath("//button[@data-t='button:action']"));
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
                        var inputNewSms1 = driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                        inputNewSms1.SendKeys(codeNew1);
                        var codeConfirm = driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                        codeConfirm.Click();
                        Thread.Sleep(1000);
                        if (driver.FindElements(
                                    By.XPath(
                                        "//div[@role ='alert'][contains(text(), 'Неправильный код подтверждения')]"))
                                .Count <
                            0) break;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }

                //Thread.Sleep(3000);
                var inputPassword = driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                inputPassword.SendKeys(Password);
                var inputPasswordOk = driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputPasswordOk.Click();
                Thread.Sleep(3000);
                //Если просит подтвердить номер при входе
                if (driver.FindElements(
                        By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }

                if (driver.FindElements(
                        By.XPath("//div[@id='field:input-passwd:hint'][contains(text(), 'Неверный пароль')]")).Count >
                    0)
                {
                    if (NewPassword != null)
                    {
                        var inputPassword1 = driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                        inputPassword1.SendKeys(NewPassword);
                        var inputPasswordOk1 =
                            driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                        inputPasswordOk1.Click();
                        //Thread.Sleep(1000);
                        Console.WriteLine($"\tПароль уже иземнен на {NewPassword}");
                    }
                    else
                    {
                        Assert.Fail($"\tНе верный пароль {Password}");
                    }
                }

                //Если просит ввести капчу
                if (driver.FindElements(By.XPath("//img[@class='captcha__image']")).Count > 0)
                {
                    await UiTestBase.InputCaptchaCode("//img[@class='captcha__image']");
                }

                Thread.Sleep(3000);
                //Если просит доп. почту
                if (driver.FindElements(By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }

                //Если просит ответ на КВ
                if (driver.FindElements(By.XPath("//input[@data-t='field:input-question']")).Count > 0)
                {
                    var inputKv = driver.FindElement(By.XPath("//input[@data-t='field:input-question']"));
                    inputKv.SendKeys("Шерлок");
                    var kvok = driver.FindElement(By.XPath(
                        "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit']"));
                    kvok.Click();
                }

                //Если просит подтвердить номер при входе
                if (driver.FindElements(
                            By.XPath(
                                "//div[@class='auth-challenge-form-hint'][contains(text(), 'Ваш номер телефона:')]"))
                        .Count > 0)
                {
                    GetSmsCodee.TelNomberStatusSend(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                    driver.FindElement(By.XPath("//button[@data-t='button:action']")).Click();
                    //Получаем новый СМС
                    Thread.Sleep(30000);
                    string codeNew1 = await GetSmsCodee.GetSmsCode(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                    //ВВодим СМС код
                    var inputNewSms1 = driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                    inputNewSms1.SendKeys(codeNew1);
                    var codeConfirm = driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                    codeConfirm.Click();
                }
            }
            return true;
        }
}