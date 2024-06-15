using System;
using System.Windows.Forms;

public partial class MoveDialog : Form
{
    public string SelectedPath { get; private set; }

    public MoveDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.textBoxPath = new System.Windows.Forms.TextBox();
        this.buttonBrowse = new System.Windows.Forms.Button();
        this.buttonOk = new System.Windows.Forms.Button();
        this.buttonCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // textBoxPath
        // 
        this.textBoxPath.Location = new System.Drawing.Point(12, 12);
        this.textBoxPath.Name = "textBoxPath";
        this.textBoxPath.Size = new System.Drawing.Size(360, 20);
        this.textBoxPath.TabIndex = 0;
        // 
        // buttonBrowse
        // 
        this.buttonBrowse.Location = new System.Drawing.Point(378, 10);
        this.buttonBrowse.Name = "buttonBrowse";
        this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
        this.buttonBrowse.TabIndex = 1;
        this.buttonBrowse.Text = "Browse...";
        this.buttonBrowse.UseVisualStyleBackColor = true;
        this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
        // 
        // buttonOk
        // 
        this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.buttonOk.Location = new System.Drawing.Point(297, 38);
        this.buttonOk.Name = "buttonOk";
        this.buttonOk.Size = new System.Drawing.Size(75, 23);
        this.buttonOk.TabIndex = 2;
        this.buttonOk.Text = "OK";
        this.buttonOk.UseVisualStyleBackColor = true;
        this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
        // 
        // buttonCancel
        // 
        this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.buttonCancel.Location = new System.Drawing.Point(378, 38);
        this.buttonCancel.Name = "buttonCancel";
        this.buttonCancel.Size = new System.Drawing.Size(75, 23);
        this.buttonCancel.TabIndex = 3;
        this.buttonCancel.Text = "Cancel";
        this.buttonCancel.UseVisualStyleBackColor = true;
        // 
        // MoveDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(465, 73);
        this.Controls.Add(this.buttonCancel);
        this.Controls.Add(this.buttonOk);
        this.Controls.Add(this.buttonBrowse);
        this.Controls.Add(this.textBoxPath);
        this.Name = "MoveDialog";
        this.Text = "Select Destination";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void buttonBrowse_Click(object sender, EventArgs e)
    {
        using (var folderDialog = new FolderBrowserDialog())
        {
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxPath.Text = folderDialog.SelectedPath;
            }
        }
    }

    private void buttonOk_Click(object sender, EventArgs e)
    {
        SelectedPath = textBoxPath.Text;
    }

    private System.Windows.Forms.TextBox textBoxPath;
    private System.Windows.Forms.Button buttonBrowse;
    private System.Windows.Forms.Button buttonOk;
    private System.Windows.Forms.Button buttonCancel;
}