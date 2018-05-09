using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HomecareManagement.Service
{
    public class ConStr
    {
        public static String getConstr(int i)
        {
            String serverIP = "";
            String serverUser = "";
            String serverPwd = "";
            String serverPort = "";
            String serverDataBaseName = "";
            String conStr = "";
            switch (i)
            {
                case 0:
                    {
                        //HK-Server
                        serverIP = "www.mobilelabhk.info";
                        serverUser = "finalproject";
                        serverPwd = "M6unm4G4jyNseWv4";
                        serverPort = "5000";
                        serverDataBaseName = "finalproject";
                        break;
                    }
                case 1:
                    {
                        //Godaddy
                        serverIP = "tomatoLab.db.8394846.hostedresource.com";
                        serverUser = "tomatoLab";
                        serverPwd = "CSIE#j504";
                        serverPort = "3306";
                        serverDataBaseName = "tomatoLab";
                        break;
                    }
                case 2:
                    {
                        //shuang.myftp.org
                        serverIP = "shuang.myftp.org";
                        serverUser = "tomatoLab";
                        serverPwd = "zxcv1111";
                        serverPort = "3306";
                        serverDataBaseName = "tomatoLab";
                        break;
                    }
            }
            conStr = "Server=" + serverIP
                  + ";Port=" + serverPort
                  + ";Database=" + serverDataBaseName
                  + ";User=" + serverUser
                  + ";Pwd=" + serverPwd
                  + ";CharSet=utf8;";
            return conStr;
        }
    }
}