﻿using PagedList;
using Said.Common;
using Said.Models;
using Said.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Said.Application
{
    /// <summary>
    /// 使用静态类为让它变得更加晦涩，如果需要静态类的语法则可以使用单例模式实现
    /// </summary>
    public static class ArticleApplication
    {

        private static IArticleService service;
        public static IArticleService Context
        {
            get { return service ?? (service = new ArticleService(new Domain.Said.Data.DatabaseFactory())); }
        }

        /// <summary>
        /// 添加一篇文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Add(Article model)
        {
            Context.Add(model);
            return service.Submit();
        }

        /// <summary>
        /// 修改一篇文章
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update(Article model)
        {
            Context.Update(model);
            return service.Submit();
        }


        #region 逻辑
        /// <summary>
        /// 验证一篇said是否是有效的said，同时矫正Said的数据
        /// </summary>
        /// <param name="model">要验证的model</param>
        /// <returns>返回null表示验证成功，否则返回验证失败的字符串，用,号分割</returns>
        public static string ValidateAndCorrectSubmit(Article model)
        {
            StringBuilder str = new StringBuilder();
            //防止tag有HTML标签，修正
            if (!string.IsNullOrWhiteSpace(model.STag))
                model.STag = HTMLCommon.HTMLTrim(model.STag);
            //model.Song = SongApplication.Context.GetById(model.SongId);//检索有没有歌曲信息
            if (SongApplication.Context.GetById(model.SongId) == null)
                str.Append("歌曲信息不正确(不可获取),");
            if (ImageApplication.Context.GetById(model.ImageId) == null)
                str.Append("缩略图信息不正确(不可获取),");

            foreach (var validateResult in model.Validate())
            {
                //validateResult.MemberNames//这个要搞懂怎么用，或许能让提示信息更全一点
                str.Append(validateResult.ErrorMessage + ",");
            }
            if (str.Length > 0)
                str.Length--;//StringBuilder的length可以用于裁剪字符串？
            else
            {
                if (string.IsNullOrEmpty(model.SaidId))
                {
                    //新增
                    if (string.IsNullOrWhiteSpace(model.SName) || ArticleApplication.FindByFileName(model.SName.Trim()) != null)    //没有文件名或文件名不合法，则生成一个新的文件名
                        model.SName = FileCommon.CreateFileNameByTime();
                }
                else {
                    //编辑
                    if (string.IsNullOrWhiteSpace(model.SName))
                    {
                        model.SName = FileCommon.CreateFileNameByTime();
                    }
                    else {
                        //从数据库中检索是否存在
                        var SaidIdLists = ArticleApplication.FindSaidIdByFileName(model.SName).ToList();
                        //文件名重复
                        if (SaidIdLists.Count > 1 && SaidIdLists.IndexOf(model.SaidId) > -1)
                        {
                            model.SName = FileCommon.CreateFileNameByTime();
                        }

                    }

                }

            }

            return str.Length > 0 ? str.ToString() : null;
        }
        #endregion


        #region 查询
        /// <summary>
        /// 查找
        /// </summary>
        /// <returns></returns>
        public static Article Find(string id)
        {
            return Context.GetById(id);
        }



        /// <summary>
        /// 查询全部文章
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Article> Find()
        {
            return Context.GetManyDesc(m => m.IsDel == 0, m => m.Date);
        }

        /// <summary>
        /// 查找Said的文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Article FindByFileName(string fileName)
        {
            return Context.Get(m => m.SName == fileName);
        }

        /// <summary>
        /// 根据文章文件名称，获取该文件名称对应的SaidId（列表），用于检索文件重名业务
        /// </summary>
        /// <param name="fileName">要检索的文件名称</param>
        /// <returns>返回SaidID列表</returns>
        public static IEnumerable<string> FindSaidIdByFileName(string fileName)
        {
            return Context.GetSaidIdByFileName(fileName);
        }



        /// <summary>
        /// 查找所有Said的文件名称（用于防止重名的业务）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> FindAllFileNames()
        {
            return Context.GetFileNames();
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page">分页对象</param>
        /// <returns>返回封装后的IPagedList对象</returns>
        public static IPagedList<Article> Find(Models.Data.Page page)
        {
            return Context.GetPageDesc(page, m => m.STitle != null, m => m.Date);
        }

        /// <summary>
        /// 贪婪获取指定个数的文章列表
        /// </summary>
        /// <param name="top">要获取的个数</param>
        /// <returns></returns>
        public static IEnumerable<Article> GetByTop(int top)
        {
            return Context.GetByTop(top);
        }

        /// <summary>
        /// 贪婪分页查询
        /// </summary>
        /// <param name="page">分页对象</param>
        /// <returns>返回封装后的IPagedList对象</returns>
        public static IPagedList<Article> FindByDateDesc(Models.Data.Page page)
        {
            return Context.FindByDateDesc(page, m => m.STitle != null, m => m.Date);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="page">分页对象</param>
        /// <param name="keywords">要查询的关键字</param>
        /// <returns>返回封装后的IPagedList对象</returns>
        public static IPagedList<Article> Find(Models.Data.Page page, string keywords)
        {
            return Context.GetPageDesc(page, m => m.STitle.Contains(keywords) || m.SContext.Contains(keywords), m => m.Date);
        }
        #endregion


        #region 删除
        /// <summary>
        /// 逻辑删除一篇Said
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int Delete(Article model)
        {
            model.IsDel = 1;
            Context.Update(model);
            return Context.Submit();
        }


        /// <summary>
        /// 无力删除一篇Said（不可恢复）
        /// </summary>
        /// <param name="id">要删除的文章id</param>
        /// <returns></returns>
        public static int RealDelete(string id)
        {
            Context.Delete(m => m.SaidId == id);
            return Context.Submit();
        }

        #endregion
    }
}
