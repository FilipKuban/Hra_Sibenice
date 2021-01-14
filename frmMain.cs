using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace sibenice
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
            GameStart();
        }
        private string sentence;
        private int errors = 0;
        private IEnumerable<char> chosedChars = new List<char>();
        private Random random = new Random();

        private void GameStart()
        {
            sentence = GetSentence();
            chosedChars = new List<char>();
            errors = 0;
            lbSentence.Text = GetMask();
            LoadPicture(errors);
            ShowAllButtons();
        }

        private string GetSentence()
        {
            string fn = GetAppDir + @"\sentences.txt";
            if (File.Exists(fn))
            {
                string[] sentences = File.ReadAllLines(fn);
                int i = random.Next(0, sentences.Count());
                return sentences[i].ToUpper();
            }
            else
            {
                MessageBox.Show($"Soubor {fn} nenalezen");
                return "";
            }
        }

        private void ShowAllButtons()
        {
            foreach (Control control in pnlButtons.Controls)
                if (control is Button)
                    (control as Button).Visible = true;
        }

        private string GetMask()
        {
            string mask = " ";
            foreach (char c in sentence)
            {
                if (c == ' ' || chosedChars.Contains(c))
                {
                    mask += c;
                }
                else
                {
                    mask += "?";
                }
            }
            return mask;
        }

        private bool Win()
        {
            foreach (char c in sentence)
            {
                if (c != ' ' && !chosedChars.Contains(c))
                {
                    return false;
                }
            }
            return true;
        }


        private IList<char> GetCharList(char c)
        {
            IList<char> list = new List<char>();
            list.Add(c);
            switch (c)
            {
                case 'I':
                    list.Add('Í');
                    break;
                case 'E':
                    list.Add('É');
                    list.Add('Ě');
                    break;
                case 'Y':
                    list.Add('Ý');
                    break;
                case 'A':
                    list.Add('Á');
                    break;
                case 'O':
                    list.Add('Ó');
                    break;
                case 'U':
                    list.Add('Ú');
                    list.Add('Ů');
                    break;
                default:
                    break;
            }
            return list;
        }
        private bool Hit(IList<char> list)
        {
            foreach (char c in sentence)
                if (list.Contains(c))
                    return true;
            return false;
        }

        private string GetAppDir
        {
            get
            {
                FileInfo fi = new FileInfo(Application.ExecutablePath);
                return fi.DirectoryName;
            }
        }


        private bool LoadPicture(int i)
        {
            string dir = GetAppDir + @"\pics\";
            string fn = dir + i.ToString().PadLeft(2, '0') + ".bmp";
            if (File.Exists(fn))
            {
                pcPicture.Image = new Bitmap(fn);
                return true;
            }
            return false;
        }
        private void GameOver()
        {
            PlaySoundFromDir("loss");
            MessageBox.Show("Toto je Prohra ! \n Věta zněla takto: " + sentence);
            GameStart();
        }

        private void Miss()
        {
            errors++;
            bool ok = LoadPicture(errors);
            if (!ok)
                GameOver();
            else
                PlaySoundFromDir("miss");
        }
        private void GameWin()
        {
            PlaySoundFromDir("win");
            MessageBox.Show("Hra vyhrána !");
            GameStart();
        }

        private void PlaySoundFromDir(string subDir)
        {
            string dir = GetAppDir + String.Format(@"\sounds\{0}\", subDir);
            string[] files = Directory.GetFiles(dir);
            int i = random.Next(0, files.Count());
            string fn = files[i];
            PlaySound(fn);
        }

        private void PlaySound(string path)
        {
            SoundPlayer sp = new SoundPlayer(path);
            try
            {
                sp.Play();
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Button btn = (sender as Button);
            if (btn != null)
            {
                char c = btn.Text[0];
                IList<char> list = GetCharList(c);
                bool hit = Hit(list);
                chosedChars = chosedChars.Concat(list);
                btn.Visible = false;
                if (hit)
                {

                    lbSentence.Text = GetMask();
                    PlaySoundFromDir("hit");
                    bool win = Win();
                    if (win)
                    {
                        GameWin();
                    }
                }
                else
                {
                    Miss();
                }
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void lbSentence_Click(object sender, EventArgs e)
        {

        }
    }
}
