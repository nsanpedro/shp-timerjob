using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Workflow;

public class TimerJobBO : SPJobDefinition
{
    public TimerJobBO() : base()
    {

    }

    public TimerJobBO(string jobName, SPService service) : base(jobName, service, null, SPJobLockType.None)
    {
        this.Title = "Timer job BO-Auditoria";
    }

    public TimerJobBO(string jobName, SPWebApplication webapp) : base(jobName, webapp, null, SPJobLockType.ContentDatabase)
    {
        this.Title = "Timer job BO-Auditoria";
    }

    public override void Execute(Guid targetInstanceId)
    {
        SPWebApplication webApp = this.Parent as SPWebApplication;
        SPSecurity.RunWithElevatedPrivileges(delegate ()
        {
            using (SPSite site = new SPSite("http://g500603sv56c/sites/desarrollo/BO_auditoria"))
            {
                using (SPWeb web = site.OpenWeb())
                {
                    SPList list = web.Lists["Matriz de Riesgos Chile"];
                    SPListItemCollection items = list.GetItems();
                    string workflowName = "Correo Mensual - Matriz de Riesgos Chile";
                    var workflowServiceManager = site.WorkflowManager;
                    SPWorkflowAssociation workflowAssociation = null;
                    foreach (SPWorkflowAssociation iteration in list.WorkflowAssociations)
                    {
                        if (iteration.Name.Equals(workflowName))
                        {
                            workflowAssociation = iteration;
                            break;
                        }
                    }

                    if (workflowAssociation != null)
                    {
                        foreach (SPListItem item in items)
                        {
                            try
                            {
                                string data = workflowAssociation.AssociationData;
                                SPWorkflow wf = workflowServiceManager.StartWorkflow(item, workflowAssociation, data, true);
                            }
                            catch (Exception ex)
                            {
                                //TODO LOGEAR EXCEPCION
                                
                            }
                            
                        }
                    }
                }
            }
        });

        //SPList taskList = webApp.Sites["/sites/desarrollo/BO_auditoria"].RootWeb.Lists["Tasks"];
        //SPListItem newTask = taskList.Items.Add();
        //newTask["Title"] = "Job runs at " + DateTime.Now.ToString();
        //newTask.Update();
    }
}





namespace timerJobBO
{
    class timerJobBO
    {
    }
}
