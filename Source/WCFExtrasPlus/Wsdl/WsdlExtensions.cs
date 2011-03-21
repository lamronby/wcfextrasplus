using System;
using System.ServiceModel.Description;
using WCFExtrasPlus.Wsdl.Documentation;

namespace WCFExtrasPlus.Wsdl
{
    public class WsdlExtensions : IEndpointBehavior, IWsdlExportExtension
    {
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(WsdlExtensions));
        #region EndpointBehavior - Not used
        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
        }

        #endregion

        public Uri Location { get; set; }

        public XmlCommentFormat ExportXmlComments { get; set; }

        public bool SingleFile { get; set; }

        internal WsdlExtensions(WsdlExtensionsConfig config)
        {
            this.Location = config.Location;
            this.SingleFile = config.SingleFile;
        }

        void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
        {
        }

        void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
        {
        }

        void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
        {
            if (SingleFile)
                SingleFileExporter.ExportEndpoint(exporter);
			else
				FlatWsdl.ExportEndpoint(exporter);

            if (Location != null)
            {
                LocationOverrideExporter.ExportEndpoint(exporter, context, Location);
            }
        }
    }
}
