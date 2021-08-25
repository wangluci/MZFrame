using System;
using System.Collections.Generic;
using System.Text;

namespace TemplateAction.Core
{
    public interface IServiceDescriptorEnumerable: IEnumerable<ServiceDescriptor>
    {
        ServiceDescriptor First { get; }
    }
}
