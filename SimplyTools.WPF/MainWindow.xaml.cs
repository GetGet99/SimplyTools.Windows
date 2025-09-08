extern alias wv2;

using Microsoft.UI.Input;
using Microsoft.UI.Windowing;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Windows.ApplicationModel;
using Windows.Graphics;
using Windows.UI.ViewManagement;
using WinWrapper.Windowing;
using wv2::Microsoft.Web.WebView2.Core;
using wv2::Microsoft.Web.WebView2.Wpf;
using Win = WinWrapper.Windowing.Window;
using Window = System.Windows.Window;

namespace SimplyTools.WPF;

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

        //Loaded += delegate
        //{
        //    incps = InputNonClientPointerSource.GetForWindowId(new((ulong)w.Handle));
        //};

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
        }
        catch
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
            }
            catch
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
        var env = await CoreWebView2Environment.CreateAsync(options: new()
        {
            AdditionalBrowserArguments = "--flag-switches-begin --enable-features=AIPromptAPI --flag-switches-end",
            ScrollBarStyle = CoreWebView2ScrollbarStyle.FluentOverlay
        });
        await webView.EnsureCoreWebView2Async(env);
        webView.CoreWebView2.Settings.UserAgent = $"SimplyTools/Windows {webView.CoreWebView2.Settings.UserAgent}";
        webView.CoreWebView2.Settings.IsZoomControlEnabled = false;
        webView.CoreWebView2.Settings.IsGeneralAutofillEnabled = false;
        webView.CoreWebView2.Settings.IsPasswordAutosaveEnabled = false;
        webView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        webView.CoreWebView2.Settings.IsPinchZoomEnabled = false;
        webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
#if !DEBUG
        webView.CoreWebView2.Settings.IsStatusBarEnabled = false;
        webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
#endif
        Directory.SetCurrentDirectory(Package.Current.InstalledLocation.Path);
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping("simplytools.local",
            "./Assets/web",
            CoreWebView2HostResourceAccessKind.Allow);
#if DEBUG
        webView.CoreWebView2.Navigate("http://localhost:3000");
        //webView.CoreWebView2.Navigate("edge://flags");
#else
        webView.CoreWebView2.Navigate("https://getget99.github.io/SimplyTools");
#endif
        webView.CoreWebView2.DocumentTitleChanged += delegate
        {
            w.TitleText = webView.CoreWebView2.DocumentTitle;
        };
        w.TitleText = webView.CoreWebView2.DocumentTitle;
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
            else if (e.HttpStatusCode is 0)
            {
                var src = webView.CoreWebView2.Source;
#if DEBUG
                if (src.StartsWith("http://localhost:3000/"))
                    webView.CoreWebView2.Navigate($"https://simplytools.local/{src["http://localhost:3000/".Length..]}");
#endif
                if (src is "https://simplytools.local/" or "https://simplytools.local")
                    webView.CoreWebView2.Navigate("https://simplytools.local/index.html");
                if (src is "https://getget99.github.io/SimplyTools" or "https://getget99.github.io/SimplyTools/")
                    webView.CoreWebView2.Navigate("https://simplytools.local/index.html");
                if (src.StartsWith("https://getget99.github.io/SimplyTools/"))
                    webView.CoreWebView2.Navigate($"https://simplytools.local/{src["https://getget99.github.io/SimplyTools/".Length..]}");
            }
        };
        webView.CoreWebView2.NewWindowRequested += (sender, e) =>
        {
            e.Handled = true;
            Process.Start(new ProcessStartInfo() { FileName = e.Uri, UseShellExecute = true });
        };
        webView.WebMessageReceived += (sender, e) =>
        {
            if (!e.Source.StartsWith("https://simplytools.local/") && !e.Source.StartsWith("https://getget99.github.io/SimplyTools/")
             &&
#if DEBUG
             !e.Source.StartsWith("http://localhost:3000/")
#endif
             )
            {
                return;
            }

            try
            {
                var payload = JsonNode.Parse(e.WebMessageAsJson);
                var API = payload?["$api"];
                var Request = payload?["$request"];
                var api = API?.GetValueKind() is not JsonValueKind.String ? null : API.GetValue<string>();

                try
                {
                    switch (api)
                    {
                        case "titlebar.setDragRegion":
                            var dragregion = payload!["dragregion"]?.AsArray();
                            var passthrough = payload!["passthrough"]?.AsArray();

                            if (dragregion is null || passthrough is null)
                            {
                                Error("invalid parameters");
                                return;
                            }

                            // Convert JSON arrays to RectInt32[]
                            var dr = new RectInt32[dragregion.Count];
                            for (int i = 0; i < dragregion.Count; i++)
                            {
                                var reg = dragregion[i]?.AsArray();
                                if (reg == null || reg.Count != 4)
                                {
                                    ErrorInvalidArguments();
                                    return;
                                }
                                dr[i] = new RectInt32(
                                    reg[0]?.GetValue<int>() ?? 0,
                                    reg[1]?.GetValue<int>() ?? 0,
                                    reg[2]?.GetValue<int>() ?? 0,
                                    reg[3]?.GetValue<int>() ?? 0
                                );
                            }

                            var pt = new RectInt32[passthrough.Count];
                            for (int i = 0; i < passthrough.Count; i++)
                            {
                                var reg = passthrough[i]?.AsArray();
                                if (reg == null || reg.Count != 4)
                                {
                                    ErrorInvalidArguments();
                                    return;
                                }
                                pt[i] = new RectInt32(
                                    reg[0]?.GetValue<int>() ?? 0,
                                    reg[1]?.GetValue<int>() ?? 0,
                                    reg[2]?.GetValue<int>() ?? 0,
                                    reg[3]?.GetValue<int>() ?? 0
                                );
                            }

                            // Call the API on InputNonClientPointerSource
                            //incps.SetRegionRects(NonClientRegionKind.Caption, dr);
                            //incps.SetRegionRects(NonClientRegionKind.Passthrough, pt);
                            aw.TitleBar.SetDragRectangles(dr);
                            break;

                        default:
                            Error("API not found");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Error($"Internal Error: {ex.Message}");
                }

                void Error(string message)
                {
                    if (Request == null) return;
                    webView.CoreWebView2.PostWebMessageAsJson($$"""
                    {
                        "$request": {{Request.ToJsonString()}},
                        "error": "{{message}}"
                    }
                    """);
                }

                void ErrorInvalidArguments() => Error("Invalid Arguments");
            } catch
            {

            }
        };
        uisettings.ColorValuesChanged += delegate
        {
            UpdateTheme();
        };
    }
    InputNonClientPointerSource incps;
}
