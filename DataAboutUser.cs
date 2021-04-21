using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;




class DataAboutUser
{
    public List<String> usernames = new List<string>();
    public List<String> CorrespondingPassword = new List<string>();
    public static Dictionary<string, string> UsnPwd = new Dictionary<string, string>();
    public void Start()
    {
        int count = 0;
        foreach (String s in usernames)
        {
            UsnPwd.Add(s, CorrespondingPassword[count]);
        }
    }
}

