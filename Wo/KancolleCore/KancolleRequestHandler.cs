﻿using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace Wo.KancolleCore
{
    class KancolleRequestHandler : IRequestHandler
    {
        public event Action<RequestInfo, string> OnAPIResponseReceived;
        public event Action<RequestInfo, string> OnSwfResponseReceived;

        private IFrame gameFrame = null;
        private KancolleResponseFilter responseFilter = new KancolleResponseFilter();
        private KancolleFrameResponseFilter gameFrameFilter = new KancolleFrameResponseFilter();
        private KancollePostResponseFilter apiResponseFilter = new KancollePostResponseFilter();


        public KancolleRequestHandler()
        {
            apiResponseFilter.OnPostResponseReceived += (request, content) =>
            {
                OnAPIResponseReceived?.Invoke(request, content);
            };


        }

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {

            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {

            if (request.Url == KancolleUtils.KanColleUrl)
            {
                return responseFilter;
            }
            else if (request.Url.StartsWith(KancolleUtils.KanColleFrameSrcPrefix))
            {
                //本打算用这个过滤器修改偏移，但是不知道为什么会导致岛风GO无法识别游戏的运行
                //return gameFrameFilter;
            }
            else if (request.Url.StartsWith(KancolleUtils.KanColleAPIUrl))
            {
                return apiResponseFilter;
            }
            return null;

        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, bool isRedirect)
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {

            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {

        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return false;
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {

            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser, CefTerminationStatus status)
        {

        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {

        }

        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {

        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, ref string newUrl)
        {

        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            if (request.Url == KancolleUtils.KanColleUrl)
            {
                if (response.ResponseHeaders["Content-Length"] != null)
                    responseFilter.ContentLength = int.Parse(response.ResponseHeaders["Content-Length"]);
            }
            else if (request.Url.StartsWith(KancolleUtils.KanColleFrameSrcPrefix))
            {//过滤掉多余的页面信息（貌似使用filter会出问题，使得岛风GO无法识别）
                gameFrame = frame;
                var script = "document.body.style.margin='0px';";
                browserControl.ExecuteScriptAsync(script);
                gameFrameFilter.ContentLength = int.Parse(response.ResponseHeaders["Content-Length"]);

            }
            else if (request.Url.Contains(KancolleUtils.KanColleAPIKeyword))//(request.Url.StartsWith(KancolleCommon.DMMUrls.KanColleAPIUrl))
            {//获取api post
                RequestInfo requestInfo = new RequestInfo();
                requestInfo.RequestUrl = request.Url;
                foreach (var postData in request.PostData.Elements)
                {
                    foreach (var kv in getParams(postData.GetBody()))
                    {
                        requestInfo.Data.Add(kv.Key, kv.Value);
                        requestInfo.DataString += postData.GetBody() + " ";
                    }
                }
                apiResponseFilter.InitFilter();
                apiResponseFilter.CurrentRequest = requestInfo;
                apiResponseFilter.ContentLength = response.ResponseHeaders.Count == 0 ? 0 : int.Parse(response.ResponseHeaders["Content-Length"]);
            }
            else if (request.Url.StartsWith(KancolleUtils.KanColleSwfUrl))
            {
                RequestInfo requestInfo = new RequestInfo();
                string requestUrl = request.Url;
                int pindex = requestUrl.IndexOf("?");
                if (pindex > 0)
                {
                    requestInfo.RequestUrl = requestUrl.Substring(0, pindex);
                    requestInfo.DataString = requestUrl.Substring(pindex + 1);
                    requestInfo.Data = getParams(requestInfo.DataString);
                }
                else
                    requestInfo.RequestUrl = request.Url;
                OnSwfResponseReceived?.Invoke(requestInfo, null);
            }
            return false;
        }

        private Dictionary<string, string> getParams(string data)
        {
            Dictionary<string, string> paramDic = new Dictionary<string, string>();
            string[] keyvalues = data.Split(new char[] { '&' });
            foreach (var keyvalue in keyvalues)
            {
                var kv = System.Web.HttpUtility.UrlDecode(keyvalue).Split(new char[] { '=' }); ;
                paramDic.Add(kv[0], kv[1]);
            }

            return paramDic;
        }

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            return;
        }
    }
}
