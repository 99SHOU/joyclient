using UnityEngine;
using System.Collections;
using NFSDK;
using System.IO;
using ProtoBuf;
using System;

public class NFCRoot : MonoBehaviour
{
    private readonly string m_Accoount = "hello";
    private readonly string m_LoginIp = "127.0.0.1";
    private readonly ushort m_LoginPort = 23000;

    private static NFCRoot _instance = null;
    public static NFCRoot Instance()
    {
        return _instance;
    }
    void Start()
    {
        _instance = this;

        Debug.Log("Root Start");

        DontDestroyOnLoad(gameObject);

        ARCSNetLogic.Instance().OnStart();
        ARCSNetLogic.Instance().ConnectServer(m_LoginIp, m_LoginPort);


        ARCSNetLogic.Instance().AddReceiveCallBack(Msg.EGameMsgID.EgmiLoginRespon, LoginResponHandler);
        ARCSNetLogic.Instance().AddReceiveCallBack(Msg.EGameMsgID.EgmiConnectToGateRespon, ConnectToGateResponHandler);

        Login();
    }
	
    void OnDestroy()
    {
        ARCSNetLogic.Instance().OnDestroy();
    }
	
	void Update () 
    {
        ARCSNetLogic.Instance().OnUpdate();
    }

    void Login()
    {
        Msg.LoginReq xData = new Msg.LoginReq();
        xData.Account = m_Accoount;
        ARCSNetLogic.Instance().Send<Msg.LoginReq>(Msg.EGameMsgID.EgmiLoginReq, xData);
    }

    void LoginResponHandler(UInt16 id, MemoryStream stream)
    {
        Msg.LoginRespon xData = new Msg.LoginRespon();
        xData = Serializer.Deserialize<Msg.LoginRespon>(stream);

        if (xData.ResponCode == Msg.LoginResponCode.LoginSuccess)
        {
            Debug.Log(xData.Account + xData.GateAddr + xData.Token + xData.ResponCode.ToString());
            ConnectToGate(m_Accoount, xData.GateAddr, xData.Token);
        }
    }

    void ConnectToGate(string account, string gateAddr, string token)
    {
        string[] gateInfo = gateAddr.Split(':');
        ARCSNetLogic.Instance().ConnectServer(gateInfo[0], ushort.Parse(gateInfo[1]));

        Msg.ConnectToGateReq xData = new Msg.ConnectToGateReq();
        xData.Account = account;
        xData.Token = token;
        ARCSNetLogic.Instance().Send<Msg.ConnectToGateReq>(Msg.EGameMsgID.EgmiConnectToGateReq, xData);
    }

    void ConnectToGateResponHandler(UInt16 id, MemoryStream stream)
    {
        Msg.ConnectToGateRespon xData = new Msg.ConnectToGateRespon();
        xData = Serializer.Deserialize<Msg.ConnectToGateRespon>(stream);

        Debug.Log(xData.Account + xData.ResponCode.ToString());
    }
}
