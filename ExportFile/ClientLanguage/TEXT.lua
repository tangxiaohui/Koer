local function setReadOnly(t) 
 	local mt = {
		__index = t,
		__newindex = function(t, k, v)
			error("attempt to update value!")
		end
	}
	local proxyTable = {}
	setmetatable(proxyTable, mt)
	return proxyTable
end

     TEXTS  = setReadOnly { 
Text_zhezhishiyigeceshi = "这是中文的",
Text_MessageBox_OK = "确定",
Text_MessageBox_Cancel = "取消",
Text_Tips_AcOrPwIsNull = "用户名密码不能为空!",
Text_Tips_TwoPwIsUnEqual = "两次输入的密码不同！",
Text_Tips_ConnectServerFail = "连接服务器失败!",
Text_Account = "用户名",
Text_Text = "密码",
Text_RegistFail = "注册失败",
Text_LoginFail = "登录失败",
Text_LoginSuccess = "登录成功",
}