using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using uFR;

namespace RFIDCheck
{

    public partial class Form1 : Form
    {
        DL_STATUS status = DL_STATUS.UFR_READER_PORT_NOT_OPENED;

        private byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        private int BlockSize = 128;

        public Form1()
        {
            InitializeComponent();
            leggi_scheda("");
        }
        String GetDlTypeName(byte dl_type_code)
        {
            String s;

            switch (dl_type_code)
            {
                case DL_MIFARE_ULTRALIGHT:
                    s = "DL_MIFARE_ULTRALIGHT";
                    break;
                case DL_MIFARE_ULTRALIGHT_EV1_11:
                    s = "DL_MIFARE_ULTRALIGHT_EV1_11";
                    break;
                case DL_MIFARE_ULTRALIGHT_EV1_21:
                    s = "DL_MIFARE_ULTRALIGHT_EV1_21";
                    break;
                case DL_MIFARE_ULTRALIGHT_C:
                    s = "DL_MIFARE_ULTRALIGHT_C";
                    break;
                case DL_NTAG_203:
                    s = "DL_NTAG_203";
                    break;
                case DL_NTAG_210:
                    s = "DL_NTAG_210";
                    break;
                case DL_NTAG_212:
                    s = "DL_NTAG_212";
                    break;
                case DL_NTAG_213:
                    s = "DL_NTAG_213";
                    break;
                case DL_NTAG_215:
                    s = "DL_NTAG_215";
                    break;
                case DL_NTAG_216:
                    s = "DL_NTAG_216";
                    break;
                case DL_MIFARE_MINI:
                    s = "DL_MIFARE_MINI";
                    break;
                case DL_MIFARE_CLASSIC_1K:
                    s = "DL_MIFARE_CLASSIC_1K";
                    break;
                case DL_MIFARE_CLASSIC_4K:
                    s = "DL_MIFARE_CLASSIC_4K";
                    break;
                case DL_MIFARE_DESFIRE:
                    s = "DL_MIFARE_DESFIRE";
                    break;
                case DL_MIFARE_DESFIRE_EV1_2K:
                    s = "DL_MIFARE_DESFIRE_EV1_2K";
                    break;
                case DL_MIFARE_DESFIRE_EV1_4K:
                    s = "DL_MIFARE_DESFIRE_EV1_4K";
                    break;
                case DL_MIFARE_DESFIRE_EV1_8K:
                    s = "DL_MIFARE_DESFIRE_EV1_8K";
                    break;
                default:
                    s = "UNSUPPORTED CARD";
                    break;
            }

            return s;
        }
        const byte DL_MIFARE_ULTRALIGHT = 0x01,
                  DL_MIFARE_ULTRALIGHT_EV1_11 = 0x02,
                  DL_MIFARE_ULTRALIGHT_EV1_21 = 0x03,
                  DL_MIFARE_ULTRALIGHT_C = 0x04,
                  DL_NTAG_203 = 0x05,
                  DL_NTAG_210 = 0x06,
                  DL_NTAG_212 = 0x07,
                  DL_NTAG_213 = 0x08,
                  DL_NTAG_215 = 0x09,
                  DL_NTAG_216 = 0x0A,
                  DL_MIFARE_MINI = 0x20,
                  DL_MIFARE_CLASSIC_1K = 0x21,
                  DL_MIFARE_CLASSIC_4K = 0x22,
                  DL_MIFARE_PLUS_S_2K = 0x23,
                  DL_MIFARE_PLUS_S_4K = 0x24,
                  DL_MIFARE_PLUS_X_2K = 0x25,
                  DL_MIFARE_PLUS_X_4K = 0x26,
                  DL_MIFARE_DESFIRE = 0x27,
                  DL_MIFARE_DESFIRE_EV1_2K = 0x28,
                  DL_MIFARE_DESFIRE_EV1_4K = 0x29,
                  DL_MIFARE_DESFIRE_EV1_8K = 0x2A;

