using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Diagnostics;

namespace SCU
{
    class ClassSCU
    {
        public string PrinterIP = ConfigurationManager.AppSettings["PrinterIP"];
        public bool PrinterFlag;
        log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public string ConfigPrinterIP(string path)
        {
            string FilePath = path + @"\dcmpstat.cfg.scu";
            try
            {
                string text = File.ReadAllText(FilePath);
                text = text.Replace("[PrinterIP]", PrinterIP.ToString());
                File.WriteAllText(FilePath, text);
                PrinterFlag = true;
                log.Debug("Change the Print SCP IP address sucessfully. Ip is: " + PrinterIP.ToString() + " and file path is : " + FilePath);
                return "true";
            }
            catch (Exception ex)
            {
                log.Error("Change the Print SCP IP address failed from file " + FilePath, ex);
                return ex.ToString();
            }
            
        }

        public string CreateDicomFile(string Path)
        {
            string SCURootPath = Path + @"SCU";
            string SCUImageFolderPath = Path + @"SCU\database";
            string ImagePath = Path + @"images";

            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(SCUImageFolderPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    dir.Delete(true);
                }
                
                log.Debug("Delete all folders and files from folder " + SCUImageFolderPath +" successfully.");
            }
            catch (Exception ex)
            {
                log.Error("Delete all folders and files from folder " + SCUImageFolderPath + " failed.", ex);
                return ex.ToString();
            }

            try
            {
                Process proc = null;
                proc = new Process();
                proc.StartInfo.WorkingDirectory = SCURootPath;
                proc.StartInfo.FileName = "dcmpsprt";
                proc.StartInfo.Arguments = " -v -c " + SCURootPath + @"\dcmpstat.cfg.scu --printer IHEFULL " + ImagePath + @"\sample.dcm";
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                proc.WaitForExit();
                proc.Close();

                string[] SP_DicomFilePath = System.IO.Directory.GetFiles(SCUImageFolderPath, "SP_*.dcm");
                if (SP_DicomFilePath.Length == 1)
                {
                    log.Debug("Create DICOM file sucessfully and path is " + SP_DicomFilePath.ToString());
                    return "true";
                }
                else
                {
                    log.Debug("Create DICOM file failed. There are no file in path :" + SCUImageFolderPath);
                    return "false";
                }

            }
            catch (Exception ex)
            {
                log.Debug("Create DICOM file failed. The opertaion crash.",ex);
                return ex.ToString();
            }


 


        }

        public string SendDicomFile(string Path)
        {

            string ImageFolderPath = Path + @"SCU" + @"\database";
            string SCURootPath = Path + @"SCU";

            try
            {
                string[] SP_DicomFilePath = System.IO.Directory.GetFiles(ImageFolderPath, "SP_*.dcm");

                Process proc = null;
                proc = new Process();
                proc.StartInfo.WorkingDirectory = SCURootPath;
                proc.StartInfo.FileName = "dcmprscu.exe";
                proc.StartInfo.Arguments = " -d -v -c " + SCURootPath + @"\dcmpstat.cfg.scu --copies 1 " + SP_DicomFilePath[0];
                proc.StartInfo.CreateNoWindow = false;
                proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                proc.Start();
                proc.WaitForExit();
                proc.Close();

                log.Debug("Send DCIOM file(" + SP_DicomFilePath[0] + ") to SCP successfully.");
                return "true";
            }
            catch (Exception ex)
            {
                log.Error("Send DCIOM file to SCP failed.",ex);
                return ex.ToString();
            }

        }




    }
}
