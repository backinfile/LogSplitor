namespace LogSplitor
{
    public partial class Form1 : Form
    {
        private string logFilePath;
        private string logDirectory;
        private string outputDirectory;

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
                textBox1.Text = Properties.Settings.Default.LastInputPath;
            }

            // 加载筛选条件
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastFilterConditions))
            {
                textBox2.Text = Properties.Settings.Default.LastFilterConditions;
            }

            // 加载匹配模式
            checkBox1.Checked = Properties.Settings.Default.LastMatchAllCondition;
            
            // 加载大小写敏感设置
            checkBox2.Checked = Properties.Settings.Default.LastCaseSensitive;

            // 加载输出目录
            if (!string.IsNullOrEmpty(Properties.Settings.Default.LastOutputPath))
            {
                outputDirectory = Properties.Settings.Default.LastOutputPath;
                textBox4.Text = outputDirectory;
            }
        }

        private void SaveSettings()
        {
            // 保存输入路径
            Properties.Settings.Default.LastInputPath = textBox1.Text;

            // 保存筛选条件
            Properties.Settings.Default.LastFilterConditions = textBox2.Text;

            // 保存匹配模式
            Properties.Settings.Default.LastMatchAllCondition = checkBox1.Checked;
            
            // 保存大小写敏感设置
            Properties.Settings.Default.LastCaseSensitive = checkBox2.Checked;

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
            bool caseSensitive = checkBox2.Checked;

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox2.Enabled = false;
            textBox4.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;

            try
            {
                string inputPath = textBox1.Text;
                if (Directory.Exists(inputPath))
                {
                    await Task.Run(() => ProcessDirectory(inputPath, filters, matchAll, caseSensitive));
                }
                else
                {
                    await Task.Run(() => ProcessSingleFile(inputPath, filters, matchAll, null, caseSensitive));
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
                checkBox2.Enabled = true;
            }
        }

        private void ProcessDirectory(string directoryPath, string[] filters, bool matchAll, bool caseSensitive)
        {
            AppendLog($"开始处理目录：{directoryPath}");
            var allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories)
                .Select(file => new { Path = file, ModifiedTime = File.GetLastWriteTime(file) })
                .OrderBy(f => f.ModifiedTime)
                .ToList();

            AppendLog($"找到 {allFiles.Count} 个文件，已按修改时间排序");

            // 创建输出文件
            string outputFilePath = Path.Combine(outputDirectory, $"filtered_logs_{DateTime.Now:yyyyMMddHHmmss}.txt");
            Directory.CreateDirectory(outputDirectory);

            int totalMatchCount = 0;
            using (StreamWriter writer = new StreamWriter(outputFilePath, append: true))
            {
                for (int i = 0; i < allFiles.Count; i++)
                {
                    var file = allFiles[i];
                    string fileName = Path.GetFileName(file.Path);
                    AppendLog($"\n正在处理第 {i + 1}/{allFiles.Count} 个文件：{fileName}");
                    int fileMatchCount = ProcessSingleFile(file.Path, filters, matchAll, writer, caseSensitive);
                    totalMatchCount += fileMatchCount;
                }
            }

            AppendLog($"\n所有文件处理完成！总共匹配 {totalMatchCount} 行");
            AppendLog($"输出文件：{outputFilePath}");
        }

        private int ProcessSingleFile(string filePath, string[] filters, bool matchAll, StreamWriter writer = null, bool caseSensitive = true)
        {
            try
            {
                int matchCount = 0;
                string fileName = Path.GetFileName(filePath);
                bool isDirectory = Directory.Exists(filePath);
                StreamWriter localWriter = null;

                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;
                    int lineCount = 0;

                    // 如果是单文件模式且没有提供 writer，创建新的输出文件
                    if (!isDirectory && writer == null)
                    {
                        Directory.CreateDirectory(outputDirectory);
                        string outputFilePath = Path.Combine(outputDirectory, 
                            $"{Path.GetFileNameWithoutExtension(filePath)}_filtered_{DateTime.Now:yyyyMMddHHmmss}.txt");
                        localWriter = new StreamWriter(outputFilePath);
                        writer = localWriter;
                        AppendLog($"输出文件：{outputFilePath}");
                    }

                    while ((line = reader.ReadLine()) != null)
                    {
                        lineCount++;
                        if (lineCount % 1000 == 0)
                        {
                            AppendLog($"处理文件 {fileName}：已处理 {lineCount} 行...");
                        }

                        string compareLine = caseSensitive ? line : line.ToUpper();
                        string[] compareFilters = caseSensitive ? filters : filters.Select(f => f.ToUpper()).ToArray();

                        bool shouldWrite = matchAll ?
                            compareFilters.All(filter => compareLine.Contains(filter)) :
                            compareFilters.Any(filter => compareLine.Contains(filter));

                        if (shouldWrite)
                        {
                            writer?.WriteLine(line);
                            matchCount++;
                        }
                    }

                    // 如果使用了本地 writer，关闭它
                    if (localWriter != null)
                    {
                        localWriter.Dispose();
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
