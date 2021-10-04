using MyAccess.Aop;
using System;
using TemplateAction.Core;

namespace Common
{
    /// <summary>
    /// 业务逻辑返回处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BusResponse<T> : ITransReturn
    {
        private int mCode;
        private string mMessage;
        private T mData;
        public BusResponse(int code, string message, T data)
        {
            mCode = code;
            mMessage = message;
            mData = data;
        }
        public int Code
        {
            get { return mCode; }
        }
        public string Message
        {
            get { return mMessage; }
        }
        public T Data
        {
            get { return mData; }
        }
        public AjaxResult ToAjaxResult(ITAContext context)
        {
            if (!IsSuccess() || mData == null)
            {
                return new AjaxResult(context, mCode, mMessage);
            }
            Type destT = typeof(T);
            if (destT == typeof(string))
            {
                return new AjaxResult(context, mCode, mMessage, AjaxResult.JsonData(mData.ToString()));
            }
            else if (destT.IsValueType)
            {
                return new AjaxResult(context, mCode, mMessage, AjaxResult.JsonData(mData.ToString().ToLower()));
            }
            else
            {
                return new AjaxResult(context, mCode, mMessage, MyAccess.Json.Json.Encode(mData));
            }
        }
        public bool IsSuccess()
        {
            return mCode == 0;
        }
        public static BusResponse<T> Success(T value = default(T))
        {
            return new BusResponse<T>(0, string.Empty, value);
        }
        public static BusResponse<T> Error(int code, string message)
        {
            return new BusResponse<T>(code, message, default(T));
        }

    }
}
