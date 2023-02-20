﻿using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

                    break;
                case "Статус":
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
                    MessageBox.Show("Hello world, hello-hello-hello world,\nЯ автоматизирую как завещал нам Генри Форд!", "Written by GHOST?",
                        MessageBoxButton.OK, MessageBoxImage.Information);
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

            if (ofd.ShowDialog() == true)
            {
                API.FrameAnalize.WayFile = ofd.FileName;
                API.FrameAnalize.Analize();
            }
                //BackProcess back = new BackProcess(UIPStatus);
                //if (!back.StartWork())
                //{
                //    new ExMessage(back.GetException()).Show();
                //}
            }




    }
}