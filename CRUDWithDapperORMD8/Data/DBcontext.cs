using CRUDWithDapperORMD8.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
namespace CRUDWithDapperORMD8.Data
{
    public class DBcontext
    {
        private readonly string _connectionString;

        public DBcontext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Project");
        }

        public IDbConnection Connection => new SqlConnection(_connectionString);

    }    
}
