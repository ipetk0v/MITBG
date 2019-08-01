using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Security;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.VendorPercentage
{
    public partial class Permission : IPermissionProvider
    {

        public static readonly PermissionRecord VendorComission = new PermissionRecord { Name = "Admin area. Vendor comissions", SystemName = "VendorComissions", Category = "Orders" };

        public IEnumerable<PermissionRecord> GetPermissions()
        {
            return new[]
            {
                VendorComission
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
                        VendorComission
                    }
                }
            };
        }
    }
}
