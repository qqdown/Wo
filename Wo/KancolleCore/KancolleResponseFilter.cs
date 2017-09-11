using CefSharp;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wo.KancolleCore
{
    [Serializable]
    public class RequestInfo
    {
        public string RequestUrl { get; set; }
        public string DataString { get; set; }
        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();

        public RequestInfo Clone()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, this);
                stream.Position = 0;
                var clonedData = formatter.Deserialize(stream) as RequestInfo;
                stream.Close();
                return clonedData;
            }
        }
    }

    class KancolleResponseFilter : IResponseFilter
    {
        public int ContentLength { get; set; } = 0;
        private List<byte> dataAll = new List<byte>();

        public void Dispose()
        {

        }

        public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                //dataOutWritten = Math.Min(dataInRead, dataOut.Length);
                dataOutWritten = 0;

                //dataIn.CopyTo(dataOut);
                dataIn.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                dataAll.AddRange(bs);
                if (dataAll.Count == this.ContentLength)
                {
                    string originHtml = Encoding.UTF8.GetString(dataAll.ToArray());
                    int index = originHtml.IndexOf("<iframe id=\"game_frame\"");// ("<iframe id=\"game_frame\"");

                    string content = originHtml.Substring(index);
                    index = content.IndexOf("</iframe>");// ("</iframe>");
                    content = content.Substring(0, index + 9);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    HtmlNode root = doc.DocumentNode.FirstChild;
                    root.Attributes.Remove("style");
                    root.Attributes["height"].Value = "480";
                    root.Attributes["width"].Value = "800";

                    content = root.OuterHtml;

                    StreamWriter sw = new StreamWriter(dataOut);
                    sw.Write(content);
                    sw.Flush();
                    dataOutWritten = content.Length;
                    return FilterStatus.Done;
                }
                else if (dataAll.Count < this.ContentLength)
                {
                    dataInRead = dataIn.Length;
                    dataOutWritten = dataIn.Length;

                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Error;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataInRead = dataIn.Length;
                dataOutWritten = 0;
                return FilterStatus.Done;
            }
        }



        public bool InitFilter()
        {
            dataAll.Clear();
            return true;
        }
    }

    class KancolleFrameResponseFilter : IResponseFilter
    {
        public int ContentLength { get; set; } = 0;

        private List<byte> dataAll = new List<byte>();
        private const string TopSpaceTag = "id=\"spacing_top\"";
        private const string TopSpaceStr = "id=\"spacing_top\" style=\"height:16px;\"";
        private const string TopSpaceModifiedStr = "id=\"spacing_top\" style=\"height:0px;\"";

        public void Dispose()
        {

        }



        public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                dataIn.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                int beforeSize = dataAll.Count;
                dataAll.AddRange(bs);

                string originHtml = Encoding.UTF8.GetString(bs.ToArray());
                string content = originHtml;

                //content = originHtml.Replace(TopSpaceStr, TopSpaceModifiedStr);

                StreamWriter sw = new StreamWriter(dataOut);
                sw.Write(content);
                sw.Flush();
                dataOutWritten = content.Length;

                if (dataAll.Count == this.ContentLength)
                {
                    return FilterStatus.Done;
                }
                else if (dataAll.Count < this.ContentLength)
                {
                    dataInRead = dataIn.Length;
                    //dataOutWritten = dataIn.Length;

                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Error;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataInRead = dataIn.Length;
                dataOutWritten = dataInRead;
                return FilterStatus.Done;
            }
        }



        public bool InitFilter()
        {
            dataAll.Clear();
            return true;
        }


    }

    class KancollePostResponseFilter : IResponseFilter
    {
        public int ContentLength { get; set; } = 0;
        public RequestInfo CurrentRequest { get; set; }
        public event Action<RequestInfo, string> OnPostResponseReceived;

        private List<byte> dataAll = new List<byte>();

        public void Dispose()
        {

        }

        public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                dataOutWritten = Math.Min(dataInRead, dataOut.Length);

                dataIn.CopyTo(dataOut);
                dataIn.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                dataAll.AddRange(bs);
                if (dataAll.Count == this.ContentLength)
                {
                    string content = Encoding.UTF8.GetString(dataAll.ToArray());
                    OnPostResponseReceived(CurrentRequest, content);
                    return FilterStatus.Done;
                }
                else if (dataAll.Count < this.ContentLength)
                {
                    dataInRead = dataIn.Length;
                    dataOutWritten = dataIn.Length;

                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Error;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("API Post Response error\n" + ex.Message);
                dataInRead = dataIn.Length;
                dataOutWritten = dataInRead;
                return FilterStatus.Done;
            }
        }



        public bool InitFilter()
        {
            dataAll.Clear();
            return true;
        }
    }
}
