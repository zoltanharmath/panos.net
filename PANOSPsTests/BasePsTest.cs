﻿namespace PANOSPsTest
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Threading;

    using NUnit.Framework;

    using PANOS;

    public class BasePsTest
    {
        protected static Connection Connection
        {
            get
            {
                return new Connection(
                    ConfigurationManager.AppSettings["FirewallHostName"],
                    SecureStringUtils.ConvertToSecureString((ConfigurationManager.AppSettings["FirewallAccessToken"])),
                    ConfigurationManager.AppSettings["Vsys"]);
            }
        }

        public IConfigCommandFactory ConfigCommandFactory { get; set; }

        public ICommitCommandFactory CommitCommandFactory { get; set; }

        protected IAddableRepository AddableRepository { get; private set; }

        protected IDeletableRepository DeletableRepository { get; private set; }

        public RandomObjectFactory RandomObjectFactory { get; set; }


        protected BasePsTest()
        {
            this.ConfigCommandFactory = 
                new ConfigCommandFactory(
                    new ApiUriFactory(
                        Connection.Host), 
                    new ConfigPostKeyValuePairFactory(
                        Connection.AccessToken,
                        Connection.Vsys));

            CommitCommandFactory = new CommitApiCommandFactory(
                new ApiUriFactory(Connection.Host),
                new CommitApiPostKeyValuePairFactory(Connection.AccessToken));

            AddableRepository = new AddableRepository(ConfigCommandFactory);
            DeletableRepository = new DeletableRepository(ConfigCommandFactory);

            RandomObjectFactory = new RandomObjectFactory(new AddableRepository(ConfigCommandFactory));
        }

        protected void CommitCandidateConfig(bool waitForCompletion = true)
        {
            var commitQueryResonse = this.CommitCommandFactory.CreateCommit(true).Execute();
            Assert.IsTrue(commitQueryResonse.Status.Equals("success"));
            Assert.AreEqual(commitQueryResonse.Code, (byte)CommitStatus.Success);
            Debug.WriteLine("CommitApi completed successfully, check job ID {0}", commitQueryResonse.Result.JobId);
            if (waitForCompletion)
            {
                Thread.Sleep(TimeSpan.FromSeconds(90)); 
            }
        }
    }
}
