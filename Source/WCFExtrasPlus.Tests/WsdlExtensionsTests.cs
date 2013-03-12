using System;
using System.IO;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using NUnit.Framework;
using WCFExtrasPlus.Wsdl;

namespace WCFExtrasPlus.Tests
{
    [TestFixture]
    public class WsdlExtensionsTest
    {
        private ServiceHost _host;

        [SetUp]
        public void SetUp()
        {
            string baseAddress = "http://localhost:99/TestService";
            _host = new ServiceHost(typeof(TestService), new Uri(baseAddress));

            ServiceMetadataBehavior metadataBehavior = _host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (metadataBehavior == null)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                metadataBehavior.HttpGetEnabled = true;
                _host.Description.Behaviors.Add(metadataBehavior);
            }
            _host.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(),
                                     "metadata");

        }

        [TearDown]
        public void TearDown()
        {
            _host.Close();
        }

        private string ReadWsdl()
        {
            string wsdl = null;
            using (WebClient wc = new WebClient())
            {
                using (var stream = wc.OpenRead("http://localhost:99/TestService?wsdl"))
                {
                    using (var sr = new StreamReader(stream))
                    {
                        wsdl = sr.ReadToEnd();
                    }
                }
            }
            return wsdl;
        }

        private void AddEndpointBehavior(ServiceEndpoint endpoint)
        {
            endpoint.Behaviors.Add(new WsdlExtensions
                                       {
                                           Location = new Uri("http://TestHost/TestService?wsdl"),
                                           SingleFile = false
                                       });
        }

        [Test]
        public void BasicHttpBindingTest()
        {
            var endpoint = _host.AddServiceEndpoint(typeof(ITestService), new BasicHttpBinding(), "endpoint");
            AddEndpointBehavior(endpoint);
            _host.Open();

            var wsdl = ReadWsdl();

            StringAssert.DoesNotContain("http://localhost:99", wsdl, "Url shoud be overriden with http://TestHost");
        }

        [Test]
        public void WSHttpBindingTest()
        {
            var endpoint = _host.AddServiceEndpoint(typeof(ITestService), new WSHttpBinding(), "endpoint");
            AddEndpointBehavior(endpoint);
            _host.Open();

            var wsdl = ReadWsdl();

            StringAssert.DoesNotContain("http://localhost:99", wsdl, "Url shoud be overriden with http://TestHost");
        }

        [Test]
        public void WebHttpBindingTest()
        {
            var endpoint = _host.AddServiceEndpoint(typeof(ITestService), new WebHttpBinding(), "endpoint");
            AddEndpointBehavior(endpoint);
            _host.Open();

            var wsdl = ReadWsdl();

            StringAssert.DoesNotContain("http://localhost:99", wsdl, "Url shoud be overriden with http://TestHost");
        }
    }

    [ServiceContract]
    public interface ITestService
    {
        [OperationContract]
        string Ping();
    }

    public class TestService : ITestService
    {
        public string Ping()
        {
            return DateTime.UtcNow.ToShortTimeString();
        }
    }
}
