using System;

namespace Ratel.IO
{
    /// <summary>
    /// IO 타입
    /// </summary>
    public enum IOType { 
        /// <summary>
        /// 입력 타입
        /// </summary>
        IN, 
        /// <summary>
        /// 출력타입
        /// </summary>
        OUT };

    public interface IIO
    {
        event EventHandler<IOChangeArgs> IOChangeEvent;

        void ReadInPort();
        void ReadOutPort();
        void StopRead();
    }
}