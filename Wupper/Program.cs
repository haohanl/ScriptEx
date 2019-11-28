using System;
using WUApiLib;

/* NOTES ON COSTURA.FODY
 * The WUApiLib file has been manually forced to be embedded in FodyWeavers.xml
 * under the name Interop.WUApiLib. The automatic detection does not work
 * for some reason. As such, copy to local has been disabled because it is not
 * necessary. 
 */

namespace Wupper
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Wupper (1.0.0), by Haohan Liu (c) 2019\n");
            WUpdateHandler.WUP();
            Console.ReadKey();
        }
    }
    static class WUpdateHandler
    {
        public static void WUP()
        {
            // Check for iAutomaticUpdates.ServiceEnabled...
            IAutomaticUpdates iAutomaticUpdates = new AutomaticUpdates();
            if (!iAutomaticUpdates.ServiceEnabled)
            {
                iAutomaticUpdates.EnableService();
                Console.WriteLine("Automatic updates enabled.");
            }

            // Start searching for updates
            Console.WriteLine("Checking for Windows updates.");
            UpdateSessionClass uSession = new UpdateSessionClass();
            IUpdateSearcher uSearcher = uSession.CreateUpdateSearcher();

            ISearchResult uResult = uSearcher.Search("IsInstalled=0 AND IsHidden=0");

            int uCount = uResult.Updates.Count;

            if (uCount > 0)
            {
                Console.WriteLine("Updates: " + uResult.Updates.Count);
            }
            else
            {
                Console.WriteLine("No updates found! You're up to date.");
                return;
            }


            // Print updates found, and accept eulas
            foreach (IUpdate update in uResult.Updates)
            {
                Console.WriteLine(update.Title);

                if (update.EulaAccepted == false)
                {
                    update.AcceptEula();
                }
            }
            Console.WriteLine();


            // Start downloading updates
            Console.WriteLine("Downloading updates (this could take a while).");
            IUpdateDownloader uDownloader = uSession.CreateUpdateDownloader();
            uDownloader.Updates = uResult.Updates;
            uDownloader.Priority = DownloadPriority.dpHigh;

            // capture download result
            IDownloadResult uDownloadResult = uDownloader.Download();

            // determine if update download was successful
            switch (uDownloadResult.ResultCode)
            {
                case OperationResultCode.orcSucceeded:
                    Console.WriteLine("Downloads complete.");
                    break;
                case OperationResultCode.orcSucceededWithErrors:
                    Console.WriteLine("Downloads incomplete. Some updates won't be installed.");
                    break;
                default:
                    Console.WriteLine("Update download error. Unable to continue.");
                    return;
            }

            // queue successfully downloaded updates
            UpdateCollection uToInstall = new UpdateCollection();
            foreach (IUpdate update in uResult.Updates)
            {
                if (update.IsDownloaded)
                {
                    uToInstall.Add(update);
                }
            }

            // Start installing updates
            Console.WriteLine("\nInstalling updates (this could take a while).");
            IUpdateInstaller uInstaller = uSession.CreateUpdateInstaller();
            uInstaller.Updates = uToInstall;

            // capture install result
            IInstallationResult uInstallResult = uInstaller.Install();

            // determine if update was successful
            switch (uInstallResult.ResultCode)
            {
                case OperationResultCode.orcSucceeded:
                    Console.WriteLine("Updates installed successfully.");
                    break;
                case OperationResultCode.orcSucceededWithErrors:
                    Console.WriteLine("Updates partially installed. Some were not able to be installed.");
                    break;
                default:
                    Console.WriteLine("Update install failed. No updates installed.");
                    break;
            }

            //End
            Console.WriteLine("\nWUPDATE has completed. Please reboot your computer.");
        }
    }
}
