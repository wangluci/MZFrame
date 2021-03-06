using System;
using System.Collections;
using System.Collections.Generic;

namespace TemplateAction.Route
{
    public class RoutePathTokenizer : IEnumerable<RouteStringSegment>
    {
        private readonly string _path;
        private int _count;

        public RoutePathTokenizer(string path)
        {
            _path = path;
            _count = -1;
        }

        public int Count
        {
            get
            {
                if (_count == -1)
                {
                    // We haven't computed the real count of segments yet.
                    if (_path.Length == 0)
                    {
                        // The empty string has length of 0.
                        _count = 0;
                        return _count;
                    }

                    // A string of length 1 must be "/" - all PathStrings start with '/'
                    if (_path.Length == 1)
                    {
                        // We treat this as empty - there's nothing to parse here for routing, because routing ignores
                        // a trailing slash.
                        _count = 0;
                        return _count;
                    }

                    // This is a non-trival PathString
                    _count = 1;

                    // Since a non-empty PathString must begin with a `/`, we can just count the number of occurrences
                    // of `/` to find the number of segments. However, we don't look at the last character, because
                    // routing ignores a trailing slash.
                    for (var i = 1; i < _path.Length - 1; i++)
                    {
                        if (_path[i] == '/')
                        {
                            _count++;
                        }
                    }
                }

                return _count;
            }
        }

        public RouteStringSegment this[int index]
        {
            get
            {
                if (index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }


                var currentSegmentIndex = 0;
                var currentSegmentStart = 1;

                // Skip the first `/`.
                var delimiterIndex = 1;
                while ((delimiterIndex = _path.IndexOf('/', delimiterIndex)) != -1)
                {
                    if (currentSegmentIndex++ == index)
                    {
                        return new RouteStringSegment(_path, currentSegmentStart, delimiterIndex - currentSegmentStart);
                    }
                    else
                    {
                        currentSegmentStart = delimiterIndex + 1;
                        delimiterIndex++;
                    }
                }

                // If we get here we're at the end of the string. The implementation of .Count should protect us
                // from these cases. 
                return new RouteStringSegment(_path, currentSegmentStart, _path.Length - currentSegmentStart);
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<RouteStringSegment> IEnumerable<RouteStringSegment>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<RouteStringSegment>
        {
            private readonly string _path;

            private int _index;
            private int _length;

            public Enumerator(RoutePathTokenizer tokenizer)
            {
                _path = tokenizer._path;

                _index = -1;
                _length = -1;
            }

            public RouteStringSegment Current
            {
                get
                {
                    return new RouteStringSegment(_path, _index, _length);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_path == null || _path.Length <= 1)
                {
                    return false;
                }

                if (_index == -1)
                {
                    // Skip the first `/`.
                    _index = 1;
                }
                else
                {
                    // Skip to the end of the previous segment + the separator.
                    _index += _length + 1;
                }

                if (_index >= _path.Length)
                {
                    // We're at the end
                    return false;
                }

                var delimiterIndex = _path.IndexOf('/', _index);
                if (delimiterIndex != -1)
                {
                    _length = delimiterIndex - _index;
                    return true;
                }

                // We might have some trailing text after the last separator.
                if (_path[_path.Length - 1] == '/')
                {
                    // If the last char is a '/' then it's just a trailing slash, we don't have another segment.
                    return false;
                }
                else
                {
                    _length = _path.Length - _index;
                    return true;
                }
            }

            public void Reset()
            {
                _index = -1;
                _length = -1;
            }
        }
    }
}
