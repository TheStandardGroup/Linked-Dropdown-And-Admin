using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using Pageflex.Interfaces.Storefront;
using PageflexServices;
using System.Web;
using System.Web.UI;

namespace CreateLinkedDropdown
{
    public class Class1: SXIExtension
    {
        //the unique name for pagefles
        public override string UniqueName
        {
            get
            {
                return "CreateLinkedDropdown.standardgroup.com";
            }
        }
       //the name displayed by the extension
        public override string DisplayName
        {
            get
            {
                return "TSG: Create Linked Dropdown";
            }
        }

        //the configuration page
        public override int GetConfigurationHtml(KeyValuePair[] parameters, out string HTML_configString)
        {
            string serve = base.Storefront.GetValue("ModuleField", "ExtensionServer", "CreateLinkedDropdown.standardgroup.com");
            string db = base.Storefront.GetValue("ModuleField", "ExtensionDatabase", "CreateLinkedDropdown.standardgroup.com");
            string table = base.Storefront.GetValue("ModuleField", "ExtensionTable", "CreateLinkedDropdown.standardgroup.com");
            string prods = base.Storefront.GetValue("ModuleField", "ExtensionProds", "CreateLinkedDropdown.standardgroup.com");
            string first = "<br><br><strong>Extension Configuration:</strong><br><br><table><tr><td>Server Name:</td><td><input type='text' size='10' name='serv' value='" + serve + "'>";
            string second = "</td></tr><tr><td>Database Name:</td><td><input type='text' size='10' name='data' value='" + db + "'>";
            string third = "</td></tr><tr><td>Table Name:</td><td><input type='text' size='10' name='tab' value='" + table + "'>";
            string fourth = "</td></tr><tr><td>Product Names(Comma Seperated):</td><td><input type='text' size='10' name='prods' value='" + prods + "'>";
            string end = "</td></tr><tr><td><br><p>You must have  a SampleLabel_Series, a SampleLabel_Name, and a Division text field in the products you want to use this extension.</p>";
            end += "<p>The user must also have a profile field that is named UserProfileDivision that is used to Autofill the Division field in Form Filling.</p></td></tr></table>";
            HTML_configString = null;
            if (parameters == null)
            {
                HTML_configString = first + second + third + fourth + end;
            }
            else
            {
                bool isServeBad = false;
                bool isDbBad = false;
                bool isTableBad = false;
                bool isProdsBad = false;
                foreach (KeyValuePair pair in parameters)
                {
                    if (pair.Name.Equals("serv") && (pair.Value.Length == 0))
                    {
                        isServeBad = true;
                        //HTML_configString = first + "<font color=red><strong>Required.</strong></font>";
                    }
                    if (pair.Name.Equals("serv") && (pair.Value.Length != 0))
                    {
                        base.Storefront.SetValue("ModuleField", "ExtensionServer", "CreateLinkedDropdown.standardgroup.com", pair.Value);
                    }
                    if (pair.Name.Equals("data") && (pair.Value.Length == 0))
                    {
                        isDbBad = true;
                        //HTML_configString = second + "<font color=red><strong>Required.</strong></font>";
                    }
                    if (pair.Name.Equals("data") && (pair.Value.Length != 0))
                    {
                        base.Storefront.SetValue("ModuleField", "ExtensionDatabase", "CreateLinkedDropdown.standardgroup.com", pair.Value);
                    }
                    if (pair.Name.Equals("tab") && (pair.Value.Length == 0))
                    {
                        isTableBad = true;
                        //HTML_configString = second + "<font color=red><strong>Required.</strong></font>";
                    }
                    if (pair.Name.Equals("tab") && (pair.Value.Length != 0))
                    {
                        base.Storefront.SetValue("ModuleField", "ExtensionTable", "CreateLinkedDropdown.standardgroup.com", pair.Value);
                    }
                    if (pair.Name.Equals("prods") && (pair.Value.Length == 0))
                    {
                        isProdsBad = true;
                        //HTML_configString = second + "<font color=red><strong>Required.</strong></font>";
                    }
                    if (pair.Name.Equals("prods") && (pair.Value.Length != 0))
                    {
                        base.Storefront.SetValue("ModuleField", "ExtensionProds", "CreateLinkedDropdown.standardgroup.com", pair.Value);
                    }
                }
                string firstReq = "";
                string secondReq = "";
                string thirdReq = "";
                string fourthReq = "";
                if (isServeBad)
                {
                    firstReq = "<font color=red><strong>Required.</strong></font>";
                }
                if (isDbBad)
                {
                    secondReq = "<font color=red><strong>Required.</strong></font>";
                }
                if (isTableBad)
                {
                    thirdReq = "<font color=red><strong>Required.</strong></font>";
                }
                if (isProdsBad)
                {
                    fourthReq = "<font color=red><strong>Required.</strong></font>";
                }
                if (isServeBad || isDbBad || isTableBad || isProdsBad)
                    HTML_configString = first + firstReq + second + secondReq + third + thirdReq + fourth + fourthReq + end;
            }
            return 0;
        }

