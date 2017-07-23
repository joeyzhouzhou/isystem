using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace FileCompression
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.ShowDialog();

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.textBox1.Text = this.openFileDialog1.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Length > 0)
            {
                if (this.chkGZip.Checked)
                    this.BeginToDo(this.GZipCompression, "使用GZip压缩文件另存为", ".gz");

                if (this.chkDeflate.Checked)
                    this.BeginToDo(this.DeflateCompression, "使用Deflate压缩文件另存为", ".de");
            }
            else
                MessageBox.Show("没有选择要压缩的文件");
        }

        /// <summary>
        /// 定义一个压缩流委托
        /// </summary>
        /// <param name="targetStream"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public delegate System.IO.Stream CompressionStream(System.IO.Stream targetStream, System.Byte[] bytes);

        private System.IO.Stream GZipCompression(System.IO.Stream targetstream, System.Byte[] bytes)
        {
            System.IO.Compression.GZipStream gzipstream = new System.IO.Compression.GZipStream(targetstream, System.IO.Compression.CompressionMode.Compress);
            gzipstream.Write(bytes, 0, bytes.Length);
            return gzipstream;
        }

        private System.IO.Stream DeflateCompression(System.IO.Stream targetstream, System.Byte[] bytes)
        {
            System.IO.Compression.DeflateStream deflatestream = new System.IO.Compression.DeflateStream(targetstream, System.IO.Compression.CompressionMode.Compress);
            deflatestream.Write(bytes, 0, bytes.Length);
            return deflatestream;
        }

        private void BeginToDo(CompressionStream del, string savetitle, string ext)
        {
            System.Byte[] source = this.OpenFile(this.textBox1.Text.Trim());
            if (source != null)
            {
                this.saveFileDialog1.Title = savetitle;
                this.saveFileDialog1.FileName = System.IO.Path.GetFileName(this.textBox1.Text.Trim()) + ext;

                if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    System.IO.FileStream targetstream = null;
                    System.IO.Stream compressedStream = null;
                    try
                    {
                        targetstream = new System.IO.FileStream(this.saveFileDialog1.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.Read);
                        compressedStream = del.Invoke(targetstream, source);

                        this.SaveFile(this.saveFileDialog1.FileName, compressedStream);
                    }
                    finally
                    {
                        if (targetstream != null)
                        {
                            targetstream.Close();
                            targetstream = null;
                        }
                        if (compressedStream != null)
                        {
                            compressedStream.Close();
                            compressedStream = null;
                        }
                    }
                }
            }
        }

        private System.Byte[] OpenFile(string filepath)
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = System.IO.File.Open(filepath, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
                byte[] bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                return bytes;
            }
            catch (System.IO.FileNotFoundException)
            {
                return null;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs = null;
                }
            }
        }

        private void SaveFile(string filepath, System.IO.Stream s)
        {
            System.IO.StreamWriter writer = null;
            try
            {
                writer = new System.IO.StreamWriter(s);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

    }
}