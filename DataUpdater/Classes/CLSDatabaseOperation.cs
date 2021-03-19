using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace DataUpdater.Classes
{
    /// <summary>
    /// -- ვერსია 0.1 -- 24 Sep 2015 17:30 ნიკა
    /// </summary>

    public static class SQL
    {
        public static DataRow FillParams(string userName)
        {
            DataRow dr;
            using (var dt = new DataTable())
            {
                try
                {
                    GlobalParameters.cn.Open();
                    var txtComm = string.Format("SELECT * FROM Params WHERE UN = '{0}'", userName);
                    new SqlDataAdapter(new SqlCommand(txtComm, GlobalParameters.cn)).Fill(dt);
                }
                catch (Exception e)
                {
                    WriteLog(e);
                    return null;
                }
                finally
                {
                    GlobalParameters.cn.Close();
                }
                if (dt.Rows.Count != 1)
                {
                    return null;
                }
                dr = dt.Rows[0];
            }

            return dr;
        }

        public static DataTable Select(string txtComm)
        {
            using (var dt = new DataTable())
            {
                try
                {
                    GlobalParameters.cn.Open();
                    new SqlDataAdapter(new SqlCommand(txtComm, GlobalParameters.cn)).Fill(dt);
                }
                catch (Exception e)
                {
                    WriteLog(e);
                }
                finally
                {
                    GlobalParameters.cn.Close();
                }
                return dt;
            }
        }

        public static DataRow GetTableRow(string txtComm)
        {
            using (var dt = Select(txtComm))
            {
                if (dt.Rows.Count == 1)
                    return dt.Rows[0];
            }

            return null;
        }

        public static string ExecuteScalar(string txtComm)
        {
            try
            {
                GlobalParameters.cn.Open();
                var comm = new SqlCommand(txtComm, GlobalParameters.cn);
                return (comm.ExecuteScalar()).ToString();
            }
            finally
            {
                GlobalParameters.cn.Close();
            }
        }

        public static DataTable Select(string tableName, string whereClause)
        {
            return Select(string.Format("SELECT * FROM {0} {1} ", tableName, whereClause));
        }

        public static void Delete(string txtComm)
        {
            Execute(txtComm);
        }

        public static DataTable Delete(string tableName, string whereClause, string strSelectCommand)
        {
            Execute(string.Format("DELETE FROM {0} {1}", tableName, whereClause));
            return Select(strSelectCommand);
        }

        public static DataTable Delete(string tableName, string whereClause)
        {
            Execute(string.Format("DELETE FROM {0} {1}", tableName, whereClause));
            return Select(tableName, "");
        }

        public static void Update(string txtComm, bool createLog)
        {
            if (createLog)
                Update(txtComm);
            else
                Execute(txtComm);
        }

        public static void Update(string txtComm)
        {
            if (!GlobalParameters.CreateLog)
            {
                Execute(txtComm);
                return;
            }
            string tableName;

            Execute(txtComm);
        }

        public static string[] GenerateUpdateIdArray(string txtComm, out string tableName)
        {
            var txtCommUpper = txtComm.ToUpper();
            var posOfSet =
                Convert.ToInt32(
                    txtCommUpper.IndexOf(" SET ", StringComparison.Ordinal).ToString(CultureInfo.InvariantCulture)) - 1;
            var posOfUpdate =
                Convert.ToInt32(
                    txtCommUpper.IndexOf("UPDATE", StringComparison.Ordinal).ToString(CultureInfo.InvariantCulture));
            var posOfFrom =
                Convert.ToInt32(
                    txtCommUpper.IndexOf("FROM", StringComparison.Ordinal).ToString(CultureInfo.InvariantCulture));
            var posOfWhere =
                Convert.ToInt32(
                    txtCommUpper.IndexOf("WHERE", StringComparison.Ordinal).ToString(CultureInfo.InvariantCulture));
            tableName = txtComm.Substring(posOfUpdate + 7, posOfSet - posOfUpdate - 6).Trim();
            var fieldName = tableName + ".Id" + tableName;
            var from = posOfFrom > 0 ? " " + txtComm.Substring(posOfFrom) : " FROM " + tableName + " ";
            var where = posOfFrom > 0 ? "" : posOfWhere > 0 ? " " + txtComm.Substring(posOfWhere) : " ";

            var sql = "SELECT " + fieldName + from + where;
            var dt = Select(sql);
            var idArray = new string[dt.Rows.Count];
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                idArray[i] = dt.Rows[i][0].ToString();
            }
            return idArray;
        }

        public static void Update(string tableName, string id, string txtValue)
        {
            Update(string.Format("UPDATE {0} SET {0} = N'{1}' WHERE Id{0} = {2}", tableName, txtValue, id));
        }

        public static int Insert(string txtComm)
        {
            if (txtComm.Contains("SCOPE_IDENTITY"))
                return Convert.ToInt32(ExecuteScalar(txtComm));
            Execute(txtComm);
            return -1;
        }

        public static int Insert(string tableName, string txtValue)
        {
            return Insert(string.Format("INSERT INTO {0} VALUES (N'{1}');SELECT SCOPE_IDENTITY()", tableName, txtValue));
        }

        public static void Execute(string txtComm)
        {
            if (txtComm == "") return;
            try
            {
                GlobalParameters.cn.Open();
                var comm = new SqlCommand(txtComm, GlobalParameters.cn);
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
            finally
            {
                GlobalParameters.cn.Close();
            }
        }

        public static void InsertVarBinary(string txtComm, string param, string fileName)
        {
            try
            {
                GlobalParameters.cn.Open();
                var comm = new SqlCommand(txtComm, GlobalParameters.cn);
                var br = new BinaryReader(new FileStream(fileName, FileMode.Open));
                comm.Parameters.Add(param, SqlDbType.VarBinary).Value = br.ReadBytes((int)br.BaseStream.Length);
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
            finally
            {
                GlobalParameters.cn.Close();
            }
        }

        public static void InsertVarBinary(string txtComm, string param, byte[] array)
        {
            try
            {
                GlobalParameters.cn.Open();
                var comm = new SqlCommand(txtComm, GlobalParameters.cn);
                comm.Parameters.Add(param, SqlDbType.VarBinary).Value = array;
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
            finally
            {
                GlobalParameters.cn.Close();
            }
        }

        private static void WriteLog(Exception ex)
        {
            CLSErrorLog.WriteLog(ex);
        }
    }
}