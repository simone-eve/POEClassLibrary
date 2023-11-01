using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEClassLibrary.Models
{
    public class UserId
    {
        public UserId(int userID)
        {
            UserID = userID;
        }

        public int UserID { get; set; }
    }
}
