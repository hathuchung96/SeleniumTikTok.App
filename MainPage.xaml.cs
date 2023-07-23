namespace SeleniumTikTok;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;
using System;
using System.Net;
using System.Text.RegularExpressions;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}
	private string urlKey = "url_list\":[\"";

	private async void OpenTikTok_Clicked(object sender, EventArgs e)
	{
		var url = UrlTikTok.Text.Trim();
		if (String.IsNullOrEmpty(url) || url.Split('/').Count() != 4)
		{
			Err.Text = "Invalid Tiktok url!";
			Err.TextColor = Colors.Red;
			return;
		}
		ChromeOptions options = new ChromeOptions();
		//options.AddArguments("--headless");
		options.AddArguments("disable-popup-blocking");
		options.AddArguments("--disable-extensions");
		options.AddArguments("--silent");
		options.AddArguments("--log-lebel=3");
		using (var driver = UndetectedChromeDriver.Create(
			driverExecutablePath:
			await new ChromeDriverInstaller().Auto(), options: options))
		{

			driver.GoToUrl(url); //"https://www.tiktok.com/@jinzng169"
			int lastlocation = -1;
			OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30));
			wait.Until((x) =>
			{
				return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
			});

			while (true)
			{
				IJavaScriptExecutor jswait = (IJavaScriptExecutor)driver;
				driver.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

				var currentlocation = int.Parse(driver.ExecuteScript("return window.pageYOffset").ToString());
                //bool security = driver.PageSource.Contains("security-capcha-"); -- Check security capcha
				Thread.Sleep(2000);
                System.Diagnostics.Debug.WriteLine($"Current height {currentlocation}");
                if (currentlocation == lastlocation) break;
				lastlocation = currentlocation;
			}

			string key = $"{driver.Url.Split('/')[3]}/video";
			var datalist = ExtractLink(driver.PageSource).Where(x => x.Contains(key));
            Err.Text += $"Total Videos: {datalist.Count()}\n";
            foreach (var d in datalist)
			{
				/*
                var videourl = d.Split('/');
                using (WebClient client = new WebClient())
                {
                    var urlt = $"https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/?aweme_id={videourl[videourl.Count() - 1]}";
                    string htmlCode = client.DownloadString(urlt);
                    Err.Text += $"{GetDownloadUrl(htmlCode)}\n";
                }
				*/
                Err.Text += $"{d}\n";
            }
		}

		Err.TextColor = Colors.Green;
	}

	private List<string> ExtractLink(string data)
	{
		List<string> r = new List<string>();
		Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);
		if (regex.IsMatch(data))
		{
			foreach(Match match in regex.Matches(data))
			{
				r.Add(match.Groups[1].Value);
			}
		}
		return r;
	}
	private string GetDownloadUrl(string htmlCode)
	{
		try
		{
            var jsond = JObject.Parse(htmlCode);
            return jsond["aweme_list"][0]["video"]["play_addr"]["url_list"][0].ToString();
        }
		catch (Exception) { }
		return "";
    }

    private void UrlTikTok_TextChanged(object sender, TextChangedEventArgs e)
    {
        Err.Text = "";
    }

	private async void LoginTikTok_Clicked(object sender, EventArgs e)
	{
		ChromeOptions options = new ChromeOptions();
		//options.AddArguments("--headless");
		options.AddArguments("disable-popup-blocking");
		options.AddArguments("--disable-extensions");
		options.AddArguments("--silent");
		options.AddArguments("--log-lebel=3");
		using (var driver = UndetectedChromeDriver.Create(
			driverExecutablePath:
			await new ChromeDriverInstaller().Auto(), options: options))
		{
			driver.GoToUrl("https://www.tiktok.com/login/phone-or-email/email"); //https://www.tiktok.com/login/phone-or-email/email https://www.tiktok.com/foryou
			OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30));
			wait.Until((x) =>
			{
				return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
			});

            Random rnd = new Random();
            try // Set Username and password
			{
				foreach(var d in Username.Text)
				{
                    driver.FindElement(By.ClassName("tiktok-11to27l-InputContainer")).SendKeys(d.ToString());
                    Thread.Sleep(rnd.Next(200,300));
                }
                Thread.Sleep(1000);
                foreach (var d in Password.Text)
                {
                    driver.FindElement(By.ClassName("tiktok-wv3bkt-InputContainer")).SendKeys(d.ToString());
                    Thread.Sleep(rnd.Next(200, 300));
                }
				while (true)
				{
                    Thread.Sleep(2000);
                    driver.FindElement(By.ClassName("tiktok-11sviba-Button-StyledButton")).Click();
                    Thread.Sleep(2000);
                    if (!driver.PageSource.Contains("Maximum number of attempts reached"))
						break;                 
                }
            }
			catch (Exception ex) { }
            Thread.Sleep(5000);
            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });

            /* Old logic code, after finish load, swith driver to iframe driver.switchTo().frame("iFrameName");
			driver.GoToUrl("https://www.tiktok.com/upload?lang=en");
			wait.Until((x) =>
			{
				return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
			});
			Thread.Sleep(10000);
			System.Diagnostics.Debug.WriteLine(driver.PageSource);
			driver.FindElement(By.ClassName("css-1db5cpb")).Click();
			*/
            driver.GoToUrl("https://www.tiktok.com/creator#/upload?lang=en");
            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });	
            Thread.Sleep(5000);
            
            //driver.FindElement(By.ClassName("css-1db5cpb")).Click();
            driver.FindElement(By.XPath("//input[@type='file']")).SendKeys(@$"{VideoPath.Text}");
            Thread.Sleep(5000);

            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });

            Thread.Sleep(1000);

            foreach (var d in Title.Text.ToString())
            {
                driver.FindElement(By.ClassName("public-DraftStyleDefault-block")).SendKeys(d.ToString());
                Thread.Sleep(rnd.Next(200, 300));
            }

            driver.FindElement(By.ClassName("css-y1m958")).Click();

            Thread.Sleep(5000);
        };
	}
}

