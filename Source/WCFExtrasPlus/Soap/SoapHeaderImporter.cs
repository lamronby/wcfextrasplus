using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.Xml.Schema;
using System.Xml;
using System.CodeDom;
using WCFExtrasPlus.Utils;

namespace WCFExtrasPlus.Soap
{
    public class SoapHeaderImporter : IWsdlImportExtension
    {
        void IWsdlImportExtension.BeforeImport(ServiceDescriptionCollection wsdlDocuments, XmlSchemaSet xmlSchemas, ICollection<XmlElement> policy)
        {
        }

        void IWsdlImportExtension.ImportContract(WsdlImporter importer, WsdlContractConversionContext context)
        {
            Dictionary<string, MessageHeaderDescription> allHeaders = new Dictionary<string, MessageHeaderDescription>();
            foreach (OperationDescription op in context.Contract.Operations)
            {
                Dictionary<string, SoapHeaderDirection> headerTypes = new Dictionary<string, SoapHeaderDirection>();
                List<MessageHeaderDescription> headers = new List<MessageHeaderDescription>();
                foreach (MessageDescription msg in op.Messages)
                {
                    foreach (MessageHeaderDescription msgHeader in msg.Headers)
                    {
                        SoapHeaderDirection direction = MessageDirectionToSoapHeaderDirection(msg.Direction);
                        SoapHeaderDirection currentDirection;
                        if (headerTypes.TryGetValue(msgHeader.Name, out currentDirection))
                        {
                            headerTypes[msgHeader.Name] = currentDirection | direction;
                        }
                        else
                        {
                            headers.Add(msgHeader);
                            headerTypes[msgHeader.Name] = direction;
                        }

                    }
                    msg.Headers.Clear();
                }

                Dictionary<MessageHeaderDescription, SoapHeaderDirection> msgHeaders = new Dictionary<MessageHeaderDescription, SoapHeaderDirection>();
                foreach (MessageHeaderDescription msgHeader in headers)
                {
                    msgHeaders[msgHeader] = headerTypes[msgHeader.Name];
                    allHeaders[msgHeader.Name] = msgHeader;
                }
                op.Behaviors.Add(new SoapHeaderOpExtension(msgHeaders));
            }
            if (allHeaders.Count > 0) //If any Soap header exists, mark the contract with SoapHeaders attribute
                context.Contract.Behaviors.Add(new SoapHeaderSvcExtension(allHeaders));
        }

        private SoapHeaderDirection MessageDirectionToSoapHeaderDirection(MessageDirection messageDirection)
        {
            switch (messageDirection)
            {
                case MessageDirection.Input: return SoapHeaderDirection.In;
                case MessageDirection.Output: return SoapHeaderDirection.Out;
                default: return SoapHeaderDirection.In;
            }
        }

        void IWsdlImportExtension.ImportEndpoint(WsdlImporter importer, WsdlEndpointConversionContext context)
        {

        }
    }

    class SoapHeaderOpExtension : IOperationBehavior, IOperationContractGenerationExtension
    {
        static SoapHeaderOpExtension()
        {
        }

        private readonly Dictionary<MessageHeaderDescription, SoapHeaderDirection> headers;

        public SoapHeaderOpExtension(Dictionary<MessageHeaderDescription, SoapHeaderDirection> headers)
        {
            this.headers = headers;
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.ClientOperation clientOperation)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription, System.ServiceModel.Dispatcher.DispatchOperation dispatchOperation)
        {
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }

        void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
        {
            foreach (KeyValuePair<MessageHeaderDescription, SoapHeaderDirection> pair in headers)
            {
                MessageHeaderDescription header = pair.Key;
                string headerTypeName = (string)ReflectionUtils.GetValue(header, "BaseType");

                CodeAttributeArgument arg1 = new CodeAttributeArgument(new CodePrimitiveExpression(header.Name));
                CodeAttributeArgument arg2 = new CodeAttributeArgument(new CodeTypeOfExpression(headerTypeName));
                CodeAttributeArgument arg3 = new CodeAttributeArgument("Direction", new CodeFieldReferenceExpression(
                    new CodeTypeReferenceExpression(typeof(SoapHeaderDirection)), pair.Value.ToString()));
                CodeAttributeDeclaration attrib = new CodeAttributeDeclaration(new CodeTypeReference(typeof(SoapHeaderAttribute)), arg1, arg2, arg3);
                context.SyncMethod.CustomAttributes.Add(attrib);
            }
        }
    }

    class SoapHeaderSvcExtension : IContractBehavior, IServiceContractGenerationExtension
    {
        private readonly Dictionary<string, MessageHeaderDescription> clientHeaders;

        public SoapHeaderSvcExtension(Dictionary<string, MessageHeaderDescription> headers)
        {
            this.clientHeaders = headers;
        }

        void IContractBehavior.AddBindingParameters(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        void IContractBehavior.ApplyClientBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
        }

        void IContractBehavior.ApplyDispatchBehavior(ContractDescription contractDescription, ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.DispatchRuntime dispatchRuntime)
        {
        }

        void IContractBehavior.Validate(ContractDescription contractDescription, ServiceEndpoint endpoint)
        {
        }

        void IServiceContractGenerationExtension.GenerateContract(ServiceContractGenerationContext context)
        {
            context.ContractType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SoapHeadersAttribute))));
        }
    }
}
