using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




class DataAboutUser
{
    public static List<String> usernames = new List<string>() { "hi"};
    public static List<String> CorrespondingPassword = new List<string>() { "hi"};
    public static Dictionary<string, string> UsnPwd = new Dictionary<string, string>();
    public static void Initialize()
    {
        int count = 0;
        foreach (String s in usernames)
        {
            UsnPwd.Add(s, CorrespondingPassword[count]);
        }
    }
}

