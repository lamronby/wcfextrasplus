using System;
using System.ServiceModel.Description;

namespace WCFExtrasPlus.Soap
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class SoapHeadersAttribute : Attribute, IContractBehavior, IWsdlExportExtension
    {
        public SoapHeadersAttribute()
        {
        }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            SoapHeadersClientHook.Hook(contractDescription, endpoint, clientRuntime);
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
        }

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            Export(exporter, context);
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
        }

        internal static void Export(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            foreach (OperationDescription op in context.Contract.Operations)
            {
                SoapHeaderAttribute[] soapHeaders = (SoapHeaderAttribute[])op.SyncMethod.GetCustomAttributes(typeof(SoapHeaderAttribute), false);
                if (soapHeaders.Length > 0)
                {
                    foreach (SoapHeaderAttribute soapHeader in soapHeaders)
                    {
                        AddSoapHeader(op, soapHeader);
                    }
                }
            }
        }

        internal static void AddSoapHeader(OperationDescription op, SoapHeaderAttribute soapHeader)
        {
            string headerNamespace = SoapHeaderHelper.GetNamespace(soapHeader.Type);
            MessageHeaderDescription header = new MessageHeaderDescription(soapHeader.Name, headerNamespace);
            header.Type = soapHeader.Type;
            bool input = ((soapHeader.Direction & SoapHeaderDirection.In) == SoapHeaderDirection.In);
            bool output = ((soapHeader.Direction & SoapHeaderDirection.Out) == SoapHeaderDirection.Out);

            foreach (MessageDescription msgDescription in op.Messages)
            {
                if ((msgDescription.Direction == MessageDirection.Input && input) ||
                    (msgDescription.Direction == MessageDirection.Output && output))
                    msgDescription.Headers.Add(header);
            }
        }
    }
}
