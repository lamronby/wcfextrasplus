using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.Web.Services.Description;
using System.Xml;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Schema;
using WCFExtrasPlus.Utils;

namespace WCFExtrasPlus.Wsdl.Documentation
{
    class XmlCommentsExporter
    {
        public static XmlDocument GeneratedDoc { get; set; }

        private static void InitXsdDataContractExporter(WsdlExporter exporter, XmlCommentFormat format)
        {
            object dataContractExporter;
            XsdDataContractExporter xsdExporter;
            if (!exporter.State.TryGetValue(typeof(XsdDataContractExporter),
                out dataContractExporter))
            {
                xsdExporter = new XsdDataContractExporter(exporter.GeneratedXmlSchemas);
                exporter.State.Add(typeof(XsdDataContractExporter), xsdExporter);
            }
            else
            {
                xsdExporter = (XsdDataContractExporter)dataContractExporter;
            }

            if (xsdExporter.Options == null)
                xsdExporter.Options = new ExportOptions();
            if (!(xsdExporter.Options.DataContractSurrogate is XmlCommentsDataSurrogate))
                xsdExporter.Options.DataContractSurrogate = new XmlCommentsDataSurrogate(xsdExporter.Options.DataContractSurrogate, format);
        }

        private static void ConvertObjectAnnotation(XmlSchemaObject schemaObj)
        {
            XmlSchemaAnnotated annObj = schemaObj as XmlSchemaAnnotated;
            if (annObj != null && annObj.Annotation != null)
            {
                XmlSchemaDocumentation comment = null;
                foreach (XmlSchemaObject annotation in annObj.Annotation.Items)
                {
                    XmlSchemaAppInfo appInfo = annotation as XmlSchemaAppInfo;
                    if (appInfo != null)
                    {
                        for (int i = 0; i < appInfo.Markup.Length; i++)
                        {
                            XmlNode markup = appInfo.Markup[i];
                            if (markup != null)
                            {
                                XmlAttribute typeAttrib = markup.Attributes["type", "http://www.w3.org/2001/XMLSchema-instance"];
                                if (typeAttrib != null)
                                {
                                    if (typeAttrib.Value.Contains(":Annotation"))
                                    {
                                        string ns = typeAttrib.Value.Split(':')[0];
                                        typeAttrib = markup.Attributes[ns, "http://www.w3.org/2000/xmlns/"];
                                        if (typeAttrib != null && typeAttrib.Value == Annotation.AnnotationNamespace)
                                        {
                                            //This is an annotation created by us. Convert it to ws:documentation element and break
                                            comment = CreateDocumentationItem(markup.InnerText);
                                            appInfo.Markup[i] = null;
                                            break;
                                        }
                                    }
                                    else if (typeAttrib.Value.Contains(":EnumAnnotation"))
                                    {
                                        string ns = typeAttrib.Value.Split(':')[0];
                                        typeAttrib = markup.Attributes[ns, "http://www.w3.org/2000/xmlns/"];
                                        if (typeAttrib != null && typeAttrib.Value == Annotation.AnnotationNamespace)
                                        {
                                            DataContractSerializer serializer = new DataContractSerializer(typeof(EnumAnnotation));
                                            using (XmlReader reader = new XmlNodeReader(markup))
                                            {
                                                EnumAnnotation enumAnn = (EnumAnnotation)serializer.ReadObject(reader, false);
                                                if (enumAnn.EnumText != null)
                                                {
                                                    //This is an annotation created by us. Convert it to ws:documentation element and break
                                                    comment = CreateDocumentationItem(enumAnn.EnumText);
                                                }
                                                if (enumAnn.Members.Count > 0)
                                                {
                                                    foreach (XmlSchemaEnumerationFacet enumObj in GetEnumItems(schemaObj))
                                                    {
                                                        string docText;
                                                        if (enumAnn.Members.TryGetValue(enumObj.Value, out docText))
                                                        {
                                                            if (enumObj.Annotation == null)
                                                                enumObj.Annotation = new XmlSchemaAnnotation();
                                                            enumObj.Annotation.Items.Add(CreateDocumentationItem(docText));
                                                        }
                                                    }
                                                }
                                                appInfo.Markup[i] = null;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (comment != null)
                    annObj.Annotation.Items.Add(comment);
            }

            foreach (XmlSchemaObject subObj in GetSubItems(schemaObj))
            {
                ConvertObjectAnnotation(subObj);
            }
        }

        private static void ConvertObjectAnnotationForXmlSerializerFormat(XmlSchemaObject schemaObj)
        {
            XmlSchemaAnnotated annObj = schemaObj as XmlSchemaAnnotated;

            object obj = annObj;

            PropertyInfo pi = obj?.GetType().GetProperty("Name");

            var name = (string)pi?.GetValue(obj, null);

            XmlNodeList elemList = GeneratedDoc.GetElementsByTagName("member");

            string commentValue = string.Empty;

            foreach (XmlNode xn in elemList)
            {
                if (xn.Attributes != null && xn.Attributes[0].InnerText.EndsWith(name ?? string.Empty))
                {
                    commentValue = xn.InnerText;
                }

            }
            if (name != null && !string.IsNullOrEmpty(commentValue))
            {
                annObj.Annotation = new XmlSchemaAnnotation();

                if (annObj.Annotation != null)
                {
                    XmlSchemaDocumentation comment = CreateDocumentationItem(commentValue);

                    annObj.Annotation.Items.Add(comment);
                }
            }

            foreach (XmlSchemaObject subObj in GetSubItems(schemaObj))
            {
                ConvertObjectAnnotationForXmlSerializerFormat(subObj);
            }
        }

        private static XmlSchemaDocumentation CreateDocumentationItem(string text)
        {
            XmlDocument doc = new XmlDocument();
            XmlSchemaDocumentation comment = new XmlSchemaDocumentation();
            XmlNode node = doc.CreateTextNode(text);
            comment.Markup = new XmlNode[] { node };
            return comment;
        }

        private static IEnumerable<XmlSchemaObject> GetEnumItems(XmlSchemaObject schemaObj)
        {
            XmlSchemaSimpleType simpleType = (schemaObj as XmlSchemaSimpleType);
            if (simpleType != null)
            {
                XmlSchemaSimpleTypeRestriction restriction = (simpleType.Content as XmlSchemaSimpleTypeRestriction);
                if (restriction != null)
                {
                    foreach (XmlSchemaObject obj in restriction.Facets)
                        yield return obj;
                }
            }

            XmlSchemaSimpleTypeList flagEnumTypeList = (simpleType.Content as XmlSchemaSimpleTypeList);
            if (flagEnumTypeList != null && flagEnumTypeList.ItemType != null)
            {
                XmlSchemaSimpleTypeRestriction flagEnumRestriction = (flagEnumTypeList.ItemType.Content as XmlSchemaSimpleTypeRestriction);
                if (flagEnumRestriction != null)
                {
                    foreach (XmlSchemaObject obj in flagEnumRestriction.Facets)
                        yield return obj;
                }
            }
        }

        private static IEnumerable<XmlSchemaObject> GetSubItems(XmlSchemaObject schemaObj)
        {
            XmlSchemaComplexType complexType = schemaObj as XmlSchemaComplexType;
            if (complexType != null)
            {
                XmlSchemaSequence seq = complexType.ContentTypeParticle as XmlSchemaSequence;
                if (seq != null)
                {
                    foreach (XmlSchemaObject subObj in seq.Items)
                    {
                        yield return subObj;
                    }
                }
            }
        }

        internal static void ExportEndpoint(WsdlExporter exporter, XmlCommentFormat format)
        {
            // xmlSerializerFormatState is deciding whether the interface is using XmlSerializerFormat attribute or not. 
            var xmlSerializerFormatState = exporter.State.Keys.FirstOrDefault(formatObj
                => formatObj.ToString().Contains("System.ServiceModel.Description.XmlSerializerOperationBehavior"));

            // If it is not using XmlSerializerFormat, then the xml comments would appear according to annotation. 
            if (xmlSerializerFormatState == null)
            {
                foreach (XmlSchema schema in exporter.GeneratedXmlSchemas.Schemas())
                {
                    foreach (XmlSchemaObject schemaObj in schema.Items)
                    {
                        ConvertObjectAnnotation(schemaObj);
                    }
                }
            }
            // If it is using XmlSerializerFormat, then annotations have been added manually  
            // in this method comparing with the generated xml file.
            else
            {
                foreach (XmlSchema schema in exporter.GeneratedXmlSchemas.Schemas())
                {
                    foreach (XmlSchemaObject schemaObj in schema.Items)
                    {
                        ConvertObjectAnnotationForXmlSerializerFormat(schemaObj);
                    }
                }
            }
            XmlCommentsUtils.ClearCache();
        }

        internal static void ExportContract(WsdlExporter exporter, WsdlContractConversionContext context, XmlCommentFormat format)
        {
            InitXsdDataContractExporter(exporter, format);

            XmlDocument commentsDoc = XmlCommentsUtils.LoadXmlComments(context.Contract.ContractType);
            if (commentsDoc == null)
                return;

            GeneratedDoc = commentsDoc;

            string comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, context.Contract.ContractType, format);
            if (comment != null)
            {
                context.WsdlPortType.Documentation = comment;
            }

            foreach (Operation op in context.WsdlPortType.Operations)
            {
                OperationDescription opDescription = context.GetOperationDescription(op);
                MemberInfo mi = opDescription.SyncMethod;
#if NET45
                if (mi == null)
                    mi = opDescription.TaskMethod;
#else
                if (mi == null)
                    mi = opDescription.BeginMethod;
#endif
                comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, mi, format);
                if (comment != null)
                {
                    op.Documentation = comment;
                }
            }
        }
    }

    class XmlCommentsDataSurrogate : IDataContractSurrogate
    {
        IDataContractSurrogate prevSurrogate;
        XmlCommentFormat format;

        public XmlCommentsDataSurrogate(IDataContractSurrogate prevSurrogate)
        {
            this.prevSurrogate = prevSurrogate;
        }

        public XmlCommentsDataSurrogate(IDataContractSurrogate prevSurrogate, XmlCommentFormat format)
            : this(prevSurrogate)
        {
            this.format = format;
        }

        object IDataContractSurrogate.GetCustomDataToExport(Type clrType, Type dataContractType)
        {
            if (dataContractType.IsGenericType && dataContractType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                dataContractType = dataContractType.GetGenericArguments()[0];
            }
            XmlDocument commentsDoc = XmlCommentsUtils.LoadXmlComments(dataContractType);
            if (commentsDoc != null)
            {
                if (dataContractType.IsEnum)
                {
                    EnumAnnotation annotation = new EnumAnnotation();
                    string comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, dataContractType, format);
                    if (comment != null)
                        annotation.EnumText = comment;
                    Dictionary<string, MemberInfo> enumMembers = ReflectionUtils.GetEnumMembers(dataContractType);
                    foreach (KeyValuePair<string, MemberInfo> member in enumMembers)
                    {
                        comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, member.Value, format);
                        if (comment != null)
                        {
                            annotation.Members.Add(member.Key, comment);
                        }
                    }
                    if (annotation.EnumText != null || annotation.Members.Count > 0)
                        return annotation;
                }
                else
                {
                    string comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, dataContractType, format);
                    if (comment != null)
                    {
                        return new Annotation(comment);
                    }
                }
            }
            return null;
        }

        object IDataContractSurrogate.GetCustomDataToExport(MemberInfo memberInfo, Type dataContractType)
        {
            XmlDocument commentsDoc = XmlCommentsUtils.LoadXmlComments(memberInfo.DeclaringType);
            if (commentsDoc != null)
            {
                string comment = XmlCommentsUtils.GetFormattedComment(commentsDoc, memberInfo, format);
                if (comment != null)
                {
                    return new Annotation(comment);
                }
            }
            return null;
        }

        Type IDataContractSurrogate.GetDataContractType(Type type)
        {
            if (prevSurrogate != null)
                return prevSurrogate.GetDataContractType(type);
            return type;
        }

        object IDataContractSurrogate.GetDeserializedObject(object obj, Type targetType)
        {
            if (prevSurrogate != null)
                return prevSurrogate.GetDeserializedObject(obj, targetType);
            return obj;
        }

        void IDataContractSurrogate.GetKnownCustomDataTypes(System.Collections.ObjectModel.Collection<Type> customDataTypes)
        {
            if (prevSurrogate != null)
                prevSurrogate.GetKnownCustomDataTypes(customDataTypes);
            customDataTypes.Add(typeof(Annotation));
            customDataTypes.Add(typeof(EnumAnnotation));
        }

        object IDataContractSurrogate.GetObjectToSerialize(object obj, Type targetType)
        {
            if (prevSurrogate != null)
                return prevSurrogate.GetObjectToSerialize(obj, targetType);
            return obj;
        }

        Type IDataContractSurrogate.GetReferencedTypeOnImport(string typeName, string typeNamespace, object customData)
        {
            if (prevSurrogate != null)
                return prevSurrogate.GetReferencedTypeOnImport(typeName, typeNamespace, customData);
            return null;
        }

        System.CodeDom.CodeTypeDeclaration IDataContractSurrogate.ProcessImportedType(System.CodeDom.CodeTypeDeclaration typeDeclaration, System.CodeDom.CodeCompileUnit compileUnit)
        {
            if (prevSurrogate != null)
                return prevSurrogate.ProcessImportedType(typeDeclaration, compileUnit);
            return typeDeclaration;
        }
    }

    [DataContract(Namespace = AnnotationNamespace)]
    class Annotation
    {
        public const string AnnotationNamespace = "XmlCommentsExporter.Annotation";

        public Annotation(string s)
        {
            this.Text = s;
        }
        [DataMember]
        public string Text;
    }

    [DataContract(Namespace = Annotation.AnnotationNamespace)]
    class EnumAnnotation
    {
        [DataMember]
        public string EnumText;

        [DataMember]
        public Dictionary<string, string> Members = new Dictionary<string, string>();
    }
}
