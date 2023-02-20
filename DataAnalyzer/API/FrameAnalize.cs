using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace API
{
    [Serializable]
    public struct Frame
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlArray("frame")]
        [XmlArrayItem("item")]
        public Item[] Items;
    }

    [Serializable]
    public struct Item
    {
        [XmlElement("size")]
        public int? Size;
        [XmlElement("name")]
        public string Name;
        [XmlElement("type")]
        public string Type;
        [XmlElement("data")]
        public byte[] Data;
    }



    public class FrameAnalize
    {
        private const int MinPartOfReadSize = 10;//минимальная часть от размера buff при которой будет продолжаться чтение (или новое считывание в buff)
        private const int MinReadSize = 100;//минимальный кол-во байт при котором будет продолжаться чтение (или новое считывание в buff)
        /// <summary>
        ///Размер выделяемой памяти для считывания
        /// </summary>
        public static int BuffSize { get; set; } = 2048;

        private static Exception LastException { get; set; }
        /// <summary>
        /// Получить последнее исключение
        /// </summary>
        /// <returns></returns>
        public static Exception GetException()
        { return LastException; }

        /// <summary>
        /// Набор структур кадров (обязателен для инициализации)
        /// </summary>
        public static Frame[] Frames { get; set; }
        /// <summary>
        /// Фоновый поток для возвращения прогресса (не обязателен)
        /// </summary>
        public static BackgroundWorker Back { get; set; }
        /// <summary>
        /// Путь к проверяемому файлу (обязателен для инициализации)
        /// </summary>
        public static string WayFile { get; set; }

        private static Dictionary<string, Report> Errors { get; set; }
        /// <summary>
        /// Возврат результата работы.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Report> GetErrors()
        { return Errors; }

        /// <summary>
        /// Проверка заданных кадровых структур(Frames) в указанном файле (WayFile)
        /// </summary>
        /// <returns></returns>
        public static bool Analize()
        {
            bool result = false;

            if (WayFile == null || Frames == null)
            {
                LastException = new Exception("Не установлены значения:\n" +
                    "путь к файлу - WayFile\n" +
                    "набор кадров - Frames");
                return result;
            }


            //try
            //{
                Errors = new Dictionary<string, Report>(Frames.Length);
                byte[] buff = new byte[BuffSize];

                List<Frame> lFrames = new List<Frame>(Frames);
                Frame lFrame = new Frame();
                Item lItemMarker = new Item();
                Item lItemIncrement = new Item();
                //Dictionary<string, Item> lastIncrements = new Dictionary<string, Item>();//последние найденые счетчики 
                //(сперва предположил что у каждого типа кадра свой счётчик)
                long? lastIncrement = null;//последний счётчик

            using (FileStream stream = File.Open(WayFile, FileMode.Open))
            {
                bool isEnd = false;//текущий массив buff последний на обработку (крайние байты)

                for (int i = 0; /*i < buff.Length || */stream.Position + i < stream.Length; i++)
                {
                    int minSize = 0;// минимальный необходимый размер для считывания маркера и счётчика

                    stream.Read(buff, 0, buff.Length);
                    stream.Position -= buff.Length;

                    bool isFind = false;
                    bool reRead = false;

                    foreach (Frame frame in lFrames)
                    {

                        foreach (Item item in frame.Items)
                        {
                            if (item.Name == "Маркер")
                            {
                                minSize += (int)item.Size;
                                lItemMarker = item;
                                break;
                            }
                        }
                        foreach (Item item in frame.Items)
                        {
                            if (item.Name == "Номер пакета")
                            {
                                minSize += (int)item.Size;
                                lItemIncrement = item;
                                break;
                            }
                        }
                        if (i + minSize > buff.Length - 1)//массив buff почти перебрали
                        {
                            reRead = true;
                            break;
                        }

                        byte[] lBuff = new byte[(int)lItemMarker.Size];
                        for (int j = 0; j < lBuff.Length; j++)
                        { lBuff[j] = buff[i + j]; }



                        if (lBuff.SequenceEqual(lItemMarker.Data))
                        {
                            isFind = true;
                            lFrame = frame;
                            if (!lFrames[0].Equals(lFrame))
                                Funcs<Frame>.MoveToFirst(ref lFrames, lFrame);//чтобы начать перебор с того же типа кадра 
                            if (!Errors.ContainsKey(lFrame.Name))
                            {
                                Report report = new Report
                                {
                                    FrameCount = 1,
                                    ErrorNumberCount = 0,
                                    ErrorCRCCount = 0
                                };
                                Errors.Add(lFrame.Name, report);
                            }
                            else
                            {
                                Report f = Errors[lFrame.Name];
                                f.FrameCount++;
                                Errors[lFrame.Name] = f;
                            }
                        }
                        if (isFind) break;
                    }

                    if (isFind)
                    {
                        byte[] lBuff = new byte[(int)lItemIncrement.Size];
                        for (int j = 0; j < lBuff.Length; j++)
                        { lBuff[j] = buff[i + (int)lItemMarker.Size + j]; }

                        lItemIncrement.Data = lBuff;
                        if (lastIncrements.ContainsKey(lFrame.Name))
                        {
                            if (lastIncrements[lFrame.Name].Data != null)
                            {
                                ulong vLast = BitConverter.ToUInt32(lastIncrements[lFrame.Name].Data, 0);
                                ulong vNext = BitConverter.ToUInt32(lItemIncrement.Data, 0);
                                if (vNext - vLast != 1)
                                {
                                    Report f = Errors[lFrame.Name];
                                    f.ErrorNumberCount++;
                                    Errors[lFrame.Name] = f;
                                }
                            }
                        }
                        lastIncrements[lFrame.Name] = lItemIncrement;
                    }



                    if ((i > buff.Length / MinPartOfReadSize || buff.Length - i < MinReadSize || reRead) && !isEnd)//возврат к перепрочтению
                    {
                        //дописать выход (когда байт осталось меньше чем маркер + счетчик)
                        if (reRead)
                            i -= minSize;
                        stream.Position += i;
                        i = 0;
                        if (buff.Length > stream.Length - stream.Position)
                        {
                            buff = new byte[(int)(stream.Length - stream.Position)];
                            isEnd = true;
                        }
                        else { buff = new byte[BuffSize]; }
                    }
                    if (isFind)
                    {
                        foreach (Item item in lFrame.Items)
                        {
                            if (item.Size != null && item.Name != "CRC")
                            {
                                stream.Position += (int)item.Size;
                            }
                        }
                        stream.Position += i;
                        i = 0;
                        if (buff.Length > stream.Length - stream.Position)
                        {
                            buff = new byte[(int)(stream.Length - stream.Position)];
                        }
                        else { buff = new byte[BuffSize]; }

                        foreach (Item item in lFrame.Items)
                        {
                            if (item.Name == "CRC")
                            {
                                byte[] lBuff = new byte[(int)item.Size];
                                for (int j = 0; j < lBuff.Length; j++)
                                { lBuff[j] = buff[i]; }
                                i += lBuff.Length;

                                //check crc
                                break;
                            }
                        }
                    }

                }

                Console.WriteLine("end");
                //Back.ReportProgress(0);
            }




                


            //}
            //catch(Exception ex) { LastException = ex; result = false; }
            return result;
        }

       

        public struct Report
        {
            //string FrameName;
            public long FrameCount;
            public long ErrorNumberCount;
            public long ErrorCRCCount;
        }
    }
}
