﻿using System;
using TemplateAction.Core;

namespace TemplateAction.Extension
{
    public static class DefaultITAServicesExtension
    {
        public static T GetService<T>(this ITAServices collection, ILifetimeFactory extOtherFactory = null) where T : class
        {
            return collection.GetService(typeof(T).FullName, extOtherFactory) as T;
        }
    }
}
