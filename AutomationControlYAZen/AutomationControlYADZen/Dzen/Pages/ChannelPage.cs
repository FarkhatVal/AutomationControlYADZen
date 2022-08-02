using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using static OpenQA.Selenium.By;

namespace AutomationControlYADZen.Dzen.Pages;

public class ChannelPage : Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;

    /*public ChannelPage(WebDriver driver)
    {
        driver.Url = _url;
    }*/

    public ChannelPage(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }

    public ChannelPage Open(string channelUrl)
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(channelUrl);
        return this;
    }

    public ReadOnlyCollection<IWebElement> GetPostsInChannel()
    {
        ReadOnlyCollection<IWebElement> posts = _wait.Until(e =>
            e.FindElements(XPath("//a[@class='card-image-compact-view__clickable']")));
        return posts;
    }

    public void SubscribeAndLikAndComment()
    {
        if (_driver.FindElements(XPath("//a[contains(text(), 'Расскажите о себе')]")).Count > 0) return;
        Subscribe();
        for (int p = 0; p < GetPostsInChannel().Count; p++)
        {
            Console.WriteLine($"\tcтатья {p+1}");
            string postLink = GetPostsInChannel()[p].GetAttribute("href");
            _driver.Navigate().GoToUrl(postLink);
            Like();
            SetComment();
            string channelUrl = _driver.FindElement(XPath("//div[@class='ui-lib-channel-info__logo-wrapper']/a"))
                .GetAttribute("href");
            _driver.Navigate().GoToUrl(channelUrl);
            Thread.Sleep(2000);
            if(_driver.FindElements(XPath("//div[@id='yabro-strip-element']")).Count > 0) _driver.SwitchTo().Alert().Dismiss();
            _wait.Until(d => d.FindElement(XPath("//div[@itemprop='name']")));
        }
    }

    private static async Task SetComment()
    {
        _wait.Until(d => d.FindElement(XPath("//span[contains(text(), 'бсуди')]"))).Click();
        if (_driver.FindElements(XPath("//div[@class='error-reload-button-view__title']")).Count > 0)
            _driver.FindElement(XPath("//button[@class='error-reload-button-view__button']")).Click();
        _wait.Until(d => d.FindElement(XPath("//textarea[@class='comment-editor__editor']"))).SendKeys("Круто!");
        _wait.Until(d => d.FindElement(XPath("//span[@class='comment-editor__tooltip-trigger']"))).Click();
        if(_driver.FindElements(XPath("//span[@class='comments-captcha__image']")).Count > 0)
            await UiTestBase.InputCaptchaCode("//img[@class='captcha__image']");
        Console.WriteLine("\t\tоставлен коммент");
        Thread.Sleep(1000);
    }

    private static void Subscribe()
    {
        if (_driver.FindElements(XPath("//button[@class='Button2 Button2_view_carrot Button2_width_max Button2_size_xl channel-subscribe-button__desktopButton-1J']")).Count > 0)
            _wait.Until(d => d.FindElement(XPath("//button[@class='Button2 Button2_view_carrot Button2_width_max Button2_size_xl channel-subscribe-button__desktopButton-1J']"))).Click();
        if (_driver.FindElements(XPath("//button[@aria-label='Подписаться']")).Count > 0)
        {
            _wait.Until(d => d.FindElement(XPath("//button[@aria-label='Подписаться']"))).Click();
            Console.WriteLine("\t\tподписался");
        }
        Thread.Sleep(1000);
    }

    private static void Like()
    {
        IWebElement like = _driver.FindElement(XPath(
            "//span[@class='left-column-button__text left-column-button__text_can-be-compact'][contains(text(), 'равится')]"));
        string likeCount = Regex.Replace(like.Text, "[^0-9]", "");
        int likeValueUpTo = 0;
        if (likeCount != "")
        {
            likeValueUpTo = Convert.ToInt32(likeCount);
        }
        IWebElement element = _wait.Until(d => d.FindElement(XPath("//button[@aria-label='Нравится']")));
        if (element.GetAttribute("aria-pressed") == "false") element.Click();
        IWebElement likeAfter = _driver.FindElement(XPath(
            "//span[@class='left-column-button__text left-column-button__text_can-be-compact'][contains(text(), 'равится')]"));
        string likeCount2 = Regex.Replace(likeAfter.Text, "[^0-9]", "");
        string likeStatus = _wait.Until(d => d.FindElement(XPath("//button[@aria-label='Нравится']")))
            .GetAttribute("aria-pressed");
        //int likeValueAfter;
        var likeValueAfter = likeCount2 == "" & likeStatus == "true"
            ? 1
            : Convert.ToInt32(likeCount2);
        Console.WriteLine(likeValueAfter > likeValueUpTo
            ? "\t\tлайкнута"
            : $"\t\tуже лайкнута. Всего лайков: {likeValueAfter}");
    }
