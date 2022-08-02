using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;
using TwoCaptcha.Captcha;

namespace AutomationControlYADZen.Dzen.BasicOperation;

public class AntiCaptcha : UiTestBase
    {
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
                    Console.WriteLine("Captcha solved: " + captcha.Code);
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
            //Разгадываем капчу //img[@src]
            string captchaCode = await AntiCaptha(XPathSelectorToCapcha);
                 var inputcaptchaCode = Driver.FindElement(By.XPath("//input[@data-t='field:input-captcha_answer']"));
                 inputcaptchaCode.SendKeys(captchaCode);
                 Thread.Sleep(5000);
                 var captchaCodeOk = Driver.FindElement(By.XPath("//button[@class='Button2 Button2_size_l Button2_view_action Button2_width_max']"));
                 captchaCodeOk.Click();
                 Thread.Sleep(5000);
                 if (Driver.FindElements(
                             By.XPath(
                                 "//div[@id='field:input-captcha_answer:hint']"))
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
                     if (Driver.FindElements(
                                 By.XPath(
                                     "//div[@id='field:input-captcha_answer:hint']")).Count > 0)
                     {
                         Assert.Fail("\t2 раза не распознана капча");
                     }
                 }
        }
    }