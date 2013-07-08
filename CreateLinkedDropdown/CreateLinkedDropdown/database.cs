using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Pageflex.Interfaces.Storefront;
using PageflexServices;

namespace CreateLinkedDropdown
{
    public class database
    {
        private string sConn = "";
        private string dbQuery = "";
        private string tableName = "";
        private DataTable dt = null;

        public database(string sC, string tabName)
        {
            sConn = sC;
            tableName = tabName;
        }

        public void retrieveData(string query)
        {
            dbQuery = query;
            try
            {
                SqlConnection conn = new SqlConnection(sConn);
                SqlCommand command = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                dt = new DataTable();
                conn.Open();
                da.Fill(dt);
                command.Dispose();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void updateRow(string myType, string myName, string myClass, int tempId) {
            int i = findIdxByTempId(tempId);
            
            if ((i != -1)){
                dt.Rows[i]["myType"] = myType;
                dt.Rows[i]["myName"] = myName;
                dt.Rows[i]["myClass"] = myClass;
                updateDataBase();
            }
        }

        public void deleteRow(int tempId) {
            int i = findIdxByTempId(tempId);
            if (i != -1) {
                dt.Rows[i].Delete();
                updateDataBase();
            }
        }

        public void addRow(string myType, string myName, string myClass) {
            DataRow newRow = dt.NewRow();
            newRow["myType"] = myType;
            newRow["myName"] = myName;
            newRow["myClass"] = myClass;
            dt.Rows.Add(newRow);
            
        }

        public void updateDataBase() {
            SqlConnection conn = new SqlConnection(sConn);
            SqlDataAdapter da = new SqlDataAdapter(dbQuery, conn);
            SqlCommandBuilder cb= new SqlCommandBuilder(da);
            conn.Open();
            da.Update(dt);
            conn.Close();

        }

        public int findIdxByTempId(int tempId) {
            int i = 0;
            bool found = false;
            for (i = 0; i < dt.Rows.Count && !found; i++)
            {
                int myInt = dt.Rows[i].Field<int>("TempID");
                if (myInt == tempId)
                {
                    return i;
                }
            }
            return -1;
        }

        public DataTable getDataTable()
        {
            return dt;
        }
    }
}
