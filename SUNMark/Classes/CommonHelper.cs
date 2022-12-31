using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using SUNMark.Models;

namespace SUNMark.Classes
{
    public class CommonHelper
    {
        public DbConnection ObjDBConnection = new DbConnection();

    }
}
