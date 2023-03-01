using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace API
{
    public class ParseXML
    {
        private static Exception LastException { get; set; }
        public static Exception GetException() { return LastException; }


        public static string WayXML { get; set; }


        public bool ReadXML()
        {
            if (!File.Exists(WayXML))
                return false;

            bool result = true;
            List<FrameStruct> frames = new List<FrameStruct>(10);

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(WayXML);
                XmlNode rootNode = xml.FirstChild;

                foreach (XmlNode node in rootNode)
                {
                    if (node.LocalName == "frame")
                    {
                        List<ItemStruct> items = new List<ItemStruct>(4);
                        foreach (XmlNode node2 in node.ChildNodes)
                        {
                            if (node2.LocalName == "item")
                            {
                                ItemStruct item1 = new ItemStruct();
                                foreach (XmlNode node3 in node2)
                                {
                                    switch (node3.LocalName)
                                    {
                                        case "size":
                                            item1.Size = Convert.ToInt32(node3.InnerText);
                                            break;
                                        case "name":
                                            item1.Name = node3.InnerText;
                                            break;
                                        case "type":
                                            item1.Type = node3.InnerText;
                                            break;
                                        case "data":
                                            item1.Data =
                                                Enumerable.Range(0, node3.InnerText.Replace(" ", "").Length).Where(x => x % 2 == 0)
                                                .Select(x => Convert.ToByte(node3.InnerText.Replace(" ", "").Substring(x, 2), 16)).ToArray();
                                            break;
                                    }
                                }
                                if (item1.Size == 0)
                                {
                                    result = false;
                                    LastException = new Exception("Кадр: " + node.Attributes.GetNamedItem("name").Value +
                                        "\n\nОписание поля кадра '" + item1.Name + "' не имеет размера.");
                                    return result;
                                }
                                items.Add(item1);
                            }
                        }

                        FrameStruct frame = new FrameStruct()
                        {
                            Name = node.Attributes.GetNamedItem("name").Value,
                            Items = items.ToArray()
                        };
                        frames.Add(frame);
                    }
                }
                FrameAnalize.Frames = frames.ToArray();
            }
            catch (Exception ex) { LastException = ex; result = false; }

            if (frames.Count == 0)
            {
                result = false;
                LastException = new Exception("Файл: " + WayXML +
                    "\n\nне содержит указанной кадровой структуры");
            }
            return result;
        }




        //Пробовал сделать через Serealize, возникли вопросы по структуре (а именно по массивам)
        //Не уверен что xml файл был сделан через данный метод
        //Воспользовался старой классикой - циклический обход по нодам

        //public void SerealizeFrames()
        //{
        //    Item item = new Item()
        //    {
        //        Name = "weq",
        //        Size = 3,
        //        Data = new byte[] { 0, 0, 0, 0, 0, },
        //        Type = "none"
        //    };
        //    Item[] items = new Item[4] { item, item, item, item };
        //    Frame frame = new Frame
        //    {
        //        Name = "kadr",
        //        Items = items
        //    };
        //    Frame[] frames = new Frame[2] { frame, frame };

        //    XmlAttributeOverrides xOver = new XmlAttributeOverrides();
        //    XmlAttributes attrs = new XmlAttributes();
        //    XmlRootAttribute xRoot = new XmlRootAttribute();
        //    xRoot.Namespace = WayXML;
        //    xRoot.Namespace = "";
        //    xRoot.ElementName = "structure";
        //    xRoot.IsNullable = true;
        //    attrs.XmlRoot = xRoot;
        //    attrs.Xmlns = false;
        //    xOver.Add(typeof(Frame), attrs);

        //    XmlSerializer serializer = new XmlSerializer(typeof(Frame[]), /*new XmlRootAttribute("structure"),*/xOver);
        //    serializer.Serialize(new FileStream(WayXML, FileMode.Create), frames);
        //}

        //public Frame[] DeserealizeFrames()
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(Frame[]), new XmlRootAttribute("structure"));
        //    return (Frame[])serializer.Deserialize(new FileStream(WayXML, FileMode.Open));
        //}

        ////дополнительные настройки для XmlSerializer
        //public XmlSerializer CreateOverrider()
        //{
        //    // Create the XmlAttributes and XmlAttributeOverrides objects.
        //    XmlAttributes attrs = new XmlAttributes();
        //    XmlAttributeOverrides xOver = new XmlAttributeOverrides();

        //    XmlRootAttribute xRoot = new XmlRootAttribute();

        //    // Set a new Namespace and ElementName for the root element.
        //    xRoot.Namespace = null;
        //    xRoot.ElementName = "structure";
        //    attrs.XmlRoot = xRoot;

        //    /* Add the XmlAttributes object to the XmlAttributeOverrides.
        //       No  member name is needed because the whole class is
        //       overridden. */
        //    xOver.Add(typeof(Frame[]), attrs);

        //    // Get the XmlAttributes object, based on the type.
        //    XmlAttributes tempAttrs;
        //    tempAttrs = xOver[typeof(Frame[])];

        //    // Print the Namespace and ElementName of the root.
        //    Console.WriteLine(tempAttrs.XmlRoot.Namespace);
        //    Console.WriteLine(tempAttrs.XmlRoot.ElementName);

        //    XmlSerializer xSer = new XmlSerializer(typeof(Frame[]), xOver);
        //    return xSer;
        //}



    }
}