        //what to do on page load
        public override int PageLoad(string pageBaseName, string eventName)
        {

            //if it is the form filling page
            if ((pageBaseName == "usereditformfilling_aspx") && (eventName == "Init"))
            {
                string iDoc = Storefront.GetValue("SystemProperty", "CurrentUserDocument", null);
                string iDocName = Storefront.GetValue("DocumentProperty", "ProductName", iDoc);
                string prods = Storefront.GetValue("ModuleField", "ExtensionProds", "CreateLinkedDropdown.standardgroup.com");
                //MessageBox.Show(iDocName + ":" + prods);
                string[] prodList = prods.Split(',');
                bool shouldDo = false;
                bool cont = true;
                //if the product name is in the list of products the dropdown list is active for
                for (int i = 0; i < prodList.Length && cont; i++) { 
                    if(iDocName.Equals(prodList[i])){
                        shouldDo = true;
                        cont = false;
                    }
                }
                string js= "";
                //adds the dropdown lists
                if (shouldDo){
                    Page page = HttpContext.Current.CurrentHandler as Page;
                    js = doWork(iDoc, iDocName);
                    page.ClientScript.RegisterStartupScript(this.GetType(), "Add_LinkedDropdown_Box", js);
                }

            }
            //if the page name contains admin
            if ((pageBaseName.Contains("admin")))
            {
                if(!pageBaseName.Equals("adminlogin_aspx")){
                    var page = HttpContext.Current.CurrentHandler as Page;  
                    string userId = Storefront.GetValue("SystemProperty", "LoggedOnUserID", null);
                    page.ClientScript.RegisterStartupScript(this.GetType(), "AdminJS", createAdminJava(userId));
                }
            }
            //if it is the base Update page
            if ((pageBaseName == "admintsgupdate_aspx"))
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                if (!url.Contains('?'))
                    HttpContext.Current.Response.Redirect("AdminTSGUpdateDivision.aspx");
                string[] findVars = url.Split('?');
                var page = HttpContext.Current.CurrentHandler as Page;
                string js = "";
                js += createMenuGroups();
                page.ClientScript.RegisterStartupScript(this.GetType(), "AdminTSGTest", js);
                //if there are variables attached to the URL
                if (findVars.Length > 0)
                {

                    string[] tandidx = findVars[1].Split('=');
                    string type = tandidx[0];

                    int i = -1;
                    //if the variable does not contain Add Update or Delete as a command
                    if (!type.Equals("add") && !type.Equals("Update") && !type.Equals("Delete"))
                    {
                        string idx = tandidx[1];
                        i = Convert.ToInt32(idx);
                        string morejs = buildUpdatePage(type, i);
                        page.ClientScript.RegisterStartupScript(this.GetType(), "AdminTSGUpdate", morejs);

                    }
                    //if the variable contains update
                    else if (type.Equals("Update"))
                    {
                        string[] myVals = tandidx[1].Split(',');
                        updateMyDatabase(myVals);
                        HttpContext.Current.Response.Redirect(findVars[0]);
                    }
                    //if the variable contains update
                    else if (type.Equals("Delete"))
                    {
                        int myVal = Convert.ToInt32(tandidx[1]);
                        deleteFromDatabase(myVal);
                        HttpContext.Current.Response.Redirect(findVars[0]);
                    }
                    //everything else
                    else
                    {
                        string[] myVals = tandidx[1].Split(',');
                        addRowToDatabase(myVals);
                        HttpContext.Current.Response.Redirect(findVars[0]);
                    }

                }
            } 
            //if it is the division update page
            if ((pageBaseName == "admintsgupdatedivision_aspx")) {
                var page = HttpContext.Current.CurrentHandler as Page;
                string js = buildTable("division",null);
                js += createMenuGroups();
                page.ClientScript.RegisterStartupScript(this.GetType(), "AdminTSGUpdateDivision", js);
            }
            //if it is the series update page
            if ((pageBaseName == "admintsgupdateseries_aspx"))
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                string[] findVars = url.Split('?');
                var page = HttpContext.Current.CurrentHandler as Page;
                string js = "";
                if (findVars.Length > 1)
                    js = buildTable("series", findVars[1]);
                else
                    js = buildTable("series", null);
                js += createMenuGroups();
                page.ClientScript.RegisterStartupScript(this.GetType(), "AdminTSGUpdateSeries", js);
            }
            //if it is the name update page
            if ((pageBaseName == "admintsgupdatename_aspx"))
            {
                string url = HttpContext.Current.Request.Url.AbsoluteUri;
                string[] findVars = url.Split('?');
                var page = HttpContext.Current.CurrentHandler as Page;
                string js = "";
                if (findVars.Length > 1)
                    js = buildTable("name", findVars[1]);
                else
                    js = buildTable("name", null);
                js += createMenuGroups();
                page.ClientScript.RegisterStartupScript(this.GetType(), "AdminTSGUpdateName", js);
            }
            return eSuccess;
        }

        //deletes an item from the database
        private void deleteFromDatabase(int tempId) {
            database mydb = getDataBase();
            mydb.deleteRow(tempId);
        }

        //updates an item in the database
        private void updateMyDatabase(string[] vals) {
            database mydb = getDataBase();
            string myType = null;
            string myName = null;
            string myClass = null;
            myType = vals[0];
            int idxOfRow = Convert.ToInt32(vals[1]);
            // the index of the first value
            int i = Convert.ToInt32(vals[2]) + 3;
            myName = "";
            for (int c = 3; c < i; c++)
            {
                myName += vals[c];
                if (c < i - 1)
                    myName += " ";
            }
            if (myType.Equals("series") || myType.Equals("name"))
            {
                myClass = "";
                int c = Convert.ToInt32(vals[i]) + i+1;
                for (int x = i+1; x < c; x++)
                {
                    myClass += vals[x];
                    if (x < c - 1)
                    {
                        myClass += " ";
                    }
                }
                if (myType.Equals("name"))
                {
                    myName = myName + "\\" + myClass;
                    myClass = "";
                    i = Convert.ToInt32(vals[c]) + c + 1;
                    for (int x = c + 1; x < i; x++)
                    {
                        myClass += vals[x];
                        if (x < i - 1)
                        {
                            myClass += " ";
                        }
                    }
                }
            }
            if(myType.Equals("division"))
                mydb.updateRow(myType, myName, myClass, idxOfRow);
            else
                mydb.updateRow(myType, myClass, myName, idxOfRow);
            
            
        }

        //adds a new row to the database
        private void addRowToDatabase(string [] vals){
            database mydb = getDataBase();
            string myType = null;
            string myName = null;
            string myClass = null;
            myType = vals[0];
            int i = Convert.ToInt32(vals[1])+2;
            myName = "";
            for (int c = 2; c < i; c++) {
                myName += vals[c];
                if (c < i-1)
                    myName += " ";
            }
            if(myType.Equals("series")||myType.Equals("name")){
                myClass = "";
                int c = Convert.ToInt32(vals[i])+i+1;
                for(int x=i+1;x<c;x++){
                    myClass+= vals[x];
                    if(x<c-1){
                        myClass+=" ";
                    }
                }
                if(myType.Equals("name")){
                    myName = myName +"\\"+myClass;
                    myClass = "";
                    i = Convert.ToInt32(vals[c]) + c+1;
                    for (int x = c+1; x < i; x++) {
                        myClass += vals[x];
                        if (x < i - 1)
                        {
                            myClass += " ";
                        }
                    }
                }
            }
            if(myType.Equals("division"))
                mydb.addRow(myType,myName,myClass);
            else
                mydb.addRow(myType, myClass, myName);
            mydb.updateDataBase();
        }
      
        //builds the update page
        private string buildUpdatePage(string type, int idx) {
            database mydb = getDataBase();
            DataTable myTable = mydb.getDataTable();
            DataRow mine = null;
            if (idx >= 1)
            {
                mine = myTable.Select("TempID = "+idx)[0];
            }
            string js = "<script type='text/javascript' src='jSINI.js'></script>";
            js += "<script type='text/javascript'>";
            js += "$(document).ready(function(){";
            js += "     var appendMe = '<table style=\"margin-left:auto;margin-right:auto;\">';";
            if (idx >= 1)
            {
                js += "     var idx = " + idx + ";";
                js += "     appendMe += '<tr><td colspan=\"2\" style = \"text-align: center;\"><h3>Update " + mine.Field<string>("myType").ToUpper() + "</h3><br><br></td></tr>';";
            }else
                js += "   appendMe += '<tr><td colspan=\"2\" style = \"text-align: center;\">Add" + type.ToUpper() + "<br><br></td></tr>';";

            js += buildBoxesForUpdate(idx, type, mine);          

            js += addLinksToUpdate(idx);
            
            js += "     appendMe += '</table>';";
            js += "     $('.UpdateTSGHead').after(appendMe);";
            js += "     showMenuItems();";
            js += "});";
            js += buildAddToDatabase(type);
            js += buildJSUpdateDatabase(type);
            js += buildJSDeleteFromDatabase();
            js += buildShowMenuItem();
            js += "</script>";
            return js;
        }

        //builds the boxes for the update page
        private string buildBoxesForUpdate(int idx, string type, DataRow mine) {
            string js = "";
            if (type.Equals("division"))
            {
                js += buildTextBox("myName", "plant", mine, idx, null);
            }
            else if (type.Equals("series"))
            {
                js += buildTextBox("myClass", "plant", mine, idx, null);
                js += buildTextBox("myName", "series", mine, idx, null);
            }
            else if (type.Equals("name"))
            {
                if (idx > 0)
                {
                    string[] myClass = mine.Field<string>("myClass").Split('\\');
                    string[] mc2 = new string[2];
                    for (int i = 0; i < myClass.Length; i++)
                    {
                        mc2[i] = myClass[i];
                    }
                    js += buildTextBox("myClass", "plant", mine, idx, mc2[0]);
                    js += buildTextBox("myName", "series", mine, idx, mc2[1]);
                    js += buildTextBox("myName", "name", mine, idx, null);
                }
                else
                {
                    js += buildTextBox("myClass", "plant", mine, idx, null);
                    js += buildTextBox("myName", "series", mine, idx, null);
                    js += buildTextBox("myName", "name", mine, idx, null);
                }

            }
            return js;
        }

        //adds links to update page
        private string addLinksToUpdate(int idx) {
            string js = "";
            if (idx >= 1)
            {
                js += " var aVar ='" + idx + "';";
                js += " appendMe += '<tr><td colspan=\"2\" style = \"text-align: center;\"><a id=\"checkMeCart\" class=\"siteButton\" href=\"javascript:UpdateDB('+aVar+')\">Update</a>";
                js += " <a id=\"checkMeCart\" class=\"siteButton\" href=\"javascript:deleteFromDB('+aVar+')\">Delete</a></td></tr>';";
            }
            else
                js += " appendMe += '<tr><td colspan=\"2\" style = \"text-align: center;\"><a id=\"checkMeCart\" class=\"siteButton\" href=\"javascript:addToDatabase();\">Add</a></td></tr>';";
            return js;
        }

        //builds javascript for the delete function
        private string buildJSDeleteFromDatabase() {
            string js = "function deleteFromDB(idx){";
            js += "     loc='AdminTSGUpdate.aspx';";
            js += "     loc += '?Delete='+idx;";
            js += "     document.location = loc;";
            js += "}";
            return js;
        }

        private string buildShowMenuItem() { 
            string js = "function showMenuItems(){";
            js += "     $('#DivMenuHeader').each(function(){$(this).show();});";
            js += "     $('#DivMenuItems').each(function(){$(this).show();});";
            js += "     $('#DivMenuItem').each(function(){$(this).show();});";
            js += "}";
            return js;
        }

        //when split by , after = the vars are [0]=type [1]=row# [2]=amount plant .... 
        // builds the javascript for the update function
        private string buildJSUpdateDatabase(string type) {
            string js = "function UpdateDB(idx){";

            js += "     loc='AdminTSGUpdate.aspx';";
            js += "     loc += '?Update=';";
            js += "     loc += '" + type + ",'+idx+',';";
            js += "     var myArr = $('#plant').val().split(' ');";
            //js += "     loc += $('#plant').val().replace(' ','%');";
            js += "      loc += myArr.length;";
            js += "     var i=0;";
            js += "     while(i<myArr.length){";
            js += "         loc += ','+myArr[i];";
            js += "         i++;";
            js += "     }";
            if (type.Equals("series") || type.Equals("name"))
            {
                js += "     var myArr = $('#series').val().split(' ');";
                //js += "     loc += $('#plant').val().replace(' ','%');";
                js += "      loc += ','+ myArr.length;";
                js += "     var i=0;";
                js += "     while(i<myArr.length){";
                js += "         loc += ','+myArr[i];";
                js += "         i++;";
                js += "     }";
            }

            if (type.Equals("name"))
            {
                js += "     var myArr = $('#name').val().split(' ');";
                //js += "     loc += $('#plant').val().replace(' ','%');";
                js += "      loc += ',' + myArr.length;";
                js += "     var i=0;";
                js += "     while(i<myArr.length){";
                js += "         loc += ','+myArr[i];";
                js += "         i++;";
                js += "     }";
            }
            //js += "     alert(loc);";
            js += "     loc += ',null';";
            js += "     document.location = loc;";


            js += "}";
            return js;
        }

        //builds the js for the add function
        private string buildAddToDatabase(string type) {
            string js = "function addToDatabase(){";
            
            js += "     loc='AdminTSGUpdate.aspx';";
            js += "     loc += '?add=';";
            js += "     loc += '" + type + ",';";
            js += "     var myArr = $('#plant').val().split(' ');";
            //js += "     loc += $('#plant').val().replace(' ','%');";
            js += "      loc += myArr.length;";
            js += "     var i=0;";
            js += "     while(i<myArr.length){";
            js += "         loc += ','+myArr[i];";
            js += "         i++;";
            js += "     }";
            if (type.Equals("series") || type.Equals("name")) {
                js += "     var myArr = $('#series').val().split(' ');";
                //js += "     loc += $('#plant').val().replace(' ','%');";
                js += "      loc += ','+ myArr.length;";
                js += "     var i=0;";
                js += "     while(i<myArr.length){";
                js += "         loc += ','+myArr[i];";
                js += "         i++;";
                js += "     }";
            }

            if (type.Equals("name"))
            {
                js += "     var myArr = $('#name').val().split(' ');";
                //js += "     loc += $('#plant').val().replace(' ','%');";
                js += "      loc += ',' + myArr.length;";
                js += "     var i=0;";
                js += "     while(i<myArr.length){";
                js += "         loc += ','+myArr[i];";
                js += "         i++;";
                js += "     }";
            }
            //js += "     alert(loc);";
            js += "     loc += ',null';";
            js += "     document.location = loc;";
            
            
            js += "}";
            return js;
        }

        //builds a text box
        private string buildTextBox(string type, string label, DataRow dr, int idx, string otherVals) {
            string js = "";
            js += "     appendMe += '<tr><td style=\"width: 30%;\">"+label.ToUpper()+"</td><td><input id = \""+label+"\" type=\"text\" name=\""+label+"\"';";
            if (idx >= 1) { 
                if(otherVals == null)
                    js += " appendMe += 'value=\"" + dr.Field<string>(type) + "\"></td></tr>';";
                else
                    js += " appendMe += 'value=\"" + otherVals + "\"></td></tr>';";
            }
            else
                js += " appendMe += '></td></tr>';";
            return js;
        }

        //builds a table
        private string buildTable(string type, string filtType) {
            database mydb = getDataBase();
            DataTable myTable = mydb.getDataTable();
            string js = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js' type='text/javascript' charset='utf-8'></script>";
            js += "<script src=\"custom/jquery/jquery.chained.js\" type=\"text/javascript\" charset=\"utf-8\"></script>";
            js += "<script type='text/javascript' src='jSINI.js'></script>";
            js += "<style type=\"text/css\">";
            js += "    .trForUpdate{";
            js += "     cursor: hand; cursor: pointer;}";
            js += "    .trHeadRow{";
            js += "         background-color:#4343ff;";
            js += "     }";
            js += "    .trAltOne{";
            js += "         background-color:#7c7cff;";
            js += "     }";
            js += "    .trAltTwo{";
            js += "         background-color:#afafff;";
            js += "     }";
            js += "</style>";
            js += "<script type='text/javascript'>";
            js += "$(document).ready(function(){";;
            js += "     var myType = \"" + type + "\";";
            //js += "alert('" + type + "');";
            if(type.Equals("division")){
                js += "var appendMe = '<table border=\"1\" style=\"width: 400px; margin-left:auto;margin-right:auto;\"><tr class=\"trHeadRow\"><th>Plant Name</th></tr>';";
                DataRow[] drs = myTable.Select("myType = 'division'","myName ASC");
                bool altOne = true;
                foreach (DataRow dr in drs) {
                    js += "appendMe += '<tr id=\"" + dr.Field<string>("myType") + "=" + dr.Field<int>("tempID") + "\" class=\"trForUpdate ";
                    if (altOne)
                        js += "trAltOne";
                    else
                        js += "trAltTwo";
                    js += " \"><td style=\"padding-top:5px;\">" + dr.Field<string>("myName") + "</td></tr>';";
                    altOne = !altOne;
                }
                js += "appendMe += '</table>';";
                
            }
            else if(type.Equals("series")){
                bool [] show = {false,false};
                js += "var appendMe = '<table style=\" margin-left: auto; margin-right: auto;\"><tr><td colspan=\"2\" style=\"margin-left: auto; margin-right: auto;\">" + buildDropdownHTML(buildDivisionsList(myTable), null, null, "display:block", show) + "</td></tr>";
                js += "<tr><td colspan=\"2\" style=\"margin-left: auto; margin-right: auto;\"><a href=\"javascript:filterTable(\\''+myType+'\\')\">Filter</a></td></tr></table>';";
                js += "appendMe += '<table border=\"1\" style=\"width: 400px; margin-left:auto;margin-right:auto;\"><tr class=\"trHeadRow\"><th>Series</th><th>Plant</th></tr>';";
                //filtType = filtType.Replace(')', ' ');
                //js += "alert('"+filtType+"');";
                string filtString = "(myType = 'series')";
                if (filtType != null)
                {
                    filtType = filtType.Replace("%20", " ");
                    //js += "alert('" + filtType + "');";
                    filtString += " AND (myClass = '" + filtType + "')";
                }
                DataRow[] drs = myTable.Select(filtString,"myClass ASC");
                bool altOne = true;
                foreach (DataRow dr in drs)
                {
                    js += "appendMe += '<tr id=\"" + dr.Field<string>("myType") + "=" + dr.Field<int>("tempID") + "\" class=\"trForUpdate ";
                    if (altOne)
                        js += "trAltOne";
                    else
                        js += "trAltTwo";   
                    js += "\"><td style=\"padding-top:5px;\">" + dr.Field<string>("myName") + "</td><td style=\"padding-top:5px;\">" + dr.Field<string>("myClass") + "</td></tr>';";
                    altOne = !altOne;
                }
                js += "appendMe += '</table>';";
                //js += buildDropdownJS("'Select:'", false, show);
            }
            else if (type.Equals("name")) {
                bool[] show = { true, false };
                js += "var appendMe = '<table style=\" width: 400px; margin-left: auto; margin-right: auto;\"><tr><td colspan=\"3\" style=\"margin-left: auto; margin-right: auto;\">" + buildDropdownHTML(buildDivisionsList(myTable), buildDict(myTable, "series"), null, "display:block", show) + "</td>";
                js += "</tr><tr><td colspan=\"3\" style=\"margin-left: auto; margin-right: auto;\"><a href=\"javascript:filterTable(\\''+myType+'\\')\">Filter</a></td></tr></table>';";
                js += "appendMe += '<table border=\"1\" style=\"width: 400px; margin-left:auto;margin-right:auto;\"><tr class=\"trHeadRow\"><th>Name</th><th>Series</th><th>Plant</th></tr>';";
                string filtString = "(myType = 'name')";
                if (filtType != null)
                {
                    filtType = filtType.Replace("%20", " ");
                    filtType = filtType.Replace("%5C", "\\");
                    //js += " alert('" + filtType + "');";
                    filtString += " AND (myClass = '" + filtType + "')";
                }
                DataRow[] drs = myTable.Select(filtString,"myClass ASC");
                int count = drs.Length;
                //js += "alert('" + count + "');";
                bool altOne = true;
                foreach (DataRow dr in drs)
                {
                    string myClass = dr.Field<string>("myClass");
                    string[] vars = myClass.Split('\\');
                    string [] myVars = new string[2];
                    for (int i = 0; i < vars.Length; i++)
                        myVars[i] = vars[i];
                    js += "appendMe += '<tr id=\"" + dr.Field<string>("myType") + "=" + dr.Field<int>("tempID") + "\" class=\"trForUpdate ";
                    if (altOne)
                        js += "trAltOne";
                    else
                        js += "trAltTwo"; 
                    js += "\"><td style=\"padding-top:5px;\">" + dr.Field<string>("myName").Replace("'","") + "</td><td style=\"padding-top:5px;\">" + myVars[1] + "</td><td style=\"padding-top:5px;\">" + myVars[0] + "</td></tr>';";
                    altOne = !altOne;
                }
                js += "appendMe += '</table>';";
                
                //js += "alert('in name');";
            }
            js += "appendMe+= '<a href=\"AdminTSGUpdate.aspx?" + type + "=-1\">Add "+type+"</a>';";
            js += "$('.UpdateTSGHead').after(appendMe);";
            js += "$('.trForUpdate').click(function(){";
            js += "     document.location = 'AdminTSGUpdate.aspx' + \"?\" + $(this).attr('id');";
            js += "});";
            js += "});";
            bool [] myshow = {true,true};
            js += buildDropdownJS("'Select:'", false, myshow);
            js += buildTableFilter();
            js += "</script>";
            return js;
        }

        //filters the table
        private string buildTableFilter(){
            string js = "function filterTable(aVar){";
            js += "     var divis = $('#division').val();";
            js += "     var ser = '';";
            js += "     var myC = divis;";
            js += "     var loc = 'AdminTSGUpdateSeries.aspx';";
            js += "     if(aVar == 'name'){";
            js += "         ser = $('#series').val();";
            js += "         myC += '\\\\'+ser;";
            js += "         loc = 'AdminTSGUpdateName.aspx';}";
            js += "     if(myC == '\\\\')";
            js += "         myC = '';";
            js += "     loc += '?'+myC;";
            js += "     document.location=loc;";
            js += "}";
            return js;
        }

        //gets the database
        private database getDataBase() {
            string server = Storefront.GetValue("ModuleField", "ExtensionServer", "CreateLinkedDropdown.standardgroup.com");
            string db = Storefront.GetValue("ModuleField", "ExtensionDatabase", "CreateLinkedDropdown.standardgroup.com");
            string table = Storefront.GetValue("ModuleField", "ExtensionTable", "CreateLinkedDropdown.standardgroup.com");
            string sConn = "Server=" + server + ";DataBase=" + db + "; Integrated Security=false; user id=PFSFAdmin; Pwd=T$Gsf2012";
            string query = String.Format("SELECT * FROM {0}", table);
            database myDb = new database(sConn, table);
            try
            {
                myDb.retrieveData(query);
                //Storefront.SetValue("VariableValue", "FirstName", iDoc, server);
            }
            catch (Exception e)
            {
                //Storefront.SetValue("VariableValue", "FirstName", iDoc, "Failed");
            }
            return myDb;
        }

        //creates the administrator javascript
        private string createAdminJava(string userId)
        {
            string js = "<script type='text/javascript' src='jSINI.js'></script>";
            
            js += "<script type = 'text/javascript'>";
            js += "Sys.WebForms.PageRequestManager.getInstance().add_endRequest(myJQueryRequestHandler);";
            js += "function myJQueryRequestHandler(sender,args){";
            js += "     showMenuItems();";
            js += "     createExtraMenu();";
            js += "     changeAdminColors();}";
            js += "$(document).ready(function(){";
            js += "     showMenuItems();";
            js += "     createExtraMenu();";
            js += "     changeAdminColors();});";
            js += "function createExtraMenu(){";
            js += "     var insertMe = '<div class=\"menuGroup\" id=\"lastMenuGroup\">';";
            js += "     insertMe += '<div id=\"AdminMaster_RepeaterMenu_ctl05_DivMenuHeader\" class=\"menuHeader\" menuheaderid=\"AdminDB\">Administer Series</div>';";
            js += "     insertMe += '<div id=\"AdminMaster_RepeaterMenu_ctl05_DivMenuItems\" class=\"menuItems\">';";
            js += "     insertMe += '<div id=\"AdminMaster_RepeaterMenu_ctl05_RepeaterMenuItems_ctl05_DivMenuItem\" class=\"menuItem\">';";
            js += "     insertMe += '<a href=\"AdminTSGUpdate.aspx\">Update Database</a></div></div></div>';";
            js += "     $('#AdminMaster_RepeaterMenu_ctl04_DivMenuHeader').parent().after(insertMe);";
            js += "     $('#AdminMenu_About').after(insertMe);";
            js += "}";
            js += changeAdminColorsJS();
            js += buildShowMenuItem();
            js += "</script>";
            return js;
        }

        //changes the skinn of the Admin functions
        private string changeAdminColorsJS() {
            
            string js = "function changeAdminColors(){";
            js += "     $('#siteHeader').parent().parent().next().children('td').css({'background-color':'#000000', 'border': '0px', 'background-image':'url(\"Custom/Images/imgHdrStruts.png\")'});";
            js += "     $('.AdminMenuMainTable2 tr').each(function(){";
            js += "           $(this).remove();";
            js += "     });";
            js += "}";
            return js;
        }

        //creates the menues on the side
        private string createMenuGroups(){
            string[] headers = { "Activity", "Users", "Content", "Purchasing", "Administration" };
            string[][] content = new string[5][];
            string[][] contentLink = new string[5][];
            content[0] = new string[8];
            content[1] = new string[6];
            content[2] = new string[5];
            content[3] = new string[3];
            content[4] = new string[13];

            contentLink[0] = new string[8];
            contentLink[1] = new string[6];
            contentLink[2] = new string[5];
            contentLink[3] = new string[3];
            contentLink[4] = new string[13];

            content[0][0] = "Orders";
            content[0][1] = "Approval Queue";
            content[0][2] = "Prep Queue";
            content[0][3] = "Production Queue";
            content[0][4] = "Shipping Queue";
            content[0][5] = "Finance";
            content[0][6] = "Ledger";
            content[0][7] = "Logs";

            content[1][0] = "User Accounts";
            content[1][1] = "User Groups";
            content[1][2] = "User Access";
            content[1][3] = "Profile Fields";
            content[1][4] = "Approvals";
            content[1][5] = "Address Books";

            content[2][0] = "Projects";
            content[2][1] = "Categories";
            content[2][2] = "Products";
            content[2][3] = "Global Library";
            content[2][4] = "Metadata Fields";

            content[3][0] = "Checkout";
            content[3][1] = "Price Tables";
            content[3][2] = "Tax Rates";

            content[4][0] = "Admin Accounts";
            content[4][1] = "Admin Groups";
            content[4][2] = "Admin Access";
            content[4][3] = "Notifications";
            content[4][4] = "Site Skinning";
            content[4][5] = "Site Options";
            content[4][6] = "Social Media / SEO";
            content[4][7] = "Extensions";
            content[4][8] = "Product Plugins";
            content[4][9] = "Scheduled Tasks";
            content[4][10] = "Database Tools";
            content[4][11] = "Overview";
            content[4][12] = "About";

            contentLink[0][0] = "AdminOrders.aspx";
            contentLink[0][1] = "AdminOrders.aspx?queue=approval";
            contentLink[0][2] = "AdminOrders.aspx?queue=prep";
            contentLink[0][3] = "AdminOrders.aspx?queue=production";
            contentLink[0][4] = "AdminOrders.aspx?queue=shipping";
            contentLink[0][5] = "AdminOrders.aspx?queue=finance";
            contentLink[0][6] = "AdminLedger.aspx";
            contentLink[0][7] = "AdminLogs.aspx";

            contentLink[1][0] = "AdminUserAccounts.aspx";
            contentLink[1][1] = "AdminManageGroups.aspx?g=User";
            contentLink[1][2] = "AdminUserPrivileges.aspx";
            contentLink[1][3] = "AdminOptionSet.aspx?ostype=UserProfile";
            contentLink[1][4] = "AdminReviews.aspx";
            contentLink[1][5] = "AdminAddressBooks.aspx";

            contentLink[2][0] = "AdminProjects.aspx";
            contentLink[2][1] = "AdminProductCategories.aspx";
            contentLink[2][2] = "AdminProducts.aspx";
            contentLink[2][3] = "AdminGlobalLibrary.aspx";
            contentLink[2][4] = "AdminOptionSet.aspx?ostype=ProductMetadata";

            contentLink[3][0] = "AdminOptionSet.aspx?ostype=ShippingStep";
            contentLink[3][1] = "AdminPriceTables.aspx";
            contentLink[3][2] = "AdminTaxTables.aspx";

            contentLink[4][0] = "AdminUserAccounts.aspx?g=Admin";
            contentLink[4][1] = "AdminManageGroups.aspx?g=Admin";
            contentLink[4][2] = "AdminAdminPrivileges.aspx";
            contentLink[4][3] = "AdminNotifications.aspx";
            contentLink[4][4] = "AdminSkinning.aspx";
            contentLink[4][5] = "AdminSiteOptions.aspx";
            contentLink[4][6] = "AdminSocialMedia.aspx";
            contentLink[4][7] = "AdminExtensions.aspx";
            contentLink[4][8] = "AdminProductPlugins.aspx";
            contentLink[4][9] = "AdminScheduledTasks.aspx";
            contentLink[4][10] = "AdminMenu_DatabaseTools";
            contentLink[4][11] = "AdminHelpOverview.aspx";
            contentLink[4][12] = "AdminAbout.aspx";
            string js = "<style = 'text/css'>";
            js += "body{background-color:#DADADA;}";
            js += "table .AdminMenuMainTable2{background-color: white;}";
            js += ".AdminMain{background-color: white; }";
            js += "td.AdminMain{padding-left:0px;height: 100%;}";
            js += ".UpdateTSGHead{width:100%;background-color:#DADADA; text-align:center;margin-top:0px;padding-top:0px;margin-bottom:10px;}";
            js += "</style>";
            js += "<script type='text/javascript'>$(document).ready(function(){";
            for (int i = 0; i < content.Length; i++) {
                js += createMenuGroupJS(headers[i], content[i], contentLink[i],i);
                js += "$('#lastMenuGroup').before(inMe);";
                js += "$('#AdminMaster_RepeaterMenu_" + i + "_DivMenuHeader').click(function(){";
                js += "$('.menuI"+i+"').each(function(){";
                js += "     if($(this).is(':hidden')){$(this).show();}else{$(this).hide();}});});";
            }
            js += "});</script>";
            return js;
        }

        //builds the javascript for the menus
        private string createMenuGroupJS(string header, string[]linkText, string [] link,int id) {

            string js = "var inMe = '<div class=\"menuGroup\">';";
            js += "     inMe += '<div id=\"AdminMaster_RepeaterMenu_"+id+"_DivMenuHeader\" class=\"menuHeader\" menuheaderid=\""+header+"\">"+header+"</div>';";
            js += "     inMe += '<div id=\"AdminMaster_RepeaterMenu_" + id + "_DivMenuItems\" class=\"menuItems menuI" + id + "\" style=\"display: none;\">';";
            for (int i = 0; i < linkText.Length; i++)
            {
                js += "     inMe += '<div id=\"AdminMaster_RepeaterMenu_" + id + "_RepeaterMenuItems_" + id + "_DivMenuItem\" class=\"menuItem\">';";
                js += "     inMe += '<a href=\""+link[i]+"\">"+linkText[i]+"</a></div>';";
            }
            js += "inMe += '</div></div>';";
            return js;
        }

        //does the work for the User page
        private string doWork(string iDoc,string iDocName) {
            database myDb = getDataBase();
            DataTable myTable = myDb.getDataTable();
            return createJavaScript(myTable);
        }

        //creates javascript for user page
        private string createJavaScript(DataTable dt) {
            ArrayList divisions = buildDivisionsList(dt);
            Dictionary<String, ArrayList> series = buildDict(dt, "series");
            Dictionary<String,ArrayList> names = buildDict(dt,"name");
            bool [] show = {true,true};
            string js = "<script src='https://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js' type='text/javascript' charset='utf-8'></script>";
            js += "<script src=\"custom/jquery/jquery.chained.js\" type=\"text/javascript\" charset=\"utf-8\"></script>";
            js += "<script type = 'text/javascript'>";
            js += "$(document).ready(function(){";
            js += "     $('#FIELD_'+ FieldIDs[\"Division\"]).after('" + buildDropdownHTML(divisions, series, names, "display:none",show) + "');";
            js += buildDropdownJS("$('#FIELD_' + FieldIDs[\"Division\"]).val()",true,show);
            
            js += "});";
            js += "</script>";
            return js;
        }

        //creates the javascript for the dropdown menues
        private string buildDropdownJS(string whereToGetVal, bool formfill, bool [] show) {
            StringBuilder js = new StringBuilder( "     $('#division').val("+whereToGetVal+");");
            if (formfill)
            {
                if (show[0])
                    js.AppendLine("     $('#series').chained('#division');");
                if (show[1])
                    js.AppendLine("     $('#name').chained('#division, #series');");
            }
            js.AppendLine("     $('#division').live('change',function() {");
            js.AppendLine("         var selectedDivision = $('#division').val();");
            if (formfill)
            {
                js.AppendLine("         PFSF_SetControlValue(PFSF_Find('FIELD_' + FieldIDs[\"Division\"]),selectedDivision );");
                js.AppendLine("         PFSF_AjaxUpdateForm();");
            }
            else
            {
                database myDb = getDataBase();
                DataTable myTable = myDb.getDataTable();
                Dictionary<String, ArrayList> series = buildDict(myTable, "series");
                
                js.AppendLine("$.ajax({");
                js.AppendLine("url: 'AdminTSGUpdateName.aspx',");
                //js.AppendLine("contentType: \"application/json; charset=utf-8\",");
                //js.AppendLine("dataType: 'json',");
                js.AppendLine("success: function(){");
                js.AppendLine("var selectedDivision = $('#division').val();");
                js.AppendLine("$('#series').empty();");
                foreach (KeyValuePair<String, ArrayList> pair in series)
                {
                    
                    js.AppendLine("if(selectedDivision == '" + pair.Key + "'){");
                    foreach (string str in pair.Value)
                    {
                        string check = str;
                        if (str.Contains("'"))
                            check = check.Replace("'", "\\'");
                        js.AppendLine("$('#series').append('<option value=\"" + check + "\" class=\"" + pair.Key.Replace("\\", "\\\\") + "\">" + check + "</option>');");
                    }
                    js.AppendLine("}");
                }
                js.AppendLine("},");
                js.AppendLine("error: function(){//alert('failed');");
                js.AppendLine("}});");
            }
            
            js.AppendLine( "     });");
            if (show[0])
            {
                js.AppendLine("     $('#series').change(function() {");
                js.AppendLine( "         var selectedSeries = $('#series').val();");
                if (formfill)
                    js.AppendLine( "         PFSF_SetControlValue(PFSF_Find('FIELD_' + FieldIDs[\"SampleLabel_Series\"]),selectedSeries);");
                

                js.AppendLine( "         PFSF_AjaxUpdateForm();");
                js.AppendLine( "     });");
            }
            if (show[1])
            {
                js.AppendLine("     $('#name').change(function() {");
                js.AppendLine("         var selectedName = $('#name').val();");
                if(formfill)
                    js.AppendLine("         PFSF_SetControlValue(PFSF_Find('FIELD_' + FieldIDs[\"SampleLabel_Name\"]),selectedName);");
                js.AppendLine("         PFSF_AjaxUpdateForm();");
                js.AppendLine("     });");
            }
            return js.ToString();
        }

        //creates the HTML for the dropdown boxes
        private string buildDropdownHTML(ArrayList div, Dictionary<String, ArrayList> series, Dictionary<String, ArrayList> names, string DivDisplay, bool[] show) {
            string html = "<div style=\"margin:10px 0px 5px 0px\">";
            html += "<label for=\"Division\" style=\""+DivDisplay+"\">Division:</label>";
            html += "<select id=\"division\" name=\"Division\" style=\"width:200px; margin-bottom:10px; "+DivDisplay+";\">";
            html += "<option value =\"\">Select:</option>";
            foreach (string str in div) {
                html += "<option value=\"" + str + "\">" + str + "</option>";
            }
            html += "</select>";
            if (show[0])
            {
                
                html += "<label for=\"Series\" style=\"display:block\">Series:</label>";
                html += "<select id=\"series\" name=\"Series\" style=\"width:200px; margin-bottom:10px;\">";
                html += buildDropDownOptionsFromDict(series);
            }
            if (show[1])
            {
                html += "<label for=\"Name\" style=\"display:block\">Name:</label>";
                html += "<select id=\"name\" name=\"Name\" style=\"width:200px; margin-bottom:10px;\">";
                html += buildDropDownOptionsFromDict(names);
                html += "</div>";
            }
            return html;
        }

        //builds options for dropdown from dictionary
        private string buildDropDownOptionsFromDict(Dictionary<String, ArrayList> dict) {
            string html = "";
            html += "<option value =\"\">Select:</option>";
            foreach(KeyValuePair<String,ArrayList> pair in dict){
                foreach (string str in pair.Value)
                {
                    string check = str;
                    if (str.Contains("'"))
                        check = check.Replace("'", "\\'");
                    html += "<option value=\"" + check + "\" class=\"" + pair.Key.Replace("\\", "\\\\") + "\">" + check + "</option>";
                }
            }
            html += "</select>";
            return html;
        }

        //builds an ArrayList
        private ArrayList buildDivisionsList(DataTable dt) {
            ArrayList divs = new ArrayList();
            DataRow[] drs = dt.Select("myType = 'division'");
            //add the values to the list
            foreach (DataRow dr in drs){
                divs.Add(dr.Field<string>("myName"));
            }

            return divs; 
        }

        //builds a Dictionary
        private Dictionary<String, ArrayList> buildDict(DataTable dt, string type) { 
            Dictionary<String,ArrayList> dict = new Dictionary<string,ArrayList>();
            DataRow[] drs = dt.Select("myType = '"+type+"'");
            foreach (DataRow dr in drs){
                string myClass = dr.Field<string>("myClass");
                string value = dr.Field<string>("myName");
                //if not in the dictionary create it
                if (!dict.ContainsKey(myClass)){
                    ArrayList al = new ArrayList();
                    al.Add(value);
                    dict.Add(myClass, al);
                }
                else {//if in the dict. add the value to the list
                    dict[myClass].Add(value);
                }
            }
            return dict;
        }
    }
}
