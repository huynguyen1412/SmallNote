using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.IO.IsolatedStorage;

namespace XapNote.Views
{
    public partial class Add : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        public Add()
        {
            InitializeComponent();
        }
        //点击菜单栏上的取消按钮时
        private void AppBar_Cancel_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }
        //点击菜单栏上的保存按钮时
        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }
        //返回到MainPage.xaml页面中
        private void NavigateBack()
        {
            //清除状态值
            settings["state"] = "";
            settings["value"] = "";
            //跳转到Add.xaml页面去
            String uri = String.Format("/XapNote;component/Views/{0}", "MainPage.xaml");
            NavigateTo(uri);
        }
        //导航到某个页面
        private void NavigateTo(String uri)
        {

            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            string state="";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state",out state))
                {
                    if(state=="add")
                    {
                        string value = "";
                        if(settings.Contains("value"))
                        {
                            if (settings.TryGetValue<string>("value", out value))
                            {
                                editTextBox.Text = value;
                            }
                        }
                    }
                }
            }

            settings["state"] = "add";//设置状态值
            settings["value"] = editTextBox.Text;
        }
        //当文件内容改变时，保存editTextBox.Text到 settings["value"]中
        private void editTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            settings["value"] = editTextBox.Text;
        }
    }
}