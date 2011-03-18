using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using WCFExtras.Soap;

namespace Sample
{
    [SoapHeaders] //This attribute signals the wsdl exporter that this contract uses SoapHeaders
    [ServiceContract(Namespace = "http://WCFExtras/Samples")]
    public interface ISoapHeadersSample
    {
        [OperationContract]
        void NoHeaders();

        [SoapHeader("MyHeader", typeof(Header), Direction = SoapHeaderDirection.In)]
        [OperationContract]
        string In();

        [SoapHeader("MyHeader", typeof(Header), Direction = SoapHeaderDirection.Out)]
        [OperationContract]
        void Out(string value);

        [SoapHeader("MyHeader", typeof(Header), Direction = SoapHeaderDirection.InOut)]
        [OperationContract]
        void InOut();
    }

    [DataContract(Namespace = "http://WCFExtras/Samples")] //The header must have a DataContract attribute
    public class Header
    {
        [DataMember]
        public string Value { get; set; }
    }
}
