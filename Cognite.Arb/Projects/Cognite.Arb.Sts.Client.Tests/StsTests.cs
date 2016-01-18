using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Cognite.Arb.Sts.Client.Tests
{
    [TestFixture]
    public class StsTests
    {
        [Test]
        public void AddAndGet()
        {
            var client = new SecurityTokenServiceClient();
            var token = client.Add(new[] { new KeyValuePair<string, string>("id", "asdf") });
            var user = client.Get(token);
            Assert.AreEqual("asdf", user.First(item => item.Key == "id").Value);
        }

        [Test]
        public void ParallelStressLoad()
        {
            var start = DateTime.Now;
            var factory = new TaskFactory();
            var tasks = new Task[20];
            for (var t = 0; t < tasks.Length; ++t)
            {
                tasks[t] = factory.StartNew(() =>
                {
                    var client = new SecurityTokenServiceClient();
                    var tokens = new string[20];
                    for (int au = 0; au < tokens.Length; ++au)
                    {
                        var token = client.Add(new[] { new KeyValuePair<string, string>("id", au.ToString()) });
                        tokens[au] = token;
                        for (int gu = 0; gu < au; ++gu)
                        {
                            var user = client.Get(tokens[gu]);
                            Assert.AreEqual(gu.ToString(), user.First(item => item.Key == "id").Value);
                        }
                    }
                });
            }
            Console.WriteLine("tasks created");
            Task.WaitAll(tasks);
            var finish = DateTime.Now;
            var elapsed = finish - start;
            Console.WriteLine(elapsed);
        }
    }
}
