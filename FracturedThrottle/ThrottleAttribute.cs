using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FracturedCode.FracturedThrottle
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Throttle : Attribute
    {
        public int Rate { get; set; }
        public RateTypeEnum RateType { get; set; }
    }
    public enum RateTypeEnum
    {
        Minute,
        Hour,
        Day
    }
}