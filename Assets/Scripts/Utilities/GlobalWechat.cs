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
        //云函数的名字
        callFunction.name = "login";
        //传入的参数
        callFunction.data = userInfo;
        callFunction.success = (res) =>
        {
            Debug.Log("登陆成功回调：" + res);
            GlobalData.userInfo = res.result[0].ToString();
        };
        callFunction.fail = (res) =>
        {
            Debug.Log("登陆失败");
        };
        WX.cloud.CallFunction(callFunction);
    }
}
