using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using AutomationControlYADZen.Dzen.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.RefreshBlockChannel;

public class RefreshBlockChannel : UiTestBase
    {
        private string _login = "you-li-ne.ster-"; 
        private string _password = "896008899gGsS!";
        private string _newPassword = "896008899gG!!";
        

        [SetUp]
        public void BeforeTest()
        {
            Driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), ChromeOptions);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        }

        /*[Test]
        [TestCase(0)]
        
        public async Task RefreshChannel(int i)
        {
            string newLogin = _login + i;
            Auth(newLogin, _password, _newPassword);
            if (Driver.FindElements(
                    By.XPath("//h1[@data-t='title'][contains(text(), 'Доступ временно ограничен')]")).Count > 0)
            {
                var restoreAcsessButton = Driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"));
                restoreAcsessButton.Click();
                Thread.Sleep(1000);
                //Разгадываем капчу //img[@src]
                await InputCaptchaCode("//img[@class='captcha__image']");
                Thread.Sleep(2000);
                if (Driver.FindElements(
                            By.XPath(
                                "//strong[@class='PhoneInput-hintNumber']"))
                        .Count > 0)
                {
                    Driver.Close();
                    Driver.Quit();
                    Console.WriteLine("Не отвязан номер");
                    Assert.Fail("Не отвязан номер");
                }
                var inputKv = Driver.FindElement(By.XPath("//input[@data-t='field:input-hint_answer']"));
                inputKv.SendKeys("Шерлок");
                var kvok = Driver.FindElement(By.XPath(
                    "//button[@data-t='button:action']"));
                kvok.Click();
                var responseGetPhoneNomber = await GetSmsCodee.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                long telNumber = responseGetPhoneNomber.TelNomber;
                string idNum = responseGetPhoneNomber.IdNum;
                //ВВодим номер телефона
                var interTel = Driver.FindElement(By.XPath("//input[@data-t='field:input-phone']"));
                interTel.SendKeys((telNumber-70000000000).ToString());
                Thread.Sleep(2000);
                var telConfirm = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                telConfirm.Click();
                //Получаем СМС код
                Thread.Sleep(30000);
                string code = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                //ВВводим код из СМС
                var inputSms = Driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                inputSms.SendKeys(code);
                Thread.Sleep(1500);
                var registrationOk = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                registrationOk.Click();
                    
                Thread.Sleep(1500);
                var interPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-password']"));
                interPassword.SendKeys(_newPassword);
                var interConfirmPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-password_confirm']"));
                interConfirmPassword.SendKeys(_newPassword);
                Thread.Sleep(2000);
                var newPasswordOk = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
                newPasswordOk.Click();
                Thread.Sleep(3000);
                var okok = Driver.FindElement(By.XPath("//a[@data-t='button:action']"));
                okok.Click();
                Thread.Sleep(5000);
                var okokok = Driver.FindElement(By.XPath("//a[@data-t='button:action']"));
                okokok.Click();
                
                Driver.Navigate().GoToUrl("https://passport.yandex.ru/profile/phones?origin=passport_profile");
                Thread.Sleep(3000);
                GetSmsCodee.TelNomberStatusSend(Host, ApiKey, idNum);
                
                //Получаем новый СМС
                Driver.Navigate().GoToUrl("https://passport.yandex.ru/profile/phones?origin=passport_profile");
                var telDelet = Driver.FindElement(By.XPath("//button[@data-t='phones.default-restore-phone']"));
                telDelet.Click();
                var telDelet1 = Driver.FindElement(By.XPath("//button[@data-t='phones.settings.restore.remove']"));
                telDelet1.Click();
                var telDelet2 = Driver.FindElement(By.XPath("//button[@data-t='phones:button:confirm.phone']"));
                telDelet2.Click();
                // ввод пароля
                Thread.Sleep(2000);
                var interNewPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-password']"));
                interNewPassword.SendKeys(_newPassword);
                Thread.Sleep(20000);
                string codeNew = await GetSmsCodee.GetSmsCode(Host, ApiKey, idNum);
                var inputNewSms = Driver.FindElement(By.XPath("//input[@data-t='field:input-phoneCode']"));
                inputNewSms.SendKeys(codeNew);
                var telDeletOk = Driver.FindElement(By.XPath("//button[@data-t='phones:button']"));
                telDeletOk.Click();
                Driver.Navigate().GoToUrl("https://zen.yandex.ru/media/zen/new");
                Thread.Sleep(3000);
                var chanel = Driver.FindElement(By.XPath("//span[@class='Link Link_view_default Link_theme_button']"));
                chanel.Click();
                Thread.Sleep(3000);
                String chanelUrl =  Driver.Url;
                Assert.IsTrue(0==10, $"{_login}{i} обнови пароль на: {_newPassword} \n ссылка на канал: {chanelUrl} ");
                //Console.WriteLine($"{Login} обнови пароль на: {NewPassword}");
            }
            else if (Driver.FindElements(By.XPath("//div[@id='field:input-login:hint'][contains(text(), 'Такого аккаунта нет')]")).Count > 0)
            {
                Driver.Close();
                Driver.Quit();
            }
            else
            {
                Driver.Navigate().GoToUrl("https://zen.yandex.ru/media/zen/new");
                Thread.Sleep(3000);
                var chanel = Driver.FindElement(By.XPath("//span[@class='Link Link_view_default Link_theme_button']"));
                chanel.Click();
                Thread.Sleep(3000);
                String chanelUrl =  Driver.Url;
                Console.WriteLine($"Канал был активен \n ссылка на канал: {chanelUrl}");
            }
        }*/
        
        [Test]
        [TestCase(63,65)]

        
        public async Task RefreshChannel1Nomber(int a, int a1)
        {
            var responseGetPhoneNomber = await GetSmsCode.GetTelephoneNomber(Host, ApiGetPhoneNomber);
            long telNumber = responseGetPhoneNomber.TelNomber;
            string idNum = responseGetPhoneNomber.IdNum;
            for (int i = a; i <= a1; i++)
            {
                Console.WriteLine($"Аккаунт {i}:");
                string newLogin = _login + i;
                if (await new AuthPage(Driver, Wait).Open().Auth(i, newLogin, _password, idNum, _newPassword, a1));
                {
                    if (Driver.FindElements(
                            By.XPath("//h1[@data-t='title'][contains(text(), 'Доступ временно ограничен')]")).Count > 0)
                    {
                        await BasicOperationForRefreshChannel.RefreshChannel(newLogin, KvAnswer, telNumber, idNum);
                        new ZenStudio(Driver, Wait).Open().GetChanelUrl(_login, i).GetChannelMetaTag();
                        //string ChanelURL = BasicOperationForRegistration.GetChanelURL(Driver, Wait, Login, i);
                        Console.WriteLine($"\tобнови пароль на: {_newPassword}");
                        //BasicOperationForRefreshChannel.GetChannelMetaTag();
                        Quit(Driver, Wait);
                    }
                    else if (Driver.FindElements(By.XPath(
                                "//div[@id='field:input-login:hint'][contains(text(), 'Такого аккаунта нет')]"))
                            .Count > 0)
                    {
                        Driver.Close();
                        Driver.Quit();
                        Console.WriteLine($"\t{_login}{i} не существует");
                    }
                    else
                    {
                        Console.WriteLine($"\tКанал был активен");
                        new ZenStudio(Driver, Wait).Open().GetChanelUrl(_login, i).GetChannelMetaTag();
                        //BasicOperationForRefreshChannel.GetChannelMetaTag();
                        Quit(Driver, Wait);
                    }
                }
            }
        }
    }