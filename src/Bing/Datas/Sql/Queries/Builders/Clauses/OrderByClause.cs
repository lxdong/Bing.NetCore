﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Bing.Datas.Sql.Queries.Builders.Abstractions;
using Bing.Datas.Sql.Queries.Builders.Core;
using Bing.Domains.Repositories;
using Bing.Properties;
using Bing.Utils.Extensions;

namespace Bing.Datas.Sql.Queries.Builders.Clauses
{
    /// <summary>
    /// Order By子句
    /// </summary>
    public class OrderByClause:IOrderByClause
    {
        /// <summary>
        /// 排序项列表
        /// </summary>
        private readonly List<OrderByItem> _items;

        /// <summary>
        /// Sql方言
        /// </summary>
        private readonly IDialect _dialect;

        /// <summary>
        /// 实体解析器
        /// </summary>
        private readonly IEntityResolver _resolver;

        /// <summary>
        /// 实体别名注册器
        /// </summary>
        private readonly IEntityAliasRegister _register;

        /// <summary>
        /// 初始化一个<see cref="OrderByClause"/>类型的实例
        /// </summary>
        /// <param name="dialect">Sql方言</param>
        /// <param name="resolver">实体解析器</param>
        /// <param name="register">实体别名注册器</param>
        public OrderByClause(IDialect dialect, IEntityResolver resolver, IEntityAliasRegister register)
        {
            _items = new List<OrderByItem>();
            _dialect = dialect;
            _resolver = resolver;
            _register = register;
        }

        /// <summary>
        /// 初始化一个<see cref="OrderByClause"/>类型的实例
        /// </summary>
        /// <param name="orderByClause">Order By子句</param>
        /// <param name="register">实体别名注册器</param>
        protected OrderByClause(OrderByClause orderByClause, IEntityAliasRegister register)
        {
            _dialect = orderByClause._dialect;
            _resolver = orderByClause._resolver;
            _register = register;
            _items = new List<OrderByItem>(orderByClause._items);
        }

        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="register">实体别名注册器</param>
        /// <returns></returns>
        public virtual IOrderByClause Clone(IEntityAliasRegister register)
        {
            return new OrderByClause(this, register);
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="order">排序列表</param>
        /// <param name="tableAlias">表别名</param>
        public void OrderBy(string order, string tableAlias = null)
        {
            if (string.IsNullOrWhiteSpace(order))
            {
                return;
            }

            order.Split(',').ToList().ForEach(column => AddItem(column, tableAlias: tableAlias));
        }

        /// <summary>
        /// 添加排序项
        /// </summary>
        /// <param name="column">排序列</param>
        /// <param name="desc">是否倒序</param>
        /// <param name="type">实体类型</param>
        /// <param name="tableAlias">表别名</param>
        protected void AddItem(string column, bool desc = false, Type type = null, string tableAlias = null)
        {
            if (column.IsEmpty())
            {
                return;
            }

            if (Exists(column, tableAlias))
            {
                return;
            }

            _items.Add(new OrderByItem(column, desc, type, prefix: tableAlias));
        }

        /// <summary>
        /// 是否已存在
        /// </summary>
        /// <param name="column">排序列</param>
        /// <param name="tableAlias">表别名</param>
        /// <returns></returns>
        protected bool Exists(string column, string tableAlias)
        {
            var item = new OrderByItem(column, prefix: tableAlias);
            return _items.Exists(t =>
                t.Column.ToLower() == item.Column.ToLower() &&
                (item.Prefix.IsEmpty() || t.Prefix?.ToLower() == item.Prefix?.ToLower()));
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="column">排序列</param>
        /// <param name="desc">是否倒序</param>
        public void OrderBy<TEntity>(Expression<Func<TEntity, object>> column, bool desc = false)
        {
            if (column == null)
            {
                return;
            }

            AddItem(_resolver.GetColumn(column), desc, typeof(TEntity));
        }

        /// <summary>
        /// 添加到OrderBy子句
        /// </summary>
        /// <param name="order">排序列表</param>
        public void AppendSql(string order)
        {
            _items.Add(new OrderByItem(order, raw: true));
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="pager">分页</param>
        public void Validate(IPager pager)
        {
            if (pager == null)
            {
                return;
            }

            if (_items.Count == 0)
            {
                throw new ArgumentException(LibraryResource.OrderIsEmptyForPage);
            }
        }

        /// <summary>
        /// 获取Sql
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            if (_items.Count == 0)
            {
                return null;
            }

            return $"Order By {_items.Select(t => t.ToSql(_dialect, _register)).Join()}";
        }
    }
}
