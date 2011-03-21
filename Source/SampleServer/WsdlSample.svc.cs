using System;

namespace Sample
{
    public class WsdlSample : IWsdlSample
    {
        public string Operation1()
        {
            throw new NotImplementedException();
        }

        public void Operation2(DataContractSample data, DataContractSample.DataContractSampleInner innerData)
        {
            throw new NotImplementedException();
        }

        public string Operation3(Struct? str, out int param1, ref long param2)
        {
            throw new NotImplementedException();
        }

        public void Operation4(RegularEnum param1)
        {
            throw new NotImplementedException();
        }
    }
}
