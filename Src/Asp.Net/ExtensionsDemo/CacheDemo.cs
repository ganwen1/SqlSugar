﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionsDemo
{
    public class CacheDemo
    {
        public static void Init()
        {
            var myCache = new SqlSugar.Extensions.HttpRuntimeCache();
            SqlSugarClient db = new SqlSugarClient(
              new ConnectionConfig()
              {
                  ConnectionString = Config.ConnectionString,
                  DbType = DbType.SqlServer,
                  IsAutoCloseConnection = true,
                  ConfigureExternalServices = new ConfigureExternalServices()
                  {
                      DataInfoCacheService = myCache
                  }
              });
    
            for (int i = 0; i < 10000; i++)
            {
                db.Queryable<Student>().Where(it => it.Id > 0).WithCache().ToList();
            }

            db.Queryable<Student, Student>((s1, s2) => s1.Id == s2.Id).Select(s1=>s1).WithCache().ToList();

            db.Queryable<Student, Student>((s1, s2) => new object[] {
                JoinType.Left,s1.Id==s2.Id
            }).Select(s1 => s1).WithCache().ToList();

            Console.WriteLine("Cache Key Count:"+myCache.GetAllKey<string>().Count());
            foreach (var item in myCache.GetAllKey<string>())
            {
                Console.WriteLine();
                Console.WriteLine(item);
                Console.WriteLine();
            }

            db.Deleteable<Student>().Where(it => it.Id == 1).RemoveDataCache().ExecuteCommand();

            Console.WriteLine("Cache Key Count:" + myCache.GetAllKey<string>().Count());
        }
 
    }
}
