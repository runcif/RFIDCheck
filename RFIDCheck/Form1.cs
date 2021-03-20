using System;
using System.Windows.Forms;
using uFR;

namespace RFIDCheck
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
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

        private void button1_Click(object sender, EventArgs e)
        {
            byte CardType = 0;
            byte[] Uid = new byte[10];
            byte UidSize = 0;

            DL_STATUS status = DL_STATUS.UFR_READER_PORT_NOT_OPENED;
            status = uFCoder.ReaderOpen();

            status = uFCoder.GetCardIdEx(ref CardType, Uid, ref UidSize);

            if (status == DL_STATUS.UFR_OK)
            {
                textBox2.Text = uFCoder.status2str(status);
                textBox1.Text = "[" + BitConverter.ToString(Uid).Replace("-", ":") + "]";
                textBox3.Text = "[" + GetDlTypeName(CardType) + "]";

            }
            else
            {
                textBox2.Text = "Errore lettura.";
                textBox1.Clear();
                textBox3.Clear();

            }


        }

    }
}
