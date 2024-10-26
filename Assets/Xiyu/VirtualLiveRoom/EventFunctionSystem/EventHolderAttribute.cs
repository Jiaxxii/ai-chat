#nullable enable
using System;
using System.Runtime.CompilerServices;

namespace Xiyu.VirtualLiveRoom.EventFunctionSystem
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class EventHolderAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        public string MethodName { get; }

        public EventHolderAttribute([CallerMemberName] string methodName = "")
        {
            MethodName = methodName;
        }
    }
}