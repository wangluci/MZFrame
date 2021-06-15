using System;
using System.Collections.Generic;
namespace MyNet.MQ.Customer
{
    public class Topic
    {
        private string _topic;

        private List<Token> _tokens;

        private bool _valid;

        public static implicit operator Topic(string s)
        {
            return new Topic(s);
        }
        public Topic(string topic)
        {
            _topic = topic;
        }

        Topic(List<Token> tokens)
        {
            _tokens = tokens;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (i == 0)
                {
                    _topic = tokens[i].Name;
                }
                else
                {
                    _topic = string.Format("{0}/{1}", _topic, tokens[i].Name);
                }
            }
            _valid = true;
        }

        public List<Token> GetTokens()
        {
            if (_tokens == null)
            {
                try
                {
                    _tokens = ParseTopic(_topic);
                    _valid = true;
                }
                catch (Exception e)
                {
                    _valid = false;
                }
            }

            return _tokens;
        }

        private List<Token> ParseTopic(string topic)
        {
            if (string.IsNullOrEmpty(topic))
            {
                throw new Exception("解释Topic异常");
            }
            List<Token> res = new List<Token>();
            string[] splitted = topic.Split("/".ToCharArray());

            if (splitted.Length == 0)
            {
                res.Add(Token.EMPTY);
            }

            if (topic.EndsWith("/"))
            {
                string[] newSplitted = new string[splitted.Length + 1];
                Array.Copy(splitted, 0, newSplitted, 0, splitted.Length);
                newSplitted[splitted.Length] = string.Empty;
                splitted = newSplitted;
            }

            for (int i = 0; i < splitted.Length; i++)
            {
                string s = splitted[i];
                if (string.IsNullOrEmpty(s))
                {
                    res.Add(Token.EMPTY);
                }
                else if (s.Equals("#"))
                {
                    if (i != splitted.Length - 1)
                    {
                        throw new Exception("topic格式错误, 符号 (#) 必需放在最后");
                    }
                    res.Add(Token.MULTI);
                }
                else if (s.Contains("#"))
                {
                    throw new Exception("topic格式错误, 子topic错误 ");
                }
                else if (s.Equals("+"))
                {
                    res.Add(Token.SINGLE);
                }
                else if (s.Contains("+"))
                {
                    throw new Exception("topic格式错误, 子topic错误 ");
                }
                else
                {
                    res.Add(new Token(s));
                }
            }

            return res;
        }

        public Token HeadToken()
        {
            List<Token> tokens = GetTokens();
            if (tokens.Count == 0)
            {
                return null;
            }
            return tokens[0];
        }

        public bool IsEmpty()
        {
            List<Token> tokens = GetTokens();
            return tokens == null || tokens.Count == 0;
        }


        public Topic ExceptHeadToken()
        {
            List<Token> tokens = GetTokens();
            if (tokens.Count == 0)
            {
                return new Topic(new List<Token>());
            }
            List<Token> tokensCopy = new List<Token>(tokens);
            tokensCopy.RemoveAt(0);
            return new Topic(tokensCopy);
        }

        public bool IsValid()
        {
            if (_tokens == null)
                GetTokens();

            return _valid;
        }

     
        public bool Match(Topic subscriptionTopic)
        {
            List<Token> msgTokens = GetTokens();
            List<Token> subscriptionTokens = subscriptionTopic.GetTokens();
            int i = 0;
            for (; i < subscriptionTokens.Count; i++)
            {
                Token subToken = subscriptionTokens[i];
                if (!Token.MULTI.Equals(subToken) && !Token.SINGLE.Equals(subToken))
                {
                    if (i >= msgTokens.Count)
                    {
                        return false;
                    }
                    Token msgToken = msgTokens[i];
                    if (!msgToken.Equals(subToken))
                    {
                        return false;
                    }
                }
                else
                {
                    if (Token.MULTI.Equals(subToken))
                    {
                        return true;
                    }
                }
            }
            return i == msgTokens.Count;
        }
        public override string ToString()
        {
            return _topic;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Topic other = obj as Topic;
            if (other == null) return false;
            return this._topic.Equals(other._topic);
        }
        public override int GetHashCode()
        {
            return this._topic.GetHashCode();
        }
    }
}
