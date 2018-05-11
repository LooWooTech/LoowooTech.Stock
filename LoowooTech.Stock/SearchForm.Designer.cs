namespace LoowooTech.Stock
{
    partial class SearchForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ValueBox3 = new System.Windows.Forms.ComboBox();
            this.ConditionBox3 = new System.Windows.Forms.ComboBox();
            this.FieldBox3 = new System.Windows.Forms.ComboBox();
            this.RelationBox2 = new System.Windows.Forms.ComboBox();
            this.ValueBox2 = new System.Windows.Forms.ComboBox();
            this.ConditionBox2 = new System.Windows.Forms.ComboBox();
            this.FieldBox2 = new System.Windows.Forms.ComboBox();
            this.RelationBox1 = new System.Windows.Forms.ComboBox();
            this.ValueBox1 = new System.Windows.Forms.ComboBox();
            this.ConditionBox1 = new System.Windows.Forms.ComboBox();
            this.FieldBox1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TableBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.DistrictBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Searchbutton = new System.Windows.Forms.Button();
            this.Closebutton = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ClearWherebutton = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(786, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "请将浙江省相关区县农村存量建设用地调查成果空间数据库放在程序文件夹Datas中";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Size = new System.Drawing.Size(824, 681);
            this.splitContainer1.SplitterDistance = 302;
            this.splitContainer1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Controls.Add(this.ClearWherebutton);
            this.groupBox1.Controls.Add(this.Closebutton);
            this.groupBox1.Controls.Add(this.Searchbutton);
            this.groupBox1.Controls.Add(this.ValueBox3);
            this.groupBox1.Controls.Add(this.ConditionBox3);
            this.groupBox1.Controls.Add(this.FieldBox3);
            this.groupBox1.Controls.Add(this.RelationBox2);
            this.groupBox1.Controls.Add(this.ValueBox2);
            this.groupBox1.Controls.Add(this.ConditionBox2);
            this.groupBox1.Controls.Add(this.FieldBox2);
            this.groupBox1.Controls.Add(this.RelationBox1);
            this.groupBox1.Controls.Add(this.ValueBox1);
            this.groupBox1.Controls.Add(this.ConditionBox1);
            this.groupBox1.Controls.Add(this.FieldBox1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.TableBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.DistrictBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(0, 66);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(824, 232);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "查询条件";
            // 
            // ValueBox3
            // 
            this.ValueBox3.FormattingEnabled = true;
            this.ValueBox3.Location = new System.Drawing.Point(303, 178);
            this.ValueBox3.Name = "ValueBox3";
            this.ValueBox3.Size = new System.Drawing.Size(166, 25);
            this.ValueBox3.TabIndex = 15;
            this.ValueBox3.DropDown += new System.EventHandler(this.ValueBox3_DropDown);
            // 
            // ConditionBox3
            // 
            this.ConditionBox3.FormattingEnabled = true;
            this.ConditionBox3.Items.AddRange(new object[] {
            "=",
            ">=",
            ">",
            "<",
            "<=",
            "Like"});
            this.ConditionBox3.Location = new System.Drawing.Point(229, 178);
            this.ConditionBox3.Name = "ConditionBox3";
            this.ConditionBox3.Size = new System.Drawing.Size(68, 25);
            this.ConditionBox3.TabIndex = 14;
            // 
            // FieldBox3
            // 
            this.FieldBox3.FormattingEnabled = true;
            this.FieldBox3.Location = new System.Drawing.Point(99, 178);
            this.FieldBox3.Name = "FieldBox3";
            this.FieldBox3.Size = new System.Drawing.Size(126, 25);
            this.FieldBox3.TabIndex = 13;
            // 
            // RelationBox2
            // 
            this.RelationBox2.FormattingEnabled = true;
            this.RelationBox2.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.RelationBox2.Location = new System.Drawing.Point(28, 178);
            this.RelationBox2.Name = "RelationBox2";
            this.RelationBox2.Size = new System.Drawing.Size(54, 25);
            this.RelationBox2.TabIndex = 12;
            // 
            // ValueBox2
            // 
            this.ValueBox2.FormattingEnabled = true;
            this.ValueBox2.Location = new System.Drawing.Point(303, 130);
            this.ValueBox2.Name = "ValueBox2";
            this.ValueBox2.Size = new System.Drawing.Size(166, 25);
            this.ValueBox2.TabIndex = 11;
            this.ValueBox2.DropDown += new System.EventHandler(this.ValueBox2_DropDown);
            // 
            // ConditionBox2
            // 
            this.ConditionBox2.FormattingEnabled = true;
            this.ConditionBox2.Items.AddRange(new object[] {
            "=",
            ">=",
            ">",
            "<",
            "<=",
            "Like"});
            this.ConditionBox2.Location = new System.Drawing.Point(229, 130);
            this.ConditionBox2.Name = "ConditionBox2";
            this.ConditionBox2.Size = new System.Drawing.Size(68, 25);
            this.ConditionBox2.TabIndex = 10;
            // 
            // FieldBox2
            // 
            this.FieldBox2.FormattingEnabled = true;
            this.FieldBox2.Location = new System.Drawing.Point(99, 130);
            this.FieldBox2.Name = "FieldBox2";
            this.FieldBox2.Size = new System.Drawing.Size(126, 25);
            this.FieldBox2.TabIndex = 9;
            // 
            // RelationBox1
            // 
            this.RelationBox1.FormattingEnabled = true;
            this.RelationBox1.Items.AddRange(new object[] {
            "AND",
            "OR"});
            this.RelationBox1.Location = new System.Drawing.Point(28, 130);
            this.RelationBox1.Name = "RelationBox1";
            this.RelationBox1.Size = new System.Drawing.Size(54, 25);
            this.RelationBox1.TabIndex = 8;
            // 
            // ValueBox1
            // 
            this.ValueBox1.FormattingEnabled = true;
            this.ValueBox1.Location = new System.Drawing.Point(303, 81);
            this.ValueBox1.Name = "ValueBox1";
            this.ValueBox1.Size = new System.Drawing.Size(166, 25);
            this.ValueBox1.TabIndex = 7;
            this.ValueBox1.DropDown += new System.EventHandler(this.comboBox5_DropDown);
            // 
            // ConditionBox1
            // 
            this.ConditionBox1.FormattingEnabled = true;
            this.ConditionBox1.Items.AddRange(new object[] {
            "=",
            ">=",
            ">",
            "<",
            "<=",
            "Like"});
            this.ConditionBox1.Location = new System.Drawing.Point(229, 81);
            this.ConditionBox1.Name = "ConditionBox1";
            this.ConditionBox1.Size = new System.Drawing.Size(68, 25);
            this.ConditionBox1.TabIndex = 6;
            // 
            // FieldBox1
            // 
            this.FieldBox1.FormattingEnabled = true;
            this.FieldBox1.Location = new System.Drawing.Point(99, 81);
            this.FieldBox1.Name = "FieldBox1";
            this.FieldBox1.Size = new System.Drawing.Size(126, 25);
            this.FieldBox1.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "条件语句：";
            // 
            // TableBox
            // 
            this.TableBox.FormattingEnabled = true;
            this.TableBox.Location = new System.Drawing.Point(488, 37);
            this.TableBox.Name = "TableBox";
            this.TableBox.Size = new System.Drawing.Size(324, 25);
            this.TableBox.TabIndex = 3;
            this.TableBox.SelectedIndexChanged += new System.EventHandler(this.TableBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(414, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "属性表名：";
            // 
            // DistrictBox
            // 
            this.DistrictBox.FormattingEnabled = true;
            this.DistrictBox.Location = new System.Drawing.Point(99, 34);
            this.DistrictBox.Name = "DistrictBox";
            this.DistrictBox.Size = new System.Drawing.Size(274, 25);
            this.DistrictBox.TabIndex = 1;
            this.DistrictBox.SelectedIndexChanged += new System.EventHandler(this.DistrictBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "区县名称：";
            // 
            // Searchbutton
            // 
            this.Searchbutton.Location = new System.Drawing.Point(488, 203);
            this.Searchbutton.Name = "Searchbutton";
            this.Searchbutton.Size = new System.Drawing.Size(75, 23);
            this.Searchbutton.TabIndex = 16;
            this.Searchbutton.Text = "立即查询";
            this.Searchbutton.UseVisualStyleBackColor = true;
            this.Searchbutton.Click += new System.EventHandler(this.Searchbutton_Click);
            // 
            // Closebutton
            // 
            this.Closebutton.Location = new System.Drawing.Point(737, 203);
            this.Closebutton.Name = "Closebutton";
            this.Closebutton.Size = new System.Drawing.Size(75, 23);
            this.Closebutton.TabIndex = 17;
            this.Closebutton.Text = "关闭";
            this.Closebutton.UseVisualStyleBackColor = true;
            this.Closebutton.Click += new System.EventHandler(this.Closebutton_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(824, 375);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // ClearWherebutton
            // 
            this.ClearWherebutton.Location = new System.Drawing.Point(617, 203);
            this.ClearWherebutton.Name = "ClearWherebutton";
            this.ClearWherebutton.Size = new System.Drawing.Size(75, 23);
            this.ClearWherebutton.TabIndex = 19;
            this.ClearWherebutton.Text = "清除条件";
            this.ClearWherebutton.UseVisualStyleBackColor = true;
            this.ClearWherebutton.Click += new System.EventHandler(this.ClearWherebutton_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Location = new System.Drawing.Point(488, 81);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(324, 116);
            this.listView1.TabIndex = 20;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "统计信息";
            this.columnHeader1.Width = 323;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(824, 681);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "查询器";
            this.Load += new System.EventHandler(this.SearchForm_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox TableBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox DistrictBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox ValueBox1;
        private System.Windows.Forms.ComboBox ConditionBox1;
        private System.Windows.Forms.ComboBox FieldBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox ValueBox3;
        private System.Windows.Forms.ComboBox ConditionBox3;
        private System.Windows.Forms.ComboBox FieldBox3;
        private System.Windows.Forms.ComboBox RelationBox2;
        private System.Windows.Forms.ComboBox ValueBox2;
        private System.Windows.Forms.ComboBox ConditionBox2;
        private System.Windows.Forms.ComboBox FieldBox2;
        private System.Windows.Forms.ComboBox RelationBox1;
        private System.Windows.Forms.Button Closebutton;
        private System.Windows.Forms.Button Searchbutton;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button ClearWherebutton;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}