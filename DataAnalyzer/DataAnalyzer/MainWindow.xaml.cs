using System;
using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using DataAnalyzer.UserControls;

namespace DataAnalyzer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //инициализация и завершение
        public MainWindow()
        {
            InitializeComponent();
        }

        //this close



        //работа с формой
        private void MenuView_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            switch (menuItem.Header)
            {
                case "Файлы":
                    e.Handled = true;
                    if (menuItem.IsChecked)
                    {
                        GridRow1.Height = new GridLength(0, GridUnitType.Pixel);
                        menuItem.IsChecked = false;
                    }
                    else
                    {
                        GridRow1.Height = new GridLength(7, GridUnitType.Star);
                        menuItem.IsChecked = true;
                    }
                    break;
                case "Таблица":
                    e.Handled = true;
                    break;
                case "Статус":
                    e.Handled = true;
                    if (menuItem.IsChecked)
                    {
                        GridRow3.Height = new GridLength(0, GridUnitType.Pixel);
                        menuItem.IsChecked = false;
                    }
                    else
                    {
                        GridRow3.Height = new GridLength(15, GridUnitType.Star);
                        menuItem.IsChecked = true;
                    }
                    break;
                case "Сброс":
                    this.WindowState = WindowState.Normal;
                    foreach (MenuItem item in Menu_View.Items)
                    {
                        if (item.Header.ToString() != "Сброс")
                            item.IsChecked = true;
                    }
                    GridRow1.Height = new GridLength(7, GridUnitType.Star);
                    GridRow3.Height = new GridLength(15, GridUnitType.Star);
                    this.Width = 800;
                    this.Height = 500;
                    break;
            }
        }

        private void MenuFile_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            switch (menuItem.Header)
            {
                case "Расположение":
                    Process.Start(Environment.CurrentDirectory);
                    break;
                case "Справка":
                    new DialogWindow("Hello world, hello-hello-hello world,\nЯ автоматизирую как завещал нам Генри Форд!", "Written by GHOST?", 0).Show();
                    break;
            }
        }

        private void BtnConfigFileXML_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Выберите XML файл",
                Filter = "Файл |*xml;"
            };

            if (ofd.ShowDialog() == true)
            {
                API.ParseXML.WayXML = ofd.FileName;
                if (! new API.ParseXML().ReadXML())                  
                    new ExMessage(API.ParseXML.GetException()).Show();
                else ChConfigFileXML.IsChecked = true;
            }
        }

        private void BtnDataFile_Click(object sender, RoutedEventArgs e)
        {
            if (ChConfigFileXML.IsChecked != true)
            {
                string mess = "Не выбран файл конфигурации";
                new UserControls.DialogWindow(mess, 0).ShowDialog();
                return;
            }

            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Выберите XML файл",
                Filter = "Файл |*cnt64; *dat;"
            };

            if (BackProcess.IsBusy()) 
            {
                if (new DialogWindow("Прервать проверку?", 1).ShowDialog() == true)
                    BackProcess.CancelAsync();
                return;
            }

            if (ofd.ShowDialog() == true)
            {
                BackProcess back = new BackProcess(UIPStatus, DGFrames, BtnDataFile);
                if (!back.StartWork(ofd.FileName))
                {
                    new ExMessage(BackProcess.GetException()).Show();
                }

            }

        }



    }
}
