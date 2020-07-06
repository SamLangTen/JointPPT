using System;
using System.Windows;
using System.Threading;
using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using static JointPPT.NativeMethods;
namespace JointPPT
{
    /// <summary>
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindowViewModel ViewModel { get; set; }




        public MainWindow()
        {
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
            InitializeComponent();
     
        }

        private void FileListBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = DragDropEffects.Link;
            else e.Effects = DragDropEffects.None;
        }

        private void FileListBox_Drop(object sender, DragEventArgs e)
        {
            string[] fileNames = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            if (fileNames.Length > 0)
                foreach (string fileName in fileNames)
                {
                    //Reject other files
                    if (fileName.EndsWith(".ppt", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".pptx", StringComparison.OrdinalIgnoreCase))
                    {
                        //Reject duplicated files
                        if (FileListBox.Items.IndexOf(fileName) == -1)
                            FileListBox.Items.Add(fileName);
                    }
                }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            FileListBox.Items.Clear();
        }

        private void Upward_Click(object sender, RoutedEventArgs e)
        {
            //Move the item up
            try
            {
                int ch = FileListBox.SelectedIndex;
                FileListBox.Items.Insert(ch - 1, FileListBox.Items[ch]);
                FileListBox.Items.RemoveAt(ch + 1);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Ignore
            }
        }

        private void Downward_Click(object sender, RoutedEventArgs e)
        {
            //Move the item down
            try
            {
                int ch = FileListBox.SelectedIndex;
                FileListBox.Items.Insert(ch + 2, FileListBox.Items[ch]);
                FileListBox.Items.RemoveAt(ch);
            }
            catch (ArgumentOutOfRangeException)
            {
                //Ignore
            }
        }

        private void FileListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FileListBox.Items.Remove(FileListBox.SelectedItem);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {

            if (FileListBox.Items.IsEmpty)
            {
                MessageBox.Show("Please drag PowerPoint files into the list box.");
                return;
            }
            if (MessageBox.Show("Please click OK to start."
                                , "Joint PPT"
                                , MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                List<string> files = new List<string>();
                //Test if file is occupied
                var occupiedFilename = ViewModel.AddedFilenames.FirstOrDefault(f => !TestFileOccupation(f));
                if (occupiedFilename != null)
                {
                    MessageBox.Show($"The file is occupied or unavailable:{occupiedFilename}");
                    return;
                }

                //Multithread
                MainUI.IsEnabled = false;
                await Task.Run(() => ViewModel.Logs = Join(files, ViewModel.UsedWideScreen));
                MainUI.IsEnabled = true;
                Activate();
            }
        }

        public delegate void UIEventHandler();


        public void AppendProgress(int append, string vinfo)
        {
            Dispatcher.BeginInvoke(new UIEventHandler(() =>
            {
                if (vinfo != "")
                {
                    StatusLabel.Content = vinfo;
                }
                Console.WriteLine(append.ToString());
                ProgressBar1.Maximum = FileListBox.Items.Count;
                ProgressBar1.Value += append;
            }));
        }

        private List<ErrorInfo> Join(List<string> files, bool isWideScreen)
        {
            List<ErrorInfo> errorInfos = new List<ErrorInfo>();
            var PreApp = new PowerPoint.Application();
            PreApp.Presentations.Add();
            PowerPoint.Presentation presentation = PreApp.ActivePresentation;
            if (!isWideScreen)
            {
                //Set to 4:3
                presentation.PageSetup.SlideSize = PowerPoint.PpSlideSizeType.ppSlideSizeOnScreen;
            }
            //For WPS users.
            int WPSoptimize = 0;
            foreach (string file in files)
            {
                bool sendAgain = true;
                int count = 0;

                while (sendAgain)
                {
                    try
                    {
                        presentation.Slides.InsertFromFile(file, presentation.Slides.Count + WPSoptimize, 1, -1);
                        AppendProgress(1, "Merging...");
                        sendAgain = false;
                    }
                    catch (ArgumentException)
                    {
                        //WPS Detected
                        WPSoptimize = 1;
                    }
                    catch (COMException e)
                    {
                        Thread.Sleep(100);
                        sendAgain = count > 100 ? false : true;
                        if (!sendAgain)
                        {
                            errorInfos.Add(new ErrorInfo(file, e.Message));
                        }
                    }
                    finally { count++; }
                }

            }
            PreApp.Visible = Microsoft.Office.Core.MsoTriState.msoCTrue;
            return errorInfos;
        }

        private bool TestFileOccupation(string filename)
        {
            IntPtr vHandle = Lopen(filename, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            {
                CloseHandle(vHandle);
                return true;
            }
            CloseHandle(vHandle);
            return false;
        }
    }
}
