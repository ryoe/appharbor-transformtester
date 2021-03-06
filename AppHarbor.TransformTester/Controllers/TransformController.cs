﻿using System.Text;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Web.Publishing.Tasks;

namespace AppHarbor.TransformTester.Controllers
{
	public class TransformController : Controller
	{
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Create(string webConfigXml, string transformXml)
		{
			try
			{
				var transformation = new XmlTransformation(transformXml, false, null);
				var document = new XmlDocument();
				document.LoadXml(webConfigXml);
				var success = transformation.Apply(document);
				if (success)
				{
					var stringBuilder = new StringBuilder();
					var xmlWriterSettings = new XmlWriterSettings();
					xmlWriterSettings.Indent = true;
					xmlWriterSettings.IndentChars = "    ";
					using (var xmlTextWriter =
						XmlTextWriter.Create(stringBuilder, xmlWriterSettings))
					{
						document.WriteTo(xmlTextWriter);
					}
					return Content(stringBuilder.ToString(), "text/xml");
				}
				else
				{
					return ErrorXml("Transformation failed for unkown reason");
				}
			}
			catch (XmlTransformationException xmlTransformationException)
			{
				return ErrorXml(xmlTransformationException.Message);
			}
			catch (XmlException xmlException)
			{
				return ErrorXml(xmlException.Message);
			}
		}

		private ContentResult ErrorXml(string errorMessage)
		{
			return Content(
					new XDocument(
						new XElement("error", errorMessage)
				).ToString(), "text/xml");
		}
	}
}
