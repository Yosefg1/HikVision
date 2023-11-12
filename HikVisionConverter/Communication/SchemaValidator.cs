using Microsoft.VisualBasic.FileIO;
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace HikVisionConverter.Communication;
public class SchemaValidator
{

    private static string PATH = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schemas");
    public static void CheckValidation<T>(T xml, bool isLog = false)
    {
        XmlDocument asset = new();

        //// Add configuration schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        //// Add basic types schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        //// Add control message schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        //// Add subscription schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        //// Add indication schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        //// Add status schema
        //asset.Schemas.Add(GetXmlSchemaFromResource());

        if (isLog)
            Console.WriteLine("Asset has loaded schemas.");

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringWriter writer = new StringWriter();
        serializer.Serialize(writer, xml);

        if (isLog)
            Console.WriteLine("XmlSerializer has serialized given xml.");

        asset.Load(new StringReader(writer.ToString()));
        if (isLog)
            Console.WriteLine("Asset has loaded given xml.");

        asset.Validate(ValidationCallBack);
        if (isLog)
            Console.WriteLine("Validation successed.");
    }

    private static XmlSchema GetXmlSchemaFromResource(string resources)
    {
        return XmlSchema.Read(new XmlTextReader(new StringReader(resources)), ValidationCallBack!)!;
    }

    private static void ValidationCallBack(object sender, ValidationEventArgs args)
    {
        throw new Exception(args.Severity == XmlSeverityType.Warning ?
            "\tWarning: Matching schema not found. No validation occurred." + args.Message : "\tValidation error: " + args.Message);
    }
}
