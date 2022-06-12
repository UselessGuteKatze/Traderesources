using AuthorizeRegistrationPlugin.Menus;
using Microsoft.Extensions.DependencyInjection;
using PostgreSQLQueryExecuterEngine;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UsersResources;
using UsersResources.QueryTables;
using Yoda.Application;
using Yoda.Application.Queries;
using Yoda.Interfaces;
using Yoda.Interfaces.Jobs;
using Yoda.Interfaces.Menu;
using YodaApp.UsersResources.QueryTables;
using YodaApp.UsersResources.QueryTables.V2;
using YodaApp.Yoda.Application.UserHelpers;
using YodaCorePlugin.AuthorizeRegistration.Menus;
using YodaQuery;

namespace TradeResourcesPlugin.Helpers {
    public class MnuMigrateUserRoles : FrmMenu {

        public MnuMigrateUserRoles(string moduleName) : base(nameof(MnuMigrateUserRoles), "Миграция ролей пользователей")
        {
            MenuType(Yoda.Interfaces.Menu.MenuType.Normal);
            Access();
            Enabled((rc) => {
                return true;
            });
            OnRendering(re => {

                var tbOldUsers = new TbUsers() { DbKey = "dbOldUsers" };
                var tbOldUserRoles = new TbUserRoles() { DbKey = "dbOldUsers", Name = "z_history_tbuserroles" };
                tbOldUserRoles.AddFilter("flishidden", false);
                var tbOldRoles = new TbRoles() { DbKey = "dbOldUsers" };

                var tbNewUsers = new TbUsers() { DbKey = "dbNewUsers" };
                var tbNewUserRoles = new TbUserRoles() { DbKey = "dbNewUsers" };
                var tbNewRoles = new TbRoles() { DbKey = "dbNewUsers" };

                var oldRolesDict = tbOldRoles.Select(new FieldAlias[] { tbOldRoles.flRoleId, tbOldRoles.flRoleName }, re.QueryExecuter)
                .AsEnumerable().ToDictionary(r => tbOldRoles.flRoleId.GetRowVal(r), r => tbOldRoles.flRoleName.GetRowVal(r));
                var oldUsersDict = tbOldUsers.Select(new FieldAlias[] { tbOldUsers.flUserId, tbOldUsers.flLogin }, re.QueryExecuter)
                .AsEnumerable().ToDictionary(r => tbOldUsers.flUserId.GetRowVal(r), r => tbOldUsers.flLogin.GetRowVal(r));

                var newRolesDict = tbNewRoles.Select(new FieldAlias[] { tbNewRoles.flRoleId, tbNewRoles.flRoleName }, re.QueryExecuter)
                .AsEnumerable().ToDictionary(r => tbNewRoles.flRoleName.GetRowVal(r), r => tbNewRoles.flRoleId.GetRowVal(r));
                var newUsersDict = tbNewUsers.Select(new FieldAlias[] { tbNewUsers.flUserId, tbNewUsers.flLogin }, re.QueryExecuter)
                .AsEnumerable().ToDictionary(r => tbNewUsers.flLogin.GetRowVal(r), r => tbNewUsers.flUserId.GetRowVal(r));

                var oldUserRoles = tbOldUserRoles.Select(new FieldAlias[] { tbOldUserRoles.flUserId, tbOldUserRoles.flRoleId }, re.QueryExecuter)
                .AsEnumerable().Select(r => new UserRole { UserId = tbOldUserRoles.flUserId.GetRowVal(r), RoleId = tbOldUserRoles.flRoleId.GetRowVal(r) }).Distinct();
                var newUserRoles = tbNewUserRoles.Select(new FieldAlias[] { tbNewUserRoles.flUserId, tbNewUserRoles.flRoleId }, re.QueryExecuter)
                .AsEnumerable().Select(r => new UserRole { UserId = tbNewUserRoles.flUserId.GetRowVal(r), RoleId = tbNewUserRoles.flRoleId.GetRowVal(r) });

                var batchQuery = new BatchQuery();

                oldUserRoles.Each(x => {

                    if (newUsersDict.ContainsKey(oldUsersDict[x.UserId]) && newRolesDict.ContainsKey(oldRolesDict[x.RoleId]))
                    {
                        x.UserId = newUsersDict[oldUsersDict[x.UserId]];
                        x.RoleId = newRolesDict[oldRolesDict[x.RoleId]];

                        if (!newUserRoles.Any(n => n.UserId == x.UserId && n.RoleId == x.RoleId))
                        {
                            batchQuery.Add(
                                tbNewUserRoles
                                .Insert()
                                .Set(t => t.flUserId, x.UserId)
                                .Set(t => t.flRoleId, x.RoleId)
                                .Query()
                            );
                        }

                    }

                });

                batchQuery.Execute(re.QueryExecuter);

            });
        }

        public class UserRole {
            public int UserId { get; set; }
            public int RoleId { get; set; }
        }

    }


}

