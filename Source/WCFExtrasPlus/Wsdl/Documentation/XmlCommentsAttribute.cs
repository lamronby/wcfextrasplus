using System;
using System.ServiceModel.Description;

namespace WCFExtrasPlus.Wsdl.Documentation
{
    public enum XmlCommentFormat { Default, Portable };

    [AttributeUsage(AttributeTargets.Interface)]
    public class XmlCommentsAttribute : Attribute, IContractBehavior, IWsdlExportExtension
    {
        bool initialized;
        XmlCommentFormat format;

        public XmlCommentsAttribute()
        {
        }

        public XmlCommentsAttribute(XmlCommentFormat format)
        {
            this.format = format;
        }

        public void AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        public void ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
        }

        public void Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        public void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
            XmlCommentsExporter.ExportContract(exporter, context, Format);
        }

        public void ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            XmlCommentsExporter.ExportEndpoint(exporter, Format);
        }

        public XmlCommentFormat Format
        {
            get
            {
                if (!initialized)
                {
                    initialized = true;
                    XmlCommentsConfig config =  XmlCommentsConfig.GetConfiguration();
                    if (config != null)
                        format = config.Format;
                }
                return format;
            }
            set
            {
                format = value;
            }
        }
    }

}
