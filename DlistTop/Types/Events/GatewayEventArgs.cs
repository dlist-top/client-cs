using System;

namespace DlistTop.Types.Events
{
    public class GatewayEventArgs<T> : EventArgs
    {
        public T Data { get; set; }

        public GatewayEventArgs(T data)
        {
            Data = data;
        }
    }
}
