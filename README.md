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
+ Typing username / password with random speed like human
```
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
```
+ Login into tiktok and try until success
```
	while (true)
	{
		Thread.Sleep(2000);
		driver.FindElement(By.ClassName("tiktok-11sviba-Button-StyledButton")).Click();
		Thread.Sleep(2000);
		if (!driver.PageSource.Contains("Maximum number of attempts reached"))
			break;                 
	}
```
+ Set video path
```
 	driver.FindElement(By.XPath("//input[@type='file']")).SendKeys(@$"{VideoPath.Text}");
	Thread.Sleep(5000);
	wait.Until((x) =>
	{
		return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
	});
```
