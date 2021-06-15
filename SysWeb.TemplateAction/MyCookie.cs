using System;
using System.Text;
using System.Web;
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class MyCookie : ITACookie
    {
        private HttpCookie mCookie;
        public HttpCookie Cookie
        {
            get { return mCookie; }
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
        /// <summary>
        /// 加密密钥
        /// </summary>
        private string mEncodeKey;
        /// <summary>
        /// 存在取出，不存在则创建
        /// </summary>
        /// <param name="name"></param>
        public MyCookie(HttpCookie cookie, string encodeKey = null)
        {
            mCookie = cookie;
            mEncodeKey = encodeKey;
        }
     
        public DateTime Expires
        {
            get { return mCookie.Expires; }
            set { mCookie.Expires = value; }
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
