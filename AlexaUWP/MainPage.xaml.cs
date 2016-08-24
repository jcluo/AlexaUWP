﻿using System;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AlexaUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BitmapImage blueIcon;
        private BitmapImage greenIcon;
        private BitmapImage whiteIcon;
        private BitmapImage purpleIcon;
        public Library Library = new Library();

        public MainPage()
        {
            this.InitializeComponent();
            var localsettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localsettings.Values["alexaWebUri"]==null)
                localsettings.Values["alexaWebUri"] = "http://192.168.2.55:5000";
            Library.alexaWebUri = new Uri((string)localsettings.Values["alexaWebUri"]);
            blueIcon = new BitmapImage(new Uri("ms-appx:///Assets/logo-blue.png"));
            greenIcon = new BitmapImage(new Uri("ms-appx:///Assets/logo-green.png"));
            whiteIcon = new BitmapImage(new Uri("ms-appx:///Assets/logo-white.png"));
            purpleIcon = new BitmapImage(new Uri("ms-appx:///Assets/logo-purple.png"));            
            recordButton.Source = blueIcon;
        }

        private void StartRecord(object sender, RoutedEventArgs e)
        {
            recordButton.Source = whiteIcon;
            Library.Record();
        }

        private async void StopRecord(object sender, RoutedEventArgs e)
        {
            recordButton.Source = blueIcon;
            InMemoryRandomAccessStream buffer = await Library.Stop();
            if (buffer!=null)
            {
                recordButton.Source = greenIcon;
                playback.SetSource(buffer, "audio/mpeg");
            }
        }

        private void onMediaEnded(object sender, RoutedEventArgs e)
        {
            recordButton.Source = blueIcon;
        }

        private async void onButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new SettingsDialog(Library);
            await dialog.ShowAsync();
        }
    }
}

