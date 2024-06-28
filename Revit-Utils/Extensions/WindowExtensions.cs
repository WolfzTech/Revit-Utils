﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows;
using System.IO.Packaging;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace WT.Revit.Extensions
{

#if NETCOREAPP3_1_OR_GREATER
    public static partial class WindowExtensions
    {
        [LibraryImport("user32.dll")]
#else
    public static class WindowExtensions
    {
        [DllImport("user32.dll")]
#endif
        [return: MarshalAs(UnmanagedType.Bool)]

#if NETCOREAPP3_1_OR_GREATER
        private static partial bool SetForegroundWindow(IntPtr hWnd);
#else
        private static extern bool SetForegroundWindow(IntPtr hWnd);
#endif


        private static void SetActivateWindow(object sender, CancelEventArgs e)
        {
            SetActivateWindow();
        }

        public static void SetRevitAsWindowOwner(this Window window)
        {
            if (window == null)
            {
                return;
            }

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var helper = new WindowInteropHelper(window)
            {
                Owner = GetActivateWindow()
            };

            window.Closing += SetActivateWindow;
        }

        /// <summary>
        /// return active windows is active
        /// </summary>
        /// <returns>Revit Window Handle</returns>
        private static IntPtr GetActivateWindow()
        {
            return RevitApp.UiApp?.MainWindowHandle ?? Process.GetCurrentProcess().MainWindowHandle;
        }

        /// <summary>
        /// Set process revert use revit
        /// </summary>
        /// <returns></returns>
        private static void SetActivateWindow()
        {
            IntPtr ptr = GetActivateWindow();
            if (ptr != IntPtr.Zero)
            {
                SetForegroundWindow(ptr);
            }
        }

        public static void LoadViewFromUri(this Window window, string baseUri)
        {
            try
            {
                var resourceLocater = new Uri(baseUri, UriKind.Relative);
                var exprCa = (PackagePart)typeof(Application).GetMethod("GetResourceOrContentPart", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [resourceLocater]);
                var stream = exprCa.GetStream();
                var uri = new Uri((Uri)typeof(BaseUriHelper).GetProperty("PackAppBaseUri", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null), resourceLocater);
                var parserContext = new ParserContext
                {
                    BaseUri = uri
                };
                typeof(XamlReader).GetMethod("LoadBaml", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, [stream, parserContext, window, true]);
            }
            catch (Exception)
            {
                //log
            }
        }
    }
}
