using NUnit.Framework;
using OpenQA.Selenium;

namespace AutomationControlYADZen.Dzen;

public class ZenChannelViewsCheck : UiTestBase
        {
            const string Url = "https://zen.yandex.ru";
            private const string UrlZenStudio = "/media/zen/new";
            private string _password = "89600362559";

            [Test]
            [TestCase("Ivan100Letov-13")]
            [TestCase("Ivan100Letov-15")]
            [TestCase("Ivan1000Letovv-0")]
            [TestCase("Ivan1000Letovv-1")]
            [TestCase("Ivan1000Letovv-2")]
            [TestCase("Ivan1000Letovv-3")]
            [TestCase("Ivan1000Letovv-4")]
            [TestCase("Ivan1000Letovv-7")]
            [TestCase("Ivan1000Letovv-9")]
            [TestCase("Ivan1000Letovv-10")]
            [TestCase("Ivan1000Letovv-11")]
            [TestCase("Ivan1000Letovv-12")]
            [TestCase("Ivan1000Letovv-13")]
            [TestCase("Ivan1000Letovv-14")]
            [TestCase("Ivan1000Letovv-15")]
            [TestCase("Ivan1000Letovv-16")]
            [TestCase("Ivan1000Letovv-17")]
            [TestCase("Ivan1000Letovv-18")]
            [TestCase("Ivan1000Letovv-19")]
            [TestCase("Ivan1000Letovv-20")]
            [TestCase("Irina100Letova-0")]
            [TestCase("Irina100Letova-1")]
            [TestCase("Irina100Letova-3")]
            [TestCase("Irina100Letova-4")]
            [TestCase("Irina100Letova-5")]
            [TestCase("Irina100Letova-6")]
           
            
            public void ZenChannelViewsChecking(string login)
            {
                Auth(login, _password, null);
                Driver.Navigate().GoToUrl(Url + UrlZenStudio);
                var stat30Day = Driver.FindElement(By.XPath(
                    "//li[2]//div[@class='Text Text_weight_medium Text_typography_headline-20-24']"));
                var stat30DayText = stat30Day.GetAttribute("innerText");

                var channel = Driver.FindElement(By.XPath(
                    "//span[@class='Link Link_view_default Link_theme_button']"));
                channel.Click();

                var allOrNon = Driver.FindElement(By.XPath(
                    "//meta[@property='robots']"));
                string allOrNonText = allOrNon.GetAttribute("content");
                //Thread.Sleep(3000);
                Assert.IsTrue(0 == 10,
                    $"Логин: {login}, Статус канала: {allOrNonText}, Просмотров в месяц всего: {stat30DayText}");


            }
        }