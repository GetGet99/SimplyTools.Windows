extern alias wv2;

using Microsoft.UI.Windowing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.UI.ViewManagement;
using WinWrapper.Windowing;
using wv2::Microsoft.Web.WebView2.Core;
using wv2::Microsoft.Web.WebView2.Wpf;
using Win = WinWrapper.Windowing.Window;
using Window = System.Windows.Window;

namespace SimpleTools.WPF;

public partial class MainWindow : Window
{
    private WebView2 webView;
    Win w;
    AppWindow aw;
    public MainWindow()
    {
        InitializeComponent();

        webView = new WebView2
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            DefaultBackgroundColor = System.Drawing.Color.Transparent
        };

        Content = webView;

        InitAsync();
    }
    private async void InitAsync()
    {
        var interop = new WindowInteropHelper(this);
        interop.EnsureHandle(); // Ensure the window handle is created
        w = Win.FromWindowHandle(interop.Handle);
        HwndSource mainWindowSrc = HwndSource.FromHwnd(w.Handle);
        mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb(0, 0, 0, 0);

        aw = AppWindow.GetFromWindowId(new((ulong)w.Handle));
        try
        {
            aw.TitleBar.ExtendsContentIntoTitleBar = true;
        } catch
        {
            // API not avaliable
        }
        var uisettings = new UISettings();
        aw.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(0x3a, 0x3a, 0x3a, 0x4c);
        void UpdateTheme()
        {
            try
            {
                if (uisettings.GetColorValue(UIColorType.Background).R < 255 / 2)
                {
                    // dark mode
                    w.DwmAttribute.Set(DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, 1);
                    aw.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 0, 0, 0);
                }
                else
                {
                    // light mode
                    w.DwmAttribute.Set(DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, 0);
                    aw.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(0, 255, 255, 255);
                }
            } catch
            {
                // API not avaliable, ignore
            }
        }
        UpdateTheme();
        try
        {
#pragma warning disable CA1416 // Validate platform compatibility
            w.DwmAttribute.SystemBackdrop = WinWrapper.Windowing.Dwm.SystemBackdropTypes.MainWindow;
            PInvoke.Methods.ExtendFrame(w.Handle, new()
            {
                cxLeftWidth = -1,
                cxRightWidth = -1,
                cyTopHeight = -1,
                cyBottomHeight = -1
            });
#pragma warning restore CA1416 // Validate platform compatibility
        }
        catch
        {
            // system backdrop not supported
        }

        await webView.EnsureCoreWebView2Async();
        webView.CoreWebView2.Settings.UserAgent = $"SimpleTools/Windows {webView.CoreWebView2.Settings.UserAgent}";
        webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
        webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
        webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
        webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        webView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
        webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping("simpletools.local", "./Assets/web", CoreWebView2HostResourceAccessKind.Allow);
        webView.CoreWebView2.Navigate("http://simpletools.local/index.html");
        webView.NavigationCompleted += async (s, e) =>
        {
            if (e.IsSuccess)
            {
                var accent = uisettings.GetColorValue(UIColorType.Accent);
                var accentlight1 = uisettings.GetColorValue(UIColorType.AccentLight2);
                var accentlight2 = uisettings.GetColorValue(UIColorType.AccentLight3);
                var accentdark1 = uisettings.GetColorValue(UIColorType.AccentDark1);
                await webView.CoreWebView2.ExecuteScriptAsync($$"""
                    (function () {
                        document.body.classList.add('app');
                        let s = document.createElement('style');
                        s.innerHTML = `
                        body.app {
                            --app-titlebar-height: {{aw.TitleBar.Height}}px;
                            --color-accent: rgba({{accent.R}}, {{accent.G}}, {{accent.B}}, {{accent.A}});
                            --color-accent-light-1: rgba({{accentlight1.R}}, {{accentlight1.G}}, {{accentlight1.B}}, {{accentlight1.A}});
                            --color-accent-light-2: rgba({{accentlight2.R}}, {{accentlight2.G}}, {{accentlight2.B}}, {{accentlight2.A}});
                            --color-accent-dark-1: rgba({{accentdark1.R}}, {{accentdark1.G}}, {{accentdark1.B}}, {{accentdark1.A}});
                        }`.trim();
                        document.head.appendChild(s);
                    })()
                    """);
            }
            else
            {
                // Handle navigation failure
            }
        };
        uisettings.ColorValuesChanged += delegate
        {
            UpdateTheme();
        };
    }
}