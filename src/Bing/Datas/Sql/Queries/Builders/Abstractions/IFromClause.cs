﻿namespace Bing.Datas.Sql.Queries.Builders.Abstractions
{
    /// <summary>
    /// From子句
    /// </summary>
    public interface IFromClause
    {
        /// <summary>
        /// 克隆
        /// </summary>
        /// <param name="register">实体别名注册器</param>
        /// <returns></returns>
        IFromClause Clone(IEntityAliasRegister register);

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="alias">别名</param>
        void From(string table, string alias = null);

        /// <summary>
        /// 设置表名
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="schema">架构名</param>
        void From<TEntity>(string alias = null, string schema = null) where TEntity : class;

        /// <summary>
        /// 添加到From子句
        /// </summary>
        /// <param name="sql">Sql语句</param>
        void AppendSql(string sql);

        /// <summary>
        /// 验证
        /// </summary>
        void Validate();

        /// <summary>
        /// 输出Sql
        /// </summary>
        /// <returns></returns>
        string ToSql();        
    }
}
