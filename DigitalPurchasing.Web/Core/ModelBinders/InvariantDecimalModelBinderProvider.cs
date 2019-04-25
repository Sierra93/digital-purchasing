using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalPurchasing.Web.Core.ModelBinders
{
    public class InvariantDecimalModelBinderProvider : Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinderProvider
    {
        private readonly ILoggerFactory _loggerFactory;

        public InvariantDecimalModelBinderProvider(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

        public Microsoft.AspNetCore.Mvc.ModelBinding.IModelBinder GetBinder(Microsoft.AspNetCore.Mvc.ModelBinding.ModelBinderProviderContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!context.Metadata.IsComplexType && (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?)))
            {
                return new InvariantDecimalModelBinder(context.Metadata.ModelType, _loggerFactory);
            }

            return null;
        }
    }
}
