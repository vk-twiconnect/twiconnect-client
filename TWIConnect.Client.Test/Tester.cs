using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TWIConnect.Client.Utilities;
using System.Xml;

namespace TWIConnect.Client.Test
{
    [TestClass]
    public class Tester
    {
        [TestMethod]
        public void SerializeConfiguration()
        {
            Domain.Configuration configuration = new Domain.Configuration
            {
                LocationKey = "qqq",
                Files = new HashSet<Domain.FileSettings>
                    {
                        new Domain.FileSettings
                            {
                                Name = "aaa"
                            },
                        new Domain.FileSettings
                            {
                                Name = "aaa"
                            },
                    }
            };

            string xml = configuration.Serialize();
        }

        [TestMethod]
        public void SerializeSelectFileRequest()
        {
            var request = new Domain.SelectFileRequest
            {
                InstallationId = "abc",
                LocationKey = "xyz",
                FolderName = "c:\\data",
                Files = new HashSet<Domain.File>
                    {
                        new Domain.File
                            {
                                Name = "aaa",
                                SizeBytes = 1024,
                                TimeStampUtc = DateTime.UtcNow,
                            },
                        new Domain.File
                            {
                                Name = "bbb",
                                SizeBytes = 2048,
                                TimeStampUtc = DateTime.UtcNow,
                            },
                    }
            };

            string xml = request.Serialize();
        }

        [TestMethod]
        public void Deserialize()
        {
            Domain.Configuration configuration = Domain.Configuration.Load();
        }

        [TestMethod]
        public void Run()
        {
            TWIConnect.Client.Processor.Run(Domain.Configuration.Load());
        }
    }
}
