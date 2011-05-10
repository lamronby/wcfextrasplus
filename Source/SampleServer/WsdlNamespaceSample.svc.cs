using System;
using System.ServiceModel;

namespace Sample
{
    [ServiceBehavior(Namespace = "http://WCFExtrasPlus/Samples/services/2011/4")]
	public class WsdlNamespaceSample : IWsdlNamespaceSample
    {
        public string Operation1(DataContractSample2 data)
        {
            throw new NotImplementedException();
        }
    }
}
