using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyChat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Client.Instance.Initialize();
            MainPage = new FirstPage();

        }
    }
}
