using TemplateAction.Label;
using System;
namespace TemplateAction.Core
{
    /// <summary>
    /// ajax结果，进行ajax操作时使用
    /// </summary>
    public class AjaxResult : IResult
    {
        protected int mCode;
        protected string mMessage;
        protected string mData;
        protected ITAContext mContext;

        private static string _codeName = "code";
        private static string _messageName = "message";
        private static string _dataName = "data";
        /// <summary>
        /// 重新设置code、message、data字段名称
        /// </summary>
        /// <param name="codeName"></param>
        /// <param name="messageName"></param>
        /// <param name="dataName"></param>
        public static void Init(string codeName, string messageName, string dataName)
        {
            _codeName = codeName;
            _messageName = messageName;
            _dataName = dataName;
        }
        /// <summary>
        /// 字符串数据字段
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allUnicode"></param>
        /// <returns></returns>
        public static string JsonData(string input, bool allUnicode = false)
        {
            return "\"" + Common.TAUtility.Unicode(input) + "\"";
        }
        /// <summary>
        /// Ajax请求结果返回
        /// </summary>
        /// <param name="isErr">错误代码</param>
        /// <param name="message">提示消息</param>
        /// <param name="jsonData">返回json字符串</param>
        public AjaxResult(ITAContext context, int code, string message, string jsonData = null)
        {
            mCode = code;
            mMessage = message;
            mData = jsonData;
            mContext = context;
        }


        public void Output()
        {
            mContext.Response.ContentType = "application/json";
            if (mData == null)
            {
                mContext.Response.Write("{\"" + _codeName + "\":" + mCode + ",\"" + _messageName + "\":\"" + Common.TAUtility.AllFilter(mMessage) + "\"}");
            }
            else
            {
                mContext.Response.Write("{\"" + _codeName + "\":" + mCode + ",\"" + _messageName + "\":\"" + Common.TAUtility.AllFilter(mMessage) + "\",\"" + _dataName + "\":" + mData + "}");
            }
        }
    }
}
