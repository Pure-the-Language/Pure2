﻿using Microsoft.Win32;
using Righteous.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Righteous
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region Data Binding Properties
        private string _DisplayText;
        public string DisplayText { get => _DisplayText; set => SetField(ref _DisplayText, value); }
        private string _CurrentFilePath;
        public string CurrentFilePath { get => _CurrentFilePath; set => SetField(ref _CurrentFilePath, value); }
        private ApplicationData _Data = new ();
        public ApplicationData Data { get => _Data; set => SetField(ref _Data, value); }
        #endregion

        #region Commands
        private void FileNewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileNewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => CreateNewFile();
        private void FileOpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => OpenFile();
        private void FileOpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileSaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
            => SaveFile();
        private void FileSaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = CurrentFilePath != null;
        #endregion

        #region Events
        private void NewWindowButton_Click(object sender, RoutedEventArgs e)
        {
            DataModel model = new DataModel();
            Data.Steps.Add(model);
            new ScriptWindow()
            {
                Owner = this,
                Model = model
            }.Show();
        }

        private void ShowAllWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
            foreach (var step in Data.Steps)
            {
                new ScriptWindow()
                {
                    Owner = this,
                    Model = step,
                    WindowStartupLocation = WindowStartupLocation.Manual,
                    Left = step.Location.X,
                    Top = step.Location.Y
                }.Show();
            }
        }
        private void CloseAllWindowsButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }
        }
        #endregion

        #region File Actions
        private void CreateNewFile()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                AddExtension = true,
                Title = "Choose location to save file"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = saveFileDialog.FileName;
                Data = new ApplicationData()
                {
                    Name = "New Analysis"
                };
                ApplicationDataSerializer.Save(CurrentFilePath, Data);
                Title = $"Righteous - {CurrentFilePath}";
            }
        }
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                Title = "Choose file to open"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = openFileDialog.FileName;
                Data = OpenFile(CurrentFilePath);
                Title = $"Righteous - {CurrentFilePath}";
            }
        }
        private void SaveFile()
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = "Righteous (*.prt)|*.prt|All (*.*)|*.*",
                AddExtension = true,
                Title = "Choose location to save file as"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                CurrentFilePath = saveFileDialog.FileName;
                ApplicationDataSerializer.Save(CurrentFilePath, Data);
                Title = $"Righteous - {CurrentFilePath}";
            }
        }
        #endregion

        #region Data Binding
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        protected bool SetField<TType>(ref TType field, TType value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TType>.Default.Equals(field, value)) return false;
            field = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion

        #region Routines
        private static ApplicationData OpenFile(string filePath)
        {
            var appData = ApplicationDataSerializer.Load(filePath);
            return appData;
        }
        #endregion
    }
}
