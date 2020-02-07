using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;
using System.Collections.Generic;

namespace Mitbg.Plugin.Misc.XmlAutomationImport
{
    public class Permission : IPermissionProvider
    {
        public static readonly PermissionRecord XmlAutomationImport = new PermissionRecord { Name = "Admin area. Xml Automation Import", SystemName = "XmlAutomationImport", Category = "Products" };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                XmlAutomationImport
            };
        }

        public IEnumerable<DefaultPermissionRecord> GetDefaultPermissions()
        {
            return new[]
            {
                new DefaultPermissionRecord
                {
                    CustomerRoleSystemName = NopCustomerDefaults.AdministratorsRoleName,
                    PermissionRecords = new[]
                    {
                        XmlAutomationImport
                    }
                }
            };
        }
    }
}
