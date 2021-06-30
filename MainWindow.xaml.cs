using System;
using System.Collections.Generic;
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

namespace FileChecksumUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CheckSumJob Job { get; set; }
        public List<CheckSumJob> Jobs { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Job = new CheckSumJob();
            Jobs = new List<CheckSumJob>();
            InitializeControls();
        }

        public void InitializeControls()
        {
            MyGrid.DataContext = Job;
        }

        private void btnChooseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            var result = dlg.ShowDialog();
            if (result == true)
            {
                Job.FilePath = dlg.FileName;
            }
        }

        private async void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Job.FilePath))
            {
                Job.Message = "Please select a file with which to calculate the checksum.";
            }
            else if (cbAlgo.SelectedItem is null)
            {
                Job.Message = "Please specify an algorithm to use to calculate the checksum.";
            }
            else
            {
                Job.StartTimeStamp = DateTime.Now;
                Job.EndTimeStamp = DateTime.Now;
                //Look to see if there are past Jobs with an EndTime
                //see if the file and algorithm are the same as current do not calculate
                var pastMatchingJobs = Jobs.Where(j => j.EndTimeStamp != null && j.EndTimeStamp < Job.StartTimeStamp && j.Algorithm == Job.Algorithm && j.FilePath == Job.FilePath).ToList();
                var mostRecent = pastMatchingJobs.OrderByDescending(j => j.EndTimeStamp).FirstOrDefault();

                if (pastMatchingJobs.Count > 0 && mostRecent != null)
                {
                    Job.CalcSum = mostRecent.CalcSum;
                }
                else
                {
                    Job.Message = "Calculating...";
                    switch (Job.Algorithm)
                    {
                        case AlgorithmType.MD5:
                            await Job.CalculateMD5();
                            break;
                        case AlgorithmType.SHA1:
                            await Job.CalculateSHA1();
                            break;
                        case AlgorithmType.SHA256:
                            await Job.CalculateSHA256();
                            break;
                        case AlgorithmType.SHA512:
                            await Job.CalculateSHA512();
                            break;
                        default:
                            break;
                    }

                }

                Job.Message = Job.CompareCheckSums();
                Job.EndTimeStamp = DateTime.Now;
                Jobs.Add(new CheckSumJob(Job));
            }
        }
    }

    public enum AlgorithmType
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }
}
