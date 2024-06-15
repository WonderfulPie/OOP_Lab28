using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Lab_27_Danylko
{
    public partial class TextEditorForm : Form
    {
        private string filePath;
        private Form1 mainForm;

        public TextEditorForm(string filePath, Form1 mainForm)
        {
            InitializeComponent();
            this.filePath = filePath;
            this.mainForm = mainForm;
            LoadFile();
        }

        private void LoadFile()
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                if (extension == ".txt")
                {
                    textBox1.Text = File.ReadAllText(filePath);
                    textBox1.Visible = true;
                    pictureBox1.Visible = false;
                    saveButton.Visible = true;
                    saveAsButton.Visible = true;
                }
                else if (IsImageFile(extension))
                {
                    pictureBox1.Image = Image.FromFile(filePath);
                    pictureBox1.Visible = true;
                    textBox1.Visible = false;
                    saveButton.Visible = false;
                    saveAsButton.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}");
            }
        }

        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return imageExtensions.Contains(extension.ToLower());
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(filePath, textBox1.Text);
                MessageBox.Show("File saved successfully!");
                mainForm.RefreshFileView(filePath);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}");
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.FileName = Path.GetFileName(filePath);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string newFilePath = saveFileDialog.FileName;
                try
                {
                    File.WriteAllText(newFilePath, textBox1.Text);
                    MessageBox.Show("File saved successfully!");
                    mainForm.RefreshFileView(newFilePath);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}");
                }
            }
        }

        private void closeWithoutSavingButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
