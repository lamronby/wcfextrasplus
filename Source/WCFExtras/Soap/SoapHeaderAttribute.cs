using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Description;

namespace WCFExtras.Soap
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SoapHeaderAttribute : Attribute
    {
        string name;
        Type type;
        SoapHeaderDirection direction = SoapHeaderDirection.In;

        public SoapHeaderAttribute(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public Type Type
        {
            get
            {
                return type;
            }
        }

        public SoapHeaderDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
    }

    [Flags]
    public enum SoapHeaderDirection
    {
        In = 1,
        Out = 2,
        InOut = 3,
    }
}
