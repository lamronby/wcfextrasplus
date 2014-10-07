using System;
using SampleWCFClient.SoapHeaders;
using WCFExtrasPlus.Soap;

namespace SampleWCFClient
{
    class Program
    {
        static void Main(string[] args)
        {
            TestSoapHeaders();
        }

        private static void TestSoapHeaders()
        {
            var client = new SoapHeadersSampleClient() { MyHeader = new Header() { Value = "Test Input" } };

            // Test Input header
            var result = client.In();
            Console.WriteLine("Input header: {0}{1}", result, (result == "Test Input" ? " - Passed" : " - Failed"));

            // Test Output header
            client.Out("Testing output");
            result = client.MyHeader.Value;
            Console.WriteLine("Output header: {0}{1}", result, (result == "Testing output" ? " - Passed" : " - Failed"));

            // Test InOut header
            client.MyHeader = new Header() { Value = "Test InOut" };
            client.InOut();
            result = client.MyHeader.Value;
            Console.Write("InOut header: {0}{1}", result, (result == "Test InOut InOut" ? " - Passed" : " - Failed"));
        }
    }

}

namespace SampleWCFClient.SoapHeaders
{
    public partial class SoapHeadersSampleClient
    {
        public Header MyHeader
        {
            get
            {
                return InnerChannel.GetHeader<Header>("MyHeader");
            }
            set
            {
                InnerChannel.SetHeader("MyHeader", value);
            }
        }
    }
}
