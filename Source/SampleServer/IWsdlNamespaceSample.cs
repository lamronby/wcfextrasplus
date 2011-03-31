using System.Runtime.Serialization;
using System.ServiceModel;
using WCFExtrasPlus.Wsdl.Documentation;

namespace Sample
{
    /// <summary>
    /// This is a sample service contract that demonstrates how 
    /// WsdlExtensions can flatten the rendered WSDL.
    /// </summary>
    [XmlComments]
	[ServiceContract(Namespace = "http://wcfextrasplus.codeplex.com/services/2011/4")]
    public interface IWsdlNamespaceSample
    {
        /// <summary>
        /// This is a simple operation with one input parameter.
        /// </summary>
        /// <returns>The operations returns a string</returns>
        [OperationContract]
        string Operation1(DataContractSample2 data);

    }

    /// <summary>
    /// A basic class marked with [DataContract]
    /// </summary>
	[DataContract(Namespace = "http://wcfextrasplus.codeplex.com/data/2011/4")]
    public class DataContractSample2
    {
        /// <summary>
        /// A simple string member.
        /// </summary>
        [DataMember]
        public string Member1 { get; set; }

        /// <summary>
        /// A second string member.
        /// </summary>
        [DataMember]
        public int Member2 { get; set; }

    }

}

