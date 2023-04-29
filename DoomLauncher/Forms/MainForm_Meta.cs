﻿using DoomLauncher.Forms;
using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private bool m_cancelMetaUpdate;

        private async void updateMetadataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IdGamesDataAdapater adapter = new IdGamesDataAdapater(AppConfiguration.IdGamesUrl, AppConfiguration.ApiPage, AppConfiguration.MirrorUrl);
            IGameFile[] localFiles = SelectedItems(GetCurrentViewControl());
            if (localFiles.Length == 0)
                return;

            bool showForm = true, showError = true, updateView = false;
            m_cancelMetaUpdate = false;
            DialogResult result = DialogResult.Cancel;
            MetaDataForm form = CreateMetaForm();
            ProgressBarForm progress = InitMetaProgressBar();
            List<string> iwadWarn = new List<string>();
            HashSet<int> iwads = new HashSet<int>(DataSourceAdapter.GetGameFileIWads().Select(x => x.GameFileID.Value));

            foreach (IGameFile localFile in localFiles)
            {
                if (iwads.Contains(localFile.GameFileID.Value))
                {
                    IWadInfo info = IWadInfo.GetIWadInfo(localFile.FileNameNoPath);
                    if (info != null && !info.HasMetadata)
                    {
                        iwadWarn.Add(localFile.FileNameNoPath);
                        continue;
                    }
                }

                try
                {

                    Enabled = false;
                    progress.DisplayText = string.Format("Searching for {0}...", localFile.FileNameNoPath);
                    progress.Show(this);

                    IEnumerable<IGameFile> remoteFiles = await Task.Run(() => GetMetaFiles(adapter, localFile.FileNameNoPath));

                    Enabled = true;
                    progress.Hide();

                    if (remoteFiles == null || m_cancelMetaUpdate)
                        break;

                    if (!remoteFiles.Any())
                    {
                        if (showError)
                            showError = HandleMetaError(localFile);
                    }
                    else
                    {
                        IGameFile remoteFile = HandleMultipleMetaFilesFound(localFile, remoteFiles);

                        if (remoteFile != null)
                        {
                            form.GameFileEdit.SetDataSource(remoteFile, new ITagData[] { });

                            if (showForm) //OK = Accept current file, Yes = Accept All files
                                result = form.ShowDialog(this);

                            if (result != DialogResult.Cancel)
                            {
                                List<GameFileFieldType> fields = form.GameFileEdit.UpdateDataSource(localFile);
                                showForm = (result == DialogResult.OK);

                                if (fields.Count > 0)
                                    updateView = HandleUpdateMetaFields(localFile, fields);
                            }
                        }
                    }
                }
                catch
                {
                    Enabled = true;
                    progress.Hide();

                    MessageBox.Show(this, "Failed to fetch metadata from the id games mirror.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break; //not expected, break from loop
                }
            }

            if (updateView)
                HandleSelectionChange(GetCurrentViewControl(), true);

            if (iwadWarn.Count > 0)
            {
                MessageBox.Show(this, "The following are IWADs and will not exist in idgames: " + string.Join(", ", iwadWarn), 
                    "IWAD Files", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private IEnumerable<IGameFile> GetMetaFiles(IdGamesDataAdapater adapter, string metaFileName)
        {
            return adapter.GetGameFilesByName(metaFileName);
        }

        private ProgressBarForm InitMetaProgressBar()
        {
            ProgressBarForm progress = new ProgressBarForm
            {
                Title = "Fetching data...",
                Minimum = 0,
                Maximum = 0
            };
            progress.SetCancelAllowed(false);
            return progress;
        }

        private IGameFile HandleMultipleMetaFilesFound(IGameFile localFile, IEnumerable<IGameFile> remoteFiles)
        {
            if (remoteFiles.Count() == 1)
                return remoteFiles.First();

            FillFileSize(localFile);
            IEnumerable<IGameFile> check = remoteFiles.Where(x => x.FileSizeBytes == localFile.FileSizeBytes);

            if (check.Count() == 1)
                return check.First();

            FileSelectForm form = new FileSelectForm();
            form.Initialize(DataSourceAdapter, m_tabHandler.TabViews.First(x => x.Key.Equals(TabKeys.IdGamesKey)), remoteFiles);
            form.ShowSearchControl(false);
            string display = localFile.FileName;
            if (!string.IsNullOrEmpty(localFile.Title))
                display = string.Format("{0}({1})", localFile.Title, localFile.FileNameNoPath);
            form.SetDisplayText(string.Format("Multiple files found for {0}. Please select intended file.", display));
            form.MultiSelect = false;
            form.StartPosition = FormStartPosition.CenterParent;

            if (form.ShowDialog() != DialogResult.Cancel)
            {
                IGameFile[] selectedFiles = form.SelectedFiles;

                if (selectedFiles.Length > 0)
                    return selectedFiles.First();
            }

            return null;
        }

        private void FillFileSize(IGameFile localFile)
        {
            FileInfo fi = new FileInfo(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), localFile.FileName));

            if (fi.Exists)
                localFile.FileSizeBytes = Convert.ToInt32(fi.Length);
        }

        private MetaDataForm CreateMetaForm()
        {
            MetaDataForm form = new MetaDataForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.GameFileEdit.SetCheckBoxesChecked(true);
            form.GameFileEdit.CommentsChecked = false;
            form.GameFileEdit.MapsChecked = false;
            return form;
        }

        private bool HandleUpdateMetaFields(IGameFile localFile, List<GameFileFieldType> fields)
        {
            DataSourceAdapter.UpdateGameFile(localFile, fields.ToArray());
            UpdateDataSourceViews(localFile);
            return true;
        }

        private bool HandleMetaError(IGameFile localFile)
        {
            MessageCheckBox errorForm = new MessageCheckBox("Meta", 
                string.Format("Failed to find {0} from the id games mirror.\n\nIf you are sure this file should exist try changing your mirror in the Settings menu.", localFile.FileNameNoPath),
                "Don't show this error again", SystemIcons.Error);
            errorForm.StartPosition = FormStartPosition.CenterParent;
            errorForm.ShowDialog(this);
            return !errorForm.Checked;
        }
    }
}
