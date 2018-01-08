<%
/*==============================================================================================================
AejW.com - Web Browse
---------------------
Build:         0070
Author:        Adam ej Woods
Modified:      27/04/2004
Ownership:     Copyright (c)2004 Adam ej Woods
Source:        http://www.aejw.com/
EULA:          In no way can this class be disturbed without my permission, this means reposting on a
               web site, cdrom, or any other form of media. The code can be used for commercial or
               personal purposes, as long as credit is given to the author. The header (this information)
               can not be modified or removed. CodeProject.com has permission to distribute this file.
==============================================================================================================*/
%>
<%@ Page Language="C#" Debug="false" %>
<%@ Import Namespace="System.IO" %>

<script language="C#" runat="server">
    //Import Namespace="System.Math"
    string lsTitle;
    string lsLink;
    string lsScriptName;
    string lsWebPath;

    public void Page_Load()
    {
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(5));
        Response.Cache.SetCacheability(HttpCacheability.Public);
        lsTitle = Request.QueryString.Get("title");
        if (lsTitle == null || lsTitle == "") { lsTitle = "Web Browse"; }
    }

    private void RptErr(string psMessage)
    {
        Response.Write("<DIV align=\"left\" width=\"100%\"><B>Script Reported Error: </B>&nbsp;" + psMessage + "</DIV><BR>");
    }

    private string GetNavLink(string psHref, string psText)
    {
        return ("/<A class=\"tdheadA\" href=\"" + lsScriptName + "?path=" + psHref + "&title=" + lsTitle + "&link=" + lsLink + "\">" + psText + "</A>");
    }
</script>

