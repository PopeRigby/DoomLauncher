﻿namespace DoomLauncher
{
    partial class StatsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("Winform Designer", "VS2015 SP1")]
        private void InitializeComponent()
        {
            this.tblStats = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.pbKills = new System.Windows.Forms.PictureBox();
            this.pbSecrets = new System.Windows.Forms.PictureBox();
            this.pbItems = new System.Windows.Forms.PictureBox();
            this.ctrlStatsSecrets = new DoomLauncher.StatBar();
            this.ctrlStatsItems = new DoomLauncher.StatBar();
            this.ctrlStatsKills = new DoomLauncher.StatBar();
            this.lblMaps = new System.Windows.Forms.Label();
            this.ctrlStatsMaps = new DoomLauncher.StatBar();
            this.pbMaps = new System.Windows.Forms.PictureBox();
            this.tblStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbKills)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSecrets)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMaps)).BeginInit();
            this.SuspendLayout();
            // 
            // tblStats
            // 
            this.tblStats.ColumnCount = 3;
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tblStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblStats.Controls.Add(this.label1, 1, 2);
            this.tblStats.Controls.Add(this.label2, 1, 3);
            this.tblStats.Controls.Add(this.label3, 1, 4);
            this.tblStats.Controls.Add(this.pbKills, 0, 2);
            this.tblStats.Controls.Add(this.pbSecrets, 0, 3);
            this.tblStats.Controls.Add(this.pbItems, 0, 4);
            this.tblStats.Controls.Add(this.ctrlStatsSecrets, 2, 3);
            this.tblStats.Controls.Add(this.ctrlStatsItems, 2, 4);
            this.tblStats.Controls.Add(this.ctrlStatsKills, 2, 2);
            this.tblStats.Controls.Add(this.lblMaps, 1, 1);
            this.tblStats.Controls.Add(this.ctrlStatsMaps, 2, 1);
            this.tblStats.Controls.Add(this.pbMaps, 0, 1);
            this.tblStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblStats.Location = new System.Drawing.Point(0, 0);
            this.tblStats.Margin = new System.Windows.Forms.Padding(0);
            this.tblStats.MinimumSize = new System.Drawing.Size(240, 0);
            this.tblStats.Name = "tblStats";
            this.tblStats.RowCount = 6;
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tblStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblStats.Size = new System.Drawing.Size(459, 231);
            this.tblStats.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(79, 87);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Kills:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(79, 123);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Secrets:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 160);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Items:";
            // 
            // pbKills
            // 
            this.pbKills.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbKills.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbKills.Location = new System.Drawing.Point(22, 81);
            this.pbKills.Margin = new System.Windows.Forms.Padding(0);
            this.pbKills.Name = "pbKills";
            this.pbKills.Size = new System.Drawing.Size(30, 31);
            this.pbKills.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbKills.TabIndex = 6;
            this.pbKills.TabStop = false;
            // 
            // pbSecrets
            // 
            this.pbSecrets.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbSecrets.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbSecrets.Location = new System.Drawing.Point(22, 118);
            this.pbSecrets.Margin = new System.Windows.Forms.Padding(0);
            this.pbSecrets.Name = "pbSecrets";
            this.pbSecrets.Size = new System.Drawing.Size(30, 31);
            this.pbSecrets.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSecrets.TabIndex = 7;
            this.pbSecrets.TabStop = false;
            // 
            // pbItems
            // 
            this.pbItems.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pbItems.Location = new System.Drawing.Point(22, 155);
            this.pbItems.Margin = new System.Windows.Forms.Padding(0);
            this.pbItems.Name = "pbItems";
            this.pbItems.Size = new System.Drawing.Size(30, 31);
            this.pbItems.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbItems.TabIndex = 8;
            this.pbItems.TabStop = false;
            // 
            // ctrlStatsSecrets
            // 
            this.ctrlStatsSecrets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStatsSecrets.Location = new System.Drawing.Point(156, 118);
            this.ctrlStatsSecrets.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.ctrlStatsSecrets.MaximumSize = new System.Drawing.Size(0, 32);
            this.ctrlStatsSecrets.Name = "ctrlStatsSecrets";
            this.ctrlStatsSecrets.Size = new System.Drawing.Size(297, 26);
            this.ctrlStatsSecrets.TabIndex = 10;
            // 
            // ctrlStatsItems
            // 
            this.ctrlStatsItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStatsItems.Location = new System.Drawing.Point(156, 153);
            this.ctrlStatsItems.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.ctrlStatsItems.MaximumSize = new System.Drawing.Size(0, 32);
            this.ctrlStatsItems.Name = "ctrlStatsItems";
            this.ctrlStatsItems.Size = new System.Drawing.Size(297, 26);
            this.ctrlStatsItems.TabIndex = 11;
            // 
            // ctrlStatsKills
            // 
            this.ctrlStatsKills.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStatsKills.Location = new System.Drawing.Point(156, 80);
            this.ctrlStatsKills.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.ctrlStatsKills.MaximumSize = new System.Drawing.Size(0, 32);
            this.ctrlStatsKills.Name = "ctrlStatsKills";
            this.ctrlStatsKills.Size = new System.Drawing.Size(297, 26);
            this.ctrlStatsKills.TabIndex = 12;
            // 
            // lblMaps
            // 
            this.lblMaps.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblMaps.AutoSize = true;
            this.lblMaps.Location = new System.Drawing.Point(79, 49);
            this.lblMaps.Margin = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.lblMaps.Name = "lblMaps";
            this.lblMaps.Size = new System.Drawing.Size(52, 20);
            this.lblMaps.TabIndex = 13;
            this.lblMaps.Text = "Maps:";
            // 
            // ctrlStatsMaps
            // 
            this.ctrlStatsMaps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlStatsMaps.Location = new System.Drawing.Point(156, 42);
            this.ctrlStatsMaps.Margin = new System.Windows.Forms.Padding(0, 2, 6, 2);
            this.ctrlStatsMaps.MaximumSize = new System.Drawing.Size(0, 32);
            this.ctrlStatsMaps.Name = "ctrlStatsMaps";
            this.ctrlStatsMaps.Size = new System.Drawing.Size(297, 26);
            this.ctrlStatsMaps.TabIndex = 14;
            // 
            // pbMaps
            // 
            this.pbMaps.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbMaps.Location = new System.Drawing.Point(22, 43);
            this.pbMaps.Margin = new System.Windows.Forms.Padding(0);
            this.pbMaps.Name = "pbMaps";
            this.pbMaps.Size = new System.Drawing.Size(30, 31);
            this.pbMaps.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMaps.TabIndex = 15;
            this.pbMaps.TabStop = false;
            // 
            // StatsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblStats);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "StatsControl";
            this.Size = new System.Drawing.Size(459, 231);
            this.tblStats.ResumeLayout(false);
            this.tblStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbKills)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSecrets)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMaps)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblStats;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.PictureBox pbKills;
        private System.Windows.Forms.PictureBox pbSecrets;
        private System.Windows.Forms.PictureBox pbItems;
        private StatBar ctrlStatsSecrets;
        private StatBar ctrlStatsItems;
        private StatBar ctrlStatsKills;
        private System.Windows.Forms.Label lblMaps;
        private StatBar ctrlStatsMaps;
        private System.Windows.Forms.PictureBox pbMaps;
    }
}
