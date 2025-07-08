namespace LogSplitor
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            textBox2 = new TextBox();
            textBox4 = new TextBox();
            label2 = new Label();
            button2 = new Button();
            textBox5 = new TextBox();
            checkBox1 = new CheckBox();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new Point(30, 21);
            button1.Name = "button1";
            button1.Size = new Size(112, 23);
            button1.TabIndex = 0;
            button1.Text = "选择文件";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Enabled = false;
            textBox1.Location = new Point(30, 50);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(521, 23);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(27, 97);
            label1.Name = "label1";
            label1.Size = new Size(156, 17);
            label1.TabIndex = 2;
            label1.Text = "筛选条件(以#分隔不同条件)";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(30, 144);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(521, 23);
            textBox2.TabIndex = 3;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(30, 209);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(518, 23);
            textBox4.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(27, 186);
            label2.Name = "label2";
            label2.Size = new Size(56, 17);
            label2.TabIndex = 6;
            label2.Text = "输出目录";
            // 
            // button2
            // 
            button2.Location = new Point(30, 251);
            button2.Name = "button2";
            button2.Size = new Size(116, 23);
            button2.TabIndex = 7;
            button2.Text = "开始切割";
            button2.UseVisualStyleBackColor = true;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(30, 285);
            textBox5.Multiline = true;
            textBox5.Name = "textBox5";
            textBox5.ReadOnly = true;
            textBox5.ScrollBars = ScrollBars.Vertical;
            textBox5.Size = new Size(518, 208);
            textBox5.TabIndex = 8;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(36, 117);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(75, 21);
            checkBox1.TabIndex = 9;
            checkBox1.Text = "同时满足";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(89, 183);
            button3.Name = "button3";
            button3.Size = new Size(112, 23);
            button3.TabIndex = 10;
            button3.Text = "浏览...";
            button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Location = new Point(148, 21);
            button4.Name = "button4";
            button4.Size = new Size(112, 23);
            button4.TabIndex = 11;
            button4.Text = "选择目录";
            button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(152, 251);
            button5.Name = "button5";
            button5.Size = new Size(116, 23);
            button5.TabIndex = 12;
            button5.Text = "打开输出目录";
            button5.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(579, 515);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(checkBox1);
            Controls.Add(textBox5);
            Controls.Add(button2);
            Controls.Add(label2);
            Controls.Add(textBox4);
            Controls.Add(textBox2);
            Controls.Add(label1);
            Controls.Add(textBox1);
            Controls.Add(button1);
            Name = "Form1";
            Text = "日志切割工具";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private TextBox textBox2;
        private TextBox textBox4;
        private Label label2;
        private Button button2;
        private TextBox textBox5;
        private CheckBox checkBox1;
        private Button button3;
        private Button button4;
        private Button button5;
    }
}
