using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Reflection;
using WCFExtrasPlus.Utils;

namespace WCFExtrasPlus.Soap
{
    public class SoapHeaderHelper
    {
        Type type;
        string headerNamespace;

        public SoapHeaderHelper(Type t)
        {
            this.type = t;
            headerNamespace = GetNamespace(t);
        }

        public object GetInputHeader(string name)
        {
            return GetHeader(name, OperationContext.Current.IncomingMessageHeaders);
        }

        internal object GetHeader(string name, MessageHeaders headers)
        {
            int index = headers.FindHeader(name, headerNamespace);
            if (index >= 0)
            {
                XmlObjectSerializer serializer = new DataContractSerializer(type, name, headerNamespace, null, Int32.MaxValue, false, false, null);
                return headers.GetHeader<object>(index, serializer);
            }
            return null;
        }

        public void SetOutputHeader(string name, object value)
        {
            AddHeader(name, value, OperationContext.Current.OutgoingMessageHeaders);
        }

        internal void AddHeader(string name, object value, MessageHeaders headers)
        {
            MessageHeader header = MessageHeader.CreateHeader(name, headerNamespace, value);
            headers.Add(header);
        }

        internal static string GetNamespace(Type type)
        {
            string result = null;
            DataContractAttribute daAttrib = ReflectionUtils.GetDataContractAttribute(type);
            if (daAttrib != null)
            {
                result = daAttrib.Namespace;
            }
            if (result == null)
            {
                string clrNs = type.Namespace;
                result = GetGlobalDataContractNamespace(clrNs, type.Module);
                if (result == null)
                    result = GetGlobalDataContractNamespace(clrNs, type.Assembly);
            }
            if (result != null)
                return result;
            return string.Empty;
        }

        private static string GetGlobalDataContractNamespace(string clrNs, ICustomAttributeProvider customAttribuetProvider)
        {
            object[] customAttributes = customAttribuetProvider.GetCustomAttributes(typeof(ContractNamespaceAttribute), false);
            for (int i = 0; i < customAttributes.Length; i++)
            {
                ContractNamespaceAttribute attribute = (ContractNamespaceAttribute)customAttributes[i];
                string clrNamespace = attribute.ClrNamespace;
                if (clrNamespace == null)
                {
                    clrNamespace = string.Empty;
                }
                if (clrNamespace == clrNs)
                {
                    return attribute.ContractNamespace;
                }
            }
            return null;
        }
    }

    public class SoapHeaderHelper<T>
    {
        private static SoapHeaderHelper soapHeaderHelper;

        static SoapHeaderHelper()
        {
            soapHeaderHelper = new SoapHeaderHelper(typeof(T));
        }

        public static T GetInputHeader(string name)
        {
            return (T)soapHeaderHelper.GetInputHeader(name);
        }

        public static void SetOutputHeader(string name, T value)
        {
            soapHeaderHelper.SetOutputHeader(name, value);
        }
    }
}
