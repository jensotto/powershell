﻿using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Utilities;
using System.Management.Automation;

namespace PnP.PowerShell.Commands.SiteDesigns
{
    [Cmdlet(VerbsCommon.Get, "PnPSiteDesignTask")]
    public class GetSiteDesignTask : PnPWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public TenantSiteDesignTaskPipeBind Identity;

        [Parameter(Mandatory = false)]
        public string WebUrl;

        protected override void ExecuteCmdlet()
        {
            var url = CurrentWeb.EnsureProperty(w => w.Url);
            var tenantUrl = Connection.TenantAdminUrl ?? UrlUtilities.GetTenantAdministrationUrl(ClientContext.Url);
            using (var tenantContext = ClientContext.Clone(tenantUrl))
            {
                if (Identity != null)
                {
                    var task = Tenant.GetSiteDesignTask(tenantContext, Identity.Id);
                    tenantContext.Load(task);
                    tenantContext.ExecuteQueryRetry();
                    WriteObject(task);
                }
                else
                {
                    var tenant = new Tenant(tenantContext);
                    var webUrl = url;
                    if (!string.IsNullOrEmpty(WebUrl))
                    {
                        try
                        {
                            var uri = new System.Uri(WebUrl);
                            webUrl = WebUrl;
                        }
                        catch
                        {
                            ThrowTerminatingError(new ErrorRecord(new System.Exception("Invalid URL"), "INVALIDURL", ErrorCategory.InvalidArgument, WebUrl));
                        }
                    }
                    var tasks = tenant.GetSiteDesignTasks(webUrl);
                    tenantContext.Load(tasks);
                    tenantContext.ExecuteQueryRetry();
                    WriteObject(tasks, true);
                }
            }
        }
    }
}