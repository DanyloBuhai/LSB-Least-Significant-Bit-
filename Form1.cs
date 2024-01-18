using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace LSB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Bitmap bitmap;

        private byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }// Функція переведення бітів в байти

        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1) { st = true; }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }// Функція переведення байтів в масив бітів

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Images (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|" + "All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    bitmap = new Bitmap(ofd.FileName);
                }
                catch
                {
                    MessageBox.Show("Помилка!");
                }
            }
        }//Завантаження зображення

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "") { MessageBox.Show("Введіть повідомлення!"); return; }
            if (bitmap == null) { MessageBox.Show("Завантажте зображення!"); return; };

            richTextBox1.Text = "";
            int charCounter = 0;
            string message = textBox1.Text + "%";
            if (message.Length > bitmap.Width * bitmap.Height)
            {
                MessageBox.Show("Забагато тексту для цього зображення!");
                return;
            }
            byte[] messageBytes = Encoding.ASCII.GetBytes(message);


            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var bitMessage = ByteToBit(messageBytes[charCounter]);

                    var colorBitArray = ByteToBit(bitmap.GetPixel(x, y).R);
                    colorBitArray[0] = bitMessage[0];
                    colorBitArray[1] = bitMessage[1];
                    byte newR = BitToByte(colorBitArray);

                    colorBitArray = ByteToBit(bitmap.GetPixel(x, y).G);
                    colorBitArray[0] = bitMessage[2];
                    colorBitArray[1] = bitMessage[3];
                    colorBitArray[2] = bitMessage[4];
                    byte newG = BitToByte(colorBitArray);

                    colorBitArray = ByteToBit(bitmap.GetPixel(x, y).B);
                    colorBitArray[0] = bitMessage[5];
                    colorBitArray[1] = bitMessage[6];
                    colorBitArray[2] = bitMessage[7];
                    byte newB = BitToByte(colorBitArray);

                    Color newColor = Color.FromArgb(newR, newG, newB);
                    bitmap.SetPixel(x, y, newColor);

                    charCounter++;
                    if (charCounter >= message.Length) break;
                }
                if (charCounter >= message.Length) break;
            }//шифрування


            //Збереження зображення з зашифрованою інформацією
            String sFilePic;
            SaveFileDialog dSavePic = new SaveFileDialog();
            dSavePic.Filter = "Файлы изображений (*.bmp)|*.bmp|Все файлы (*.*)|*.*";
            if (dSavePic.ShowDialog() == DialogResult.OK)
            {
                sFilePic = dSavePic.FileName;
            }
            else
            {
                sFilePic = ""; return;
            };
            FileStream wFile; try
            {
                wFile = new FileStream(sFilePic, FileMode.Create);
            }
            catch (IOException)
            {
                MessageBox.Show("Помилка відкриття файлу для запису", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bitmap.Save(wFile, System.Drawing.Imaging.ImageFormat.Bmp);
            wFile.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Розшифровка інформації з зображення
            if (bitmap == null) return;
            string letter = "2";
            int index = 0;
            string ms = "";
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var R = ByteToBit(bitmap.GetPixel(x, y).R);
                    var G = ByteToBit(bitmap.GetPixel(x, y).G);
                    var B = ByteToBit(bitmap.GetPixel(x, y).B);
                    var charResBit = R;
                    charResBit[0] = R[0];
                    charResBit[1] = R[1];
                    charResBit[2] = G[0];
                    charResBit[3] = G[1];
                    charResBit[4] = G[2];
                    charResBit[5] = B[0];
                    charResBit[6] = B[1];
                    charResBit[7] = B[2];
                    int value = BitToByte(charResBit);
                    char c = Convert.ToChar(value);
                    letter = System.Text.Encoding.ASCII.GetString(new byte[] { Convert.ToByte(c) });
                    if (letter == "%") break;
                    ms = ms + letter;
                    index++;
                }
                if (letter == "%") break;
            }
            richTextBox1.Text += ms;
        }
    }
}
