using Ratel.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ratel.IO
{
    /// <summary>
    /// 모든 IO 클래스의 기본이 되는 클래스
    /// </summary>
    public abstract class IOBase : IIO
    {
        public NLog.Logger log => RatelLib.Log;
        const int maxCount = 1024;
        public event EventHandler<IOChangeArgs> IOChangeEvent = delegate { };

        public Action BasicInterlock = delegate { };
        public Action<IOChangeArgs> BasicInterative = delegate { };

        public int InCount { get; private set; } = maxCount;
        public int OutCount { get; private set; } = maxCount;

        protected bool[] preIn = new bool[maxCount];
        protected bool[] preOut = new bool[maxCount];

        protected bool[] In = new bool[maxCount];
        protected bool[] Out = new bool[maxCount];
        public static bool Close { get; private set; } = false; 

        protected Task autoReadTask = null;
        
        protected CancellationTokenSource cancelTokenSource = new CancellationTokenSource();

        /// <summary>
        /// 입출력 점수를 기반으로 한 클래스를 초기화 한다
        /// </summary>
        /// <param name="inCount">입력 점수</param>
        /// <param name="outCount">출력 점수</param>
        public IOBase(int inCount, int outCount)
        {
            InCount = inCount;
            OutCount = outCount;

            preIn = new bool[inCount];
            In = new bool[inCount];

            preOut = new bool[outCount];
            Out = new bool[outCount];
        }

        /// <summary>
        /// IO의 생성자 대신 호출한다.
        /// vIO 를 true로 설정하면 VIO를 만들어서 리턴 해 준다.
        /// </summary>
        /// <param name="defNew"></param>
        /// <param name="vIO"></param>
        /// <returns></returns>
        public static IOBase CreateIO(IOBase defNew, bool vIO = false)
        {
            if (vIO)
                return new VIO(defNew.InCount, defNew.OutCount);
            else
                return defNew;
        }

        public static void CloseIO()
        {
            Close = true;
        }

        public abstract void DestroyIO();

        /// <summary>
        /// 포트 번호을 이용해서 입력포트를 읽어온다.
        /// 입력포트가 없으면 null을 리턴한다.
        /// </summary>
        /// <param name="bitNo"></param>
        /// <returns>입력포트가 없으면 null을 리턴한다.</returns>
        public bool? GetInBit(int bitNo)
        {
            if (bitNo >= 0 && bitNo < InCount)
                return In[bitNo];
            return null;
        }

        /// <summary>
        /// io  값이 value 가 뙬 때 까지 기다린다.
        /// </summary>
        /// <param name="bitNo"></param>
        /// <param name="value"></param>
        /// <param name="timeout"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public void WaitInOneBit(int bitNo, bool value, int timeout = -1, string msg = "")
        {
            try
            {
                var cts = new CancellationTokenSource();
                if (timeout > 0)
                {
                    cts.CancelAfter(timeout);
                }
                MTimeout.CheckTimeout(() => In[bitNo] == value, true, timeout);
            }
            catch 
            {
                throw;
            }
        }

        public void WaitOutOneBit(int bitNo, bool value, int timeout = -1, string msg = "")
        {
            try
            {
                var cts = new CancellationTokenSource();
                if (timeout > 0)
                {
                    cts.CancelAfter(timeout);
                }
                MTimeout.CheckTimeout(() => Out[bitNo] == value, true, timeout);
            }
            catch
            {
                throw;
            }
        }


        /// <summary>
        /// VIO 사용시에만 사용가능한 함수임 
        /// </summary>
        /// <param name="bitNo"></param>
        /// <param name="value"></param>
        virtual public void SetInBit(int bitNo, bool value)
        {
            //Error.RaiseError(Error.E_CANNOT_USE_SETINBIT_FUNCTION);
        }

        /// <summary>
        /// 출력 포트를 포트 번호로 직접 읽어온다.
        /// 특별한 경우 하나면 GetOutBit 함수를 사용할것
        /// </summary>
        /// <param name="bitNo">포트 번호</param>
        /// <returns></returns>
        public bool? GetOutBit(int bitNo)
        {
            if (bitNo >= 0)
                return Out[bitNo];
            return null;
        }


        /// <summary>
        /// OutPort의 값을 직접 설정한다.
        /// 이 함수는 가상 IO에서만 사용할 수 있음
        /// 일반 IO값 변경은 SetBit함수를 사용
        /// </summary>
        /// <param name="bitNo"></param>
        /// <param name="value"></param>
        public virtual void SetOutBit(int bitNo, bool value)
        {
            //Error.RaiseError(Error.E_CANNOT_USE_SETOUTBIT_FUNCTION);
        }

        /// <summary>
        /// 입출력 포트의 값을 자동으로 읽는다.
        /// </summary>
        /// <param name="interval">읽을 인터벌</param>
        public void AutoRead(int interval = 50)
        {
            if (autoReadTask != null)
                return;
            cancelTokenSource = new CancellationTokenSource();

            // 초기 세팅
            ReadInPort();
            ReadOutPort();
            Array.Copy(In, preIn, In.Length);
            Array.Copy(Out, preOut, Out.Length);
            Thread.Sleep(10);

            autoReadTask = Task.Run(() =>
            {
                while (true)
                {
                    if (Close)
                        break;

                    ReadInPort();
                    ReadOutPort();

                    CheckChange(In, preIn, IOType.IN);
                    CheckChange(Out, preOut, IOType.OUT);

                    Array.Copy(In, preIn, In.Length);
                    Array.Copy(Out, preOut, Out.Length);

                    Thread.Sleep(interval);
                    //Console.WriteLine(ToBinString(inVal));
                    if (cancelTokenSource.Token.IsCancellationRequested)
                        return;
                }
            }, cancelTokenSource.Token);
        }

        static string ToBinString(bool[] value, string sep = "")
        {
            string str = "";
            for (int i = 0; i < value.Length; i++)
            {
                str += value[i] ? "1" : "0";
                if ((i + 1) % 8 == 0)
                    str += sep;
            }
            return str;
        }

        /// <summary>
        /// 입출력 포트의 변화를 체크한다.
        /// </summary>
        /// <param name="cur">현재 포트 값</param>
        /// <param name="pre">이전 포트 값</param>
        /// <param name="type">In/Out</param>
        void CheckChange(bool[] cur, bool[] pre, IOType type)
        {
            for (int i = 0; i < cur.Length; i++)
            {
                if (cur[i] != pre[i])
                {
                    IOChangeEvent(this, new IOChangeArgs { IOType = type, IsOn = cur[i], PortNo = i });
                    BasicInterative(new IOChangeArgs { IOType = type, IsOn = cur[i], PortNo = i});
                }
            }
        }

        /// <summary>
        /// 출력 비트를 토글시킨다.
        /// </summary>
        /// <param name="type">포트 번호</param>
        /// <param name="port">포트 번호</param>
        /// <param name="timeOut">타임아웃</param>
        virtual public void ToggleBit(IOType type, int port, int timeOut = 0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 출력 접점을 켜거나 끈다.
        /// </summary>
        /// <param name="port">포트번호</param>
        /// <param name="value">true->On, false->Off</param>
        abstract public void SetBit(int port, bool value);

        /// <summary>
        /// 입력포트의 전체의 값을 읽는다.
        /// 모든 IO 관련 클래스는 이 함수를 기본으로 만들어야 하며, 
        /// 이 함수는 입력 IO를 읽어서 In값을 채운다.
        /// </summary>
        abstract public void ReadInPort();

        /// <summary>
        /// 출력포트 전체의 값을 읽는다.
        /// 모든 IO 관련 클래스는 이 함수를 기본으로 만들어야 하며, 
        /// 이 함수는 출력 IO를 읽어서 In값을 채운다.
        /// </summary>
        abstract public void ReadOutPort();


        /// <summary>
        /// 자동으로 IO을 읽는 기능을 정지 시킨다.
        /// </summary>
        virtual public void StopRead()
        {
            cancelTokenSource.Cancel();
            autoReadTask = null;
        }
    }

    /// <summary>
    /// IO의 변화 정보를 가지고 있는 클래스
    /// </summary>
    public class IOChangeArgs : EventArgs
    {
        /// <summary>
        /// IOType
        /// </summary>
        public IOType IOType { get; set; }
        /// <summary>
        /// 포트번호
        /// </summary>
        public int PortNo { get; set; }
        /// <summary>
        /// true: off->on
        /// false : on->off
        /// </summary>
        public bool IsOn { get; set; }

        public override string ToString()
        {
            return $"{IOType} : {PortNo} => {IsOn}";
        }
    }

}
