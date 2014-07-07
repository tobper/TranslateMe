using System;
using System.Reflection;
using System.Windows;
using TranslateMe.Commands.Support;
using TranslateMe.Model.GitHub;
using TranslateMe.Properties;
using TranslateMe.UI;
using TranslateMe.UI.Windows;

namespace TranslateMe.Commands
{
    public class CheckForUpdatesCommand : Command<MainWindow>
    {
        private readonly GitHubClient _gitHubClient;

        public CheckForUpdatesCommand()
        {
            _gitHubClient = new GitHubClient();
        }

        public async override void Execute(MainWindow window)
        {
            var release = await _gitHubClient.GetLatestRelease();
            if (release == null)
                return;

            var currentVersion = GetCurrentVersion();
            var releaseVersion = GetReleaseVersion(release);
            if (releaseVersion <= currentVersion)
                return;

            if (ConfirmDownload(releaseVersion) &&
                window.CloseDocument())
            {
                var updateCommand = new UpdateApplicationCommand();
                if (updateCommand.CanExecute(release))
                    updateCommand.Execute(release);
            }
        }

        private static bool ConfirmDownload(Version releaseVersion)
        {
            var question = string.Format("A new version ({0}) has been released.{1}{1}Do you want to download and install it?", releaseVersion, Environment.NewLine);
            var result = QuestionBox.Show(question, false);

            return result == MessageBoxResult.Yes;
        }

        private static Version GetReleaseVersion(GitHubRelease release)
        {
            return Version.Parse(release.TagName.Substring(1));
        }

        private static Version GetCurrentVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

    }
}