        public static byte[] ToByteArray(String HexString)
        {

            int NumberChars = HexString.Length;
            byte[] bytes = new byte[NumberChars / 2];

            if (HexString.Length % 2 != 0)
            {
                return bytes;
            }

            for (int i = 0; i < NumberChars; i += 2)
            {
                try
                {
                    bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
                }
                catch (System.FormatException)
                {
                    MessageBox.Show("Incorrect format!");
                    break;
                }
            }

            return bytes;
        }


        public static string ConvertHex(String hexString)
        {
            try
            {
                string ascii = string.Empty;

                for (int i = 0; i < hexString.Length; i += 2)
                {
                    String hs = string.Empty;

                    hs = hexString.Substring(i, 2);
                    uint decval = System.Convert.ToUInt32(hs, 16);
                    char character = System.Convert.ToChar(decval);
                    ascii += character;

                }

                return ascii;
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            return string.Empty;
        }

        public static string AsciiToHex(string asciiString)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in asciiString)
            {
                builder.Append(Convert.ToInt32(c).ToString("X"));
            }
            return builder.ToString();
        }

        public string Encrypt(String textToEncrypt,String password)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(textToEncrypt);
                //Encrypt
                SymmetricAlgorithm crypt = Aes.Create();
                HashAlgorithm hash = MD5.Create();
                crypt.BlockSize = BlockSize;
                crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(password));
                crypt.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream =
                       new CryptoStream(memoryStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(bytes, 0, bytes.Length);
                    }

                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            } 
            catch (Exception e)
            {            
                MessageBox.Show("" + e);
                return "0";
            }
           }

        public string Decrypt(String textToDecrypt, String password)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(textToDecrypt);
                SymmetricAlgorithm crypt = Aes.Create();
                HashAlgorithm hash = MD5.Create();
                crypt.Key = hash.ComputeHash(Encoding.Unicode.GetBytes(password));
                crypt.IV = IV;

                using (MemoryStream memoryStream = new MemoryStream(bytes))
                {
                    using (CryptoStream cryptoStream =
                       new CryptoStream(memoryStream, crypt.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] decryptedBytes = new byte[bytes.Length];
                        cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                        return Encoding.Unicode.GetString(decryptedBytes);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("" + e);
                return "0";
                
            }
        }
        public void leggi_scheda(String password)
        {
            byte CardType = 0;
            byte[] Uid = new byte[10];
            byte UidSize = 0;

            byte[] LinearData = new byte[2048];
            String LinAddr = "0";
            String Len = "100";
            ushort linear_address = 0;
            ushort data_len = 0;

            try
            {

                linear_address = ushort.Parse(LinAddr);

            }
            catch (System.FormatException)
            {

                MessageBox.Show("Byte di partenza sbagliato!");
                textBox2.Clear();
            }

            try
            {
                data_len = ushort.Parse(Len);

            }
            catch (System.FormatException)
            {
                MessageBox.Show("Lunghezza sbagliata!");
                textBox2.Clear();
            }


            ushort bytes_written = 0;

            status = uFCoder.ReaderOpen();

            status = uFCoder.GetCardIdEx(ref CardType, Uid, ref UidSize);

            if (status == DL_STATUS.UFR_OK)
            {
                textBox2.Text = uFCoder.status2str(status);
                textBox1.Text = "[" + BitConverter.ToString(Uid).Replace("-", ":") + "]";
                textBox3.Text = "[" + GetDlTypeName(CardType) + "]";
                status = uFCoder.LinearRead(LinearData, linear_address, data_len, ref bytes_written, 0x60, 0);

                textBox2.Text = uFCoder.status2str(status);
                if (status == 0)
                {

                    byte[] LinearShow = new byte[data_len];

                    Array.Copy(LinearData, LinearShow, data_len);

                    richTextBox1.Text = BitConverter.ToString(LinearShow).Replace("-", ":");
                    richTextBox1.AppendText("\n\r"+ConvertHex(BitConverter.ToString(LinearShow).Replace("-", "")));
                    String stringa_ottenuta = ConvertHex(BitConverter.ToString(LinearShow).Replace("-", ""));

                    if (!String.IsNullOrEmpty(password))
                    {
                        textBox4.Text = Decrypt(stringa_ottenuta.Replace("\0", string.Empty), password);
                    }
                    else
                    {
                        textBox4.Text = stringa_ottenuta.Replace("\0", string.Empty);

                    }
                }
                else
                {
                    richTextBox1.Clear();
                }
                

            }
            else
            {
                textBox2.Text = "Errore lettura.";
                textBox1.Clear();
                textBox3.Clear();
                textBox4.Clear();
                richTextBox1.Clear();

            }


        }

        public void scrivi_scheda(String testo, String password)
        {
            if (status == DL_STATUS.UFR_OK)
            {
                byte[] LWData = new byte[2048];
                String LinAddr = "0";
                String LWData_Str = AsciiToHex(Encrypt(testo, password));

                ushort linear_address = 0;


                try
                {
                    linear_address = ushort.Parse(LinAddr);

                }
                catch (System.FormatException)
                {
                    MessageBox.Show("Byte di partenza sbagliato!");

                }

                ushort byte_written = 0;
                LWData = ToByteArray(LWData_Str);
                Int32 data_len = LWData_Str.Length / 2;

                status = uFCoder.LinearWrite(LWData, linear_address, (ushort)data_len, ref byte_written, 0x60, 0);

                textBox2.Text = uFCoder.status2str(status);
                MessageBox.Show("Scrittura Eseguita!");

            }
            else
            {
                MessageBox.Show("Scheda non pronta!");
            }
        }

        public void format_scheda()
        {
            if (status == DL_STATUS.UFR_OK)
            {
                byte[] LWData = new byte[2048];
                String LinAddr = "0";
                String LWData_Str = "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";

                ushort linear_address = 0;


                try
                {
                    linear_address = ushort.Parse(LinAddr);

                }
                catch (System.FormatException)
                {
                    MessageBox.Show("Byte di partenza sbagliato!");

                }

                ushort byte_written = 0;
                LWData = ToByteArray(LWData_Str);
                Int32 data_len = LWData_Str.Length / 2;

                status = uFCoder.LinearWrite(LWData, linear_address, (ushort)data_len, ref byte_written, 0x60, 0);

                textBox2.Text = uFCoder.status2str(status);
                MessageBox.Show("Formattazione Eseguita!");

            }
            else
            {
                MessageBox.Show("Scheda non pronta!");
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            
            textBox1.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox2.Clear();
            richTextBox1.Clear();

            leggi_scheda("");
            format_scheda();
            leggi_scheda("");

        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count_scrivi = CountChars(textBox4.Text.ToString());
            int count_psw = CountChars(textBox5.Text.ToString());
            if (count_scrivi <= 30 && count_psw <= 30)
            {
                scrivi_scheda(textBox4.Text.ToString(), textBox5.Text.ToString());
                leggi_scheda(textBox5.Text.ToString());
                textBox4.Clear();
            }
            else
            {
                MessageBox.Show("Non puoi superare i 30 caratteri!");
            }

           
        }

        static int CountChars(string value)
        {
            int result = 0;
            bool lastWasSpace = false;

            foreach (char c in value)
            {
                if (char.IsWhiteSpace(c))
                {
                    // A.
                    // Only count sequential spaces one time.
                    if (lastWasSpace == false)
                    {
                        result++;
                    }
                    lastWasSpace = true;
                }
                else
                {
                    // B.
                    // Count other characters every time.
                    result++;
                    lastWasSpace = false;
                }
            }
            return result;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            
           leggi_scheda(textBox5.Text.ToString());
            
        }





    }
}
