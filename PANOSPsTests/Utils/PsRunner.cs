﻿namespace PANOSPsTest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    using PANOS;

    public static class PsRunner
    {
        private static readonly InitialSessionState InitialSessionState =  InitialSessionState.CreateDefault();
        private static readonly Runspace Runspace;
        
        static PsRunner()
        {
            InitialSessionState.ImportPSModule(new[] { "PANOS" });
            Runspace = RunspaceFactory.CreateRunspace(InitialSessionState);
            Runspace.Open();
        }

        public static Collection<PSObject> ExecutePanosPowerShellScript(string scriptToTest)
        {
            var connectionPropertiesCommand =
                $"$connection = New-PANOSConnection -HostName '{ConfigurationManager.AppSettings["FirewallHostName"]}' -Vsys '{ConfigurationManager.AppSettings["Vsys"]}' -AccessToken (ConvertTo-SecureString '{ConfigurationManager.AppSettings["FirewallAccessToken"]}' -AsPlainText -Force) -StoreInSession | Out-Null";

            var script = $"{connectionPropertiesCommand};{scriptToTest}";

            Debug.WriteLine("PsRunner about to execute {0} {1}", Environment.NewLine, script);

            var pipleLine = Runspace.CreatePipeline();
            pipleLine.Commands.AddScript(script);
            return pipleLine.Invoke();
        }

        public static bool PipelineContainsFirewallObjects<T>(
            IEnumerable<PSObject> results,
            IReadOnlyCollection<T> firewallObjects) where T:FirewallObject
        {
            var psObjects = results as PSObject[] ?? results.ToArray();
           
            foreach (var firewallObject in firewallObjects)
            {
                // This is will throw an exception if match is not foudn in the Pipeline
                var obj = psObjects.Single(o => o.BaseObject.Equals(firewallObject));
            }

            return true;
        }

        public static bool PipelineContainsFirewallObject<T>(
            IEnumerable<PSObject> results,
            T firewallObject) where T : FirewallObject
        {
            var psObjects = results as PSObject[] ?? results.ToArray();
            // This is will throw an exception if match is not foudn in the Pipeline
            var obj = psObjects.Single(o => o.BaseObject.Equals(firewallObject));
            return true;
        }


        
    }
}
