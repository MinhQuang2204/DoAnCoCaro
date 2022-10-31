using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CoCaro
{
    public partial class FormMusic : Form
    {
        public FormMusic()
        {
            InitializeComponent();
        }

        private void libMusic_SelectedIndexChanged(object sender, EventArgs e)
        {
            MusicFile mediaFile = libMusic.SelectedItem as MusicFile;
            if (mediaFile != null)
            {
                axWindowsMediaPlayer1.URL = mediaFile.Path;
                axWindowsMediaPlayer1.Ctlcontrols.play();
            }
        }
        private void FormMusic_Load(object sender, EventArgs e)
        {
            libMusic.ValueMember = "Path";
            libMusic.DisplayMember = "FileName";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, Filter = "WMV|*.wmv|WAV|*.wav|MP3|*.mp3|MP4|*.mp4|MKV|*.mkv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    List<MusicFile> files = new List<MusicFile>();
                    foreach (string fileName in ofd.FileNames)
                    {
                        FileInfo fi = new FileInfo(fileName);
                        files.Add(new MusicFile() { FileName = Path.GetFileNameWithoutExtension(fi.FullName), Path = fi.FullName });
                    }
                    libMusic.DataSource = files;
                }
            }
        }
    }
}
