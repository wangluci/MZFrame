using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace TemplateAction.Core
{
    public class ServiceDescriptorList : IServiceDescriptorEnumerable
    {
        private ServiceDescriptorList() { }

        private List<ServiceDescriptor> _list = new List<ServiceDescriptor>();
        public ServiceDescriptor First
        {
            get
            {
                return _list[0];
            }
        }
        public static ServiceDescriptorList Create(ServiceDescriptor sd)
        {
            ServiceDescriptorList sdlist = new ServiceDescriptorList();
            sdlist.Add(sd);
            return sdlist;
        }
        public void Add(ServiceDescriptor sd)
        {
            _list.Add(sd);
        }
        public IEnumerator<ServiceDescriptor> GetEnumerator()
        {
            foreach(ServiceDescriptor sd in _list)
            {
                yield return sd;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (ServiceDescriptor sd in _list)
            {
                yield return sd;
            }
        }
    }
}
