# Research Download all video from tiktok profile (without tiktok watermark) by Selenium.
Tech: Net 7.0 / Maui. 

1/ Tools -> NuGet Package Manager -> Manage NuGet Packages for Solution:
  + Selenium.WebDriver
  + Selenium.Support
  + Selenium.WebDriver.ChromeDriver
  + Cause tiktok detect selenium so we should install Selenium.UndetectedChromeDriver

2/ Logic:

  + Init driver 
```
  using (var driver = UndetectedChromeDriver.Create( driverExecutablePath: await new ChromeDriverInstaller().Auto()))
	{
		driver.GoToUrl(url);
        	var htmlbody = driver.PageSource;
	}
```
 + Allow driver run background.
```
	ChromeOptions options = new ChromeOptions();
	options.AddArguments("--headless");
```
+ Auto scroll until reach all video
```
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
		if (currentlocation == lastlocation) break;
		lastlocation = currentlocation;
	}
```

