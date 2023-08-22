using UnityEngine;
using WeChatWASM;

public class GlobalWechat
{
    public static void OnRegisterUser(string userInfo)
    {
        CallFunctionInitParam callFunctionInit = new CallFunctionInitParam();
        callFunctionInit.env = "antigravity-9g6r95jq072d0af7";
        WX.cloud.Init(callFunctionInit);

        CallFunctionParam callFunction = new CallFunctionParam();
        //�ƺ���������
        callFunction.name = "login";
        //����Ĳ���
        callFunction.data = userInfo;
        callFunction.success = (res) =>
        {
            Debug.Log("��½�ɹ��ص���" + res);
            GlobalData.userInfo = res.result[0].ToString();
        };
        callFunction.fail = (res) =>
        {
            Debug.Log("��½ʧ��");
        };
        WX.cloud.CallFunction(callFunction);
    }
}
