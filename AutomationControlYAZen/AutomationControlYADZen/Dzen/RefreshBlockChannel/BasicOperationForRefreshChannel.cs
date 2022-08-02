using System;
using System.Threading;
using System.Threading.Tasks;
using AutomationControlYADZen.Dzen.BasicOperation;
using AutomationControlYADZen.Dzen.Pages;
using OpenQA.Selenium;

namespace AutomationControlYADZen.Dzen.RefreshBlockChannel;

public class BasicOperationForRefreshChannel : UiTestBase
    {
        public static void PhoneAssociatedWithAccount(string NewLogin)
        {
            if (Driver.FindElements(By.XPath("//strong[@class='PhoneInput-hintNumber']")).Count == 0) return;
            Driver.Close();
            Driver.Quit();
            Console.WriteLine($"{NewLogin} Не отвязан номер");
            Driver.Navigate().GoToUrl(Url);
        }

        public static void InputAnswerKv(string kvAnswer)
        {
            var inputKv = Wait.Until(d => d.FindElement(By.XPath("//input[@data-t='field:input-hint_answer']")));
            inputKv.SendKeys(kvAnswer);
            var kvok = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
            kvok.Click();
        }

        public static void SetNewPassword(string NewPassword)
        {
            var interPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-password']"));
            interPassword.SendKeys(NewPassword);
            var interConfirmPassword = Driver.FindElement(By.XPath("//input[@data-t='field:input-password_confirm']"));
            interConfirmPassword.SendKeys(NewPassword);
            Thread.Sleep(2000);
            var newPasswordOk = Driver.FindElement(By.XPath("//button[@data-t='button:action']"));
            newPasswordOk.Click();
            Thread.Sleep(3000);
            var okok = Driver.FindElement(By.XPath("//a[@data-t='button:action']"));
            okok.Click();
            Thread.Sleep(5000);
            var okokok = Driver.FindElement(By.XPath("//a[@data-t='button:action']"));
            okokok.Click();
        }

        public static async Task RefreshChannel(string NewLogin, string kvAnswer, long? telNumber, string? idNum)
        {
            if (telNumber == null & idNum == null)
            {
                var responseGetPhoneNomber = await GetSmsCode.GetTelephoneNomber(Host, ApiGetPhoneNomber);
                telNumber = responseGetPhoneNomber.TelNomber;
                idNum = responseGetPhoneNomber.IdNum;
            }
            var restoreAcsessButton = Driver.FindElement(By.XPath(
                    "//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"));
                restoreAcsessButton.Click();
                Thread.Sleep(1000);
                await InputCaptchaCode("//img[@class='captcha__image']");
                BasicOperationForRefreshChannel.PhoneAssociatedWithAccount(NewLogin);
                BasicOperationForRefreshChannel.InputAnswerKv(kvAnswer);
                await BasicOperation.InputAndConfirmTelNumber(Driver, Wait, telNumber, idNum);
                //await BasicOperationForRegistration.InputTelNomber(Driver, Wait, telNumber, idNum);
                await GetSmsCode.TelNomberStatusSend(Host, ApiKey, idNum);
                await BasicOperation.GetAndInputSmsCode(Driver, Wait, idNum, null);
                //await BasicOperationForRegistration.GetAndInputSmsCode(Driver, Wait, idNum, NewLogin);
                BasicOperationForRefreshChannel.SetNewPassword(NewPassword);
                await new AccountPhoneNumbers(Driver, Wait).Open().DeleteTelNomber(Password, idNum, NewLogin);
                //await BasicOperationForRegistration.DeleteTelNomber(Driver, Wait, NewPassword, idNum, NewLogin);
        }
    }