﻿using ICSharpCode.AvalonEdit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Parcel.CoreEngine.Helpers;
using Parcel.NExT.Interpreter;
using Parcel.NExT.Interpreter.Helpers;

namespace Notebook
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Construction
        public MainWindow()
        {
            // Hide console
            Win32.HideConsoleWindow();

            InitializeComponent();

            // Remark-cz: 1st argument is executable path (aka. Notebook.exe), 2nd argument is taken to be open file path
            var args = Environment.GetCommandLineArgs();
            Interpreter = new Interpreter($"""
                    Notebook for Pure 2 (Core Version: {Interpreter.DistributionVersion})
                    """, null, args.Length > 2 ? args.Skip(2).ToArray() : null, null, null);
            // Remark-cz: Update for Notebook post .Net 8 upgrade, where we build Notebook.exe in dedicated Windows folder
            string assemblyParentFolder = Path.GetDirectoryName(AssemblyHelper.ExecutingAssemblyDirectory);
            string librariesPath = Path.Combine(assemblyParentFolder, "Libraries");
            Interpreter.Start(OutputHandlerNonMainThread, additionalLibraryDllPaths: Directory.Exists(librariesPath) ? [librariesPath] : null);

            if (args.Length >= 2)
            {
                string filepath = args[1];
                if (File.Exists(filepath))
                    OpenFile(filepath);
            }
        }
        private readonly Dispatcher MainUIDispatcher = Dispatcher.CurrentDispatcher;
        private readonly Interpreter Interpreter;
        private void OutputHandlerNonMainThread(string message)
        {
            MainUIDispatcher.Invoke(OutputHandler);

            void OutputHandler()
            {
                if (CurrentExecutingCell != null && Data.Cells.Contains(CurrentExecutingCell))
                    GenerateOutputCell(CurrentExecutingCell, message, false);
            }
        }
        #endregion

        #region Data Binding Properties
        private Visibility _RunningIndicatorVisibility = Visibility.Collapsed;
        private CellBlock _CurrentEditingCell = null;
        private CellBlock _CurrentExecutingCell = null;
        private ApplicationData _Data = NotebookManager.Load();

        private TextEditor CurrentEditingCellContext;
        private CellBlock CurrentExecutingCell { 
            get => _CurrentExecutingCell; 
            set
            {
                _CurrentExecutingCell = value;
                RunningIndicatorVisibility = value == null ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public CellBlock CurrentEditingCell { get => _CurrentEditingCell; set => SetField(ref _CurrentEditingCell, value); }
        public ApplicationData Data { get => _Data; set => SetField(ref _Data, value); }
        public Visibility RunningIndicatorVisibility { get => _RunningIndicatorVisibility; set => SetField(ref _RunningIndicatorVisibility, value); }
        #endregion

        #region Windows Events
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            CellBlock cellBlock = control.DataContext as CellBlock;
            CurrentEditingCell = cellBlock;
        }
        private void AvalonTextEditor_GotFocus(object sender, RoutedEventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock cellBlock = editor.DataContext as CellBlock;
            CurrentEditingCell = cellBlock;
            CurrentEditingCellContext = editor;
        }
        private void AvalonTextEditor_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            editor.Text = block.Content;
        }
        private void AvalonTextEditor_Initialized(object sender, System.EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            editor.Text = block.Content;
        }
        private void AvalonTextEditor_OnTextChanged(object sender, System.EventArgs e)
        {
            TextEditor editor = sender as TextEditor;
            CellBlock block = editor.DataContext as CellBlock;
            block.Content = editor.Text;
        }
        private void AvalonTextEditor_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Remark-cz: This is a workaround that deals with AvalonEdit TextEditor's "eating the scroll event" issue, as per here: https://github.com/icsharpcode/AvalonEdit/issues/395
            ScrollViewer scrollViewer = DataItemsControl.Parent as ScrollViewer;
            if (e.Delta > 0)
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - 50);
            else
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset + 50);
            e.Handled = true;
        }
        private void AvalonTextEditor_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            // Remark-cz: This is a workaround that handles the jumpiness of large texts, as per here: https://github.com/icsharpcode/AvalonEdit/issues/396
            e.Handled = true;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NotebookManager.Save(Data, true);
        }
        #endregion

        #region Menu Items
        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                Filter = "Pure Notebook Files (*.pnb)|*.pnb|Literate Markdown Files (*.md)|*.md|All (*.*)|*.*",
                CheckFileExists = false,
                OverwritePrompt = false,
                CreatePrompt = false,
                CheckPathExists = true,
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                OpenFile(filepath);
            }
            e.Handled = true;
        }
        private void RefreshFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Data = NotebookManager.Load();
        }
        private void OpenFileContainingFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenAndSelectFileInFileExplorer(NotebookManager.GetCurrentNotebookFilePath());

            void OpenAndSelectFileInFileExplorer(string filepath)
            {
                if (!File.Exists(filepath))
                    return;

                // Combine the arguments together it doesn't matter if there is a space after ','
                string argument = "/select, \"" + filepath + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
            }
        }
        private void SaveFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data);
        }
        private void AddMarkdownCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Markdown
            });
        }
        private void AddCSharpCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.CSharp
            });
        }
        private void AddPythonCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Python
            });
        }
        private void AddCSharpCellWithCopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.CSharp
            }, true);
        }
        private void AddPythonCellWithCopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Python
            }, true);
        }
        private void AddMarkdownCellWithCopyMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddCell(new CellBlock()
            {
                CellType = CellType.Markdown
            }, true);
        }
        private void DeleteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentEditingCell != null && Data.Cells.Contains(CurrentEditingCell))
                DeleteCell(CurrentEditingCell);
        }
        private void DeleteAllCellsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Data.Cells.Clear();
        }
        private void ExecuteCellMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data, true);
            if (CurrentEditingCell != null && Data.Cells.Contains(CurrentEditingCell)
                && CurrentEditingCell.CellType != CellType.Markdown
                && CurrentEditingCell.CellType != CellType.CacheOutput)
                ExecuteCell(CurrentEditingCell);
        }
        private void ExecuteAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            NotebookManager.Save(Data, true);
            Data.Cells = new (Data.Cells.Where(c => c.CellType != CellType.CacheOutput));
            foreach (var cell in Data.Cells.ToArray()
                .Where(c => c.CellType == CellType.CSharp || c.CellType == CellType.Python))
                ExecuteCell(cell);
        }
        private void ExportMarkdownMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                FileName = "Export",
                Filter = "Markdown (*.md)|*.md|All (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                MarkdownHelper.SaveDataTo(filepath, Data);
            }
            e.Handled = true;
        }
        private void ExportMarkdownMenuItemWithCache_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                FileName = "Export",
                Filter = "Markdown (*.md)|*.md|All (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                MarkdownHelper.SaveDataTo(filepath, Data, true);
            }
            e.Handled = true;
        }
        private void ExportCodeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Title = "Select path to save file",
                FileName = "Export",
                Filter = "C# (*.cs)|*.cs|Python (*.py)|*.py"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string filepath = saveFileDialog.FileName;
                string extension = Path.GetExtension(filepath);
                if (extension == ".cs")
                    File.WriteAllText(filepath, string.Join("\n", Data.Cells.Where(c => c.CellType == CellType.CSharp).Select(c => c.Content)));
                else if (extension == ".py")
                    File.WriteAllText(filepath, string.Join("\n", Data.Cells.Where(c => c.CellType == CellType.Python).Select(c => c.Content)));
            }
            e.Handled = true;
        }
        private void PreloadScriptsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select the script to load",
                Filter = "C# (*.cs)|*.cs|All (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string filepath = openFileDialog.FileName;
                string extension = Path.GetExtension(filepath);
                if (extension == ".cs")
                {
                    RunningIndicatorVisibility = Visibility.Visible;
                    Task.Run(() => ExecuteScriptThreaded(File.ReadAllText(filepath), CellType.CSharp));
                }
            }
        }
        private void SetArgumentsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var window = new EntryWindow(string.Join(" ", Interpreter.Arguments?.Select(a => a.Contains(' ') ? $"\"{a}\"" : a) ?? Array.Empty<string>()))
            {
                Owner = this
            };
            if (window.ShowDialog() == true && !string.IsNullOrEmpty(window.Result))
            {
                string[] arguments = window.Result.SplitCommandLineArguments(' ');
                Interpreter.UpdateScriptArguments(arguments);
            }
            e.Handled = true;
        }
        #endregion

        #region Keyboard Commands
        private void FileOpenCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileOpenCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => OpenFileMenuItem_Click(null, null);
        private void FileSaveCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void FileSaveCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => SaveFileMenuItem_Click(null, null);
        private void CreateCSharpCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateCSharpCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddCSharpCellMenuItem_Click(null, null);
        private void CreatePythonCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreatePythonCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddPythonCellMenuItem_Click(null, null);
        private void CreateMarkdownCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateMarkdownCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddMarkdownCellMenuItem_Click(null, null);
        private void CreateCSharpCellWithCopyCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateCSharpCellWithCopyCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddCSharpCellWithCopyMenuItem_Click(null, null);
        private void CreatePythonCellWithCopyCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreatePythonCellWithCopyCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddPythonCellWithCopyMenuItem_Click(null, null);
        private void CreateMarkdownCellWithCopyCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void CreateMarkdownCellWithCopyCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => AddMarkdownCellWithCopyMenuItem_Click(null, null);
        private void ExecuteCellCommand_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
            => e.CanExecute = true;
        private void ExecuteCellCommand_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
            => ExecuteCellMenuItem_Click(null, null);
        #endregion

        #region Routines
        private void OpenFile(string filepath)
        {
            filepath = Path.GetFullPath(filepath);
            Interpreter.UpdateNugetRepositoryIdentifier(filepath.GetDeterministicHashCode().ToString());
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath));

            NotebookManager.CurrentNotebookFilePath = filepath;
            Interpreter.UpdateScriptFilePath(filepath);
            Title = $"Pure - {filepath}";
            Data = NotebookManager.Load();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(filepath));
        }
        private void AddCell(CellBlock newCell, bool copyCellContent = false)
        {
            if (CurrentEditingCell == null)
                Data.Cells.Insert(0, newCell);
            else
            {
                int cellIndex = Data.Cells.IndexOf(CurrentEditingCell);
                if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                    cellIndex++;
                Data.Cells.Insert(cellIndex + 1, newCell);
            }
            if (copyCellContent)
                newCell.Content = CurrentEditingCell?.Content;   // Auto-copy existing cell content
            CurrentEditingCell = newCell;

            // Auto-focus
            Task.Delay(300).ContinueWith(t =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (CurrentEditingCell.CellType == CellType.Python || CurrentEditingCell.CellType == CellType.CSharp)
                    {
                        var te = FindVisualChildren<TextEditor>(DataItemsControl).SingleOrDefault(te => (te.DataContext as CellBlock) == CurrentEditingCell);
                        te.Focus();
                    }
                });
            });
        }
        private void GenerateOutputCell(CellBlock codeCell, string message, bool reInitialize)
        {
            int cellIndex = Data.Cells.IndexOf(codeCell);
            CellBlock outputCell;
            if (Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
                outputCell = Data.Cells[cellIndex + 1];
            else
            {
                outputCell = new CellBlock()
                {
                    CellType = CellType.CacheOutput,
                    Content = string.Empty
                };
                Data.Cells.Insert(cellIndex + 1, outputCell);
            }

            if (reInitialize)
                outputCell.Content = message;
            else
                outputCell.Content += message;
        }
        private void DeleteCell(CellBlock cell)
        {
            int cellIndex = Data.Cells.IndexOf(cell);
            if (cell.CellType != CellType.CacheOutput && Data.Cells.Count > cellIndex + 1 && Data.Cells[cellIndex + 1].CellType == CellType.CacheOutput)
            {
                CellBlock outputCell = Data.Cells[cellIndex + 1];
                Data.Cells.Remove(outputCell);
            }
            Data.Cells.Remove(cell);
        }
        private void ExecuteCell(CellBlock cell)
        {
            if (CurrentExecutingCell != null)
            {
                MessageBox.Show("Cannot execute code when a cell is already running.");
                return;
            }

            CurrentExecutingCell = cell;
            string scriptContent = (CurrentEditingCellContext != null && CurrentEditingCellContext.DataContext == cell && !string.IsNullOrWhiteSpace(CurrentEditingCellContext.SelectedText)) ? CurrentEditingCellContext.SelectedText : cell.Content;
            Task.Run(() => {
                if (cell.CellType == CellType.CSharp || cell.CellType == CellType.Python)
                    Dispatcher.Invoke(() => GenerateOutputCell(cell, string.Empty, true));
                ExecuteScriptThreaded(scriptContent, cell.CellType);
            });
        }
        void ExecuteScriptThreaded(string scriptContent, CellType scriptType)
        {
            if (scriptType == CellType.CacheOutput || scriptType == CellType.Markdown)
                throw new InvalidOperationException($"Invalid cell type: {scriptType}");

            try
            {
                switch (scriptType)
                {
                    case CellType.CSharp:
                        foreach (var script in Interpreter.SplitScripts(scriptContent))
                            Interpreter.Parse(script);
                        break;
                    case CellType.Python:
                        Interpreter.Parse("Import(Python)");
                        Interpreter.Parse($"""""
                        Python.Main.Parse("""
                        {scriptContent}
                        """);
                        """"");
                        break;
                    case CellType.Markdown:
                    case CellType.CacheOutput:
                    default:
                        throw new ArgumentException($"Invalid cell type: {scriptType}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"""
                    Some serious error happened during interpretative execution using scripting engines and it's not handled properly by the scripting context. This indicates an error within the implementation of script-evaluation routines.
                    Details:
                    {e}
                    """);
            }
            CurrentExecutingCell = null;
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

        #region Helpers
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindVisualChildren<T>(ithChild)) yield return childOfChild;
            }
        }
        #endregion
    }
}
