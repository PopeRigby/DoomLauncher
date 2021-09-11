﻿using DoomLauncher.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DoomLauncher
{
    public partial class MainForm
    {
        private ProgressBarForm m_progressBarSync;

        private async Task SyncLocalDatabase(string[] fileNames, FileManagement fileManagement, bool updateViews, ITagData tag = null)
        {
            if (m_progressBarSync == null)
            {
                m_progressBarSync = CreateProgressBar("Updating...", ProgressBarStyle.Continuous);
                ProgressBarStart(m_progressBarSync);
            }

            SyncLibraryHandler handler = await Task.Run(() => ExecuteSyncHandler(fileNames, fileManagement, tag));

            ProgressBarEnd(m_progressBarSync);
            m_progressBarSync = null;
            SyncLocalDatabaseComplete(handler, updateViews);
        }

        void SyncLocalDatabaseComplete(SyncLibraryHandler handler, bool updateViews)
        {
            if (updateViews)
            {
                UpdateLocal();
                HandleTabSelectionChange();

                foreach (IGameFile updateGameFile in handler.UpdatedGameFiles)
                    UpdateDataSourceViews(updateGameFile);
            }

            if (handler != null &&
                (handler.InvalidFiles.Length > 0 || m_zdlInvalidFiles.Count > 0))
            {
                DisplayInvalidFilesError(handler.InvalidFiles.Union(m_zdlInvalidFiles));
            }
            else if (m_launchFile != null)
            {
                IGameFile launchFile = DataSourceAdapter.GetGameFile(Path.GetFileName(m_launchFile));
                m_launchFile = null;
                if (launchFile != null)
                    HandlePlay(new IGameFile[] { launchFile });
            }
        }

        private void DisplayInvalidFilesError(IEnumerable<InvalidFile> invalidFiles)
        {
            StringBuilder sb = new StringBuilder();

            foreach (InvalidFile file in invalidFiles)
            {
                sb.Append(file.FileName);
                sb.Append(": ");
                sb.Append(file.Reason);
                sb.Append(Environment.NewLine);
            }

            ShowTextBoxForm("Processing Errors",
                "The information on these files may be incomplete.\n\nFor ZDL files adding the missing Source Port/IWAD name and re-adding will update the information.\n\nFor zip archive/pk3 errors: Doom Launcher uses a zip library that has very strict zip rules that not all applications respect.\n\nVerify the zip by opening it with your favorite zip application. Create a new zip file and extract the files from the original zip into the newly created one. Then add the newly created zip to Doom Launcher.", 
                sb.ToString(), true);
        }

        private SyncLibraryHandler ExecuteSyncHandler(string[] files, FileManagement fileManagement, ITagData tag = null)
        {
            SyncLibraryHandler handler = null;

            try
            {
                handler = new SyncLibraryHandler(DataSourceAdapter, DirectoryDataSourceAdapter,
                    AppConfiguration.GameFileDirectory, AppConfiguration.TempDirectory, AppConfiguration.DateParseFormats, fileManagement);
                handler.SyncFileChange += syncHandler_SyncFileChange;
                handler.GameFileDataNeeded += syncHandler_GameFileDataNeeded;

                handler.Execute(files);

                if (m_pendingZdlFiles != null)
                {
                    SyncPendingZdlFiles();
                    m_pendingZdlFiles = null;
                }

                if (tag != null)
                    TagSyncFiles(handler, tag);
        }
            catch (Exception ex)
            {
                Util.DisplayUnexpectedException(this, ex);
        }

            return handler;
        }

        private void TagSyncFiles(SyncLibraryHandler handler, ITagData tag)
        {
            DataCache.Instance.AddGameFileTag(handler.AddedGameFiles, tag, out _);
            DataCache.Instance.AddGameFileTag(handler.UpdatedGameFiles, tag, out _);
            DataCache.Instance.TagMapLookup.Refresh(new ITagData[] { tag });
        }

        private void SyncPendingZdlFiles()
        {
            foreach(IGameFile gameFile in m_pendingZdlFiles)
            {
                IGameFile libraryGameFile = DataSourceAdapter.GetGameFile(gameFile.FileName);

                if (libraryGameFile != null)
                {
                    libraryGameFile.SettingsSkill = gameFile.SettingsSkill;
                    libraryGameFile.SettingsMap = gameFile.SettingsMap;
                    libraryGameFile.SettingsExtraParams = gameFile.SettingsExtraParams;
                    libraryGameFile.SourcePortID = gameFile.SourcePortID;
                    libraryGameFile.IWadID = gameFile.IWadID;
                    libraryGameFile.SettingsSkill = gameFile.SettingsSkill;
                    libraryGameFile.SettingsFiles = gameFile.SettingsFiles;

                    if (string.IsNullOrEmpty(libraryGameFile.Comments))
                        libraryGameFile.Comments = gameFile.Comments;

                    DataSourceAdapter.UpdateGameFile(libraryGameFile);
                }
            }
        }

        void syncHandler_SyncFileChange(object sender, EventArgs e)
        {
            SyncLibraryHandler handler = sender as SyncLibraryHandler;

            if (handler != null)
            {
                if (InvokeRequired)
                    Invoke(new Action<SyncLibraryHandler>(ProgressBarUpdate), new object[] { handler });
                else
                    ProgressBarUpdate(handler);
            }
        }

        void syncHandler_GameFileDataNeeded(object sender, EventArgs e)
        {
            SyncLibraryHandler handler = sender as SyncLibraryHandler;

            if (handler != null)
            {
                if (InvokeRequired)
                    Invoke(new Action<SyncLibraryHandler>(HandleGameFileDataNeeded), new object[] { handler });
                else
                    HandleGameFileDataNeeded(handler);
            }
        }

        void HandleGameFileDataNeeded(SyncLibraryHandler handler)
        {
            if (CurrentDownloadFile != null && CurrentDownloadFile.FileName == handler.CurrentGameFile.FileName)
            {
                handler.CurrentGameFile.Title = CurrentDownloadFile.Title;
                handler.CurrentGameFile.Author = CurrentDownloadFile.Author;
                handler.CurrentGameFile.ReleaseDate = CurrentDownloadFile.ReleaseDate;
            }
        }

        void ProgressBarUpdate(SyncLibraryHandler handler)
        {
            if (m_progressBarSync != null)
            {
                m_progressBarSync.Maximum = handler.SyncFileCount;
                m_progressBarSync.Value = handler.SyncFileCurrent;
                m_progressBarSync.DisplayText = string.Format("Reading {0}...", handler.CurrentSyncFileName);
            }
        }

        void ProgressBarForm_Cancelled(object sender, EventArgs e)
        {
            Enabled = true;
            BringToFront();
        }

        private void SyncIWads(FileAddResults fileAddResults)
        {
            foreach (string file in fileAddResults.GetAllFiles())
            {
                IGameFile gameFile = DataSourceAdapter.GetGameFile(file);

                if (gameFile != null && !gameFile.IWadID.HasValue)
                {
                    DataSourceAdapter.InsertIWad(new IWadData() { GameFileID = gameFile.GameFileID.Value, FileName = file, Name = file });
                    var iwad = DataSourceAdapter.GetIWads().OrderBy(x => x.IWadID).LastOrDefault();

                    IWadInfo wadInfo = IWadInfo.GetIWadInfo(gameFile.FileName);
                    gameFile.Title = wadInfo == null ? Path.GetFileNameWithoutExtension(gameFile.FileName).ToUpper() : wadInfo.Title;
                    DataSourceAdapter.UpdateGameFile(gameFile, new GameFileFieldType[] { GameFileFieldType.Title });

                    if (iwad != null)
                    {
                        gameFile.IWadID = iwad.IWadID;
                        DataSourceAdapter.UpdateGameFile(gameFile, new[] { GameFileFieldType.IWadID });
                    }
                }
            }

            UpdateLocal();
            HandleTabSelectionChange();
        }

        private async void HandleSyncStatus()
        {
            IEnumerable<string> dsFiles = DirectoryDataSourceAdapter.GetGameFileNames();
            IEnumerable<string> dbFiles = DataSourceAdapter.GetGameFileNames();
            IEnumerable<string> diff = dsFiles.Except(dbFiles);

            SyncStatusForm form = ShowSyncStatusForm("Sync Status", "Files that exist in the GameFiles directory but not the Database:", diff,
                new string[] { "Do Nothing", "Add to Library", "Delete" });
            Task task = HandleSyncStatusGameFilesOption((SyncFileOption)form.SelectedOptionIndex, form.GetSelectedFiles());
            await task;

            diff = dbFiles.Except(dsFiles);

            form = ShowSyncStatusForm("Sync Status", "Files that exist in the Database but not the GameFiles directory:", diff,
                new string[] { "Do Nothing", "Find in idgames", "Delete" });
            task = HandleSyncStatusLibraryOptions((SyncFileOption)form.SelectedOptionIndex, form.GetSelectedFiles());
            await task;
        }

        private async Task HandleSyncStatusGameFilesOption(SyncFileOption option, IEnumerable<string> files)
        {
            ProgressBarForm form = CreateProgressBar(string.Empty, ProgressBarStyle.Marquee);

            switch (option)
            {
                case SyncFileOption.Add:
                    m_progressBarSync = CreateProgressBar("Updating...", ProgressBarStyle.Continuous);
                    ProgressBarStart(m_progressBarSync);

                    SyncLibraryHandler handler = await Task.Run(() => ExecuteSyncHandler(files.ToArray(), FileManagement.Managed));

                    ProgressBarEnd(m_progressBarSync);
                    SyncLocalDatabaseComplete(handler, true);
                    break;

                case SyncFileOption.Delete:
                    form.DisplayText = "Deleting...";
                    ProgressBarStart(form);

                    await Task.Run(() => DeleteLocalGameFiles(files));

                    ProgressBarEnd(form);
                    break;

                default:
                    break;
            }
        }

        private void DeleteLocalGameFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                try
                {
                    File.Delete(Path.Combine(AppConfiguration.GameFileDirectory.GetFullPath(), file));
                }
                catch
                {
                    //failed, nothing to do
                }
            }
        }

        private async Task HandleSyncStatusLibraryOptions(SyncFileOption option, IEnumerable<string> files)
        {
            ProgressBarForm form = new ProgressBarForm();
            form.ProgressBarStyle = ProgressBarStyle.Marquee;

            switch (option)
            {
                case SyncFileOption.Add:
                    form.DisplayText = "Searching...";
                    ProgressBarStart(form);

                    List<IGameFile> gameFiles = await Task.Run(() => FindIdGamesFiles(files));
                    foreach (IGameFile gameFile in gameFiles)
                        m_downloadHandler.Download(IdGamesDataSourceAdapter, gameFile as IGameFileDownloadable);

                    ProgressBarEnd(form);
                    DisplayFilesNotFound(files, gameFiles);

                    if (gameFiles.Count > 0)
                        DisplayDownloads();

                    break;

                case SyncFileOption.Delete:
                    form.DisplayText = "Deleting...";
                    ProgressBarStart(form);

                    await Task.Run(() => DeleteLibraryGameFiles(files));

                    ProgressBarEnd(form);
                    UpdateLocal();
                    break;

                default:
                    break;
            }
        }

        private void DisplayFilesNotFound(IEnumerable<string> files, List<IGameFile> gameFiles)
        {
            IEnumerable<string> filesNotFound = files.Except(gameFiles.Select(x => x.FileName));

            if (filesNotFound.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (string file in filesNotFound)
                {
                    sb.Append(file);
                    sb.Append(Environment.NewLine);
                }

                TextBoxForm form = new TextBoxForm(true, MessageBoxButtons.OK);
                form.Text = "Files Not Found";
                form.HeaderText = "The following files were not found in the idgames database:";
                form.DisplayText = sb.ToString();
                form.ShowDialog(this);
            }
        }

        private List<IGameFile> FindIdGamesFiles(IEnumerable<string> files)
        {
            List<IGameFile> gameFiles = new List<IGameFile>();

            foreach (string file in files)
            {
                IGameFile gameFile = IdGamesDataSourceAdapter.GetGameFile(file);
                if (gameFile != null)
                    gameFiles.Add(gameFile);
            }

            return gameFiles;
        }

        private void DeleteLibraryGameFiles(IEnumerable<string> files)
        {
            foreach (string file in files)
            {
                IGameFile gameFile = DataSourceAdapter.GetGameFile(file);
                if (gameFile != null && gameFile.GameFileID.HasValue)
                    DeleteGameFileAndAssociations(gameFile);
            }
        }

        private SyncStatusForm ShowSyncStatusForm(string title, string header, IEnumerable<string> files, IEnumerable<string> dropDownOptions)
        {
            SyncStatusForm form = new SyncStatusForm();
            form.Text = title;
            form.SetHeaderText(header);
            form.SetData(files, dropDownOptions);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
            return form;
        }
    }
}
