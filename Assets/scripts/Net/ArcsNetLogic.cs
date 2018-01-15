
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;
using UnityEngine;

namespace NFSDK
{
    public class ARCSNetLogic
    {
        private static ARCSNetLogic _instance = null;
        public static ARCSNetLogic Instance()
        {
            if (_instance == null)
            {
                _instance = new ARCSNetLogic();
            }
            return _instance;
        }

        public ARCSNetLogic()
        {
            _instance = this;
        }

        public void OnStart()
        {

        }

        public void OnUpdate()
        {
            NFCNet.Instance().doUpdate();
        }

        public void OnDestroy()
        {

        }

        public void ConnectServer(string ip, UInt16 port)
        {
            if (NFCNet.Instance().isConnected())
                NFCNet.Instance().shutDown();
            
            NFCNet.Instance().ready(ip, port);
            NFCNet.Instance().connect();
        }



        {
            NFCNetDispatcher.Instance().AddReceiveCallBack((UInt16)id, netHandler);
        }
        
        public void Send<T>(Msg.EGameMsgID unMsgID, T data)
        {
            MemoryStream stream = new MemoryStream();
            Serializer.Serialize<T>(stream, data);

            MemoryStream pack = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(pack);
            UInt32 msgLen = (UInt32)stream.Length + ConstDefine.NF_PACKET_LEN_MSG_ID;
            writer.Write(NFCNet.ConvertUint32((UInt32)msgLen));
            writer.Write(NFCNet.ConvertUint16((UInt16)unMsgID));
            stream.WriteTo(pack);
            NFCNet.Instance().sendMsg(pack);
        }

    }
}