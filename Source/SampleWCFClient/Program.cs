using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WCFExtras.Soap;
using SampleWCFClient.WsdlSample;
using SampleWCFClient.SoapHeaders;

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
            SoapHeadersSampleClient client = new SoapHeadersSampleClient();

            //Test Input header
            client.MyHeader = new Header() { Value = "Test Input" };
            string result = client.In();
            Console.Write("Input header: " + result);
            if (result == "Test Input")
                Console.WriteLine(" - Passed");
            else
                Console.WriteLine(" - Failed");

            //Test Output header
            client.Out("Testing output");
            result = client.MyHeader.Value;
            Console.Write("Output header: " + result);
            if (result == "Testing output")
                Console.WriteLine(" - Passed");
            else
                Console.WriteLine(" - Failed");

            //Test InOut header
            client.MyHeader = new Header() { Value = "Test InOut" };
            client.InOut();
            result = client.MyHeader.Value;

            Console.Write("InOut header: " + result);
            if (result == "Test InOut InOut")
                Console.WriteLine(" - Passed");
            else
                Console.WriteLine(" - Failed");
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
