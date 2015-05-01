﻿namespace PANOSLibTest
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using PANOS;

    public class BaseConfigTest : BaseTest
    {
        public IConfigCommandFactory ConfigCommandFactory { get; set; }

        public ICommitCommandFactory CommitCommandFactory { get; set; }

        // Need to get rid of this
        public IConfigRepository ConfigRepository { get; private set; }

        protected IAddableRepository AddableRepository { get; private set; }

        protected IDeletableRepository DeletableRepository { get; private set; }

        public RandomObjectFactory RandomObjectFactory { get; set; }

        protected BaseConfigTest()
        {
            this.ConfigCommandFactory = 
                new ConfigCommandFactory(
                    new ApiUriFactory(
                        Connection.Host), 
                    new ConfigApiPostKeyValuePairFactory(
                        Connection.AccessToken,
                        Connection.Vsys));

            CommitCommandFactory = new CommitApiCommandFactory(
                new ApiUriFactory(Connection.Host),
                new CommitApiPostKeyValuePairFactory(Connection.AccessToken));

            ConfigRepository = new ConfigRepository(ConfigCommandFactory);
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
