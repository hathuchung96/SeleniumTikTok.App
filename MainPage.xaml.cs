namespace SeleniumTikTok;
using Newtonsoft.Json.Linq;
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
		options.AddArguments("--headless");
        options.AddArguments("disable-popup-blocking");
        options.AddArguments("--disable-extensions");
        options.AddArguments("--silent");
        options.AddArguments("--log-lebel=3");
        using (var driver = UndetectedChromeDriver.Create(
			driverExecutablePath:
			await new ChromeDriverInstaller().Auto(),options:options))
		{

			driver.GoToUrl(url); //"https://www.tiktok.com/@jinzng169"
			var htmlbody = driver.PageSource;
			string key = $"{driver.Url.Split('/')[3]}/video";
			var datalist = ExtractLink(htmlbody).Where(x => x.Contains(key));
			foreach (var d in datalist)
			{
				var videourl = d.Split('/');
				using (WebClient client = new WebClient())
				{
					var urlt = $"https://api16-normal-c-useast1a.tiktokv.com/aweme/v1/feed/?aweme_id={videourl[videourl.Count() - 1]}";
					string htmlCode = client.DownloadString(urlt);
					Err.Text += $"{GetDownloadUrl(htmlCode)}\n";
				}
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
}

