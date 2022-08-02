using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AutomationControlYADZen.Dzen.Pages;

public class ZenStudio : Page
{
    private static WebDriver _driver;
    private static WebDriverWait _wait;
    private readonly string _url = UrlZen + Endpoints.ZenStudio;
    
    public ZenStudio(WebDriver driver)
    {
        driver.Url = _url;
    }
    
    public ZenStudio(WebDriver driver, WebDriverWait wait)
    {
        _driver = driver;
        _wait = wait;
    }
    
    public ZenStudio Open()
    {
        _driver.Manage().Window.Maximize();
        _driver.Navigate().GoToUrl(_url);
        return this;
    }
    public void SetChannelSettings(string channnelName)
    {
        var chanalNameSend = _wait.Until( d => d.FindElement(By.XPath(ZenStudioXPathSelectors.ChannelNameInputField)));
        chanalNameSend.SendKeys(channnelName);
        var chanalNameOk = _driver.FindElement(By.XPath(ZenStudioXPathSelectors.ChannelNameSave));
        chanalNameOk.Click();
        //Настройки
        //Thread.Sleep(3000);
        if (_driver.FindElements(
                    By.XPath(ZenStudioXPathSelectors.SetingButton1))
                .Count > 0)
        {
            _wait.Until( d => d.FindElement(By.XPath(ZenStudioXPathSelectors.SetingButton1))).Click();
        }
        else
        {
            var settings = _wait.Until( d => d.FindElement(By.XPath(ZenStudioXPathSelectors.SetingButton2)));
            settings.Click();
        }
        ChannelSettings();
    }

    public static void ChannelSettings()
    {
        var settings1 = _wait.Until( d => d.FindElement(By.XPath("//input[@type='checkbox']")));
        settings1.Click();
        var settings2 = _wait.Until( d => d.FindElement(By.XPath("//button[@type='submit']")));
        if (_driver.FindElements(By.XPath("//input[@aria-checked='false']")).Count > 0) settings1.Click();
        Thread.Sleep(1000);
        settings2.Click();
    }

    public ZenStudio GetChanelUrl(string Login, int i)
    {
        var chanel = _wait.Until( d => d.FindElement(By.XPath(ZenStudioXPathSelectors.GoToChannelButton)));
        chanel.Click();
        string chanelUrl =  _driver.Url;
        Console.WriteLine($"\tссылка на канал: {chanelUrl} ");
        return this;
    }
    public void GetChannelMetaTag()
    {
        var allOrNon = _driver.FindElement(By.XPath("//meta[@property='robots']"));
        string allOrNonText = allOrNon.GetAttribute("content");
        Console.WriteLine($"\tСтатус канала: {allOrNonText}");
    }

    public void CreateNewPost(string? service, string zagolovok, string postBody, string tema)
    {
        
        _driver.FindElement(By.XPath(ZenStudioXPathSelectors.Add)).Click();
        _driver.FindElement(By.XPath(ZenStudioXPathSelectors.CreatePost)).Click();
        Thread.Sleep(2000);
        if (_driver.FindElements(By.XPath(ZenStudioXPathSelectors.PopUp)).Count > 0)
        {
            var target = _wait.Until(d => d.FindElement(By.XPath(ZenStudioXPathSelectors.PopUp)));
            Actions builder = new Actions(_driver);
            builder.MoveToElement(target, -375, -436).Click().Build().Perform();
        }

        switch (service)
        {
            case "akket.ru" :
                BasicOperationsForAutoPosting.AddPhotoInPost();
                AddTextBody(zagolovok, postBody);
                //DeleteEmptyLinesInNewPost();
                Thread.Sleep(500);
                BasicOperationsForAutoPosting.AddProdazhka(tema);
                break;
            case "YZen" :
                AddTextBody(zagolovok, postBody);
                // DeletEmptyLinesInNewPost();
                BasicOperationsForAutoPosting.AddPhotoInPost();
                BasicOperationsForAutoPosting.AddProdazhka(tema);
                break;
        }
        Thread.Sleep(10000);
    }

    public void AddTextBody(string zagolovok, string postBody)
    {
        var zagolovokInput = _wait.Until(d => d.FindElement(By.XPath(ZenStudioXPathSelectors.Zagolovok)));
        zagolovokInput.SendKeys(zagolovok);
        var textBody = _wait.Until(d => d.FindElement(By.XPath(ZenStudioXPathSelectors.TextBody)));
        textBody.SendKeys(postBody + "\n");
    }


    private static void DeleteEmptyLinesInNewPost()
    {
        ReadOnlyCollection<IWebElement> paragraph = _wait.Until(e =>
            e.FindElements(By.XPath("//div[@class='public-DraftStyleDefault-block public-DraftStyleDefault-ltr']")));
        var count = paragraph.Count;
        //string textText = "";
        for (int i = 0; i < count; i++)
        {
            if (_driver.FindElements(By.XPath("//div[@class='public-DraftStyleDefault-block public-DraftStyleDefault-ltr']"))[i].GetAttribute("data-text") == null)
            {
                _wait.Until(e =>
                    e.FindElements(By.XPath("//div[@class='public-DraftStyleDefault-block public-DraftStyleDefault-ltr']")))[i].SendKeys("\b");
            }
        }
        
    }

    public static async Task PublishPost(string Login)
    {
        Thread.Sleep(3000);
        _wait.Until( d => d.FindElement(By.XPath(ZenStudioXPathSelectors.Opublikovat))).Click();
        Thread.Sleep(1000);
        if (_driver.FindElements(By.XPath(ZenStudioXPathSelectors.SettingsWindowOpened)).Count > 0)
        {
            ChannelSettings();
        }
        if (_driver.FindElements(By.XPath(ZenStudioXPathSelectors.SettingsWindowOpened)).Count > 0)
        {
            _driver.FindElement(By.XPath(ZenStudioXPathSelectors.CloseSettingsWindowButton)).Click();
        }
        //_driver.FindElement(By.XPath(ZenStudioXPathSelectors.Opublikovat)).Click();
        _driver.FindElement(By.XPath(ZenStudioXPathSelectors.PicSelect)).Click();
        _driver.FindElement(By.XPath(ZenStudioXPathSelectors.OpublikovatOk)).Click();
        Thread.Sleep(2000);
        if (_driver.FindElements(By.XPath("//img[@class='captcha__image']")).Count > 0)
        {
            await UiTestBase.InputCaptchaCode("//img[@class='captcha__image']");
            Thread.Sleep(5000);
            _driver.FindElement(By.XPath(ZenStudioXPathSelectors.OpublikovatOk)).Click();
        }
        Thread.Sleep(1000);
        if (_driver.FindElements(By.XPath("//h1[@class='article__title']")).Count > 0) Console.WriteLine($"\t{Login}");
    }
