# Research Download tiktok videos with Selenium.
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

