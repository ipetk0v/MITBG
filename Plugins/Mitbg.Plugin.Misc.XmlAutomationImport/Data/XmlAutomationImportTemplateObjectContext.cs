using Microsoft.EntityFrameworkCore;
using Nop.Core;
using Nop.Data;
using Nop.Data.Extensions;
using System;
using System.Linq;

namespace Mitbg.Plugin.Misc.XmlAutomationImport.Data
{
    public class XmlAutomationImportTemplateObjectContext : DbContext, IDbContext
    {
        #region Ctor

        public XmlAutomationImportTemplateObjectContext(DbContextOptions<XmlAutomationImportTemplateObjectContext> options) : base(options)
        {
        }

        #endregion

        #region Utilities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new XmlAutomationImportTemplateMap());
            base.OnModelCreating(modelBuilder);
        }
        #endregion

        #region Methods
        public new virtual DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        public string GenerateCreateScript()
        {
            return this.Database.GenerateCreateScript();
        }

        public IQueryable<TQuery> QueryFromSql<TQuery>(string sql, params object[] parameters) where TQuery : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public int ExecuteSqlCommand(RawSqlString sql, bool doNotEnsureTransaction = false, int? timeout = null,
            params object[] parameters)
        {
            using (var transaction = this.Database.BeginTransaction())
            {
                var result = this.Database.ExecuteSqlCommand(sql, parameters);
                transaction.Commit();

                return result;
            }
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            throw new NotImplementedException();
        }

        public void Install()
        {
            //create tables
            this.ExecuteSqlScript(this.GenerateCreateScript());
        }

        public void Uninstall()
        {
            //drop the table
            this.DropPluginTable("Mitbg_XmlAutomationImportTemplate");
        }

        #endregion
    }
}
