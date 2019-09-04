using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Mitbg.Plugin.Misc.VendorsCore.Data;
using Nop.Core;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Plugin.Shipping.Speedy.Data
{
    public class VendorsCoreObjectContext : DbContext, IDbContext
    {
        #region Ctor

        public VendorsCoreObjectContext(DbContextOptions<VendorsCoreObjectContext> options) : base(options)
        {
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Further configuration the model
        /// </summary>
        /// <param name="modelBuilder">Model muilder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new VendorComissionMap());
            modelBuilder.ApplyConfiguration(new ShipmentTaskMap());
            base.OnModelCreating(modelBuilder);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a DbSet that can be used to query and save instances of entity
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>A set for the given entity type</returns>
        public virtual new DbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
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

        /// <summary>
        /// Install object context
        /// </summary>
        public void Install()
        {
            //create tables
            this.ExecuteSqlScript(this.GenerateCreateScript());
        }

        /// <summary>
        /// Uninstall object context
        /// </summary>
        public void Uninstall()
        {
            //drop the table
            this.DropPluginTable("Mitbg_ShipmentTasks");
            this.DropPluginTable("Mitbg_VendorComissions");
        }

        #endregion
    }
}
