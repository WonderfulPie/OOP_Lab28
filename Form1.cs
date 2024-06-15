namespace Lab_27_Danylko
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.VisualBasic;

    public static class TreeViewExtensions
    {
        public static List<TreeNode> SelectedNodes(this TreeView treeView)
        {
            List<TreeNode> selectedNodes = new List<TreeNode>();
            GetSelectedNodes(treeView.Nodes, selectedNodes);
            return selectedNodes;
        }

        private static void GetSelectedNodes(TreeNodeCollection nodes, List<TreeNode> selectedNodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.BackColor == SystemColors.Highlight && node.ForeColor == SystemColors.HighlightText)
                {
                    selectedNodes.Add(node);
                }
                GetSelectedNodes(node.Nodes, selectedNodes);
            }
        }
    }

    public partial class Form1 : Form
    {
        private ImageList imageList;
        private string copySourcePath;
        private bool isCutOperation;
        private List<string> copySourcePaths = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeImageList();
            LoadDrives();
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            listView1.MultiSelect = true;
            listView1.MouseDown += listView1_MouseDown;
            listView1.KeyDown += listView1_KeyDown;
            treeView1.KeyDown += treeView1_KeyDown;
            treeView1.ContextMenuStrip = contextMenuStrip1;
            listView1.ContextMenuStrip = contextMenuStrip1;

            listView1.ItemSelectionChanged += listView1_ItemSelectionChanged; // Додайте цей рядок

            ToolStripMenuItem selectToolStripMenuItem = new ToolStripMenuItem("Highlight");
            selectToolStripMenuItem.Click += SelectToolStripMenuItem_Click;
            contextMenuStrip1.Items.Add(selectToolStripMenuItem);
            treeView1.MouseDown += treeView1_MouseDown;

            contextMenuStrip1.Opening += ContextMenuStrip1_Opening;

            ToolStripMenuItem createToolStripMenuItem = new ToolStripMenuItem("Create");
            ToolStripMenuItem createFolderToolStripMenuItem = new ToolStripMenuItem("Folder");
            ToolStripMenuItem createFileToolStripMenuItem = new ToolStripMenuItem("File");
            createFolderToolStripMenuItem.Click += CreateFolderToolStripMenuItem_Click;
            createFileToolStripMenuItem.Click += CreateFileToolStripMenuItem_Click;
            createToolStripMenuItem.DropDownItems.Add(createFolderToolStripMenuItem);
            createToolStripMenuItem.DropDownItems.Add(createFileToolStripMenuItem);
            contextMenuStrip1.Items.Add(createToolStripMenuItem);
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = treeView1.GetNodeAt(e.X, e.Y);
                if (node != null)
                {
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        node.BackColor = node.BackColor == SystemColors.Highlight ? treeView1.BackColor : SystemColors.Highlight;
                        node.ForeColor = node.ForeColor == SystemColors.HighlightText ? treeView1.ForeColor : SystemColors.HighlightText;
                    }
                    else
                    {
                        ClearTreeViewSelection(treeView1.Nodes);
                        node.BackColor = SystemColors.Highlight;
                        node.ForeColor = SystemColors.HighlightText;
                    }
                }
                else
                {
                    ClearTreeViewSelection(treeView1.Nodes); // Знімаємо виділення, якщо натиснуто на пусте місце
                }
            }
        }

        private void ClearTreeViewSelection(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                node.BackColor = treeView1.BackColor;
                node.ForeColor = treeView1.ForeColor;
                ClearTreeViewSelection(node.Nodes);
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ListViewItem item = listView1.GetItemAt(e.X, e.Y);
                if (item != null)
                {
                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
                    {
                        item.Selected = !item.Selected; // Перемикаємо стан виділення
                    }
                    else
                    {
                        listView1.SelectedItems.Clear();
                        item.Selected = true;
                    }
                }
                else
                {
                    listView1.SelectedItems.Clear(); // Знімаємо виділення, якщо натиснуто на пусте місце
                }
            }
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItems();
                e.Handled = true;
            }
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedItems();
                e.Handled = true;
            }
        }

        private void ListView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                copyToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                pasteToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
        }

        private void TreeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                copyToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                pasteToolStripMenuItem_Click(sender, e);
                e.Handled = true;
            }
        }

        private void ContextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Tag is DriveInfo)
            {
                e.Cancel = true; // Забороняємо відкриття контекстного меню на дисках
            }
        }


        private void SelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.FocusedItem != null)
            {
                listView1.FocusedItem.Selected = true;
            }

            if (treeView1.SelectedNode != null)
            {
                treeView1.SelectedNode.BackColor = SystemColors.Highlight;
                treeView1.SelectedNode.ForeColor = SystemColors.HighlightText;
            }
        }

        private void InitializeImageList()
        {
            imageList = new ImageList();
            imageList.ImageSize = new Size(16, 16);

            try
            {
                imageList.Images.Add("drive", ShellIcon.GetSmallIcon("shell32.dll", 8).ToBitmap());
                imageList.Images.Add("folder", ShellIcon.GetSmallIcon("shell32.dll", 3).ToBitmap());
                imageList.Images.Add("file", ShellIcon.GetSmallIcon("shell32.dll", 0).ToBitmap());
                imageList.Images.Add("text", Image.FromFile("Icons/text.png"));
                imageList.Images.Add("image", Image.FromFile("Icons/image.png"));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"Icon file not found: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading icons: {ex.Message}");
            }

            treeView1.ImageList = imageList;
            listView1.SmallImageList = imageList;
        }

        private void LoadDrives()
        {
            try
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    TreeNode node = new TreeNode(drive.Name, 0, 0) { Tag = drive };
                    treeView1.Nodes.Add(node);
                    if (drive.IsReady)
                        node.Nodes.Add(new TreeNode());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading drives: {ex.Message}");
            }
        }

        private void LoadDirectoriesAndFiles(TreeNode node)
        {
            DirectoryInfo dir = node.Tag as DirectoryInfo ?? (node.Tag as DriveInfo)?.RootDirectory;
            if (dir == null) return;

            try
            {
                node.Nodes.Clear(); // Очищаємо дочірні вузли перед завантаженням нових

                foreach (var subDir in dir.GetDirectories())
                {
                    try
                    {
                        TreeNode subNode = new TreeNode(subDir.Name, 1, 1) { Tag = subDir };
                        subNode.Nodes.Add(new TreeNode()); // Додаємо пустий вузол для відображення кнопки "плюс"
                        node.Nodes.Add(subNode);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }

                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        string imageKey = GetFileIconKey(file.Extension);
                        TreeNode fileNode = new TreeNode(file.Name, imageKey == "text" ? 3 : imageKey == "image" ? 4 : 2, imageKey == "text" ? 3 : imageKey == "image" ? 4 : 2) { Tag = file };
                        node.Nodes.Add(fileNode);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (FileNotFoundException) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading directories and files: {ex.Message}");
            }
        }

        private string GetFileIconKey(string extension)
        {
            switch (extension.ToLower())
            {
                case ".txt":
                    return "text";
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".bmp":
                case ".gif":
                    return "image";
                default:
                    return "file";
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            if (node.Nodes.Count == 1 && node.Nodes[0].Tag == null)
            {
                LoadDirectoriesAndFiles(node);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode selectedNode = e.Node;
            propertyGrid1.SelectedObject = selectedNode.Tag;
            listView1.Items.Clear();

            if (selectedNode.Tag is DriveInfo drive)
            {
                if (drive.IsReady)
                    LoadFilesAndDirectories(drive.RootDirectory);
            }
            else if (selectedNode.Tag is DirectoryInfo dir)
            {
                LoadFilesAndDirectories(dir);
            }
            else if (selectedNode.Tag is FileInfo file)
            {
                LoadFileContent(file);
                openButton.Visible = file.Extension == ".txt" || IsImageFile(file.Extension);
            }
        }

        private void LoadFilesAndDirectories(DirectoryInfo dir)
        {
            try
            {
                foreach (var subDir in dir.GetDirectories())
                {
                    try
                    {
                        ListViewItem item = new ListViewItem(subDir.Name, "folder") { Tag = subDir };
                        item.SubItems.Add("Directory");
                        listView1.Items.Add(item);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }

                foreach (var file in dir.GetFiles())
                {
                    try
                    {
                        string imageKey = GetFileIconKey(file.Extension);
                        ListViewItem item = new ListViewItem(file.Name, imageKey) { Tag = file };
                        item.SubItems.Add("File");
                        listView1.Items.Add(item);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (FileNotFoundException) { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading files and directories: {ex.Message}");
            }
        }
        private bool IsImageFile(string extension)
        {
            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            return imageExtensions.Contains(extension.ToLower());
        }
        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                var file = e.Item.Tag as FileInfo;
                openButton.Visible = file != null && (file.Extension == ".txt" || IsImageFile(file.Extension));
                propertyGrid1.SelectedObject = e.Item.Tag;
                if (file != null)
                {
                    LoadFileContent(file);
                }
            }
            else if (listView1.SelectedItems.Count == 0)
            {
                openButton.Visible = false;
            }
        }

        private void LoadFileContent(FileInfo file)
        {
            try
            {
                if (file.Extension == ".txt")
                {
                    textBox1.Text = File.ReadAllText(file.FullName);
                    textBox1.Visible = true;
                    pictureBox1.Image = null;
                    pictureBox1.Visible = false;
                    openButton.Visible = true;
                    previewLabel.Visible = true;
                }
                else if (IsImageFile(file.Extension))
                {
                    pictureBox1.Image = Image.FromFile(file.FullName);
                    pictureBox1.Visible = true;
                    textBox1.Clear();
                    textBox1.Visible = false;
                    openButton.Visible = true;
                    previewLabel.Visible = true; // Показати напис Preview
                }
                else
                {
                    textBox1.Clear();
                    textBox1.Visible = false;
                    pictureBox1.Image = null;
                    pictureBox1.Visible = false;
                    openButton.Visible = false;
                    previewLabel.Visible = false;
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show($"File not found: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Access denied to file: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading file content: {ex.Message}");
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string filter = comboBox1.SelectedItem.ToString().ToLower();
            foreach (ListViewItem item in listView1.Items)
            {
                if (filter == "all" || item.SubItems[1].Text.ToLower().Contains(filter))
                {
                    item.ForeColor = SystemColors.WindowText;
                }
                else
                {
                    item.ForeColor = SystemColors.GrayText;
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;
        }

        private void RefreshTreeNode(TreeNode node)
        {
            if (node == null) return;

            DirectoryInfo dir = node.Tag as DirectoryInfo ?? (node.Tag as DriveInfo)?.RootDirectory;
            if (dir == null) return;

            node.Nodes.Clear();  // Очищаємо дочірні вузли перед завантаженням нових

            try
            {
                foreach (var subDir in dir.GetDirectories())
                {
                    TreeNode subNode = new TreeNode(subDir.Name, 1, 1) { Tag = subDir };
                    subNode.Nodes.Add(new TreeNode());  // Додаємо пустий вузол для відображення кнопки "плюс"
                    node.Nodes.Add(subNode);
                }

                foreach (var file in dir.GetFiles())
                {
                    string imageKey = GetFileIconKey(file.Extension);
                    TreeNode fileNode = new TreeNode(file.Name, imageKey == "text" ? 3 : imageKey == "image" ? 4 : 2, imageKey == "text" ? 3 : imageKey == "image" ? 4 : 2) { Tag = file };
                    node.Nodes.Add(fileNode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while refreshing directories and files: {ex.Message}");
            }
        }

        // Оновлюємо метод створення каталога
        private void CreateFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var selectedNode = treeView1.SelectedNode;
                DirectoryInfo dir = selectedNode.Tag as DirectoryInfo ?? (selectedNode.Tag as DriveInfo)?.RootDirectory;

                if (dir != null)
                {
                    string newDirName = "New Folder";
                    string newDirPath = Path.Combine(dir.FullName, newDirName);
                    int count = 1;
                    while (Directory.Exists(newDirPath))
                    {
                        newDirName = $"New Folder ({count++})";
                        newDirPath = Path.Combine(dir.FullName, newDirName);
                    }

                    try
                    {
                        Directory.CreateDirectory(newDirPath);
                        // Оновлюємо вузол після створення каталога
                        RefreshTreeNode(selectedNode);
                        MessageBox.Show("Directory created successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while creating directory: {ex.Message}");
                    }
                }
            }
        }

        // Оновлюємо метод створення файлу
        private void CreateFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var selectedNode = treeView1.SelectedNode;
                DirectoryInfo dir = selectedNode.Tag as DirectoryInfo ?? (selectedNode.Tag as DriveInfo)?.RootDirectory;

                if (dir != null)
                {
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Enter the name and extension of the file:", "Create File", "NewFile.txt");
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        MessageBox.Show("You must enter a name and extension for the file.");
                        return;
                    }

                    string newFilePath = Path.Combine(dir.FullName, input);

                    try
                    {
                        using (File.Create(newFilePath)) { }
                        // Оновлюємо вузол після створення файлу
                        RefreshTreeNode(selectedNode);
                        MessageBox.Show("File created successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while creating file: {ex.Message}");
                    }
                }
            }
        }


        private void moveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var selectedNodes = treeView1.SelectedNodes();
                using (var moveDialog = new MoveDialog())
                {
                    if (moveDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (TreeNode selectedNode in selectedNodes)
                        {
                            string sourcePath = (selectedNode.Tag as DirectoryInfo)?.FullName ?? (selectedNode.Tag as FileInfo)?.FullName;
                            if (sourcePath == null) continue;

                            string destPath = Path.Combine(moveDialog.SelectedPath, Path.GetFileName(sourcePath));
                            try
                            {
                                if (Directory.Exists(sourcePath))
                                {
                                    if (Directory.Exists(destPath))
                                    {
                                        DialogResult result = MessageBox.Show($"Directory {Path.GetFileName(destPath)} already exists. Replace?", "Directory Exists", MessageBoxButtons.YesNoCancel);
                                        if (result == DialogResult.No)
                                        {
                                            continue;
                                        }
                                        else if (result == DialogResult.Yes)
                                        {
                                            Directory.Delete(destPath, true);
                                        }
                                        else if (result == DialogResult.Cancel)
                                        {
                                            return;
                                        }
                                    }
                                    Directory.Move(sourcePath, destPath);
                                }
                                else if (File.Exists(sourcePath))
                                {
                                    if (File.Exists(destPath))
                                    {
                                        DialogResult result = MessageBox.Show($"File {Path.GetFileName(destPath)} already exists. Replace?", "File Exists", MessageBoxButtons.YesNoCancel);
                                        if (result == DialogResult.No)
                                        {
                                            continue;
                                        }
                                        else if (result == DialogResult.Yes)
                                        {
                                            File.Delete(destPath);
                                        }
                                        else if (result == DialogResult.Cancel)
                                        {
                                            return;
                                        }
                                    }
                                    File.Move(sourcePath, destPath);
                                }
                                selectedNode.Remove();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"An error occurred while moving item: {ex.Message}");
                            }
                        }

                        // Оновлюємо дерево після переміщення
                        var parentNode = FindNodeByPath(treeView1.Nodes, moveDialog.SelectedPath);
                        if (parentNode != null)
                        {
                            foreach (TreeNode selectedNode in selectedNodes)
                            {
                                string destPath = Path.Combine(moveDialog.SelectedPath, Path.GetFileName((selectedNode.Tag as DirectoryInfo)?.FullName ?? (selectedNode.Tag as FileInfo)?.FullName));
                                if (Directory.Exists(destPath))
                                {
                                    TreeNode newNode = new TreeNode(Path.GetFileName(destPath), 1, 1) { Tag = new DirectoryInfo(destPath) };
                                    parentNode.Nodes.Add(newNode);
                                    LoadDirectoriesAndFiles(newNode); // Завантажуємо вміст нового каталогу
                                }
                                else if (File.Exists(destPath))
                                {
                                    TreeNode newNode = new TreeNode(Path.GetFileName(destPath), 2, 2) { Tag = new FileInfo(destPath) };
                                    parentNode.Nodes.Add(newNode);
                                }
                            }
                            parentNode.Expand();
                        }

                        MessageBox.Show("Items moved successfully!");
                    }
                }
            }
        }

        private TreeNode FindNodeByPath(TreeNodeCollection nodes, string path)
        {
            foreach (TreeNode node in nodes)
            {
                string nodePath = (node.Tag as DirectoryInfo)?.FullName ?? (node.Tag as DriveInfo)?.RootDirectory.FullName;
                if (nodePath == null) continue;

                if (nodePath.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    return node;
                }

                var foundNode = FindNodeByPath(node.Nodes, path);
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            copySourcePaths = new List<string>();

            if (listView1.SelectedItems.Count > 0)
            {
                copySourcePaths = listView1.SelectedItems.Cast<ListViewItem>()
                    .Select(item => (item.Tag as DirectoryInfo)?.FullName ?? (item.Tag as FileInfo)?.FullName)
                    .Where(path => path != null)
                    .ToList();
                isCutOperation = false;
                MessageBox.Show("Items copied successfully!");
            }
            else if (treeView1.SelectedNode != null)
            {
                copySourcePaths = treeView1.SelectedNodes()
                    .Select(node => (node.Tag as DirectoryInfo)?.FullName ?? (node.Tag as FileInfo)?.FullName)
                    .Where(path => path != null)
                    .ToList();
                isCutOperation = false;
                MessageBox.Show("Items copied successfully!");
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null && copySourcePaths != null && copySourcePaths.Count > 0)
            {
                var selectedNode = treeView1.SelectedNode;
                DirectoryInfo dir = selectedNode.Tag as DirectoryInfo ?? (selectedNode.Tag as DriveInfo)?.RootDirectory;

                if (dir != null)
                {
                    foreach (var copySourcePath in copySourcePaths)
                    {
                        string destPath = Path.Combine(dir.FullName, Path.GetFileName(copySourcePath));
                        try
                        {
                            if (Directory.Exists(copySourcePath))
                            {
                                if (Directory.Exists(destPath))
                                {
                                    DialogResult result = MessageBox.Show($"Directory {Path.GetFileName(destPath)} already exists. Replace?", "Directory Exists", MessageBoxButtons.YesNoCancel);
                                    if (result == DialogResult.No)
                                    {
                                        continue;
                                    }
                                    else if (result == DialogResult.Yes)
                                    {
                                        Directory.Delete(destPath, true);
                                    }
                                    else if (result == DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                }
                                CopyDirectory(copySourcePath, destPath);
                            }
                            else if (File.Exists(copySourcePath))
                            {
                                if (File.Exists(destPath))
                                {
                                    DialogResult result = MessageBox.Show($"File {Path.GetFileName(destPath)} already exists. Replace?", "File Exists", MessageBoxButtons.YesNoCancel);
                                    if (result == DialogResult.No)
                                    {
                                        continue;
                                    }
                                    else if (result == DialogResult.Yes)
                                    {
                                        File.Delete(destPath);
                                    }
                                    else if (result == DialogResult.Cancel)
                                    {
                                        return;
                                    }
                                }
                                File.Copy(copySourcePath, destPath, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"An error occurred while pasting item: {ex.Message}");
                        }
                    }
                    MessageBox.Show("Items pasted successfully!");
                    LoadDirectoriesAndFiles(selectedNode);
                }
            }
        }
        public void RefreshFileView(string filePath)
        {
            var file = new FileInfo(filePath);
            var node = FindNodeByPath(treeView1.Nodes, file.DirectoryName);

            if (node != null)
            {
                LoadDirectoriesAndFiles(node);
            }

            if (Path.GetExtension(filePath).ToLower() == ".txt")
            {
                textBox1.Text = File.ReadAllText(filePath);
                textBox1.Visible = true;
                previewLabel.Visible = true;
            }
        }
        private void openButton_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                var selectedItem = listView1.SelectedItems[0];
                var file = selectedItem.Tag as FileInfo;
                if (file != null && (file.Extension == ".txt" || IsImageFile(file.Extension)))
                {
                    var textEditorForm = new TextEditorForm(file.FullName, this);
                    textEditorForm.Show();
                }
            }
            else if (treeView1.SelectedNode != null)
            {
                var selectedNode = treeView1.SelectedNode;
                var file = selectedNode.Tag as FileInfo;
                if (file != null && (file.Extension == ".txt" || IsImageFile(file.Extension)))
                {
                    var textEditorForm = new TextEditorForm(file.FullName, this);
                    textEditorForm.Show();
                }
            }
        }

        private void DeleteSelectedItems()
        {
            List<TreeNode> nodesToDelete = new List<TreeNode>();
            List<ListViewItem> itemsToDelete = new List<ListViewItem>();

            // Збираємо вузли для видалення
            if (treeView1.SelectedNode != null)
            {
                var selectedNodes = treeView1.SelectedNodes();
                nodesToDelete.AddRange(selectedNodes);
            }

            // Збираємо елементи для видалення
            foreach (ListViewItem selectedItem in listView1.SelectedItems)
            {
                itemsToDelete.Add(selectedItem);
            }

            // Підтвердження видалення всіх виділених об'єктів
            var result = MessageBox.Show("Are you sure you want to delete the selected item(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result != DialogResult.Yes)
            {
                return;
            }

            int successCount = 0;
            int errorCount = 0;

            // Видаляємо файли і каталоги з файлової системи
            foreach (TreeNode node in nodesToDelete)
            {
                string path = (node.Tag as DirectoryInfo)?.FullName ?? (node.Tag as FileInfo)?.FullName;
                if (path != null)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        // Видаляємо вузол з TreeView
                        if (node.Parent != null)
                        {
                            node.Parent.Nodes.Remove(node);
                        }
                        else
                        {
                            treeView1.Nodes.Remove(node);
                        }

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting {path}: {ex.Message}");
                        errorCount++;
                    }
                }
            }

            foreach (ListViewItem item in itemsToDelete)
            {
                string path = (item.Tag as DirectoryInfo)?.FullName ?? (item.Tag as FileInfo)?.FullName;
                if (path != null)
                {
                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path, true);
                        }
                        else if (File.Exists(path))
                        {
                            File.Delete(path);
                        }

                        // Видаляємо елемент з ListView
                        listView1.Items.Remove(item);

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting {path}: {ex.Message}");
                        errorCount++;
                    }
                }
            }

            // Очищуємо вибір після видалення
            treeView1.SelectedNode = null;
            listView1.SelectedItems.Clear();

            MessageBox.Show($"{successCount} items deleted successfully! {errorCount} errors occurred.");
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedItems();
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode != null)
            {
                var selectedNode = treeView1.SelectedNode;
                string path = (selectedNode.Tag as DirectoryInfo)?.FullName ?? (selectedNode.Tag as FileInfo)?.FullName;
                if (path == null) return;

                string newName = Interaction.InputBox("Enter new name:", "Rename", Path.GetFileName(path));
                if (!string.IsNullOrEmpty(newName))
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(path), newName);

                    try
                    {
                        if (Directory.Exists(path))
                        {
                            Directory.Move(path, newPath);
                        }
                        else if (File.Exists(path))
                        {
                            File.Move(path, newPath);
                        }
                        selectedNode.Text = newName;
                        selectedNode.Tag = Directory.Exists(newPath) ? (object)new DirectoryInfo(newPath) : new FileInfo(newPath);
                        MessageBox.Show("Item renamed successfully!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while renaming item: {ex.Message}");
                    }
                }
            }
        }

        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                if (File.Exists(destFile))
                {
                    DialogResult result = MessageBox.Show($"File {Path.GetFileName(destFile)} already exists. Replace?", "File Exists", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes)
                    {
                        File.Copy(file, destFile, true);
                    }
                    else if (result == DialogResult.No)
                    {
                        continue;
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }
                else
                {
                    File.Copy(file, destFile);
                }
            }
            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string destDirectory = Path.Combine(destDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDirectory);
            }
        }

        public static class ShellIcon
        {
            [DllImport("Shell32.dll")]
            public static extern int ExtractIconEx(string file, int index, IntPtr[] largeIcon, IntPtr[] smallIcon, int icons);

            public static Icon GetSmallIcon(string file, int index)
            {
                IntPtr[] smallIcon = new IntPtr[1];
                ExtractIconEx(file, index, null, smallIcon, 1);
                return Icon.FromHandle(smallIcon[0]);
            }
        }
    }
}