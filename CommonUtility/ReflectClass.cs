using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace CommonUtility
{
    public class ReflectClass
    {
        public static void ReflectObject(object objReflect, DataRow drData)
        {
            try
            {
                Type T = objReflect.GetType();
                PropertyInfo[] properties = T.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (drData.Table.Columns.Contains(property.Name) && !String.IsNullOrEmpty(drData[property.Name].ToString()))
                        property.SetValue(objReflect, drData[property.Name], null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
