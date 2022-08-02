using System;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using AutomationControlYADZen.Dzen.Pages;
using BasicOperations;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.RegistrationNewAccount;

public class RegistrationNewWhithOneNumber8Hours : UiTestBase
    {
        public partial class RegistrationNewNomber8Hours
        {
            /*private const string Host = "https://vak-sms.com/api";
            private const string ApiGetPhoneNomber =
                $"/getNumber/?apiKey={ApiKey}&service=ya&country=ru";*/
            //private const string ApiGetPhoneNomber =
                //"/getNumber/?apiKey=95fb1d66d8ca4c2fb81b8506df1516cd&service=ya&country=ru&operator=mtt&softId={softId}&rent=true";

            //private const string ApiKey = "73185b5922b94ef192afa06644857bac";
            private static ResponseGetPhoneNomber _responseGetPhoneNumber;
            //private ResponseGetSMSCode responseGetSMSCode;
            //private TelStatusGetNewSMS telStatusGetNewSMS;
            private WebDriver _driver;
            //private static ChromeOptions _chromeOptions = new ChromeOptions();
            private string _firstName = "Юлия";
            private string _surname = "Нестерова";
            private string _login = "you-li-ne.ster-";
            //private string _password = "896008899gGsS!";
            private string _channnelName = "Hовости";
            private string _kvAnswer = "Шерлок";
            
            /*[OneTimeSetUp]
            public async Task Setup()
            {
                ChromeOptions = new ChromeOptions();
                //Отключить "Браузером управляет автоматизированное ПО"
                ChromeOptions.AddAdditionalCapability("useAutomationExtension", false);
                ChromeOptions.AddExcludedArgument("enable-automation");
                
            }*/

            [SetUp]
            public void BeforeTest()
            {
                _driver = new ChromeDriver(ChromeDriverService.CreateDefaultService(), ChromeOptions);
                Wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            }
            [TearDown]
            public void TearDown()
            {
                //Driver.Close();
                //Driver.Quit();
            }
            
            [Test]
            //[TestCase(127,132)]
            //[TestCase(130, 132)]
            //[TestCase(135, 138)]
            [TestCase(139, 139)]
            [TestCase(142, 144)]
            [TestCase(145, 147)]
            [TestCase(148, 150)]
            [TestCase(151, 153)]
            [TestCase(155, 144)]

            public async Task RegistrationNewAccount1Nomber(int a, int a1)
            {
                //Получаем номер телефона //Для всех аккаунтов 1 номер
                _responseGetPhoneNumber = await GetSmsCodee.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                long telNumber = _responseGetPhoneNumber.TelNomber;
                string idNum = _responseGetPhoneNumber.IdNum;
                for (int i = a; i <= a1; i++)
                {
                    //Вводим данные нового пользователя
                    if (!new RegistrationPage(_driver, Wait).Open().InputLoginAndPassword(_firstName, i,
                            _surname, _login, Password, a1)) continue;
                    Console.WriteLine($"Аккаунт {i}:");
                    //вводим номер телефона
                    await BasicOperation.InputAndConfirmTelNumber(_driver, Wait, telNumber, idNum);
                    //###Если взять 8часовой номер. иначе закомитить
                    await BasicOperation.GetSmsCodee.TelNomberStatusSend(Host, ApiKey, idNum);
                    //Получаем и вводим СМС код
                    await BasicOperation.GetAndInputSmsCode(_driver, Wait, idNum, i);
                    //await BasicOperationForRegistration.GetAndInputSmsCode(_driver, Wait, idNum, _login);
                    //Авторизация сразу после регистрации
                    string newLogin = _login + i;
                    await new AuthPage(_driver, Wait).Open().Auth(i, newLogin, Password, idNum, null, a1);
                    //await BasicOperationForRegistration.AuthAfterRegistration(_driver, Wait, i, newLogin, _password, idNum, null, a1);
                    //создание и установка канала
                    new ZenStudio(_driver, Wait).Open().SetChannelSettings(_channnelName); 
                    //BasicOperationForRegistration.ChannelSettings(_driver, Wait, _channnelName);
                    //Установка контрольного вопроса
                    new SecurityQuestion(_driver, Wait).Open().SetKv(_kvAnswer);
                    //BasicOperationForRegistration.SetKv(_driver, Wait, _kvAnswer);
                    //Удаление номера телефона
                    await new AccountPhoneNumbers(_driver, Wait).Open().DeleteTelNomber(Password, idNum, newLogin);
                    //await BasicOperationForRegistration.DeleteTelNomber(Driver, Wait, Password, idNum, NewLogin);
                    //Получение URL канала
                    /*string ChanelURL = BasicOperationForRegistration.GetChanelURL(Driver, Wait, Login, i);
                    Console.WriteLine($"\tссылка на канал: {ChanelURL} ");*/
                    new ZenStudio(_driver, Wait).Open().GetChanelUrl(_login, i);
                    Quit(_driver, Wait);
                }
            }
            [Test]
            [TestCase(0)]
            
            public async Task RegistrationNewAccount(int i)
            {
                new RegistrationPage(_driver, Wait).Open().InputLoginAndPassword(_firstName, i,
                    _surname, _login, Password, null);
                _responseGetPhoneNumber = await GetSmsCode.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                long telNumber = _responseGetPhoneNumber.TelNomber;
                string idNum = _responseGetPhoneNumber.IdNum;
                Console.WriteLine($"Аккаунт {i}:");
                var newLogin = _login + i;
                await BasicOperation.InputAndConfirmTelNumber(_driver, Wait, telNumber, idNum);
                await GetSmsCode.TelNomberStatusSend(Host, ApiKey, idNum);
                await BasicOperation.GetAndInputSmsCode(_driver, Wait, idNum, i);
                await BasicOperation.Auth(_driver, Wait, i, newLogin, Password, idNum, null, null);
                new ZenStudio(_driver, Wait).Open().SetChannelSettings(_channnelName);
                new SecurityQuestion(_driver, Wait).Open().SetKv(_kvAnswer);
                await new AccountPhoneNumbers(_driver, Wait).Open().DeleteTelNomber(Password, idNum, newLogin);
                new ZenStudio(_driver, Wait).Open().GetChanelUrl(_login, i);
                Quit(_driver, Wait);
            }

            [Test]
            [TestCase(158, 160)]
            public async Task Registration3NewAccount1Nomber(int a, int a1)
            {
                //Получаем номер телефона //Для всех аккаунтов 1 номер
                _responseGetPhoneNumber = await GetSmsCode.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                long telNumber = _responseGetPhoneNumber.TelNomber;
                string idNum = _responseGetPhoneNumber.IdNum;
                for (int i = a; i <= a1; i++)
                {
                    if (!new RegistrationPage(_driver, Wait).Open().InputLoginAndPassword(_firstName, i,
                            _surname, _login, Password, a1)) continue;
                    Console.WriteLine($"Аккаунт {i}:");
                    var newLogin = _login + i;
                    await BasicOperation.InputAndConfirmTelNumber(_driver, Wait, telNumber, idNum);
                    await GetSmsCode.TelNomberStatusSend(Host, ApiKey, idNum);
                    await BasicOperation.GetAndInputSmsCode(_driver, Wait, idNum, i);
                    await new AuthPage(_driver, Wait).Open().Auth(i, newLogin, Password, idNum, null, a1);
                    //await BasicOperation.Auth(Driver, Wait, i, NewLogin, Password, idNum, null, a1);
                    new ZenStudio(_driver, Wait).Open().SetChannelSettings(_channnelName);
                    new SecurityQuestion(_driver, Wait).Open().SetKv(KvAnswer);
                    await new AccountPhoneNumbers(_driver, Wait).Open().DeleteTelNomber(Password, idNum, newLogin);
                    new ZenStudio(_driver, Wait).Open().GetChanelUrl(_login, i);
                    Quit(_driver, Wait);
                }
            }
        }
}