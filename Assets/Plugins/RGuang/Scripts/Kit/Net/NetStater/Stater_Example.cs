/// <summary>
///********************************************
/// ClassName    ：  Stater_Example
/// Description  ：  状态机测试案例
///********************************************/
/// </summary>
using System;

namespace RGuang.Kit.Net.NetState
{
    public enum NetState
    {
        None,
        Connecting,
        Authing,
        Connected,
        DisConnected
    }

    public enum OpEnum
    {
        None,
        Connect,
        Auth,
        DisConnect,
    }

    public class Stater_Example
    {
        public int pct = 90;
        private string mName;
        private Stater<NetState> mStater;

        StaterInput InputConnectOp = new StaterInputEnum<OpEnum>(OpEnum.Connect);
        StaterInput InputAuthOp = new StaterInputEnum<OpEnum>(OpEnum.Auth);
        StaterInput InputDataOp = new StaterInputData(null);
        StaterInput InputDisConnectOp = new StaterInputEnum<OpEnum>(OpEnum.DisConnect);

        public void Init(string name)
        {
            this.mName = name;
            mStater = new Stater<NetState>(NetState.None);

            mStater.AddGlobalTransition(InputConnectOp, NetState.Connecting, TryConnectServer);
            mStater.AddLocalTransition(NetState.Connecting, InputAuthOp, NetState.Authing, TrySendAuthInfo);
            mStater.AddLocalTransition(NetState.Authing, InputDataOp, NetState.Connected, TryAuthResult);
            mStater.AddLocalTransition(NetState.Connected, InputDataOp, NetState.Connected, TryHandleNetPkg);

            mStater.AddGlobalTransition(InputDisConnectOp, NetState.DisConnected, TryDisconnect);


            mStater.TransCallBack = (NetState before, NetState after) =>
            {
                //this.ColorLog(LogColor.Green, mName + ":" + before + " switch to " + after);
                UnityEngine.Debug.Log(mName + ":" + before + " switch to " + after);

            };

            while (true)
            {
                string ipt = Console.ReadLine();
                switch (ipt)
                {
                    case "a":
                        SendSwitchCMD(InputConnectOp);
                        break;
                    case "b":
                        SendSwitchCMD(InputAuthOp);
                        break;
                    case "c":
                        SendSwitchCMD(InputDataOp);
                        break;
                    case "d":
                        SendSwitchCMD(InputDataOp);
                        break;
                    case "e":
                        SendSwitchCMD(InputDisConnectOp);
                        break;
                    default:
                        break;
                }
            }
        }

        void SendSwitchCMD(StaterInput ipt)
        {
            mStater.Input(ipt);
        }

        bool TryConnectServer(NetState curState, StaterInput input, NetState dstState)
        {
            System.Random rd = new System.Random();
            int value = rd.Next(0, 100);
            if (value < pct)
            {
                SendSwitchCMD(InputAuthOp);
                return true;
            }
            else
            {
                SendSwitchCMD(InputDisConnectOp);
                return false;
            }
        }

        bool TrySendAuthInfo(NetState curState, StaterInput input, NetState dstState)
        {
            System.Random rd = new System.Random();
            int value = rd.Next(0, 100);
            if (value < pct)
            {
                SendSwitchCMD(InputDataOp);
                return true;
            }
            else
            {
                SendSwitchCMD(InputDisConnectOp);
                return false;
            }
        }
        bool TryAuthResult(NetState curState, StaterInput input, NetState dstState)
        {
            System.Random rd = new System.Random();
            int value = rd.Next(0, 100);
            if (value < pct)
            {
                SendSwitchCMD(InputDataOp);
                return true;
            }
            else
            {
                SendSwitchCMD(InputDisConnectOp);
                return false;
            }
        }
        bool TryHandleNetPkg(NetState curState, StaterInput input, NetState dstState)
        {
            System.Random rd = new System.Random();
            int value = rd.Next(0, 100);
            if (value < pct)
            {
                SendSwitchCMD(InputDataOp);
                return true;
            }
            else
            {
                SendSwitchCMD(InputDisConnectOp);
                return false;
            }
        }
        bool TryDisconnect(NetState curState, StaterInput input, NetState dstState)
        {
            //TODO 连接断开，清理资源
            return true;
        }

        public void UnInit()
        {
            mStater.ClearInputCache();
        }
    }
}
