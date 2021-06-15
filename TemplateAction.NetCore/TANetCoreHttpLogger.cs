using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using TemplateAction.Core;

namespace TemplateAction.NetCore
{
    public class TANetCoreHttpLogger : ITALogger
    {
        private ILogger _logger;
        public TANetCoreHttpLogger(ILogger logger)
        {
            _logger = logger;
        }
        public void Debug(string message)
        {
            _logger.LogDebug(message);
        }

        public void Error(string message)
        {
            _logger.LogError(message);
        }

        public void Info(string message)
        {
            _logger.LogInformation(message);
        }

        public void Warn(string message)
        {
            _logger.LogWarning(message);
        }
    }
}
