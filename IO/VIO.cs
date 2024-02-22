using Ratel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ratel.IO
{
    /// <summary>
    /// 가상 IO
    /// </summary>
    public class VIO : IOBase
    {
        /// <summary>
        ///  가상 출력값을 가지고 있는 비트
        /// </summary>
        private bool[] outBit;
        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="inCount">입력 점수</param>
        /// <param name="outCount">출력 점수</param>
        public VIO(int inCount, int outCount) 
            : base(inCount, outCount)
        {
            outBit = new bool[outCount];
        }

        public override void DestroyIO()
        {
        }

        /// <summary>
        /// 입력 비트를 설정한다.
        /// 가상 IO에만 존재하는 함수
        /// </summary>
        /// <param name="bitNo"></param>
        /// <param name="value"></param>
        public override void SetInBit(int bitNo, bool value)
        {
            if (bitNo >= 0 && bitNo < OutCount)
                In[bitNo] = value;
        }

        /// <summary>
        /// OutPort의 값을 직접 설정한다.
        /// 이 함수는 가상 IO에서만 사용할 수 있음
        /// 일반 IO값 변경은 SetBit함수를 사용
        /// </summary>
        /// <param name="bitNo"></param>
        /// <param name="value"></param>
        public override void SetOutBit(int bitNo, bool value)
        {
            if (bitNo >= 0 && bitNo < OutCount)
                outBit[bitNo] = value;
        }

        public override void SetBit(int bitNo, bool value)
        {
            if (bitNo >= 0 && bitNo < OutCount)
                outBit[bitNo] = value;
        }

        ///// <summary>
        ///// 입력 비트의 값을 토글시킨다.
        ///// 이 함수는 가상 IO 에서만 사용가능하다.
        ///// </summary>
        ///// <param name="port">포트 번호</param>
        ///// <param name="timeOut">타임아웃</param>
        //public override void ToggleInBit(InIONames port, int timeOut = 0)
        //{
        //    if (port == InIONames.Reserved)
        //        return;
        //    var no = InNames[port].No;
        //    In[no] = !In[no];
        //}

        /// <summary>
        /// 출력 비트를 토글시킨다.
        /// </summary>
        /// <param name="type">IO Type</param>
        /// <param name="port">포트 번호</param>
        /// <param name="timeOut">타임아웃</param>
        public override void ToggleBit(IOType type,  int port, int timeOut = 0)
        {
            if (type == IOType.OUT)
                outBit[port] = !outBit[port];
            else
                In[port] = !In[port];
        }

        /// <summary>
        /// 입력포트의 전체의 값을 읽는다.
        /// 모든 IO 관련 클래스는 이 함수를 기본으로 만들어야 하며, 
        /// 이 함수는 입력 IO를 읽어서 In값을 채운다.
        /// </summary>
        public override void ReadInPort()
        {
        }

        /// <summary>
        /// 출력포트 전체의 값을 읽는다.
        /// 모든 IO 관련 클래스는 이 함수를 기본으로 만들어야 하며, 
        /// 이 함수는 출력 IO를 읽어서 In값을 채운다.
        /// </summary>
        public override void ReadOutPort()
        {
            if (outBit != null)
                Array.Copy(outBit, Out, OutCount);
        }
    }
}
