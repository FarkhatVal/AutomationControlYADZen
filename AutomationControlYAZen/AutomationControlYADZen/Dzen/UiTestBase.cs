using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using BasicOperations;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen;

    [TestFixture]
    public class UiTestBase

    {
    protected static ChromeDriver Driver;
    protected static WebDriverWait Wait;
    protected static ChromeOptions ChromeOptions = new ChromeOptions();
    protected const string Url = "https://zen.yandex.ru/";
    protected const string Password = "896008899gGsS!";
    protected const string NewPassword = "896008899gG!!";
    internal static string KvAnswer = "Шерлок";
    private static string _path = Directory.GetCurrentDirectory();
    protected internal static List<string> ListOfPhoto = new List<string>();
    protected static string PathToScreen = Directory
        .GetParent(Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString())
        .ToString();
    internal static ResponseGetPhoneNomber ResponseGetPhoneNomber;
    protected internal static ResponseGetSmsCode ResponseGetSmsCode;
    private TelStatusGetNewSms _telStatusGetNewSms;
    protected internal const string ApiKey = "95fb1d66d8ca4c2fb81b8506df1516cd";
    protected internal const string Host = "https://vak-sms.com/api";
    private const string OperatorDefault = "None";
    private const string OperatorBeeline = "beeline";
    private const string OperatorLycamobile = "lycamobile";
    private const string OperatorMegafon = "megafon";
    private const string OperatorMts = "mts";
    private const string OperatorMtt = "mtt";
    private const string OperatorRostelecom = "rostelecom";
    private const string OperatorTele2 = "tele2";
    private const string OperatorYota = "yota";
    private const string Rent20Min = "false";
    private const string Rent8Hours = "true";
    protected internal const string ApiGetPhoneNomber = $"/getNumber/?apiKey={ApiKey}&service=ya&country=ru&operator={OperatorMts}&rent={Rent20Min}";
    
    [OneTimeSetUp]
    public void Setup()
    {
        ChromeOptions = new ChromeOptions();
        //Отключить "Браузером управляет автоматизированное ПО"
        ChromeOptions.AddAdditionalChromeOption("useAutomationExtension", false);
        ChromeOptions.AddExcludedArgument("enable-automation");
        if (!Directory.Exists(PathToScreen + $"/Dzen/AutoPostingDzen/PhotoFromPost/"))
            Directory.CreateDirectory(PathToScreen + $"/Dzen/AutoPostingDzen/PhotoFromPost/");
        if (!Directory.Exists(PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/"))
            Directory.CreateDirectory(PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/");
    }

    [SetUp]
    public void BeforeTest()
    {
        Driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), ChromeOptions);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(15));
    }

    [TearDown]
    public void TearDown()
    {
        //Driver.Close();
        //Driver.Quit();
        DirectoryInfo folder = new DirectoryInfo(PathToScreen + $"/Dzen/AutoPostingDzen/PhotoFromPost/");
        foreach (FileInfo file in folder.GetFiles())
        {
            file.Delete();
        }
        DirectoryInfo foldercapcha = new DirectoryInfo(PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/");
        foreach (FileInfo file in foldercapcha.GetFiles())
        {
            file.Delete();
        }
    }

    protected static async Task Auth(string Login, string Password, string? NewPassword)
    {
        Driver.Navigate().GoToUrl(Url);
        Driver.Manage().Window.Maximize();
        var auth = Driver.FindElement(
            By.XPath("//a[@class='auth-header-buttons-view__right-link _is-carrot-accents'][2]"));
        auth.Click();
        Thread.Sleep(1000);
        //Если открылось поле ввода пароля другого аккаунта
        if (Driver.FindElements(
                By.XPath("//span[@class='CurrentAccount-login']")).Count > 0)
        {
            var quitLogin = Wait.Until( d => d.FindElement(By.XPath("//span[@class='CurrentAccount-login']")));
            quitLogin.Click();
            var addNewLogin = Wait.Until( d => d.FindElement(By.XPath("//span[@class='AddAccountButton-text']")));
            addNewLogin.Click();
        }
        Thread.Sleep(1000);
        //Если добавлены 2 аккаунта
        if (Driver.FindElements(
                By.XPath("//span[contains(text(), 'другой аккаунт')]")).Count > 0)
        {
            var addLogin = Wait.Until( d => d.FindElement(By.XPath("//span[contains(text(), 'другой аккаунт')]")));
            addLogin.Click();
        }
        //Если при входе открылось поле для ввода телефона, нажимаем на кнопку для перехода в ввод логина
        if (Driver.FindElements(By.XPath("//input[@placeholder='+7 123 456-78-90']")).Count > 0)
        {
            var inpuLogin1 = Driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_clear']"));
            inpuLogin1.Click();
        }
        var inputLogin = Wait.Until( d => d.FindElement(By.XPath("//input[@data-t='field:input-login']")));
        inputLogin.SendKeys(Login);
        var inputLoginOk = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
        inputLoginOk.Click();
        Thread.Sleep(3000);
        if (Driver.FindElements(By.XPath("//div[@id='field:input-login:hint'][contains(text(), 'Такого аккаунта нет')]")).Count > 0)
        {
            Assert.Fail($"Акааунт {Login} еще не зарегистрирован");
        }
        var inputPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
        inputPassword.SendKeys(Password);
        var inputPasswordOk = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
        inputPasswordOk.Click();
        Thread.Sleep(1000);
        if (Driver.FindElements(By.XPath("//div[@id='field:input-passwd:hint'][contains(text(), 'Неверный пароль')]")).Count > 0 )
        {
            if (NewPassword != null)
            {
                var inputPassword1 = Driver.FindElement(By.XPath("//input[@data-t='field:input-passwd']"));
                inputPassword1.SendKeys(NewPassword);
                var inputPasswordOk1 = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
                inputPasswordOk1.Click();
                Thread.Sleep(1000);
                Console.WriteLine($"\tДля Логина {Login} Пароль уже иземнен на {NewPassword}");
            }
            else
            {
                Assert.Fail($"\tНе верный пароль {Password}");
            }
        }
        //Если просит ввести капчу
        if (Driver.FindElements(By.XPath("//img[@class='captcha__image']")).Count > 0 )
        {
            string captchaCode = await AntiCaptha("//img[@class='captcha__image']");
            Thread.Sleep(5000);
            var inputcaptchaCode = Driver.FindElement(By.XPath("//input[@data-t='field:input-captcha_answer']"));
            inputcaptchaCode.SendKeys(captchaCode);
            Thread.Sleep(5000);
            var captchaCodeOk = Driver.FindElement(By.XPath("//button[@data-t='button:action:passp:sign-in']"));
            captchaCodeOk.Click();
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
    }
    public static async Task<string> AntiCaptha(string XPathSelectorToCapcha)
        {
            Thread.Sleep(2000);
            var photo =
                        Wait.Until(e => e.FindElement(By.XPath(XPathSelectorToCapcha)));
            Screenshot screenshotPhoto = ((ITakesScreenshot)photo).GetScreenshot();
            screenshotPhoto.SaveAsFile(
                        UiTestBase.PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/captcha+.png",
                        ScreenshotImageFormat.Png); 
            string pathToScreen = UiTestBase.PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/captcha+.png";
            ListOfPhoto.Add(new string(pathToScreen));
                    
                TwoCaptcha.TwoCaptcha solver = new TwoCaptcha.TwoCaptcha("b8ff7987bf8b6a56623e620e5379aaa0");
                Normal captcha = new Normal();
                captcha.SetFile(pathToScreen);
                captcha.SetMinLen(4);
                captcha.SetMaxLen(20);
                captcha.SetCaseSensitive(true);
                captcha.SetLang("ru");

                try
                {
                    await solver.Solve(captcha);
                    //Console.WriteLine("Captcha solved: " + captcha.Code);
                    return captcha.Code;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error occurred: " + e.Message);
                    return null;
                }
        }

        public static async Task InputCaptchaCode(string XPathSelectorToCapcha)
        {
            Thread.Sleep(1000);
            var captchaCode = await AntiCaptha(XPathSelectorToCapcha);
            if (Driver.FindElements(By.XPath("//input[@placeholder='Введите символы']")).Count > 0)
            {
                var inputcaptchaCode1 = Driver.FindElement(By.XPath("//input[@placeholder='Введите символы']"));
                inputcaptchaCode1.SendKeys(captchaCode);
            }
            else
            {
                var inputcaptchaCode = Driver.FindElement(By.XPath("//input[@data-t='field:input-captcha_answer']"));
                inputcaptchaCode.SendKeys(captchaCode);
                Thread.Sleep(5000);
                if (Driver.FindElements(
                            By.XPath("//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"))
                        .Count > 0)
                {
                    var captchaCodeOk = Driver.FindElement(By.XPath(
                            "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"));
                    captchaCodeOk.Click();
                }
                Thread.Sleep(5000);
                if (Driver.FindElements(By.XPath("//div[@id='field:input-captcha_answer:hint']"))
                        .Count > 0)
                {
                    DirectoryInfo foldercapcha = new DirectoryInfo(PathToScreen + $"/Dzen/AutoPostingDzen/Captcha/");
                    foreach (FileInfo file in foldercapcha.GetFiles())
                    {
                        file.Delete();
                    
                    }
                    string captchaCode1 = await AntiCaptha("//img[@class='captcha__image']");
                    var inputcaptchaCode1 = Driver.FindElement(By.XPath("//input[@data-t='field:input-captcha_answer']"));
                    inputcaptchaCode1.SendKeys("\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b\b");
                    inputcaptchaCode1.SendKeys(captchaCode1);
                    Thread.Sleep(5000);
                    var captchaCodeOk1 = Driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"));
                    captchaCodeOk1.Click();
                    if (Driver.FindElements(By.XPath("//div[@id='field:input-captcha_answer:hint']")).Count > 0)
                    {
                        Assert.Fail("2 раза не распознана капча");
                    }
                }
            }

            if (Driver.FindElements(
                        By.XPath("//span[@class='zen-ui-button__content-wrapper'][contains(text(), 'Отправить')]"))
                    .Count > 0)
            {
                Driver.FindElement(
                    By.XPath("//span[@class='zen-ui-button__content-wrapper'][contains(text(), 'Отправить')]")).Click();
            }
        }

        public static void Quit(WebDriver Driver, WebDriverWait Wait)
        {
            //Driver.Navigate().GoToUrl(Url);
            //Driver.Manage().Window.Maximize();
            if (Driver.FindElements(By.XPath("//button[@aria-label='Меню профиля']"))
                    .Count > 0)
            {
                Driver.FindElement(By.XPath("//button[@aria-label='Меню профиля']")).Click();
            }
            else if (Driver.FindElements(By.XPath("//button[@class='zen-ui-avatar _is-button _icon-size_32']"))
                .Count > 0)
            {
                var menuProfil = Driver.FindElement(By.XPath("//button[@class='zen-ui-avatar _is-button _icon-size_32']"));
                menuProfil.Click();
            }
            else
            {
                Driver.Navigate().GoToUrl(Url);
                Driver.FindElement(By.XPath("//button[@aria-label='Меню профиля']")).Click();
            }
            Thread.Sleep(1000);
            if (Driver.FindElements(By.XPath("//div[contains(text(), 'Выйти')]"))
                    .Count > 0)
            {
                Driver.FindElement(By.XPath("//div[contains(text(), 'Выйти')]")).Click();
            }
            else
            {
                var quit = Driver.FindElement(By.XPath("//span[contains(text(), 'Выйти')]"));
                quit.Click(); }
            
            Thread.Sleep(1000);
        }
    }
