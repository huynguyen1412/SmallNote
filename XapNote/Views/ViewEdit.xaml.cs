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

namespace XapNote.Views
{
    public partial class ViewEdit : PhoneApplicationPage
    {
        //从MainPage中noteListBox列表中传递过来的文件名,
        //在PhoneApplicationPage_Loaded中取得了
        private String fileName="";
        //生成这两个对象，用来保存程序运行时的状态值
        IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
        PhoneApplicationService phoneAppService = PhoneApplicationService.Current;
        public ViewEdit()
        {
            InitializeComponent();
           
        }
        #region  菜单栏上的单击事件
        //点击了菜单栏上的退回按钮,返回到MainPage.xaml页面
        private void AppBar_Back_Click(object sender, EventArgs e)
        {
            NavigateBack();
        }
        //点击了菜单栏上的编辑按钮
        private void AppBar_Edit_Click(object sender, EventArgs e)
        {
                    //判断ViewEditTextBlock是否可见
            if (ViewEditTextBlock.Visibility == Visibility.Visible)//可见
            {
                bindEdit(ViewEditTextBlock.Text);//将ViewEditTextBlock的文字显示在ViewEditBox上
            }
        }
        //点击了菜单栏上的保存按钮
        private void AppBar_Save_Click(object sender, EventArgs e)
        {
            if (ViewEditTextBox.Visibility==Visibility.Visible)
            {
                //保存文件内容
                //文件名还是一样
                SaveFile(fileName, ViewEditTextBox.Text);
                //将ViewEditTextBlock展示框设置为不可见
                ViewEditTextBlock.Visibility = Visibility.Visible;
                //将ViewEditTextBox编辑框设置为可见
                ViewEditTextBox.Visibility = Visibility.Collapsed;
                //显示文字
                ViewEditTextBlock.Text = ViewEditTextBox.Text;
            }
            NavigateBack();
           
        }
        //点击了菜单栏上的删除按钮
        private void AppBar_Delete_Click(object sender, EventArgs e)
        {
            //删除以fileName为文件名的文件
            if(ViewEditTextBlock.Visibility==Visibility.Visible)
            {
            
                if (DeleteFile(fileName))
                {
                    //TODO 2012.3.19
                   // MessageBox.Show("delete!");
                    ViewEditTextBlock.Text = "";
                    ViewEditTextBox.Text = "";
                    NavigateBack();
                } 
               }
        }
        #endregion
        //导航到MainActivity页面
        private void NavigateBack()
        {
            //跳转到MainPage.xaml页面去
            String uri = String.Format("/XapNote;component/Views/{0}", "MainPage.xaml");
            NavigateTo(uri);
            settings["state"] = "";
            settings["value"] = "";
            settings[fileName]="";
        }
        //导航到某个页面
        private void NavigateTo(String uri)
        {
            NavigationService.Navigate(new Uri(uri, UriKind.Relative));
        }
        //当ViewEdit.xaml页面启动时
        //使用传递过来的文件名来加载文件内容
        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            string state = "";
            if (settings.Contains("state"))
            {
                if (settings.TryGetValue<string>("state", out state))
                {
                    if (state == "edit")//如果ViewEdit页面处于编辑状态
                    {
                        //下面的代码逻辑性较强，小心
                        string value = "";
                        if (settings.Contains("fileName"))  //先取得fileName
                        {
                            if (settings.TryGetValue<string>("fileName", out value))
                            {
                                fileName = value;
                            }
                        }
                        
                        if (settings.Contains("value"))//再取得文件内容value
                        {
                            if (settings.TryGetValue<string>("value", out value))
                            {
                                //将value的值显示在ViewEditTextBox上
                                bindEdit(value);
                            }


                        }
                    }
                    else//如果ViewEdit页面处于查看状态
                    {
                        bindView();
                    }
                }
            }
            else
            {
                bindView();
            }
           
            }
        private void bindView()
        {
            //取得  从MainPage页面noteListBox列表中跳转过来的 数据
            //1.fileName
            fileName = NavigationContext.QueryString["fileName"];
            //设置文件标题
            //ViewEditTextBlock.Text = String.Format("fileName:  {0}", fileName);

            //2.根据fileName从独立存储区中找到匹配的文件名,并打开
            //调用这个方法
            ViewEditTextBlock.Text = LoadFile(fileName);
        
        }
        private void bindEdit(string content)
        {
    
                //将ViewEditTextBlock展示框设置为不可见
                ViewEditTextBlock.Visibility = Visibility.Collapsed;
                //将ViewEditTextBox编辑框设置为可见
                ViewEditTextBox.Visibility = Visibility.Visible;
                //显示文字
                ViewEditTextBox.Text = content;
                //使得ViewEditTextBox聚焦
                ViewEditTextBox.Focus();
                //再将光标定位到文本后面
                ViewEditTextBox.SelectionStart = ViewEditTextBox.Text.Length;

                settings["state"] = "edit";
                settings["value"] = ViewEditTextBox.Text;
                settings["fileName"] = fileName;
        }

        #region 对文件的操作  
        //根据fileName从独立存储区中找到匹配的文件名,返回string
        private String LoadFile(String fileName)
        {
            String s="";
            //首先取得使用者的独立存储区
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                try
                {
                    //生成一个读取数据流
                    using (StreamReader sr = new StreamReader(store.OpenFile(fileName, FileMode.Open)))
                    {
                        s = sr.ReadToEnd();
                        sr.Close();
                        return s;
                    }
                }
                catch (Exception)
                {
                    return ( "没有在手机中找到所需要的文件名为" + fileName + "的文件!!");

                }
            }
        }

        //保存文件
        private void SaveFile(String fileName, String FileContent)
        {
            //首先取得使用者的独立存储区，
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            if (appStorage.FileExists(fileName))//文件存在
            {
                //陈述式的代码块
                //OpenFile(...)中的参数，第1是文件名称，第2个是文件生成模式，第3个是文件访问模式
                ///创建独立存储文件流
                using (var file = appStorage.OpenFile(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
                {
                    //生成一个写入流
                    using (var writer = new StreamWriter(file))
                    {
                        try
                        {
                            //再将TextBox中的内容写进文件中
                            writer.Write(FileContent);
                            writer.Close();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            else//文件不存在
            {
                using (var newFile = appStorage.CreateFile(fileName))
                {
                    using (var writer = new StreamWriter(newFile))
                    {
                        writer.WriteLine(FileContent);
                        writer.Close();
                    }
                }
            }
        }
        
        //删除文件
        private Boolean DeleteFile(String fileName)
        {
            //首先取得使用者的独立存储区，
            var appStorage = IsolatedStorageFile.GetUserStoreForApplication();
            //判断文件是否存在
            if (appStorage.FileExists(fileName))
            {
                appStorage.DeleteFile(fileName);
                return true;
            }
            else
            {
                //文件不存在
                return false;
            }
           
        }

        #endregion

        //当ViewEditTextBox里的文字改变时,保存它的文字到static["ViewEditValue"]
        private void ViewEditTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            //TODO 2012.3.20
            if(ViewEditTextBox.Visibility==Visibility.Visible)//处于编辑状态
            {
                //phoneAppService.State["ViewEditValue"] = ViewEditTextBox.Text;
                settings["value"] = ViewEditTextBox.Text;
            }
           
        }
    }
}