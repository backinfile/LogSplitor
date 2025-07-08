namespace LogSplitor
{
    public partial class Form1 : Form
    {
        private string logFilePath;
        private string logDirectory;
        private string outputDirectory;
        private bool isDirectoryMode;

        public Form1()
        {
            InitializeComponent();
            InitializeEvents();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // 加载上次的配置
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastInputPath))
            {
                if (Properties.Settings.Default.IsDirectoryMode)
                {
                    logDirectory = Properties.Settings.Default.LastInputPath;
                    textBox1.Text = logDirectory;
                    isDirectoryMode = true;
                }
                else
                {
                    logFilePath = Properties.Settings.Default.LastInputPath;
                    textBox1.Text = logFilePath;
                    isDirectoryMode = false;
                }
            }

            // 加载筛选条件
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastFilterConditions))
            {
                textBox2.Text = Properties.Settings.Default.LastFilterConditions;
            }

            // 加载匹配模式
            checkBox1.Checked = Properties.Settings.Default.LastMatchAllCondition;

            // 加载输出目录
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastOutputPath))
            {
                outputDirectory = Properties.Settings.Default.LastOutputPath;
                textBox4.Text = outputDirectory;
            }
        }

        private void SaveSettings()
        {
            // 保存输入路径和模式
            Properties.Settings.Default.LastInputPath = textBox1.Text;
            Properties.Settings.Default.IsDirectoryMode = isDirectoryMode;

            // 保存筛选条件
            Properties.Settings.Default.LastFilterConditions = textBox2.Text;

            // 保存匹配模式
            Properties.Settings.Default.LastMatchAllCondition = checkBox1.Checked;

            // 保存输出目录
            Properties.Settings.Default.LastOutputPath = outputDirectory;

            // 保存更改
            Properties.Settings.Default.Save();
        }

        private void InitializeEvents()
        {
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            button4.Click += Button4_Click;
            button5.Click += Button5_Click;

            // 添加窗体关闭事件
            this.FormClosing += (s, e) => SaveSettings();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputDirectory) || !Directory.Exists(outputDirectory))
            {
                MessageBox.Show("输出目录不存在！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                System.Diagnostics.Process.Start("explorer.exe", outputDirectory);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开目录失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "所有文件 (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    logFilePath = openFileDialog.FileName;
                    textBox1.Text = logFilePath;
                    isDirectoryMode = false;

                    // 设置默认输出目录为输入文件所在目录
                    outputDirectory = Path.GetDirectoryName(logFilePath);
                    textBox4.Text = outputDirectory;
                }
            }
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    logDirectory = folderDialog.SelectedPath;
                    textBox1.Text = logDirectory;
                    isDirectoryMode = true;

                    // 设置默认输出目录为选择的目录
                    outputDirectory = logDirectory;
                    textBox4.Text = outputDirectory;
                }
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    outputDirectory = folderDialog.SelectedPath;
                    textBox4.Text = outputDirectory;
                }
            }
        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请先选择日志文件或目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox2.Text))
            {
                MessageBox.Show("请输入筛选条件！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请选择输出目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            outputDirectory = textBox4.Text;
            string[] filters = textBox2.Text.Split('#');
            bool matchAll = checkBox1.Checked;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
            checkBox1.Enabled = false;

            try
            {
                if (isDirectoryMode)
                {
                    await Task.Run(() => ProcessDirectory(filters, matchAll));
                }
                else
                {
                    await Task.Run(() => ProcessSingleFile(logFilePath, filters, matchAll));
                }
                MessageBox.Show("日志切割完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"处理过程中出现错误：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
                textBox2.Enabled = true;
                textBox4.Enabled = true;
                checkBox1.Enabled = true;
            }
        }

        private void ProcessDirectory(string[] filters, bool matchAll)
        {
            AppendLog($"开始处理目录：{logDirectory}");
            var allFiles = Directory.GetFiles(logDirectory, "*.*", SearchOption.AllDirectories)
                .Select(file => new { Path = file, ModifiedTime = File.GetLastWriteTime(file) })
                .OrderBy(f => f.ModifiedTime)
                .ToList();

            AppendLog($"找到 {allFiles.Count} 个文件，已按修改时间排序");

            // 创建输出文件
            string outputFilePath = Path.Combine(outputDirectory, $"filtered_logs_{DateTime.Now:yyyyMMddHHmmss}.txt");
            Directory.CreateDirectory(outputDirectory);

            int totalMatchCount = 0;
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var file = allFiles[i];
                    string fileName = Path.GetFileName(file.Path);
                    AppendLog($"\n正在处理第 {i + 1}/{allFiles.Count} 个文件：{fileName}");
                    int fileMatchCount = ProcessSingleFile(file.Path, filters, matchAll, writer);
                    totalMatchCount += fileMatchCount;
                }
            }

            AppendLog($"\n所有文件处理完成！总共匹配 {totalMatchCount} 行");
            AppendLog($"输出文件：{outputFilePath}");
        }

        private int ProcessSingleFile(string filePath, string[] filters, bool matchAll, StreamWriter writer = null)
        {
            try
            {
                int matchCount = 0;
                string fileName = Path.GetFileName(filePath);
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    int lineCount = 0;

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineCount++;
                        if (lineCount % 1000 == 0)
                        {
                            AppendLog($"处理文件 {fileName}：已处理 {lineCount} 行...");
                        }

                        bool shouldWrite = matchAll ?
                            filters.All(filter => line.Contains(filter)) :
                            filters.Any(filter => line.Contains(filter));

                        if (shouldWrite)
                        {
                            if (isDirectoryMode)
                            {
                                writer?.WriteLine(line);
                            }
                            else
                            {
                                // 单文件模式，需要创建新的输出文件
                                if (writer == null)
                                {
                                    Directory.CreateDirectory(outputDirectory);
                                    string outputFilePath = Path.Combine(outputDirectory, 
                                        $"{Path.GetFileNameWithoutExtension(filePath)}_filtered_{DateTime.Now:yyyyMMddHHmmss}.txt");
                                    using (var singleFileWriter = new StreamWriter(outputFilePath))
                                    {
                                        singleFileWriter.WriteLine(line);
                                    }
                                    AppendLog($"输出文件：{outputFilePath}");
                                }
                            }
                            matchCount++;
                        }
                    }

                    AppendLog($"文件 {fileName} 处理完成！共处理 {lineCount} 行，匹配 {matchCount} 行。");
                }
                return matchCount;
            }
            catch (Exception ex)
            {
                AppendLog($"处理文件 {Path.GetFileName(filePath)} 时出错：{ex.Message}");
                throw;
            }
        }


        private void AppendLog(string message)
        {
            if (textBox5.InvokeRequired)
            {
                textBox5.Invoke(new Action(() => AppendLog(message)));
                return;
            }

            textBox5.AppendText(message + Environment.NewLine);
            textBox5.ScrollToCaret();
        }
    }
}
