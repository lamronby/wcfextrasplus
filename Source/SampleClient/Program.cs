using System;
using SampleClient.localhost;

namespace SampleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            SoapHeadersSample client = new SoapHeadersSample();
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
