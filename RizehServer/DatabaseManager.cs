using Microsoft.CSharp.RuntimeBinder;
using Parsnet.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Parsnet
{


    public class DatabaseManager
    {

        public static ChangePasswordResult ChangePassword(int userId, string oldPass, string newPass)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@OldPassword", oldPass),
                new SqlParameter("@NewPassword", newPass)
            };
            int num = (int)DataProvider.GetValue(parameter, "sp_ChangePassword");
            if (num > 0)
            {
                return ChangePasswordResult.Success;
            }
            return ChangePasswordResult.InvalidOldPassword;
        }

        public static bool ConfirmUser(string email, string code)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@Email", email),
                new SqlParameter("@ConfirmCode", code)
            };
            return (((int)DataProvider.GetValue(parameter, "sp_ConfirmUser")) > 0);
        }

        public static bool DeleteSite(int siteId, bool delFromDatabase = false)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@DelFromDatabase", delFromDatabase)
            };
            return (((int)DataProvider.GetValue(parameter, "sp_DeleteSite")) > 0);
        }

        public static string GetConfirmCode(string email)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@Email", email)
            };
            return DataProvider.GetValue(parameter, "sp_GetConfirmCode").ToString();
        }

        public static DataTable GetPaymentsList(int page, int total)
        {
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@Page", page),
                new SqlParameter("@TotalRows", total)
            };
            return DataProvider.Export(param, "sp_GetPaymentsList");
        }

        public static int GetProductCost(int id)
        {
            SqlParameter parameter = new SqlParameter("@ProductId", id);
            return (int)DataProvider.GetValue(parameter, "sp_GetProductCost");
        }

        public static string GetProductsList()
        {
            DataTable table = DataProvider.Export("sp_GetProductList");
            StringWriter writer = new StringWriter();
            table.WriteXml(writer);
            return writer.ToString();
        }

        public static DataTable GetProductsListAdmin()
        {
            return DataProvider.Export("sp_GetProductListAdmin");
        }

        public static Sites GetRandomSite(int userId, int lastSiteId, string ip, DataTable dtSiteIds, out bool clearTable)
        {
            int runResult = 0;
            Sites sites = null;
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@LastSaitId", lastSiteId),
                new SqlParameter("@IP", ip),
                new SqlParameter("@ScoreValue", Settings.Default.ScoreStep)
            };
            SqlParameter item = new SqlParameter("@VisitedSiteIds", dtSiteIds)
            {
                SqlDbType = SqlDbType.Structured
            };
            param.Add(item);
            DataTable table = DataProvider.Export(param, "sp_GetRandomSite", out runResult);
            clearTable = runResult == 0;
            if (table.Rows.Count <= 0)
            {
                return sites;
            }
            DataRow row = table.Rows[0];
            return new Sites { SiteId = (int)Cast(row["SiteId"]), Url = (string)Cast(row["Url"]), OwnerId = (int)Cast(row["OwnerId"]) };
        }

        public static int GetSiteOwnerId(int siteId)
        {
            SqlParameter parameter = new SqlParameter("@SiteId", siteId);
            return (int)DataProvider.GetValue(parameter, "sp_GetSiteOwnerId");
        }

        public static DataTable GetSitesList(int page, int total, bool deleted)
        {
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@Page", page),
                new SqlParameter("@TotalRows", total),
                new SqlParameter("@OnlyDeleted", deleted)
            };
            return DataProvider.Export(param, "sp_GetSiteList");
        }

        public static Status GetStatus(int userId)
        {
            Status status = new Status();
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@UserId", userId)
            };
            DataTable table = DataProvider.Export(param, "sp_GetStatus");
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                status.Scores = (int)Cast(row["Scores"]);
                status.TotalSites = (int)Cast(row["TotalSites"]);
                status.TotalUsers = (int)Cast(row["TotalUsers"]);
            }
            return status;
        }

        public static int GetUserIdFromEmail(string email)
        {
            SqlParameter parameter = new SqlParameter("@Email", email);
            return (int)DataProvider.GetValue(parameter, "sp_GetUserIdFromEmail");
        }

        public static string GetUserPayments(int userId)
        {
            SqlParameter param = new SqlParameter("@UserId", userId);
            DataTable table = DataProvider.Export(param, "sp_GetUserPayments");
            StringWriter writer = new StringWriter();
            table.WriteXml(writer, XmlWriteMode.WriteSchema);
            return writer.ToString();
        }

        public static DataTable GetUsers(int page, int total)
        {
            BindingList<Users> list = new BindingList<Users>();
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@Page", page),
                new SqlParameter("@TotalRows", total)
            };
            DataTable table = DataProvider.Export(param, "sp_GetUsersList");
            return table;
        }

        public static bool UpdateUser(Users user)
        {
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@UserId", user.UserId),
                new SqlParameter("@IsApproved", user.IsApproved),
                new SqlParameter("@StateId", user.StateId),
                new SqlParameter("@ScoreStep", user.ScoreStep),
                new SqlParameter("@ConfirmCode", user.ConfirmCode),
                new SqlParameter("@ExpireDate", user.ExpireDate)
            };

            return DataProvider.Run(param, "sp_UpdateUser");
        }

        public static DataTable GetUserStateList()
        {
            DataTable table = DataProvider.Export("sp_GetUserStateList");
            return table;
        }

        public static DataTable GetProductTypeList()
        {
            DataTable table = DataProvider.Export("sp_GetProductTypeList");
            return table;
        }

        public static string GetUserWebsites(int userId)
        {
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@UserId", userId)
            };
            DataTable table = DataProvider.Export(param, "sp_GetUserWebsites");
            StringWriter writer = new StringWriter();
            table.WriteXml(writer);
            return writer.ToString();
        }

        public static bool InsertPayment(Payments pay)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@UserId", pay.UserId),
                new SqlParameter("@ProductId", pay.ProductId),
                new SqlParameter("@IdGet", pay.IdGet),
                new SqlParameter("@Status", pay.Status),
                new SqlParameter("@Amount", pay.Amount)
            };
            return (((int)DataProvider.GetValue(parameter, "sp_InsertPayment")) > 0);
        }

        public static Sites InsertSite(Sites site, out InsertSiteResult result)
        {
            Sites sites = null;
            result = InsertSiteResult.Error;
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@OwnerId", site.OwnerId),
                new SqlParameter("@Url", site.Url),
                new SqlParameter("@Topic", site.Topic),
                new SqlParameter("@Description", site.Description)
            };
            DataTable table = DataProvider.Export(param, "sp_InsertSite");
            if (table.Rows.Count <= 0)
            {
                return sites;
            }
            DataRow row = table.Rows[0];
            result = (InsertSiteResult)row["Result"];
            if (result != InsertSiteResult.Success)
            {
                return sites;
            }
            return new Sites { SiteId = (int)Cast(row["SiteId"]), OwnerId = (int)Cast(row["OwnerId"]), Url = (string)Cast(row["Url"]), Topic = (string)Cast(row["Topic"]), Description = (string)Cast(row["Description"]), RegisterDate = (DateTime)Cast(row["RegisterDate"]), IsActive = (bool)Cast(row["IsActive"]) };
        }

        public static LoginStatus Login(string email, string password, string ip, string systemId, out int userId)
        {
            LoginStatus validationError = LoginStatus.ValidationError;
            List<SqlParameter> param = new List<SqlParameter> {
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", password),
                new SqlParameter("@SystemId", systemId),
                new SqlParameter("@IP", ip)
            };
            DataTable table = DataProvider.Export(param, "sp_Login");
            if (table.Rows.Count > 0)
            {
                DataRow row = table.Rows[0];
                userId = (int)Cast(row["UserId"]);
                return (LoginStatus)row["Result"];
            }
            userId = 0;
            return validationError;
        }

        public static int Signup(string email, string password)
        {
            int num = -1;
            string str = RandomPassword.Create(6, true);
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@Email", email),
                new SqlParameter("@Password", password),
                new SqlParameter("@StateId", 1),
                new SqlParameter("@ConfirmCode", str)
            };
            num = (int)DataProvider.GetValue(parameter, "sp_InsertUser");
            if (num > 0)
            {
                EmailManager.Send(email, "کد تایید ایمیل", str);
            }
            return num;
        }

        public static bool UpdateSite(int siteId, string desc, string topic, bool? isActive)
        {
            List<SqlParameter> parameter = new List<SqlParameter> {
                new SqlParameter("@SiteId", siteId),
                new SqlParameter("@Topic", topic),
                new SqlParameter("@Description", desc),
                new SqlParameter("@IsActive", isActive)
            };
            return (((int)DataProvider.GetValue(parameter, "sp_UpdateSite")) > 0);
        }

        public static bool UpdateSiteByAdmin(Sites site)
        {
            var param = new List<SqlParameter> 
            {
                new SqlParameter("@SiteId", site.SiteId),
                new SqlParameter("@Topic", site.Topic),
                new SqlParameter("@Description", site.Description),
                new SqlParameter("@Url", site.Url),
                new SqlParameter("@IsBlocked", site.IsBlocked),
                new SqlParameter("@IsActive", site.IsActive)
            };
            return DataProvider.Run(param, "sp_UpdateSiteByAdmin");
        }



        public static dynamic Cast(object obj)
        {
            if (obj == null || obj is DBNull)
            {
                return null;
            }

            Type objType = obj.GetType();
            return Cast(obj, objType);
        }

        public static dynamic Cast(dynamic obj, Type castTo)
        {
            if (castTo == null)
            {
                throw new ArgumentNullException("conversionType");
            }
            if (castTo.IsGenericType && castTo.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (obj == null || obj is DBNull)
                {
                    return null;
                }
                NullableConverter nullableConverter = new NullableConverter(castTo);
                castTo = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(obj, castTo);
        }


    }
}

