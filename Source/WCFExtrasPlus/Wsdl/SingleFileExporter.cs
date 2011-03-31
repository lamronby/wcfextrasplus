using System;
using System.ServiceModel.Description;
using System.Xml.Schema;
using System.Xml.Serialization;
using ServiceDescription = System.Web.Services.Description.ServiceDescription;
using log4net;

namespace WCFExtrasPlus.Wsdl
{
    class SingleFileExporter
    {
		private static readonly ILog Log = LogManager.GetLogger(typeof(FlatWsdl));

		internal static void ExportEndpoint(WsdlExporter wsdlExporter)
        {
			if (Log.IsInfoEnabled)
			{
				Log.InfoFormat("Endpoint has {0} wsdl documents", wsdlExporter.GeneratedWsdlDocuments.Count);
				Log.InfoFormat("Endpoint has {0} schemas", wsdlExporter.GeneratedXmlSchemas.Count);
			}

			if (wsdlExporter.GeneratedWsdlDocuments.Count > 1)
                throw new ApplicationException("Single file option is not supported in multiple wsdl files");

            ServiceDescription rootDescription = wsdlExporter.GeneratedWsdlDocuments[0];
            XmlSchemas imports = new XmlSchemas();
            foreach (XmlSchema schema in wsdlExporter.GeneratedXmlSchemas.Schemas())
            {
				if (Log.IsInfoEnabled)
				{
					var logMsg = new System.Text.StringBuilder("Adding schema for namespaces:");
					foreach (var ns in schema.Namespaces.ToArray())
						logMsg.Append(" ").Append(ns.Namespace);

					Log.Info(logMsg.ToString());
				}
				imports.Add(schema);
            }
            foreach (XmlSchema schema in imports)
            {
                schema.Includes.Clear();
            }

            rootDescription.Types.Schemas.Clear();
            rootDescription.Types.Schemas.Add(imports);
        }
    }
}
