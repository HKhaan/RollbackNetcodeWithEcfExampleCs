using RollBackExample;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace RollBackExample
{
    [System.AttributeUsage(System.AttributeTargets.Field | System.AttributeTargets.Class |
                           System.AttributeTargets.Struct)]
    public class RollBackPropAttribute : System.Attribute
    {
    }

    public class RollbackFields
    {
        public Component Component;

        public Dictionary<int, Dictionary<FieldInfo, object>> Values =
            new Dictionary<int, Dictionary<FieldInfo, object>>();

        public List<FieldInfo> Fields = new List<FieldInfo>();
    }

    public class RollbackLogic
    {
        private RollbackWorld engine;
        public static RollbackLogic Instance { get; set; }

        public RollbackLogic(RollbackWorld engine)
        {
            this.engine = engine;
            Instance = this;
        }
        //public Dictionary<string, System.Reflection.FieldInfo> savedFields =
        //    new Dictionary<string, System.Reflection.FieldInfo>();
        //
        //public Dictionary<int, Dictionary<string, object>> values = new Dictionary<int, Dictionary<string, object>>();

        public List<RollbackFields> rollbackFields = new List<RollbackFields>();


        public void CacheFields()
        {
            var assembly = Assembly.GetAssembly(typeof(RollbackFields));
            var types = new List<Type>();
            foreach (var component in engine.components)
            {
                var rollbackField = new RollbackFields{Component = component};
                rollbackField.Values[0] = new Dictionary<FieldInfo, object>();
                var type = component.GetType();
                var fields = type.GetFields();
                var fieldInfo = new List<FieldInfo>();
                foreach (var prop in fields)
                {
                    var attrs = prop.CustomAttributes.FirstOrDefault(x =>
                        x.AttributeType == typeof(RollBackPropAttribute));
                    if (attrs != null)
                    {
                        rollbackField.Fields.Add(prop);
                        object value = null;
                        if (prop.FieldType == typeof(string))
                        {
                            value = String.Copy(((string)prop.GetValue(component)));
                        }
                        else
                            value = prop.GetValue(component);

                        rollbackField.Values[0].Add(prop, value);
                    }
                }

                rollbackFields.Add(rollbackField);
            }



        }

        //This method is heavy, it's only needed for synctests. Checksum doesn't realy mather that much during multiplayer gameplay.
        public int GetChecksumForFrame(int index) {
            //checksum
            var data = new List<byte>();
            foreach (var rollbackfield in rollbackFields)
            {
                foreach (var value in rollbackfield.Values[index])
                {
                    var obj = value.Value;
                    if (obj == null)
                        continue;

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, obj);
                    var arr = ms.ToArray();
                    data.AddRange(arr);
                }
            }
            int sum1 = 0xffff, sum2 = 0xffff;
            int len = data.Count;
            int currentCount = 0;
            while (len != 0)
            {
                int tlen = len > 360 ? 360 : len;
                len -= tlen;
                do
                {
                    sum1 += data[currentCount];
                    currentCount++;
                    sum2 += sum1;
                } while ((--tlen) != 0);
                sum1 = (sum1 & 0xffff) + (sum1 >> 16);
                sum2 = (sum2 & 0xffff) + (sum2 >> 16);
            }

            /* Second reduction step to reduce sums to 16 bits */
            sum1 = (sum1 & 0xffff) + (sum1 >> 16);
            sum2 = (sum2 & 0xffff) + (sum2 >> 16);
            return (sum2 << 16 | sum1);
        }

        public void SetBack(int index)
        {
            foreach (var rollbackfield in rollbackFields)
            {
                foreach (var value in rollbackfield.Values[index])
                {
                    value.Key.SetValue(rollbackfield.Component, value.Value);
                }
            }

            
        }

        public void BackupValues(int frame)
        {
            foreach (var rollbackfield in rollbackFields)
            {
                rollbackfield.Values[frame] = new Dictionary<FieldInfo, object>();
                foreach (var fieldInfo in rollbackfield.Fields)
                {
                    var val = fieldInfo.GetValue(rollbackfield.Component);
                    object value = null;
                    if (fieldInfo.FieldType == typeof(string))
                    {
                        value = String.Copy(((string)fieldInfo.GetValue(rollbackfield.Component)));
                    }
                    else
                        value = fieldInfo.GetValue(rollbackfield.Component);

                    rollbackfield.Values[frame].Add(fieldInfo, value);
                }
            }
        }
    }
}