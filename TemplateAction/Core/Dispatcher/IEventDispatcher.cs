﻿using System;
namespace TemplateAction.Core
{
    public interface IEventDispatcher : IDispatcher
    {
        void Register<T, X>(string key, X handler) where T : class where X : ITAEventHandler<T>;
        void RegisterLoadAfter(Action<TAApplication> ac);
        void DispathLoadAfter(TAApplication app);
        /// <summary>
        /// 通过监听器监听事件
        /// </summary>
        /// <param name="listener"></param>
        void AddListener(object listener);
    }
}
