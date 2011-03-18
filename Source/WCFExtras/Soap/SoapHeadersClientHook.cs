using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Reflection;
using System.Collections;

namespace WCFExtras.Soap
{
    class SoapHeadersClientHook : IClientMessageInspector, IChannelInitializer,
        IExtension<IContextChannel>
    {
        class OperationHeaders
        {
            public List<SoapHeaderAttribute> In = new List<SoapHeaderAttribute>();
            public List<SoapHeaderAttribute> Out = new List<SoapHeaderAttribute>();
        }

        Dictionary<Type, SoapHeaderHelper> shHelpers;
        Dictionary<string, OperationHeaders> headersFromAction;
        internal Hashtable Headers = new Hashtable();

        private SoapHeadersClientHook(Dictionary<string, OperationHeaders> headersFromAction,
            Dictionary<Type, SoapHeaderHelper> soapHelpers)
        {
            this.headersFromAction = headersFromAction;
            this.shHelpers = soapHelpers;
        }

        void IClientMessageInspector.AfterReceiveReply(ref Message reply, object correlationState)
        {
            OperationHeaders operationHeaders;
            if (headersFromAction.TryGetValue((string)correlationState, out operationHeaders))
            {
                if (operationHeaders.Out.Count > 0)
                {
                    foreach (SoapHeaderAttribute header in operationHeaders.Out)
                    {
                        string headerName = header.Name;
                        SoapHeaderHelper sh = shHelpers[header.Type];
                        Headers[headerName] = sh.GetHeader(headerName, reply.Headers);
                    }
                }
            }
        }

        object IClientMessageInspector.BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            OperationHeaders operationHeaders;
            if (headersFromAction.TryGetValue(request.Headers.Action, out operationHeaders))
            {
                if (operationHeaders.In.Count > 0)
                {
                    foreach (SoapHeaderAttribute header in operationHeaders.In)
                    {
                        string headerName = header.Name;
                        object headerValue = Headers[headerName];
                        if (headerValue != null)
                        {
                            SoapHeaderHelper sh = shHelpers[header.Type];
                            sh.AddHeader(headerName, headerValue, request.Headers);
                        }
                    }
                }
            }
            return request.Headers.Action;
        }

        internal static void Hook(ContractDescription contractDescription, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //Create a mapping between an operation and it's relevant headers
            Dictionary<string, OperationHeaders> headersFromAction = new Dictionary<string, OperationHeaders>();
            Dictionary<Type, SoapHeaderHelper> soapHelpers = new Dictionary<Type, SoapHeaderHelper>();

            foreach (OperationDescription op in contractDescription.Operations)
            {
                SoapHeaderAttribute[] soapHeaders = (SoapHeaderAttribute[])op.SyncMethod.GetCustomAttributes(typeof(SoapHeaderAttribute), false);
                if (soapHeaders.Length > 0)
                {
                    OperationHeaders headers = new OperationHeaders();
                    string action = contractDescription.Namespace + "/" + contractDescription.Name + "/" + op.Name;
                    foreach (SoapHeaderAttribute soapHeader in soapHeaders)
                    {
                        //prepare a cache of which headers needed in which Action
                        if ((soapHeader.Direction & SoapHeaderDirection.In) == SoapHeaderDirection.In)
                            headers.In.Add(soapHeader);
                        if ((soapHeader.Direction & SoapHeaderDirection.Out) == SoapHeaderDirection.Out)
                            headers.Out.Add(soapHeader);

                        //prepare a cache of SoapHeaderHelpers for each header type
                        if (!soapHelpers.ContainsKey(soapHeader.Type))
                            soapHelpers.Add(soapHeader.Type, new SoapHeaderHelper(soapHeader.Type));
                    }
                    headersFromAction.Add(action, headers);
                }
            }

            SoapHeadersClientHook clientHook = new SoapHeadersClientHook(headersFromAction, soapHelpers);
            clientRuntime.MessageInspectors.Add(clientHook);
            clientRuntime.ChannelInitializers.Add(clientHook);
        }

        void IChannelInitializer.Initialize(IClientChannel channel)
        {
            channel.Extensions.Add(this);
        }

        void IExtension<IContextChannel>.Attach(IContextChannel owner)
        {
        }

        void IExtension<IContextChannel>.Detach(IContextChannel owner)
        {
        }
    }
}
