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

namespace Md5Tools
{
    public partial class Form1 : Form
    {
        public List<string> list = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
                string strPath = dlg.SelectedPath;
                textBox1.Text = strPath + "\\";    //对话框
                string basePath = textBox1.Text;
                list.Clear();

                string filetxt = basePath + "files.txt";

                File.Delete(filetxt);
                //打开文件
                GetFileList(strPath);

                FileStream stream = File.Open(basePath + "files.txt", FileMode.CreateNew);
                progressBar1.Value = 0;
                progressBar1.Minimum = 0;
                progressBar1.Maximum = list.Count;

                for (int i = 0; i < list.Count; ++i )
                {
                    string filepath = list[i];

                    string md5value = GetFileMd5(filepath);
                    string strFileName = filepath.Replace(basePath, "");
                    string value = strFileName + "|" + md5value + "\r\n";
                    byte[] bytes = Encoding.Default.GetBytes(value);
                    //获取值的md5
                    stream.Write(bytes, 0, bytes.Length);
                    progressBar1.Value++;
                    bool scroll = false;
                    if (this.listBox1.TopIndex == this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight))
                       scroll = true;
                    this.listBox1.Items.Add(strFileName + "........(" + progressBar1.Value + "/" + progressBar1.Maximum + ")");
                   if (scroll)
                     this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
                }

                stream.Close();

                //完成
                MessageBox.Show("所有文件md5加密完成", "md5加密");
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
    }
}
