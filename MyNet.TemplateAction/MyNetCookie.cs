using MyNet.Common;
using MyNet.Middleware.Http;
using System;
using System.Text;
using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class MyNetCookie : ITACookie
    {
        private DateTime _expires;
        private HttpCookie mCookie;
        public HttpCookie GenerateHttpCookie()
        {
            if (_expires.Ticks == 0)
            {
                mCookie.Expires = -1;
            }
            else
            {
                mCookie.Expires = Converter.Cast<long>(_expires);
            }
            return mCookie;
        }
        /// <summary>
        /// 路径cookie所在的目录，默认为/，就是根目录
        /// </summary>
        public string Path
        {
            get { return mCookie.Path; }
            set { mCookie.Path = value; }
        }
        /// <summary>
        /// 域，用在跨域cookie上
        /// </summary>
        public string Domain
        {
            get { return mCookie.Domain; }
            set { mCookie.Domain = value; }
        }

        public bool HttpOnly
        {
            get { return mCookie.HttpOnly; }
            set { mCookie.HttpOnly = value; }
        }
        public bool Secure
        {
            get { return mCookie.Secure; }
            set { mCookie.Secure = value; }
        }
        public DateTime Expires
        {
            get { return _expires; }
            set { _expires = value; }
        }
        /// <summary>
        /// 加密密钥
        /// </summary>
        private string mEncodeKey;
        /// <summary>
        /// 存在取出，不存在则创建
        /// </summary>
        /// <param name="name"></param>
        public MyNetCookie(HttpCookie cookie, string encodeKey = null)
        {
            mCookie = cookie;
            mEncodeKey = encodeKey;
            if (mCookie.Expires != -1)
            {
                _expires = Converter.Cast<DateTime>(mCookie.Expires);
            }
            else
            {
                _expires = new DateTime(0);
            }

        }

    
        /// <summary>
        /// 由于cookie编码问题所以应该先进行url编码
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="value"></param>
        public void SetValue(string value)
        {
            if (string.IsNullOrEmpty(mEncodeKey))
            {
                mCookie.Value = value;
            }
            else
            {
                mCookie.Value = Crypter.Encrypt(value, mEncodeKey);
            }

        }
        public string GetValue()
        {
            if (string.IsNullOrEmpty(mEncodeKey))
            {
                return mCookie.Value;
            }
            else
            {
                return Crypter.Decrypt(mCookie.Value, mEncodeKey);
            }

        }
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(mCookie.Value);
        }

    }
}
