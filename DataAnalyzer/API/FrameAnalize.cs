using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace API
{
    [Serializable]
    public struct FrameStruct
    {
        [XmlAttribute("name")]
        public string Name;
        [XmlArray("frame")]
        [XmlArrayItem("item")]
        public ItemStruct[] Items;
    }

    [Serializable]
    public struct ItemStruct
    {
        [XmlElement("size")]
        public int Size;
        [XmlElement("name")]
        public string Name;
        [XmlElement("type")]
        public string Type;
        [XmlElement("data")]
        public byte[] Data;
    }


    public struct ReportStruct
    {
        public string FrameName;
        public uint FrameCount;
        public uint ErrorNumberCount;
        public uint ErrorCRCCount;
    }



    public class FrameAnalize
    {
        private const int MinReadSize = 4096;//минимальный кол-во байт при котором будет продолжаться чтение (или новое считывание в buff)
        /// <summary>
        ///Размер выделяемой памяти для считывания
        /// </summary>
        public static int BuffSize { get; set; } = 4096 * 100;//400 кБайт (оптимальный объём - от 200 до 400)

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
        public static FrameStruct[] Frames { get; set; }
        /// <summary>
        /// Фоновый поток для возвращения прогресса (не обязателен)
        /// </summary>
        public static BackgroundWorker Back { get; set; }
        /// <summary>
        /// Путь к проверяемому файлу (обязателен для инициализации)
        /// </summary>
        public static string WayFile { get; set; }


        private static ReportStruct[] Errors { get; set; }
        public static ReportStruct[] GetErrors()
        { return Errors; }

        /// <summary>
        /// Проверка заданных кадровых структур(Frames) в указанном файле(WayFile)
        /// </summary>
        /// <returns></returns>
        public static bool Analize()
        {
            bool result = false;
            ReportStruct[] errors = new ReportStruct[Frames.Length];

            if (WayFile == null || Frames == null)
            {
                LastException = new Exception("Не установлены значения:\n" +
                    "путь к файлу - WayFile\n" +
                    "набор кадров - Frames");
                return result;
            }


            try
            {
                byte[] buff = new byte[BuffSize];
                int index = 0;
                uint?[] lastIncrements = new uint?[Frames.Length];//последние найденые счетчики 
                for (int i = 0; i < lastIncrements.Length; i++)
                    lastIncrements[i] = null;
                for (int i = 0; i < Frames.Length; i++)
                    errors[i].FrameName = Frames[i].Name;

                using (FileStream stream = File.Open(WayFile, FileMode.Open))
                {
                    long oldPosition = stream.Position;//для передачи прогресса
                    bool isEnd = false;//текущий массив buff последний на обработку (крайние байты)

                    stream.Read(buff, 0, buff.Length);
                    stream.Position -= buff.Length;
                    for (int i = 0; i < buff.Length - 10; i++)
                    {
                        //убрать сообщение о прогрессе +100 к скорости
                        if (stream.Position - oldPosition > 10e+6)
                        {
                            Back.ReportProgress((int)(stream.Position * 100 / stream.Length), stream.Position);
                            oldPosition = stream.Position;
                            if (Back.CancellationPending)
                            {
                                result = false;
                                new Funcs().TotalCount(ref errors);
                                Errors = errors;
                                return result;
                            }
                        }

                        //считывание
                        if ((buff.Length - i < MinReadSize) && !isEnd)
                        {
                            stream.Position += i;
                            i = 0;
                            if (buff.Length > stream.Length - stream.Position)
                            {
                                buff = new byte[(int)(stream.Length - stream.Position)];
                                isEnd = true;
                            }
                            else { buff = new byte[BuffSize]; }
                            stream.Read(buff, 0, buff.Length);
                            stream.Position -= buff.Length;
                        }

                        //поиск
                        for (int k = 0; k < Frames.Length; k++)
                        {
                            for (int p = 0; p < Frames[k].Items.Length; p++)
                            {
                                if (Frames[k].Items[p].Name == "Маркер")
                                {
                                    byte[] lBuff = new byte[Frames[k].Items[p].Size];
                                    for (int j = 0; j < lBuff.Length; j++)
                                    { lBuff[j] = buff[i + j]; }

                                    if (lBuff.SequenceEqual(Frames[k].Items[p].Data))
                                    {
                                        index = k;
                                        goto Found;
                                    }
                                }
                            }
                        }
                        continue;

                    //отправка на проверку
                    Found:
                        {
                            int lSize = 0;
                            uint? newIncr = null;

                            for (int p = 0; p < Frames[index].Items.Length; p++)
                            {
                                if (Frames[index].Items[p].Name == "Номер пакета")
                                {
                                    byte[] rBuff = new byte[Frames[index].Items[p].Size];
                                    for (int j = 0; j < rBuff.Length; j++)
                                    { rBuff[j] = buff[i + lSize + j]; };
                                    Array.Reverse(rBuff);
                                    newIncr = BitConverter.ToUInt32(rBuff, 0);
                                }
                                lSize += Frames[index].Items[p].Size;
                            }
                            byte[] lBuff = new byte[lSize];
                            for (int j = 0; j < lBuff.Length; j++)
                            { lBuff[j] = buff[i + j]; }

                            Check check = new Check();
                            if(check.CheckPack(lBuff, Frames[index].Items, lastIncrements[index], ref errors[index]))

                                i+= lSize;
                            lastIncrements[index] = newIncr;
                        }
                    }
                }
                result = true;
            }
            catch (Exception ex) { LastException = ex; result = false; }
            new Funcs().TotalCount(ref errors);
            Errors = errors;
            return result;
        }

    }
}
