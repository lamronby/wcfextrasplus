using System.Runtime.Serialization;
using System.ServiceModel;
using WCFExtrasPlus.Soap;

namespace Sample
{
    [SoapHeaders] //This attribute signals the wsdl exporter that this contract uses SoapHeaders
    [ServiceContract(Namespace = "http://WCFExtrasPlus/Samples")]
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

    [DataContract(Namespace = "http://WCFExtrasPlus/Samples")] //The header must have a DataContract attribute
    public class Header
    {
        [DataMember]
        public string Value { get; set; }
    }
}
