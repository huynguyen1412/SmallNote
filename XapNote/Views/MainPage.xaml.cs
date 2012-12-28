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
using System.IO;
using Microsoft.Phone.Shell;

namespace XapNote
{
    public partial class MainPage : PhoneApplicationPage
    {
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
        // 建構函式
        public MainPage()
        {
            InitializeComponent();
        }

        private void AppBar_Add_Click(object sender, EventArgs e)
        {
            //跳转到Add.xaml页面去
            String uri = String.Format("/XapNote;component/Views/{0}", "Add.xaml");
            NavigateTo(uri);
        }

        private void AppBar_Help_Click(object sender, EventArgs e)
        {
            //MessageBoxButton mbb = new MessageBoxButton();
            createNewFile("1.txt", "helloWorld!");
            //显示一个Canvas对话框
            String MessageBoxText = "By yangqinjiang!";
            String MessageCaption = "About Xap Notes!";
            MessageBox.Show(MessageBoxText, MessageCaption, new MessageBoxButton());
        }
        //导航到某个页面
        private void NavigateTo(String uri)
        {
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
        //从独立存储中遍历所有文件，返回文件名集合
        private String[] FindAllFileName()
        {
            //得到IsolatedStorageFile里面的UserStore对象
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            //得到appStorage里面的所有文件名
            return appStorage.GetFileNames();
        }
        //程序启动时，执行这个方法
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            //得到文件名集合（数组）,并且绑定到noteListBox列表中
            noteListBox.ItemsSource=FindAllFileName();
            //当程序运行时，加载运行的状态值
            string state = "";
            if (settings.Contains("state"))
            {
                    if(settings.TryGetValue<string>("state",out state))
                    {
                        if(state=="add")
                        {
                            //导航到Add.xaml页面
                            String uri = String.Format("/XapNote;component/Views/{0}", "Add.xaml");
                            NavigateTo(uri);
                        }
                        else if (state=="edit")
                        {
                            //导航到ViewEdit.xaml页面
                            String uri = String.Format("/XapNote;component/Views/{0}", "ViewEdit.xaml");
                            NavigateTo(uri);
                        }

                    }
                    
            }

        }
        //创建新的文件
        private void createNewFile(string fileName, string fileContent)
        {
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (!appStorage.FileExists(fileName))//判断文件是否存在？
            {
                using (var file = appStorage.CreateFile(fileName))//文件不存在则新建文件名为fileName的文件
                {
                    using (var writer = new StreamWriter(file))
                    {
                        writer.WriteLine(fileContent);
                        writer.Close();
                    }
                }
            }

        }

        //当点击noteListBox上的某一项时，执行这个方法
        //再取出文件名，Navigate到查看文件页面
        private void noteListBox_Item_Click(object sender, RoutedEventArgs e)
        {
            
            //转换类型
            HyperlinkButton fileNameHyBtn = (HyperlinkButton)sender;
            //新建Uri  带有一个文件名，并跳转到ViewEdit.xaml页面
            String uri = String.Format("/XapNote;component/Views/{0}?fileName={1}", "ViewEdit.xaml", fileNameHyBtn.Content);
            NavigateTo(uri);
        }

    }
}