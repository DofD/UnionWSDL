using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace UnionWSDL
{
    internal class CombinerWSDL
    {
        public const string WSDLNamespace = "http://schemas.xmlsoap.org/wsdl/";

        public const string XSDNamespace = "http://www.w3.org/2001/XMLSchema";

        public static void Union(string wsdlPath, string resultFilename)
        {
            try
            {
                var resultWsdl = new XmlDocument();
                if (File.Exists(resultFilename))
                {
                    resultWsdl.Load(resultFilename);
                }

                var wsdl = new XmlDocument();
                wsdl.Load(wsdlPath);

                var manager = PrepareNamespaceManager(wsdl);

                UnionDefinitions(wsdl, resultWsdl, manager);
                UnionTypes(wsdl, resultWsdl, manager);
                UnionMessages(wsdl, resultWsdl, manager);
                UnionPortType(wsdl, resultWsdl, manager);
                UnionBinding(wsdl, resultWsdl, manager);
                UnionService(wsdl, resultWsdl, manager);

                Console.WriteLine("Сохраняем WSDL");
                resultWsdl.Save(resultFilename);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Ошибка: {0}", exception.Message);
            }
        }

        private static void UnionService(XmlDocument wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultDefinitions = CreateOrFindDefinitions(resultWsdl, manager);

            var sourceServices = wsdl.SelectNodes("/wsdl:definitions/wsdl:service", manager);
            var resultServices = resultWsdl.SelectNodes("/wsdl:definitions/wsdl:service", manager);

            if (sourceServices != null && resultServices != null)
            {
                var importServices = sourceServices.Cast<XmlElement>().Where(sourceNode =>
                {
                    return
                        resultServices.Cast<XmlElement>()
                            .All(
                                element =>
                                    element.GetAttribute("name")
                                    != sourceNode.GetAttribute("name"));
                });

                foreach (var importNode in importServices.Select(sourceNode => resultDefinitions.OwnerDocument.ImportNode(sourceNode, true)))
                {
                    resultDefinitions.AppendChild(importNode);
                }
            }
        }

        private static void UnionBinding(XmlDocument wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultDefinitions = CreateOrFindDefinitions(resultWsdl, manager);

            var sourceBindings = wsdl.SelectNodes("/wsdl:definitions/wsdl:binding", manager);
            var resultBindings = resultWsdl.SelectNodes("/wsdl:definitions/wsdl:binding", manager);

            if (sourceBindings != null && resultBindings != null)
            {
                var importBindings = sourceBindings.Cast<XmlElement>().Where(sourceNode =>
                {
                    return
                        resultBindings.Cast<XmlElement>()
                            .All(
                                element =>
                                    element.GetAttribute("name")
                                    != sourceNode.GetAttribute("name"));
                });

                foreach (var importNode in importBindings.Select(sourceNode => resultDefinitions.OwnerDocument.ImportNode(sourceNode, true)))
                {
                    resultDefinitions.AppendChild(importNode);
                }
            }
        }

        private static void UnionPortType(XmlDocument wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultDefinitions = CreateOrFindDefinitions(resultWsdl, manager);

            var sourcePortTypes = wsdl.SelectNodes("/wsdl:definitions/wsdl:portType", manager);
            var resultPortTypes = resultWsdl.SelectNodes("/wsdl:definitions/wsdl:portType", manager);

            if (sourcePortTypes != null && resultPortTypes != null)
            {
                var importPortTypes = sourcePortTypes.Cast<XmlElement>().Where(sourceNode =>
                {
                    return
                        resultPortTypes.Cast<XmlElement>()
                            .All(
                                element =>
                                    element.GetAttribute("name")
                                    != sourceNode.GetAttribute("name"));
                });

                foreach (var importNode in importPortTypes.Select(sourceNode => resultDefinitions.OwnerDocument.ImportNode(sourceNode, true)))
                {
                    resultDefinitions.AppendChild(importNode);
                }
            }
        }

        private static void UnionMessages(XmlNode wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultDefinitions = CreateOrFindDefinitions(resultWsdl, manager);

            var sourceMessages = wsdl.SelectNodes("/wsdl:definitions/wsdl:message", manager);
            var resultMessages = resultWsdl.SelectNodes("/wsdl:definitions/wsdl:message", manager);

            if (sourceMessages != null && resultMessages != null)
            {
                var importMessages = sourceMessages.Cast<XmlElement>().Where(sourceNode =>
                {
                    return
                        resultMessages.Cast<XmlElement>()
                            .All(
                                element =>
                                    element.GetAttribute("name")
                                    != sourceNode.GetAttribute("name"));
                });

                foreach (var importNode in importMessages.Select(sourceNode => resultDefinitions.OwnerDocument.ImportNode(sourceNode, true)))
                {
                    resultDefinitions.AppendChild(importNode);
                }
            }
        }

        private static void UnionDefinitions(XmlDocument wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultDefinitions = CreateOrFindDefinitions(resultWsdl, manager);
            var sourceDefinitions = CreateOrFindDefinitions(wsdl, manager);

            UnionAttributeDefinitions(sourceDefinitions, resultDefinitions);
        }

        private static void UnionTypes(XmlDocument wsdl, XmlDocument resultWsdl, XmlNamespaceManager manager)
        {
            var resultTypes = CreateOrFindTypes(resultWsdl, manager);
            var sourceTypes = CreateOrFindTypes(wsdl, manager);

            UnionTypes(sourceTypes, resultTypes, manager);
        }

        private static void UnionTypes(XmlElement sourceTypes, XmlElement resultTypes, XmlNamespaceManager manager)
        {
            var sourceNodes = sourceTypes.SelectNodes("xsd:schema", manager);
            var resultNodes = resultTypes.SelectNodes("xsd:schema", manager);

            if (sourceNodes != null && resultNodes != null)
            {
                var importNodes = sourceNodes.Cast<XmlElement>().Where(sourceNode =>
                {
                    return
                        resultNodes.Cast<XmlElement>()
                            .All(
                                element =>
                                    element.GetAttribute("targetNamespace")
                                    != sourceNode.GetAttribute("targetNamespace"));
                });

                foreach (var importNode in importNodes.Select(sourceNode => resultTypes.OwnerDocument.ImportNode(sourceNode, true)))
                {
                    resultTypes.AppendChild(importNode);
                }
            }
        }

        private static void UnionAttributeDefinitions(XmlNode sourceDefinitions, XmlElement resultDefinitions)
        {
            foreach (var attribute in sourceDefinitions.Attributes.Cast<XmlAttribute>().Where(attribute => string.IsNullOrEmpty(resultDefinitions.GetAttribute(attribute.Name))))
            {
                resultDefinitions.SetAttribute(attribute.Name, attribute.Value);
            }
        }

        private static XmlNamespaceManager PrepareNamespaceManager(XmlDocument wsdl)
        {
            var manager = new XmlNamespaceManager(wsdl.NameTable);
            manager.AddNamespace("wsdl", WSDLNamespace);
            manager.AddNamespace("xsd", XSDNamespace);
            return manager;
        }

        private static XmlElement CreateOrFindDefinitions(XmlDocument wsdl, XmlNamespaceManager manager)
        {
            var node = wsdl.SelectSingleNode("/wsdl:definitions", manager);
            if (node != null)
            {
                return node as XmlElement;
            }

            var definitions = wsdl.CreateElement("wsdl", "definitions", WSDLNamespace);
            definitions.SetAttribute("name", ConfigurationHelper.AppSetting("ServiceName", "UnionService"));

            wsdl.AppendChild(definitions);
            return definitions;
        }

        private static XmlElement CreateOrFindTypes(XmlDocument wsdl, XmlNamespaceManager manager)
        {
            var node = wsdl.SelectSingleNode("/wsdl:definitions/wsdl:types", manager);
            if (node != null)
            {
                return node as XmlElement;
            }

            var types = wsdl.CreateElement("wsdl", "types", WSDLNamespace);

            var import = wsdl.SelectSingleNode("/wsdl:definitions/wsdl:import", manager);
            if (import == null)
            {
                wsdl.DocumentElement.InsertBefore(types, wsdl.DocumentElement.FirstChild);
            }
            else
            {
                wsdl.DocumentElement.InsertAfter(types, import);
            }

            return types;
        }
    }
}