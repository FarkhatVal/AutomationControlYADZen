using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.Pages;
using BasicOperations;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen;

public class BasicOperations : UiTestBase
{
    internal static ResponseGetPhoneNomber ResponseGetPhoneNomber;
    private static ResponseGetSmsCode _responseGetSmsCode;
    private TelStatusGetNewSms _telStatusGetNewSms;
    
    public static async Task InputAndConfirmTelNumber(WebDriver Driver, WebDriverWait Wait, long? telNumber, string idNum)
        {
            //ВВодим номер телефона
            var interTel = Wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-phone']")));
            interTel.SendKeys((telNumber-70000000000).ToString());
            Thread.Sleep(2000);
            if (Driver.FindElements(By.XPath("//button[@data-t='button:pseudo']"))
                    .Count > 0)
            {
                var telConfirm = Wait.Until(d => d.FindElement(By.XPath("//button[@data-t='button:pseudo']")));
                telConfirm.Click();
            }
            else
            {
                var telConfirm = Wait.Until(d => d.FindElement(By.XPath("//button[@data-t='button:action']")));
                telConfirm.Click();
            }
            if (Driver.FindElements(By.XPath("//div[@class='error-message'][contains(text(), 'Недопустимый формат номера')]"))
                    .Count > 0)
            {
                Console.WriteLine("Недопустимый формат номера");
            }
            //если ошибка Превышен лимит звонков, попробуйте позж
            Thread.Sleep(1000);
            for (int i = 1; i < 20; i++)
            {
                if (Driver.FindElements(
                        By.XPath("//div[contains(text(), 'Превышен лимит звонков, попробуйте позже')]")).Count > 0)
                {
                    var telConfirm1 = Wait.Until( d => d.FindElement(By.XPath("//button[@data-t='button:pseudo']")));
                    telConfirm1.Click();
                    Thread.Sleep(3000);
                }
                if (Driver.FindElements(
                        By.XPath("//div[@class='error-message'][contains(text(), 'удалось дозвониться')]")).Count > 0)
                {
                    var telConfirm1 = Wait.Until( d => d.FindElement(By.XPath("//button[@data-t='button:pseudo']")));
                    telConfirm1.Click();
                    Thread.Sleep(3000);
                }
            }
            //Если ошибка 'Превышен лимит отправляемых сообщений, попробуйте позже' то отмена номера
            //Thread.Sleep(1000);
            if (Driver.FindElements(By.XPath(
                        "//div[contains(text(), 'Превышен лимит отправляемых сообщений, попробуйте позже')]"))
                    .Count > 0)
                {
                    // Установка статутса номер уже использован, забанен на номер
                    await GetSmsCodee.TelNomberStatusEnd(UiTestBase.Host, UiTestBase.ApiKey, idNum);
                    var interTel2 = Driver.FindElement(By.XPath("//input[@data-t='field:input-phone']"));
                    interTel2.SendKeys("\b\b\b\b\b\b\b\b\b\b");
                    ResponseGetPhoneNomber = await GetSmsCodee.GetTelephoneNomber(UiTestBase.Host, UiTestBase.ApiGetPhoneNomber);
                    long telNumber2 = ResponseGetPhoneNomber.TelNomber;
                    string idNum2 = ResponseGetPhoneNomber.IdNum;
                    interTel.SendKeys((telNumber2 - 70000000000).ToString());
                    Thread.Sleep(2000);
                    if (Driver.FindElements(By.XPath("//button[@data-t='button:pseudo']"))
                            .Count > 0)
                    {
                        var telConfirm = Wait.Until(d => d.FindElement(By.XPath("//button[@data-t='button:pseudo']")));
                        telConfirm.Click();
                    }
                    else
                    {
                        var telConfirm = Wait.Until(d => d.FindElement(By.XPath("//button[@data-t='button:action']")));
                        telConfirm.Click();
                    }
                    Thread.Sleep(2000);
                    if (Driver.FindElements(By.XPath(
                                    "//div[contains(text(), 'Превышен лимит отправляемых сообщений, попробуйте позже')]"))
                            .Count > 0)
                    {
                        // Установка статутса номер уже использован, забанен на номер
                        await GetSmsCodee.TelNomberStatusEnd(UiTestBase.Host, UiTestBase.ApiKey, idNum2);
                        Driver.Close();
                        Driver.Quit();
                        Assert.Fail("Оба номера недоступны. Дальнейшие действия бесполезны");
                    }
                }
        }
     public static async Task GetAndInputSmsCode(WebDriver Driver, WebDriverWait Wait, string idNum, int? i)
        {
            Thread.Sleep(30000);
            for (int x = 0; x <= 2; x++)
            {
                int retryRequestCodeCount = 0;
                if (Driver.FindElements(
                        By.XPath("//span[@class='registration__form-text'][contains(text(), 'вам позвоним')]")).Count >
                    0)
                {
                    Driver.FindElement(By.XPath(
                            "//*[@class='registration__pseudo-link']/*[contains(text(), 'Отправить код sms')]"))
                        .Click();
                }
                //Получаем СМС код
                Thread.Sleep(20000);
                string code = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                if (code == null)
                {
                    int countM = 0;
                    for (int m = 1; m <= 3; m++)
                    {
                        Thread.Sleep(3000);
                        code = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                        countM += 1;
                        if (code != null) break;
                    }
                    Console.WriteLine($"\t\tбыло первых попыток получить СМС при регистрации: {countM}");
                    if (code == null)
                    {
                        if (Driver.FindElements(By.XPath(
                                    "//span[@data-t='link:default'][contains(text(), 'Отправить код повторно')]"))
                                .Count > 0)
                        {
                            Wait.Until(d => d.FindElement(
                                    By.XPath("//span[@data-t='link:default'][contains(text(), 'Отправить код повторно')]")))
                                .Click();
                        }
                        else if (Driver.FindElements(By.XPath(
                                         "//button[@type='button']/*[contains(text(), 'Отправить ещё код')]"))
                                     .Count > 0)
                        {
                            Thread.Sleep(10000);
                            Wait.Until(d => d.FindElement(
                                    By.XPath("//button[@type='button']/*[contains(text(), 'Отправить ещё код')]")))
                                .Click();
                        }
                        retryRequestCodeCount += 1;
                        int countN = 0;
                        for (int n = 0; n <= 2; n++)
                        {
                            code = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                            countN += 1;
                            if (code != null) break;
                        }
                        Console.WriteLine(
                            $"\t\tбыло попыток получить СМС при регистрации с повторным отправлением СМС: {countN}");
                        Console.WriteLine($"\tБыла повторная отправка СМС при регистрации: {retryRequestCodeCount}");
                    }
                    if (code == null)
                    {
                        Console.WriteLine($"\tДля Логина {i} не пришли СМС-коды. повтори занова");
                    }
                }
                //ВВводим код из СМС
                var inputSms = Driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                inputSms.SendKeys(code);
                if (Driver.FindElements(
                        By.XPath("//div[@role='alert'][contains(text(), 'Неправильный')]")).Count == 0) break;
            }
            Thread.Sleep(1500);
            var registrationOk = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
            registrationOk.Click();
            Thread.Sleep(1500);
            await GetSmsCodee.TelNomberStatusSend(Host, ApiKey, idNum);
        }
     public static async Task<bool> Auth(WebDriver Driver, WebDriverWait Wait, int i, string Login, string Password,
         string idNum, string? NewPassword, int? a1)
        {
            Thread.Sleep(1500);
            if (Driver.FindElements(By.XPath("//h2[contains(text(), 'Ваш аккаунт готов!')]")).Count > 0)
            {
                Driver.Navigate().GoToUrl(Page.UrlZen);
                //Console.WriteLine("вошел сразу");
            }
            else if (Driver.FindElements(By.XPath("//button[@aria-label='Меню профиля']")).Count > 0)
            {
                return true;
            }
            else
            {
                if (Driver.FindElements(
                        By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]")).Count > 0)
                {
                    Driver.FindElement(
                        By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]")).Click();
                }
                else
                {
                    var auth = Driver.FindElement(By.XPath("//span[contains(text(), 'Войти')]"));
                    auth.Click();
                }
                Thread.Sleep(1000);
                //Если открылось поле ввода пароля другого аккаунта
                if (Driver.FindElements(
                        By.XPath("//span[@class='CurrentAccount-login']")).Count > 0)
                {
                    var quitLogin = Wait.Until(d => d.FindElement(By.XPath("//span[@class='CurrentAccount-login']")));
                    quitLogin.Click();
                    Thread.Sleep(1000);
                    var addNewLogin =
                        Wait.Until(d => d.FindElement(By.XPath("//span[@class='AddAccountButton-text']")));
                    addNewLogin.Click();
                }
                Thread.Sleep(1000);
                //Если добавлены 2 аккаунта
                if (Driver.FindElements(
                        By.XPath("//span[contains(text(), 'другой аккаунт')]")).Count > 0)
                {
                    var addLogin =
                        Wait.Until(d => d.FindElement(By.XPath("//span[contains(text(), 'другой аккаунт')]")));
                    addLogin.Click();
                }
                //Если при входе открылось поле для ввода телефона, нажимаем на кнопку для перехода в ввод логина
                if (Driver.FindElements(By.XPath("//input[@placeholder='+7 123 456-78-90']")).Count > 0)
                {
                    var inpuLogin1 =
                        Driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_clear']"));
                    inpuLogin1.Click();
                }
                var inputLogin = Wait.Until(d => d.FindElement(By.XPath("//input[@data-t='field:input-login']")));
                inputLogin.SendKeys(Login);
                var inputLoginOk = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputLoginOk.Click();
                Thread.Sleep(3000);
                if (Driver.FindElements(
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
                if (Driver.FindElements(By.XPath(
                                "//div[@class='auth-challenge-descr']/*[contains(text(), 'Пожалуйста, подтвердите номер телефона,')]"))
                        .Count > 0)
                {
                    for (int m = 0; m < 3; m++)
                    {
                        if (Driver.FindElements(
                                By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Count > 0)
                        {
                            Driver.FindElement(By.XPath("//button/*[contains(text(), 'Отправить ещё код')]")).Click();
                        }

                        var telConfirm1 = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                        telConfirm1.Click();
                        string apiTelStatusGetNewSms1 = $"/setStatus/?apiKey={ApiKey}&status=send&idNum={idNum}";
                        var addressTelStatusGetNewSms1 = new Uri(Host + apiTelStatusGetNewSms1);
                        var telStatusGetNewSms1 = new HttpClient() { BaseAddress = addressTelStatusGetNewSms1 };
                        var responseTelStatusGetNewSms1 =
                            await telStatusGetNewSms1.GetAsync(addressTelStatusGetNewSms1, new CancellationToken());
                        //Получаем новый СМС
                        Thread.Sleep(30000);
                        ResponseGetPhoneNomber = await GetSmsCodee.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                        string codeNew1 = ResponseGetSmsCode.SmsCode;
                        //ВВодим СМС код
                        var inputNewSms1 = Driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                        inputNewSms1.SendKeys(codeNew1);
                        var codeConfirm = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                        codeConfirm.Click();
                        Thread.Sleep(1000);
                        if (Driver.FindElements(
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
                var inputPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                inputPassword.SendKeys(Password);
                var inputPasswordOk = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputPasswordOk.Click();
                Thread.Sleep(3000);
                //Если просит подтвердить номер при входе
                if (Driver.FindElements(
                        By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = Driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }

                if (Driver.FindElements(
                        By.XPath("//div[@id='field:input-passwd:hint'][contains(text(), 'Неверный пароль')]")).Count >
                    0)
                {
                    if (NewPassword != null)
                    {
                        var inputPassword1 = Driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                        inputPassword1.SendKeys(NewPassword);
                        var inputPasswordOk1 =
                            Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
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
                if (Driver.FindElements(By.XPath("//img[@class='captcha__image']")).Count > 0)
                {
                    await InputCaptchaCode("//img[@class='captcha__image']");
                }

                Thread.Sleep(3000);
                //Если просит доп. почту
                if (Driver.FindElements(By.XPath("//button[@data-t='button:pseudo']")).Count > 0)
                {
                    var skipNewMail = Driver.FindElement(By.XPath("//button[@data-t='button:pseudo']"));
                    skipNewMail.Click();
                }

                //Если просит ответ на КВ
                if (Driver.FindElements(By.XPath("//input[@data-t='field:input-question']")).Count > 0)
                {
                    var inputKv = Driver.FindElement(By.XPath("//input[@data-t='field:input-question']"));
                    inputKv.SendKeys("Шерлок");
                    var kvok = Driver.FindElement(By.XPath(
                        "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max Button2_type_submit']"));
                    kvok.Click();
                }

                //Если просит подтвердить номер при входе
                if (Driver.FindElements(
                            By.XPath(
                                "//div[@class='auth-challenge-form-hint'][contains(text(), 'Ваш номер телефона:')]"))
                        .Count > 0)
                {
                    GetSmsCodee.TelNomberStatusSend(Host, ApiKey, idNum);
                    Driver.FindElement(By.XPath("//button[@data-t='button:action']")).Click();
                    //Получаем новый СМС
                    Thread.Sleep(30000);
                    string codeNew1 = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                    //ВВодим СМС код
                    var inputNewSms1 = Driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                    inputNewSms1.SendKeys(codeNew1);
                    var codeConfirm = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                    codeConfirm.Click();
                }
            }
            return true;
        }
    
}