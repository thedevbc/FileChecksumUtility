using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileChecksumUtility
{
    public class CheckSumJob : INotifyPropertyChanged
    {
        private string _filePath;
        public string FilePath 
        {
            get
            {
                return _filePath;
            } 
            set 
            { 
                if(value != _filePath)
                {
                    _filePath = value;
                    OnPropertyChanged("FilePath");
                }
            } 
        }

        private string _expSum;
        public string ExpectedSum
        {
            get
            {
                return _expSum;
            }
            set
            {
                if (value != _expSum)
                {
                    _expSum = value;
                    OnPropertyChanged("ExpectedSum");
                }
            }
        }

        private string _calcSum;
        public string CalcSum
        {
            get
            {
                return _calcSum;
            }
            set
            {
                if (value != _calcSum)
                {
                    _calcSum = value;
                    OnPropertyChanged("CalcSum");
                }
            }
        }

        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        private AlgorithmType _algo;
        public AlgorithmType Algorithm
        {
            get
            {
                return _algo;
            }
            set
            {
                if (value != _algo)
                {
                    _algo = value;
                    OnPropertyChanged("Algorithm");
                }
            }
        }

        public DateTime StartTimeStamp { get; set; }
        public DateTime EndTimeStamp { get; set; }

        public CheckSumJob()
        {
            FilePath = "";
            ExpectedSum = "";
            CalcSum = "";
            Message = "";
        }

        public CheckSumJob(CheckSumJob oldjob)
        {
            FilePath = oldjob.FilePath;
            ExpectedSum = oldjob.ExpectedSum;
            CalcSum = oldjob.CalcSum;
            Algorithm = oldjob.Algorithm;
            Message = CompareCheckSums();
            StartTimeStamp = oldjob.StartTimeStamp;
            EndTimeStamp = oldjob.EndTimeStamp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Task CalculateMD5()
        {
            return Task.Run(() =>
            {
                using (var fs = File.OpenRead(FilePath))
                {
                    using (var md5 = MD5.Create())
                    {
                        var hash = md5.ComputeHash(fs);
                        CalcSum = ByteArrayToHexString(hash);
                    }
                }
            });
        }

        public Task CalculateSHA1()
        {
            return Task.Run(() =>
            {
                using (var fs = File.OpenRead(FilePath))
                {
                    using (var sha = SHA1.Create())
                    {
                        var hash = sha.ComputeHash(fs);
                        CalcSum = ByteArrayToHexString(hash);
                    }
                }
            });
        }

        public Task CalculateSHA256()
        {
            return Task.Run(() =>
            {
                using (var fs = File.OpenRead(FilePath))// File.ReadAllBytes(FilePath);
                {
                    using (var sha = SHA256.Create())
                    {
                        var hash = sha.ComputeHash(fs);

                        CalcSum = ByteArrayToHexString(hash);
                    }
                }
            });
        }

        public Task CalculateSHA512()
        {
            return Task.Run(() =>
            {
                using (var fs = File.OpenRead(FilePath))
                {
                    using (var sha = SHA512.Create())
                    {
                        var hash = sha.ComputeHash(fs);
                        CalcSum = ByteArrayToHexString(hash);
                    }
                }
            });
        }

        public string CompareCheckSums()
        {
            if (string.IsNullOrWhiteSpace(ExpectedSum))
            {
                return "Done.";
            }
            else if (ExpectedSum.Trim().ToUpper() == CalcSum.Trim().ToUpper())
            {
                return "Done. Checksums match!";
            }
            else
            {
                return "Done. Checksums do NOT match.";
            }
        }

        public static string ByteArrayToHexString(byte[] Bytes)
        {
            StringBuilder Result = new StringBuilder(Bytes.Length * 2);
            string HexAlphabet = "0123456789ABCDEF";

            foreach (byte B in Bytes)
            {
                Result.Append(HexAlphabet[(int)(B >> 4)]);
                Result.Append(HexAlphabet[(int)(B & 0xF)]);
            }

            return Result.ToString();
        }
    }
}
