using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using TranslateMe.Commands.Support;
using TranslateMe.Model.GitHub;

namespace TranslateMe.Commands
{
    public class UpdateApplicationCommand : Command<GitHubRelease>
    {
        private readonly GitHubClient _gitHubClient;

        public UpdateApplicationCommand()
        {
            _gitHubClient = new GitHubClient();
        }

        public async override void Execute(GitHubRelease release)
        {
            var asset = GetInstallationAsset(release);
            var fileName = await _gitHubClient.DownloadAsset(release, asset);

            StartInstallation(fileName);
            Shutdown();
        }

        private static GitHubAsset GetInstallationAsset(GitHubRelease release)
        {
            var asset =  release.Assets.FirstOrDefault(a => a.ContentType == "application/octet-stream");
            if (asset == null)
                throw new InvalidOperationException("Release is missing installation asset.");

            return asset;
        }

        private static void StartInstallation(string fileName)
        {
            Process.Start(fileName);
        }

        private static void Shutdown()
        {
            Application.Current.Shutdown();
        }
    }
}