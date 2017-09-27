using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace timerJobBO.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("72cedfc0-ac06-4342-b7df-b7a191aa70eb")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.
        const string JobName = "Timer job BO-Auditoria";
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                    DeleteExistingJob(JobName, parentWebApp);
                    CreateJob(parentWebApp);
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool CreateJob(SPWebApplication site)
        {
            bool jobCreated = false;
            try
            {
                TimerJobBO job = new TimerJobBO(JobName, site);
                SPMonthlySchedule monthly = new SPMonthlySchedule();
                monthly.BeginDay = 1;
                monthly.BeginHour = 4;
                monthly.BeginMinute = 0;
                monthly.BeginSecond = 0;
                monthly.EndDay = 1;
                monthly.EndHour = 5;
                monthly.EndMinute = 59;
                monthly.EndSecond = 59;
                job.Schedule = monthly;

                job.Update();
            }
            catch (Exception)
            {
                return jobCreated;
            }

            return jobCreated;
        }
        public bool DeleteExistingJob(string jobName, SPWebApplication site)
        {
            bool jobDeleted = false;
            try
            {
                foreach (SPJobDefinition job in site.JobDefinitions)
                {
                    if (job.Name == jobName)
                    {
                        job.Delete();
                        jobDeleted = true;
                    }
                }
            }
            catch (Exception)
            {
                return jobDeleted;
            }

            return jobDeleted;
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {

            lock (this)
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        SPWebApplication parentWebApp = (SPWebApplication)properties.Feature.Parent;
                        DeleteExistingJob(JobName, parentWebApp);
                    });
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
