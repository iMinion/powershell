﻿using System.Management.Automation;
using Microsoft.SharePoint.Client;

using PnP.PowerShell.Commands.Base.PipeBinds;
using PnP.PowerShell.Commands.Enums;
using System;
using PnP.PowerShell.Commands.Base;

namespace PnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "PnPSite")]
    public class NewSite : PnPSharePointCmdlet, IDynamicParameters
    {
        private const string ParameterSet_COMMUNICATIONBUILTINDESIGN = "Communication Site with Built-In Site Design";
        private const string ParameterSet_COMMUNICATIONCUSTOMDESIGN = "Communication Site with Custom Design";
        private const string ParameterSet_TEAM = "Team Site";

        [Parameter(Mandatory = true)]
        public SiteType Type;

        [Parameter(Mandatory = false)]
        public Guid HubSiteId;

        private CommunicationSiteParameters _communicationSiteParameters;
        private TeamSiteParameters _teamSiteParameters;

        [Parameter(Mandatory = false)]
        public SwitchParameter Wait;

        public object GetDynamicParameters()
        {
            switch (Type)
            {
                case SiteType.CommunicationSite:
                    {
                        _communicationSiteParameters = new CommunicationSiteParameters();
                        return _communicationSiteParameters;
                    }
                case SiteType.TeamSite:
                    {
                        _teamSiteParameters = new TeamSiteParameters();
                        return _teamSiteParameters;
                    }
            }
            return null;
        }

        protected override void ExecuteCmdlet()
        {
            if (Type == SiteType.CommunicationSite)
            {
                if (!ParameterSpecified("Lcid"))
                {
                    ClientContext.Web.EnsureProperty(w => w.Language);
                    _communicationSiteParameters.Lcid = ClientContext.Web.Language;
                }
                var creationInformation = new PnP.Framework.Sites.CommunicationSiteCollectionCreationInformation();
                creationInformation.Title = _communicationSiteParameters.Title;
                creationInformation.Url = _communicationSiteParameters.Url;
                creationInformation.Description = _communicationSiteParameters.Description;
                creationInformation.Classification = _communicationSiteParameters.Classification;
#pragma warning disable CS0618 // Type or member is obsolete
                creationInformation.ShareByEmailEnabled = _communicationSiteParameters.ShareByEmailEnabled;
#pragma warning restore CS0618 // Type or member is obsolete
                creationInformation.Lcid = _communicationSiteParameters.Lcid;
                if (ParameterSpecified(nameof(HubSiteId)))
                {
                    creationInformation.HubSiteId = HubSiteId;
                }
                if (ParameterSetName == ParameterSet_COMMUNICATIONCUSTOMDESIGN)
                {
                    creationInformation.SiteDesignId = _communicationSiteParameters.SiteDesignId;
                }
                else
                {
                    creationInformation.SiteDesign = _communicationSiteParameters.SiteDesign;
                }
                creationInformation.Owner = _communicationSiteParameters.Owner;
                creationInformation.PreferredDataLocation = _communicationSiteParameters.PreferredDataLocation;
                creationInformation.SensitivityLabel = _communicationSiteParameters.SensitivityLabel;

                var returnedContext = PnP.Framework.Sites.SiteCollection.Create(ClientContext, creationInformation, noWait: !Wait);
                //var results = ClientContext.CreateSiteAsync(creationInformation);
                //var returnedContext = results.GetAwaiter().GetResult();
                WriteObject(returnedContext.Url);
            }
            else
            {
                var creationInformation = new PnP.Framework.Sites.TeamSiteCollectionCreationInformation();
                creationInformation.DisplayName = _teamSiteParameters.Title;
                creationInformation.Alias = _teamSiteParameters.Alias;
                creationInformation.Classification = _teamSiteParameters.Classification;
                creationInformation.Description = _teamSiteParameters.Description;
                creationInformation.IsPublic = _teamSiteParameters.IsPublic;
                creationInformation.Lcid = _teamSiteParameters.Lcid;
                if (ParameterSpecified(nameof(HubSiteId)))
                {
                    creationInformation.HubSiteId = HubSiteId;
                }
                creationInformation.Owners = _teamSiteParameters.Owners;
                creationInformation.PreferredDataLocation = _teamSiteParameters.PreferredDataLocation;
                creationInformation.SensitivityLabel = _teamSiteParameters.SensitivityLabel;

                var returnedContext = PnP.Framework.Sites.SiteCollection.Create(ClientContext, creationInformation, noWait: !Wait, graphAccessToken: PnPConnection.CurrentConnection.TryGetAccessTokenAsync(TokenAudience.MicrosoftGraph).GetAwaiter().GetResult());
                //var results = ClientContext.CreateSiteAsync(creationInformation);
                //var returnedContext = results.GetAwaiter().GetResult();
                WriteObject(returnedContext.Url);
            }
        }

        public class CommunicationSiteParameters
        {
            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string Title;

            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string Url;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string Description;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string Classification;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public SwitchParameter ShareByEmailEnabled;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            public PnP.Framework.Sites.CommunicationSiteDesign SiteDesign = PnP.Framework.Sites.CommunicationSiteDesign.Topic;

            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public Guid SiteDesignId;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public uint Lcid;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string Owner;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public PnP.Framework.Enums.Office365Geography? PreferredDataLocation;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONBUILTINDESIGN)]
            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_COMMUNICATIONCUSTOMDESIGN)]
            public string SensitivityLabel;
        }

        public class TeamSiteParameters
        {
            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_TEAM)]
            public string Title;

            [Parameter(Mandatory = true, ParameterSetName = ParameterSet_TEAM)]
            public string Alias;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public string Description;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public string Classification;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public SwitchParameter IsPublic;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public uint Lcid;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public string[] Owners;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public PnP.Framework.Enums.Office365Geography? PreferredDataLocation;

            [Parameter(Mandatory = false, ParameterSetName = ParameterSet_TEAM)]
            public string SensitivityLabel;
        }
    }
}