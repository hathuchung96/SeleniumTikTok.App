namespace SeleniumTikTok;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumUndetectedChromeDriver;
using System.Text.RegularExpressions;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OpenTikTok_Clicked(object sender, EventArgs e)
	{
		var url = UrlTikTok.Text.Trim();
		if (String.IsNullOrEmpty(url) || url.Split('/').Count() != 4)
		{
			Err.Text = "Invalid Tiktok url!";
			Err.TextColor= Colors.Red;
            return;
		}

		using (var driver = UndetectedChromeDriver.Create( driverExecutablePath: await new ChromeDriverInstaller().Auto()))
		{

			driver.GoToUrl(url); //"https://www.tiktok.com/@jinzng169"

            var htmlbody = driver.PageSource;
			string key = $"{driver.Url.Split('/')[3]}/video";
			var datalist = ExtractLink(htmlbody).Where(x => x.Contains(key));
			foreach (var d in datalist)
			{
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

    private void UrlTikTok_TextChanged(object sender, TextChangedEventArgs e)
    {
        Err.Text = "";;
    }
}

