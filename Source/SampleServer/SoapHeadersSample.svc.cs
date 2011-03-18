using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFExtras.Soap;

namespace Sample
{
    public class SoapHeadersSample : ISoapHeadersSample
    {
        public void NoHeaders()
        {
        }

        public string In()
        {
            //Use the value from a input header
            Header soapHeader = SoapHeaderHelper<Header>.GetInputHeader("MyHeader");
            if (soapHeader != null)
            {
                return soapHeader.Value;
            }
            return null;
        }

        public void Out(string value)
        {
            //set the value of a output header
            SoapHeaderHelper<Header>.SetOutputHeader("MyHeader", new Header() { Value = value });
        }

        public void InOut()
        {
            //Use input header
            Header soapHeader = SoapHeaderHelper<Header>.GetInputHeader("MyHeader");
            if (soapHeader != null)
            {
                //and set the value back in the output header
                soapHeader.Value += " InOut";
                SoapHeaderHelper<Header>.SetOutputHeader("MyHeader", soapHeader);
            }
        }
    }
}
