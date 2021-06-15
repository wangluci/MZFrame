
namespace MyNet.MQ.Customer
{
    public class Token
    {
        public static readonly Token EMPTY = new Token(string.Empty);
        public static readonly Token MULTI = new Token("#");
        public static readonly Token SINGLE = new Token("+");
        private string _name;
        public string Name { get { return _name; } }

        public Token(string s)
        {
            _name = s;
        }

        protected bool Match(Token t)
        {
            if (MULTI.Equals(t) || SINGLE.Equals(t))
            {
                return false;
            }

            if (MULTI.Equals(this) || SINGLE.Equals(this))
            {
                return true;
            }

            return Equals(t);
        }
        public override int GetHashCode()
        {
            int hash = 7;
            hash = 29 * hash + (this._name != null ? this._name.GetHashCode() : 0);
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
  
            Token other = obj as Token;
            if (other == null) return false;
            if ((this._name == null) ? (other._name != null) : !this._name.Equals(other._name))
            {
                return false;
            }
            return true;
        }
        public override string ToString()
        {
            return _name;
        }

    }
}
