﻿using Microsoft.SharePoint.Client;
using PnP.Framework.Utilities;

using System.Management.Automation;
using File = Microsoft.SharePoint.Client.File;

namespace PnP.PowerShell.Commands.Files
{
    [Cmdlet(VerbsCommon.Get, "PnPFileVersion", DefaultParameterSetName = "Return as file object")]
 
    public class GetFileVersion : PnPWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Url;

        protected override void ExecuteCmdlet()
        {
            var serverRelativeUrl = string.Empty;

            var webUrl = SelectedWeb.EnsureProperty(w => w.ServerRelativeUrl);

            if (!Url.ToLower().StartsWith(webUrl.ToLower()))
            {
                serverRelativeUrl = UrlUtility.Combine(webUrl, Url);
            }
            else
            {
                serverRelativeUrl = Url;
            }

            File file;

            file = SelectedWeb.GetFileByServerRelativePath(ResourcePath.FromDecodedUrl(serverRelativeUrl));

            ClientContext.Load(file, f => f.Exists, f => f.Versions.IncludeWithDefaultProperties(i => i.CreatedBy));
            ClientContext.ExecuteQueryRetry();

            if (file.Exists)
            {
                var versions = file.Versions;
                ClientContext.ExecuteQueryRetry();
                WriteObject(versions);
            }
        }
    }
}
