using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AlexaUWP
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        private Library Library;
        public SettingsDialog(Library library)
        {
            Library = library;
            this.InitializeComponent();
            url.Text = library.alexaWebUri.ToString();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //save
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values["alexaWebUri"] = url.Text;
            Library.alexaWebUri = new Uri(url.Text);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
