using Microsoft.Azure.Commands.Common.Authentication;
using Microsoft.Azure.Commands.Common.Authentication.Abstractions;
using Microsoft.Azure.Management.Blueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Azure.Commands.Blueprint.Cmdlets
{
    using Models;

    [Cmdlet(VerbsCommon.Get, ResourceManager.Common.AzureRMConstants.AzureRMPrefix + "Blueprint")]
    public class GetAzureRMBlueprint : BlueprintCmdletBase
    {
        #region Parameters
        [Parameter(Mandatory = false, Position = 0)]
        [ValidateNotNull]
        public string[] Name { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateNotNullOrEmpty]
        public string ManagementGroupName { get; set; }

        #endregion Parameters

        #region Cmdlet Overrides
        public override void ExecuteCmdlet()
        {
            IAzureContext context = DefaultProfile.DefaultContext;
            //var scc = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context, AzureEnvironment.Endpoint.ResourceManager);
            var scc = AzureSession.Instance.AuthenticationFactory.GetServiceClientCredentials(context);
            var client = new BlueprintManagementClient(new Uri(ARMFrontDoor), scc);

            if (Name == null)
            {
                WriteObject(GetBlueprints(client, ManagementGroupName), true);
            }
            else
            {
                WriteObject(GetNamedBlueprints(client, ManagementGroupName), true);
            }
        }
        #endregion Cmdlet Overrides

        #region Private Methods
        private IList<PSBlueprint> GetBlueprints(IBlueprintManagementClient client, string managementGroupName)
        {
            var rv = new List<PSBlueprint>();
            //TODO: Get list of blueprints

            return rv;
        }

        private IList<PSBlueprint> GetNamedBlueprints(IBlueprintManagementClient client, string managementGroupName)
        {
            var rv = new List<PSBlueprint>();

            foreach (var name in Name)
            {
                var bp = client.Blueprints.Get(managementGroupName, name);
                var psbp = new PSBlueprint
                {
                    Id = bp.Id,
                    Type = bp.Type,
                    Name = bp.Name,
                    DisplayName = bp.DisplayName,
                    Description = bp.Description,
                    Status = new PSBlueprintStatus { TimeCreated = bp.Status.TimeCreated, LastModified = bp.Status.LastModified },
                    TargetScope = bp.TargetScope,
                    Parameters = new Dictionary<string, PSParameterDefinition>(),
                    ResourceGroups = new Dictionary<string, PSResourceGroupDefinition>(),
                    Versions = bp.Versions,
                    Layout = bp.Layout
                };

                foreach (var item in bp.Parameters)
                    psbp.Parameters.Add(item.Key,
                                        new PSParameterDefinition
                                        {
                                            Type = item.Value.Type,
                                            DisplayName = item.Value.DisplayName,
                                            Description = item.Value.Description,
                                            StrongType = item.Value.StrongType,
                                            DefaultValue = item.Value.DefaultValue,
                                            AllowedValues = item.Value.AllowedValues.ToList()
                                        });

                foreach (var item in bp.ResourceGroups)
                    psbp.ResourceGroups.Add(item.Key,
                                            new PSResourceGroupDefinition
                                            {
                                                Name = item.Value.Name,
                                                Location = item.Value.Location,
                                                DisplayName = item.Value.DisplayName,
                                                Description = item.Value.Description,
                                                StrongType = item.Value.StrongType,
                                                DependsOn = item.Value.DependsOn.ToList()
                                            });

                rv.Add(psbp);
            }

            return rv;
        }
        #endregion Private Methods
    }
}
