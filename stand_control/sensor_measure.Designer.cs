namespace com_port
{
    partial class sensor_measure
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.begin_button = new System.Windows.Forms.Button();
            this.save_button = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.listView2 = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // begin_button
            // 
            this.begin_button.Location = new System.Drawing.Point(31, 12);
            this.begin_button.Name = "begin_button";
            this.begin_button.Size = new System.Drawing.Size(75, 23);
            this.begin_button.TabIndex = 0;
            this.begin_button.Text = "начать";
            this.begin_button.UseVisualStyleBackColor = true;
            this.begin_button.Click += new System.EventHandler(this.begin_button_Click);
            // 
            // save_button
            // 
            this.save_button.Location = new System.Drawing.Point(153, 12);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(75, 23);
            this.save_button.TabIndex = 1;
            this.save_button.Text = "сохранить";
            this.save_button.UseMnemonic = false;
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // listView1
            // 
            this.listView1.Location = new System.Drawing.Point(31, 81);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(352, 312);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(665, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 41);
            this.button1.TabIndex = 3;
            this.button1.Text = "Обновить эталонные значения";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listView2
            // 
            this.listView2.Location = new System.Drawing.Point(415, 81);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(351, 312);
            this.listView2.TabIndex = 4;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // sensor_measure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listView2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.save_button);
            this.Controls.Add(this.begin_button);
            this.Name = "sensor_measure";
            this.Text = "sensor_measure";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button begin_button;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView listView2;
    }
}