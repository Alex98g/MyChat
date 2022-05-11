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
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "account.txt");
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
                MainPage = new FirstPage();
            } else
            {
                List<string> lst = File.ReadAllLines(fileName).ToList();
                if (lst[3] == "false")
                {
                    MainPage = new FirstPage();
                }
                else { MainPage = new MainPage(); }
            }
            
        }
    }
}
