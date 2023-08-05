namespace SeleniumTikTok;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using SeleniumUndetectedChromeDriver;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

public partial class MainPage : ContentPage
{

    ObservableCollection<LoginObject> loginData = new ObservableCollection<LoginObject>();
    public ObservableCollection<LoginObject> LoginData { get { return loginData; } }
    public MainPage()
	{
		InitializeComponent();
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.Window.MinimumHeight = 800;
		this.Window.MaximumHeight = 800;
        this.Window.MinimumWidth = 800;
        this.Window.MaximumWidth = 800;
    }
    private async void btnLogin_Clicked(object sender, EventArgs e)
    {
        var customFileType = new FilePickerFileType(
                new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                       { DevicePlatform.iOS, new[] { "public.text" } }, // UTType values  
                       { DevicePlatform.Android, new[] { "text/plain" } }, // MIME type  
                       { DevicePlatform.WinUI, new[] { ".txt" } }, // file extension  
                       { DevicePlatform.macOS, new[] { "txt" } },
                });
        var txtlogin = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Chọn file đăng nhập",
            FileTypes = customFileType
        });

        loginData.Clear();
        if (txtlogin != null)
        {
            txtLoginPath.Text = txtlogin.FullPath;
            List<String> logindata = getLoginData(txtLoginPath.Text);
            foreach (var user in logindata)
            {
                loginData.Add(new LoginObject(loginData.Count(), user.Split('|')[0], user.Split('|')[1], "C:\\Users\\hathu\\OneDrive\\Tài liệu\\Datatest\\Videos", "Test tài khoản tải lên"+ loginData.Count(), false, ""));
            }

            btnAction.IsEnabled = true;
        }
        videoobjectCollectionView.ItemsSource = loginData;
    }
    private List<String> getLoginData(string path)
    {
        List<String> r = new List<String>();
        try
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.Split('|').Count() == 3)
                    {
                        r.Add(line);
                    }
                }
            }
        }
        catch (Exception) { }
        return r;

    }

    private void btnAction_Clicked(object sender, EventArgs e)
    {
        List<LoginObject> t = new List<LoginObject>();
        foreach (var item in loginData) { t.Add(item); }
        System.Threading.ThreadPool.QueueUserWorkItem(async delegate
        {
            PostVideoBackJob(t);
        }, null);
    }
    private async void PostVideoBackJob(List<LoginObject> loginobjectdata)
    {
        //Chạy cho từng tài khoản
        foreach (var user in loginobjectdata)
        {
            string username = user.Username;
            string password = user.Password;
            ChromeOptions options = new ChromeOptions();
            #region Init options and extension
            options.AddArguments("--disable-extensions");
            options.AddArguments("--window-size=2020,1880");
            options.AddArguments("--start-maximized");
            options.AddArguments("--proxy-server='direct://'");
            options.AddArguments("--proxy-bypass-list=*");
            options.AddArguments("--start-maximized");
            if (user.isHeadless)
            {
                options.AddArguments("--headless=new");
            }
            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("--allow-running-insecure-content");
            options.AddArguments("--no-sandbox");
            options.AddArgument("--log-level=3");
            options.AddArguments("--mute-audio");
            #endregion

            using (var driver = UndetectedChromeDriver.Create(
          driverExecutablePath:
          await new ChromeDriverInstaller().Install("114.0.5735.16"), options: options, commandTimeout: TimeSpan.FromSeconds(100), hideCommandPromptWindow: true))
            {

                driver.Manage().Window.Size = new System.Drawing.Size(1920, 1380);
                OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(60));
                #region Login Function
                LoginZone(driver, username, password);
                Thread.Sleep(2000);
                wait.Until((x) =>
                {
                    return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
                });
                #endregion
                #region Start Job Video
                #endregion
                List<String> videos = getVideos(user.VideoPath);
                var dateup = DateTime.Now.AddDays(1);
                StartJobVideo(driver, username, password, dateup, user.Caption, videos);
            }
        }

    }
    private void LoginZone(UndetectedChromeDriver driver, string username, string password)
    {
        try
        {
            OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(60));
            driver.GoToUrl("https://www.tiktok.com/login/phone-or-email/email");
            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });

            Random rnd = new Random();
            try // Set Username and password
            {
                foreach (var d in username)
                {
                    try
                    { // With user-agent
                        driver.FindElement(By.Name("username")).SendKeys($"{d}");
                    }
                    catch (Exception) { }

                    Thread.Sleep(rnd.Next(200, 300));
                }
                Actions action = new Actions(driver);
                action.SendKeys(OpenQA.Selenium.Keys.Tab);
                action.Build().Perform();
                foreach (var d in password)
                {
                    action = new Actions(driver);
                    action.SendKeys($"{d}");
                    action.Build().Perform();
                    Thread.Sleep(rnd.Next(200, 300));
                }
                for (int v = 0; v < 3; v++)
                {
                    action = new Actions(driver);
                    action.SendKeys(OpenQA.Selenium.Keys.Tab);
                    action.Build().Perform();
                }
                int i = 0;
                while (true)
                {
                    try
                    {
                        driver.FindElement(By.XPath($"//button[text()='Log in']")).Click();
                    }
                    catch (Exception) { }
                    Thread.Sleep(8000);
                    if (!driver.PageSource.Contains("Maximum number of attempts reached"))
                    {
                        Thread.Sleep(3000);
                        if (driver.PageSource.Contains("Drag the slider to fit the puzzle"))
                        {
                            while (true)
                            {
                                if (!driver.PageSource.Contains("Drag the slider to fit the puzzle"))
                                    break;
                            }
                            Thread.Sleep(3000);
                        }
                        else { break; }
                    }
                }
            }
            catch (Exception ex) { }

            driver.Manage().Timeouts().PageLoad.Add(System.TimeSpan.FromSeconds(20));
        }
        catch (Exception ex) { }
    }

    private void StartJobVideo(UndetectedChromeDriver driver, string username, string password, DateTime dateup, string caption, List<String> videos)
    {
        OpenQA.Selenium.Support.UI.WebDriverWait wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(60));
        Random rnd = new Random();
        foreach (var video in videos)
        {
            driver.GoToUrl("https://www.tiktok.com/creator#/upload?lang=en");

            Thread.Sleep(2000);
            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });
            Thread.Sleep(10000);

            #region Start Upload Video
            driver.FindElement(By.XPath("//input[@type='file']")).SendKeys(video.ToString());
            Thread.Sleep(1000);

            wait.Until((x) =>
            {
                return ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete");
            });
            while (true)
            {
                var status = driver.FindElement(By.ClassName("css-y1m958"));
                if (status != null && status.Enabled) break;
                Thread.Sleep(2000);
            }
            Thread.Sleep(5000);
            try
            {

                driver.FindElement(By.ClassName("css-72rvq0")).Click(); // Click not now to split video.
            }
            catch (Exception ex) { }
            #endregion

            #region Schedule Video                 
            Thread.Sleep(3000);
            try
            {
                driver.FindElement(By.XPath("//input[@id='tux-3']")).Click();
            }
            catch (Exception ex) { }
            try
            {

                driver.FindElement(By.XPath("//div[@id='tux-3']")).Click();
            }
            catch (Exception ex) { }
            #region Turn off slpit alert and allow permission.
            Thread.Sleep(2000);
            try
            {

                driver.FindElement(By.XPath("//div[contains(@class, 'tiktok-modal__modal-button is-highlight')]")).Click();
            }
            catch (Exception ex) { }
            Thread.Sleep(2000);
            try
            {

                driver.FindElement(By.ClassName("css-72rvq0")).Click();
            }
            catch (Exception ex) { }
            Thread.Sleep(2000);
            try
            {

                driver.FindElement(By.XPath("//div[contains(@class, 'tiktok-modal__modal-button is-highlight')]")).Click();
            }
            catch (Exception ex) { }
            Thread.Sleep(2000);
            #endregion
            #endregion

            var hour = dateup.ToString("HH");
            var min = dateup.ToString("mm");


            var t = driver.FindElements(By.XPath("//span[@class='jsx-3471246984']"));
            if (t != null && t.Any())
            {
                foreach (var item in t)
                {
                    if (!string.IsNullOrEmpty(item.Text))
                    {
                        // Check Datetime
                        if (item.Text.Split('-').Count() == 3)
                        {
                            try
                            {
                                if (dateup.Day != DateTime.Now.Day)
                                {
                                    driver.FindElement(By.ClassName("date-picker-input")).Click();
                                    Thread.Sleep(2000);
                                    driver.FindElement(By.XPath($"//span[contains(@class,'jsx-4172176419') and contains(@class,'valid')][text()='{dateup.Day}']")).Click();
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                }
            }
            driver.FindElement(By.ClassName("public-DraftStyleDefault-block")).SendKeys(OpenQA.Selenium.Keys.Control + "a");
            driver.FindElement(By.ClassName("public-DraftStyleDefault-block")).SendKeys(OpenQA.Selenium.Keys.Delete);
            foreach (var d in caption)
            {
                driver.FindElement(By.ClassName("public-DraftStyleDefault-block")).SendKeys(d.ToString());
                Thread.Sleep(rnd.Next(200, 300));
            }
            Thread.Sleep(2000);
            driver.FindElement(By.ClassName("css-y1m958")).Click();
            Thread.Sleep(10000);
        }
    }
    public List<String> getVideos(string path)
    {
        List<String> r = new List<String>();
        foreach (string f in Directory.GetFiles(path))
        {
            if (f.Contains(".mp4"))
                r.Add(f);
        }
        return r;
    }
}

