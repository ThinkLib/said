﻿using Said.Application;
using Said.Common;
using Said.Controllers.Attrbute;
using Said.Controllers.Filters;
using Said.Helper;
using Said.Models;
using Said.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Said.Controllers
{
    [UserFilterAttribute]
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        //[WapFilterAttribute]
        public ActionResult Index()
        {
            if (Request.Browser.IsMobileDevice)//wap
            {
                var res = ArticleApplication.FindByDateDesc(new Page
                {
                    PageNumber = 1,
                    PageSize = 10
                });
                ViewData["articleList"] = res.ToList<Article>();
                ViewData["maxPage"] = res.TotalItemCount % 10 == 0 ? res.TotalItemCount / 10 : res.TotalItemCount / 10 + 1;
            }
            else {
                ViewData["articleList"] = ArticleApplication.GetByTop(3).ToList<Article>();
            }
            //dipslay mode - 直接访问域名：http://tasaid.com ，无法自动识别到mobile，需要访问 http://tasaid.com/home/index 才可以自动识别mobile，所以手动修正这个问题
            return Request.Browser.IsMobileDevice ? View("Index.Mobile") : View();
        }


        /// <summary>
        /// 404
        /// </summary>
        /// <returns></returns>
        public ActionResult NotFound()
        {
            return View();
        }


        /// <summary>
        /// 404
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            ViewData["NavigatorIndex"] = 5;
            return View();
        }


        /// <summary>
        /// 统计方法
        /// </summary>
        /// <returns></returns>
        [NoFilter]
        public ActionResult Cl(string url, string referrer = null)
        {
            //收集统计信息
            string key = Request[SaidRecordCommon.KEY];
            //修正
            if (string.IsNullOrWhiteSpace(key))
                key = string.Empty;

            //请求来源
            Uri urlReferrer = null;
            //请求的url
            Uri requestUrl = null;
            //检测并修正来源
            if (string.IsNullOrWhiteSpace(referrer) || !Uri.TryCreate(referrer, UriKind.RelativeOrAbsolute, out urlReferrer))
            {
                urlReferrer = Request.UrlReferrer;
            }
            //检测请求的url是否合法
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out requestUrl))
            {
                SaidRecordCommon.AddFail(key, url, urlReferrer == null ? null : urlReferrer.OriginalString);
                return Redirect(url);
            }
            //检测通过
            if (urlReferrer == null)
                SaidRecordCommon.Add(key, url);
            else
                SaidRecordCommon.Add(key, url, urlReferrer);
            return Redirect(url);
        }


        #region Service


        #endregion
    }
}