<html>
<head>
    <title><%=lsTitle%></title>
    <style>
        BODY {font-family: tahoma; font-size: 9pt;}
        TABLE {border-collapse: collapse; border: 1px inset #606060;}
        TD { border: 1px solid #606060; font-family: tahoma; font-size: 9pt; }
        H1 { margin-bottom: 1px; font-family: vendara,tahoma; font-size: 12pt; color: #202020; }
        A { font-family: tahoma; font-size: 9pt; color: #3C6B96; }
        .tdDir { border: 0px; font-family: tahoma; font-size: 9pt; color: #202020; background-color: #EFEFEF; }
        .tdFile { border: 0px; font-family: tahoma; font-size: 9pt; color: #202020; background-color: #FAFAFA; }
        .tdhead { border: 0px; font-family: tahoma; font-size: 9pt; color: #ffffff; background-color: #3C6B96; }
        .tdheadA { border: 1px #3C6B96 solid; font-family: tahoma; font-size: 9pt; color: #eeeeee; background-color: #3C6B96; }
        .tdheadA:Hover { border: 1px #eeeeee solid; text-decoration: none; font-family: tahoma; font-size: 9pt; color: #eeeeee; background-color: #2879C3; }
    </style>
</head>
<body>
    <%
        try
        {
            //Variables used in script
            string sSubDir; 
            int i; int j;
            string sPrevLink = "";
            decimal iLen; string sLen;

            //Write header, get link param
            lsLink = Request.QueryString.Get("link");
            Response.Write("<CENTER><H1>" + lsTitle + "</H1>");
            if (lsLink != null && lsLink != "") { Response.Write("<A href=\"" + lsLink + "\">[&nbsp;Return&nbsp;]</A><BR>"); }

            //Work on path and ensure no back tracking
            sSubDir = Request.QueryString.Get("path");
            if (sSubDir == null || sSubDir == "") { sSubDir = "/"; }

            sSubDir = sSubDir.Replace("\\", ""); sSubDir = sSubDir.Replace("//", "/");
            sSubDir = sSubDir.Replace("..", "./"); sSubDir = sSubDir.Replace("/", "\\");

            //Clean path for processing and collect path varitations
            if (sSubDir.Substring(0, 1) != "\\") { sSubDir = "\\" + sSubDir; }
            if (sSubDir.Substring(sSubDir.Length - 1, 1) != "\\") { sSubDir = sSubDir + "\\"; }

            //Get name of the browser script file
            lsScriptName = Request.ServerVariables.Get("SCRIPT_NAME");
            j = lsScriptName.LastIndexOf("/");
            if (j > 0) { lsScriptName = lsScriptName.Substring(j + 1, lsScriptName.Length - (j + 1)).ToLower(); }

            //Create navigation string and other path strings
            sPrevLink += GetNavLink("", "root");
            if (sSubDir != "\\")
            {
                j = 0; i = 0;
                do
                {
                    i = sSubDir.IndexOf("\\", j + 1);
                    lsWebPath += sSubDir.Substring(j + 1, i - (j + 1)) + "/";
                    sPrevLink += GetNavLink(lsWebPath, sSubDir.Substring(j + 1, i - (j + 1)));
                    j = i;
                } while (i != sSubDir.Length - 1);
            }

            //Output header
            Response.Write("<BR><TABLE cellpadding=3 cellspacing=1 width=\"100%\"><TBODY>");
            Response.Write("<TR><TD colspan=4 class=\"tdhead\">&nbsp;Location: &nbsp;" + sPrevLink + "/</TD></TR>");
            Response.Write("<TR><TD width=\"100%\" class=\"tdhead\">&nbsp;Directory Name&nbsp;</TD><TD width=\"40px\" class=\"tdhead\">&nbsp;Type&nbsp;</TD><TD class=\"tdhead\" width=\"140px\" nowrap>&nbsp;Last Write&nbsp;</TD><TD class=\"tdhead\" width=\"100px\" align=\"right\" nowrap>&nbsp;Details&nbsp;</TD></TR>");

            //Output directorys
            DirectoryInfo oDirInfo = new DirectoryInfo(Server.MapPath("") + sSubDir);
            DirectoryInfo[] oDirs = oDirInfo.GetDirectories();
            foreach (DirectoryInfo oDir in oDirs)
            {
                try
                {
                    Response.Write("<TR><TD class=\"tdDir\"><A href=\"" + lsScriptName + "?path=" + lsWebPath + oDir.Name + "&title=" + lsTitle + "&link=" + lsLink + "\">" + oDir.Name + "</A></TD><TD class=\"tdDir\">Dir</TD><TD class=\"tdDir\" align=\"right\">" + oDir.LastWriteTime + "</TD><TD class=\"tdDir\" align=\"right\">" + oDir.GetFiles().Length + " files</TD></TR>");
                }
                catch (Exception ex)
                {
                    Response.Write("<TR><TD class=\"tdDir\">" + oDir.Name + " (Access Denied)</TD><TD class=\"tdDir\">Dir</TD><TD class=\"tdDir\" align=\"right\">" + oDir.LastWriteTime + "</TD><TD class=\"tdDir\" align=\"right\">? files</TD></TR>");
                }
            }

            //Ouput files
            FileInfo[] oFiles = oDirInfo.GetFiles();
            foreach (FileInfo oFile in oFiles)
            {
                if (oFile.Name.ToLower() != lsScriptName)
                {
                    iLen = oFile.Length;
                    if (iLen >= 1048960) { iLen = iLen / 1048960; sLen = "mb"; } else { iLen = iLen / 1024; sLen = "kb"; }
                    sLen = Decimal.Round(iLen, 2).ToString() + sLen;
                    Response.Write("<TR><TD class=\"tdFile\"><A href=\"" + lsWebPath + oFile.Name + "\">" + oFile.Name + "</A></TD><TD class=\"tdFile\">File</TD><TD class=\"tdFile\" align=\"right\">" + oFile.LastWriteTime + "</TD><TD class=\"tdFile\" align=\"right\">" + sLen + " </TD></TR>");
                }
            }

            //Output footer
            Response.Write("</TBODY></TABLE></CENTER><BR>");

        }
        catch (Exception ex)
        {
            RptErr(ex.Message);
        }
    %>
    <p style="text-align: center; font-size: small;">
        <br />
        <a href="http://www.aejw.com/" style="color: #aaaaaa;">AejW.com - Web Browse 0070</a>
    </p>
</body>
</html>