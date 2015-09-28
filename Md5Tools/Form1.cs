using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace Md5Tools
{
    public partial class Form1 : Form
    {
        public List<string> list = new List<string>();
        public string strPath = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //多线程调用
           CheckForIllegalCrossThreadCalls = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //按钮
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //打开对话框
                strPath = dlg.SelectedPath;
                textBox1.Text = strPath + "\\";    //对话框

               Thread thread = new Thread(UpdateProcess);
                thread.Start(this);
            }
        }

        private void GetFileList(string strPath)
        {
            if (string.IsNullOrEmpty(strPath))
                return;

            //递归获取文件列表
            string[] Files = Directory.GetFiles(strPath);

            string[] dir = Directory.GetDirectories(strPath);

            list.AddRange(Files);

            for (int i = 0; i < dir.Length; ++i)
            {
                GetFileList(dir[i]);
            }
        }

        private string GetFileMd5(string strFile)
        {
            //md5
            MD5 md5 = new MD5CryptoServiceProvider();
            //MD5 md5 = new MD5();
            //文件加密
            FileStream stream = new FileStream(strFile, FileMode.Open);

            byte[] result1 = md5.ComputeHash(stream);//文件字符串
            StringBuilder builder1 = new StringBuilder();
            for (int i = 0; i < result1.Length; ++i)
            {
                builder1.Append(result1[i].ToString("x2"));
            }

            stream.Close();
            
            return builder1.ToString();
        }

        //更新
        static void UpdateProcess(object param)
        {
            Form1 pForm = (Form1)param;
            if (pForm != null)
            {
                string basePath = pForm.textBox1.Text;
                pForm.list.Clear();

                string filetxt = basePath + "files.txt";

                File.Delete(filetxt);
                //打开文件
                pForm.GetFileList(pForm.strPath);

                FileStream stream = File.Open(basePath + "files.txt", FileMode.CreateNew);
                pForm.progressBar1.Value = 0;
                pForm.progressBar1.Minimum = 0;
                pForm.progressBar1.Maximum = pForm.list.Count;

                for (int i = 0; i < pForm.list.Count; ++i)
                {
                    string filepath = pForm.list[i];

                    string md5value = pForm.GetFileMd5(filepath);
                    string strFileName = filepath.Replace(basePath, "");
                    string value = strFileName + "|" + md5value + "\r\n";
                    byte[] bytes = Encoding.Default.GetBytes(value);
                    //获取值的md5
                    stream.Write(bytes, 0, bytes.Length);
                    pForm.progressBar1.Value++;
                    bool scroll = false;
                    if (pForm.listBox1.TopIndex == pForm.listBox1.Items.Count - (int)(pForm.listBox1.Height / pForm.listBox1.ItemHeight))
                        scroll = true;
                    pForm.listBox1.Items.Add(strFileName + "........(" + pForm.progressBar1.Value + "/" + pForm.progressBar1.Maximum + ")");
                    if (scroll)
                        pForm.listBox1.TopIndex = pForm.listBox1.Items.Count - (int)(pForm.listBox1.Height / pForm.listBox1.ItemHeight);
                }

                stream.Close();

                //完成
                MessageBox.Show("所有文件md5加密完成", "md5加密");
            }
        }
    }
}
