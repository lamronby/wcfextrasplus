# WCFExtrasPlus

WCFExtrasPlus is a collection of useful WCF extensions:
* SOAP Header support
* Support for integrating Source Code XML Comments into WSDL Documentation
* Override SOAP Address Location URL
* Single, flattened WSDL file for better compatibility with older SOAP tools

Available as a NuGet package: [WCFExtrasPlus](https://www.nuget.org/packages/WCFExtrasPlus)

## SOAP Header support for WCF
WCF does not directly support the SOAP Header model introduced by asmx Web services. Although WCF allows you to work with SOAP headers using a message contract-based programming model, it is sometimes easier to keep the more intuitive operation contract model but still expose SOAP headers and be able to access them inside WCF calls. The SoapHeader attribute introduced in the project allows you to use SOAP headers in code while still using the Operation Contract-based model. WCFExtras+ also includes a client side WSDL importer that customizes the WSDL import process and creates a client proxy which greatly simplifies the process of sending and receiving Soap Headers.
## Support for integrating Source Code XML Comments into WSDL Documentation
This extension allows you to add WSDL documentation (annotation) directly from XML comments in your source file. These comments will be published as part of the WSDL and are available for WSDL tools that know how to take advantage of them (e.g. Apache Axis wsdl2java and others). Release 2.0 also includes a client side WSDL importer that will turn those WSDL comments to XML comments in the generated proxy code.
## Override SOAP Address Location URL
The need for this extensions came from a specific scenario where an IIS hosted WCF service was located behind a load balancer\SSL accelerator (F5 in this case) which handled the SSL traffic and passed all communication in http to the web server. After all configrations were done and the service was working correctly when accessed by client talking to the F5 box, the only problem left was that the resulting WSDL file contained an endpoint with an HTTP address instead of HTTPS. This extensions allows you to set the URL that will be rendered in the WSDL (in this case to simply replace the http url with an https url).
## Single WSDL File
Some older SOAP toolkits fail when trying to import a WSDL file that contains references to external .xsd files. This extensions simply merges the external types into the wsdl file and produces a single flat wsdl file that contains all the definitions it needs.


This project was based on the WCFExtras Codeplex project, which is now defunct. Any copyrights from WCFExtras have been maintained in WCFExtrasPlus.